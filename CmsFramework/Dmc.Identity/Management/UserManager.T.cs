using Dmc.Identity.Utilities;
using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class UserManager<TUser> : UserManager<TUser, int, IdentityRole, IdentityClaim, IdentityLogin>
        where TUser : class, IUser
    {
        #region Private Fields

        private readonly IUserStore<TUser> _UserStore;

        #endregion

        #region Constructor

        public UserManager(IUserStore<TUser> userStore) 
            : base(userStore)
        {
            _UserStore = userStore;
        }

        #endregion

        #region Properties

        public override IUserStore<TUser, int, IdentityRole, IdentityClaim, IdentityLogin> UserStore => _UserStore;

        #endregion
    }

    // this whole thing is very very bad. 
    // store should do other things than manager.
    // we assume that e-mail is unique. this is not good

    public class UserManager<TUser, TKey, TRole, TClaim, TLogin>
        where TUser : class, IUser<TKey, TRole, TClaim, TLogin>
        where TKey : IEquatable<TKey>
        where TRole : class, IRole<TKey>
        where TLogin : class, ILogin<TKey>
        where TClaim : class, IClaim<TKey>
    {
        #region Constants

        private const string Purpose_VerifyEmail =  "VerifyEmail";
        private const string Purpose_PasswordReset = "PasswordReset";
        private const string Purpose_DeleteAccount = "DeleteAccount";

        #endregion

        #region Private Fields

        private readonly IUserStore<TUser, TKey, TRole, TClaim, TLogin> _UserStore;
        private readonly static TimeSpan _DefaultTokenDuration = TimeSpan.FromHours(4);
        private readonly static TimeSpan _DeleteAccountTokenDuration = TimeSpan.FromMinutes(30);

        #endregion

        #region Constructors

        public UserManager(IUserStore<TUser, TKey, TRole, TClaim, TLogin> userStore)
        {
            _UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }

        #endregion

        #region Properties

        public IPasswordProvider PasswordProvider { get; protected set; } = new PasswordProvider();

        public TimeSpan EmailTokenDuration { get; set; } = _DefaultTokenDuration;

        public TimeSpan DeleteAccountTokenDuration { get; set; } = _DeleteAccountTokenDuration;

        public virtual IUserStore<TUser, TKey, TRole, TClaim, TLogin> UserStore => _UserStore;

        #endregion

        #region Public Methods

        public virtual string CreateEmailVerificationCode(TUser user)
        {
            return TokenCreator.Create<TUser, TKey>(user, Purpose_VerifyEmail); //TODO: Encryption should not be done inside. instead, encryption should be done here, outside, using provider.
        }

        public virtual string CreatePasswordResetCode(TUser user)
        {
            return TokenCreator.Create<TUser, TKey>(user, Purpose_PasswordReset);
        }

        public virtual string CreateDeleteAccountCode(TUser user)
        {
            return TokenCreator.Create<TUser, TKey>(user, Purpose_DeleteAccount);
        }

        public async Task<MembershipResult> VerifyEmailAsync(Guid userId, string token) // we match with the token
        {
            var user = await _UserStore.FindByIdUniqueIdAsync(userId);

            if (user == null)
            {
                return MembershipResult.Failed(IdentityCodes.VerifyEmailCodeIncorrect, "Confirm E-mail Token is Incorrect.");
            }

            if (!TokenCreator.TryParseToken(token, out ParseTokenResult parsed))
            {
                return MembershipResult.Failed(IdentityCodes.VerifyEmailCodeIncorrect, "Confirm E-mail Token is Incorrect.");
            }

            // duration
            DateTime whenDoesTokenExpire = parsed.Created.Add(EmailTokenDuration);
            if (DateTime.UtcNow > whenDoesTokenExpire)
            {
                return MembershipResult.Failed(IdentityCodes.VerifyEmailCodeExpired, "Confirm E-mail Token Has Expired.");
            }

            if (user.EmailConfirmed && parsed.UniqueId == user.UniqueId) // nothing to do
            {
                return MembershipResult.Success;
            }

            if (!TokenCreator.IsTokenValid<TUser, TKey>(user, parsed, Purpose_VerifyEmail))
            {
                return MembershipResult.Failed(IdentityCodes.VerifyEmailCodeIncorrect, "Confirm E-mail Token is Incorrect.");
            }

            // confirm it here
            user.EmailConfirmed = true;

            // reset security stamp
            ResetSecurityStamp(user);

            return await UpdateInner(user);
        }

        public async Task<bool> IsResetPasswordCodeCorrectAsync(Guid uniqueId, string token)
        {
            var user = await _UserStore.FindByIdUniqueIdAsync(uniqueId);

            if (user == null)
            {
                return false;
            }

            if (!TokenCreator.TryParseToken(token, out ParseTokenResult parsed))
            {
                return false;
            }

            if (!TokenCreator.IsTokenValid<TUser, TKey>(user, parsed, Purpose_PasswordReset))
            {
                return false;
            }

            return true;
        }

        public virtual async Task<MembershipResult> DeleteUserAsync(TUser user, string token)
        {
            if (user == null)
            {
                return MembershipResult.Failed(IdentityCodes.UserNotFound, "Reset Password Token is Incorrect.");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null, empty, or white-space only.", nameof(token));
            }

            if (!TokenCreator.TryParseToken(token, out ParseTokenResult parsed))
            {
                return MembershipResult.Failed(IdentityCodes.TokenIncorrect, "Token is Incorrect.");
            }

            DateTime whenDoesTokenExpire = parsed.Created.Add(DeleteAccountTokenDuration);
            if (DateTime.UtcNow > whenDoesTokenExpire)
            {
                return MembershipResult.Failed(IdentityCodes.TokenExpired, "Token Has Expired.");
            }

            if (!TokenCreator.IsTokenValid<TUser, TKey>(user, parsed, Purpose_DeleteAccount))
            {
                return MembershipResult.Failed(IdentityCodes.TokenIncorrect, "Token is Incorrect.");
            }

            return await _UserStore.DeleteAsync(user);
        }

        public async Task<MembershipResult> ResetPasswordAsync(TUser user, string token, string newPassword) // we require user to enter the email or whatever. we find and match with token
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null, empty, or white-space only.", nameof(token));
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("Password cannot be null, empty, or white-space only.", nameof(newPassword));
            }

            if (!user.EmailConfirmed) // it's possible to verify this before
            {
                return MembershipResult.Failed(IdentityCodes.TokenIncorrect, "Reset Password Token is Incorrect.");
            }

            if (IsLockedOut(user))
            {
                return MembershipResult.Failed(IdentityCodes.UserLockedOut, "Your account is currently not active. Please contact us for more info.");
            }

            if (!TokenCreator.TryParseToken(token, out ParseTokenResult parsed))
            {
                return MembershipResult.Failed(IdentityCodes.ResetPasswordCodeIncorrect, "Reset Password Token is Incorrect.");
            }

            DateTimeOffset whenDoesTokenExpire = parsed.Created.Add(EmailTokenDuration);
            if (DateTimeOffset.UtcNow > whenDoesTokenExpire)
            {
                return MembershipResult.Failed(IdentityCodes.ResetPasswordCodeExpired, "Reset Password Token Has Expired.");
            }

            if (!TokenCreator.IsTokenValid<TUser, TKey>(user, parsed, Purpose_PasswordReset))
            {
                return MembershipResult.Failed(IdentityCodes.ResetPasswordCodeIncorrect, "Reset Password Token is Incorrect.");
            }

            // reset security stamp to reset old sessions
            ResetSecurityStamp(user);

            // set password
            user.PasswordHash = PasswordProvider.HashPassword(newPassword);

            // Save
            return await UpdateInner(user);
        }

        public bool IsLockedOut(TUser user)
        {
            return user.LockoutEnd.HasValue && user.LockoutEnd.Value.ToUniversalTime() > DateTimeOffset.UtcNow;
        }

        public virtual string CreateTwoFactorCode(TUser user) // TODO!!
        {
            return string.Empty;
        }

        public virtual async Task<MembershipResult> AddRoleToUser(TUser user, TRole role)
        {
            user.Roles.Add(role);
            return await UpdateInner(user);
        }

        public virtual Task<TUser> FindUserByIdAsync(TKey id)
        {
            return _UserStore.FindByIdAsync(id);
        }

        public virtual Task<TUser> FindUserByUniqueIdAsync(Guid uniqueId)
        {
            return _UserStore.FindByIdUniqueIdAsync(uniqueId);
        }

        public virtual Task<TUser> FindUserByUserNameAsync(string userName)
        {
            return _UserStore.FindByUserNameAsync(userName);
        }

        public virtual Task<TUser> FindUserByEmailAsync(string email)
        {
            return _UserStore.FindByEmailAsync(email);
        }

        public virtual Task<TUser> GetByExternalLoginAsync(string provider, string providerId)
        {
            return _UserStore.GetByExternalLoginAsync(provider, providerId);
        }

        public virtual async Task<MembershipResult> AddPasswordToUserAsync(TUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            // set password
            user.PasswordHash = PasswordProvider.HashPassword(password);

            // store
            return await UpdateInner(user);
        }

        public virtual async Task<MembershipResult> CreateUserAsync(TUser user) // without password
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.PasswordHash != null)
            {
                throw new InvalidOperationException("When CreateUserAsync(TUser user) method is used, user cannot already have a password set. Use the CreateUserAsync(TUser user, string password) method.");
            }

            // store
            return await InsertInner(user);
        }

        public virtual async Task<MembershipResult> CreateUserAsync(TUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            // store should have the possibility to query by any.

            // find if alrady exists
            var existingByUsername = await _UserStore.FindByUserNameAsync(user.UserName);

            if (existingByUsername != null)
            {
                return MembershipResult.Failed(IdentityCodes.UsernameExists, "Username already exists.");
            }

            var existingByEmail = _UserStore.FindByEmailAsync(user.Email);

            if (existingByUsername != null)
            {
                return MembershipResult.Failed(IdentityCodes.EmailExists, "E-mail already exists.");
            }

            // set password
            user.PasswordHash = PasswordProvider.HashPassword(password);

            // store
            return await InsertInner(user);
        }

        public async Task<MembershipResult> AddLoginToUserAsync(TUser user, TLogin login)
        {
            if (user.Logins.Any(o => 
                o.LoginProvider.Equals(login.LoginProvider, StringComparison.OrdinalIgnoreCase)
                &&
                o.ProviderUniqueId.Equals(login.ProviderUniqueId, StringComparison.OrdinalIgnoreCase)))
            {
                return MembershipResult.Failed("Login already exists.");
            }

            user.Logins.Add(login);
            return await UpdateInner(user);
        }

        public async Task<MembershipResult> RemoveLoginFromUserAsync(Guid userId, string provider, string id)
        {
            var user = await FindUserByUniqueIdAsync(userId);

            if (user == null)
            {
                return MembershipResult.Failed("User not found.");
            }

            var login = user.Logins?.Where(o =>
               o.LoginProvider.Equals(provider, StringComparison.OrdinalIgnoreCase)
               &&
               o.ProviderUniqueId.Equals(id, StringComparison.OrdinalIgnoreCase)
            )
            .FirstOrDefault();

            if (login == null) // is this OK?
            {
                return MembershipResult.Success;
            }

            return await UserStore.DeleteLoginFromUserAsync(login, user);
        }

        //TODO: better
        public async Task<bool> DoesUserHaveAPasswordAsync(Guid userId)
        {
            var user = await _UserStore.FindByIdUniqueIdAsync(userId);
            return user != null && user.PasswordHash != null;
        }

        public async Task<MembershipResult> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                throw new ArgumentNullException(nameof(oldPassword));
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentNullException(nameof(newPassword));
            }

            var user = await FindUserByUniqueIdAsync(userId);

            if (user == null)
            {
                return MembershipResult.Failed(IdentityCodes.UserNotFound, "An error occured."); // Do not reveal what the problem was.
            }

            if (user.PasswordHash == null)
            {
                return MembershipResult.Failed(IdentityCodes.NoPasswordSet, "You do not have a password set.");
            }

            if (!IsPasswordCorrect(user, oldPassword))
            {
                return MembershipResult.Failed(IdentityCodes.IncorrectUsernameOrPassword, "Old password is incorrect.");
            }

            if (!user.EmailConfirmed) // it's possible to verify this before
            {
                return MembershipResult.Failed(IdentityCodes.IncorrectUsernameOrPassword, "Incorrect username or password."); // can we reveal this?
            }

            // reset security stamp to reset old sessions
            ResetSecurityStamp(user);

            // set password
            user.PasswordHash = PasswordProvider.HashPassword(newPassword);

            // Save
            return await UpdateInner(user);
        }

        public async Task<MembershipResult> VerifyUserCredentialsAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            var user = await FindUserByUserNameAsync(userName);

            if (user == null)
            {
                return MembershipResult.Failed(IdentityCodes.IncorrectUsernameOrPassword, "Incorrect username or password.");
            }

            if (user.PasswordHash == null)
            {
                return MembershipResult.Failed(IdentityCodes.IncorrectUsernameOrPassword, "Incorrect username or password.");
            }

            if (!user.EmailConfirmed) // it's possible to verify this before
            {
                return MembershipResult.Failed(IdentityCodes.IncorrectUsernameOrPassword, "Incorrect username or password."); // can we reveal this?
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                return MembershipResult.Failed(IdentityCodes.UserLockedOut, "Your account is currently not active. Please contact us for more info.");
            }

            if (!IsPasswordCorrect(user, password))
            {
                await IncrementFailedCountAndSave(user);
                return MembershipResult.Failed(IdentityCodes.IncorrectUsernameOrPassword, "Incorrect username or password.");
            }

            await ResetFailedCountAndSave(user);
            return MembershipResult.Success;
        }

        private bool IsPasswordCorrect(TUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.PasswordHash == null)
            {
                throw new ArgumentException("User does not have Hashed password set.", nameof(user.PasswordHash));
            }

            if (password == null) 
            {
                throw new ArgumentNullException(nameof(password));
            }

            return PasswordProvider.VerifyPassword(user.PasswordHash, password);
        }

        public virtual Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UniqueId.ToString())
            };

            AppendRolesIsPOssible(user, claims);

            // create the identity.
            var identity = new ClaimsIdentity(claims, authenticationType);

            // return the result
            return Task.FromResult(identity);
        }

        private static void AppendRolesIsPOssible(TUser user, List<Claim> claims)
        {
            if (user.Roles == null)
            {
                return;
            }

            foreach (var item in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item.Name));
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task ResetFailedCountAndSave(TUser user)
        {
            user.LoginFailedCount = 0;
            await UpdateInner(user);
        }

        private async Task IncrementFailedCountAndSave(TUser user)
        {
            user.LoginFailedCount++;
            await UpdateInner(user);
        }

        private void ResetSecurityStamp(TUser user)
        {
            user.SecurityStamp = Guid.NewGuid().ToString();
        }

        #endregion

        #region Private Save Methods

        private async Task<MembershipResult> UpdateInner(TUser user)
        {
            return await _UserStore.UpdateAsync(user);
        }

        private async Task<MembershipResult> InsertInner(TUser user)
        {
            // set security stamp
            ResetSecurityStamp(user);

            // do it inner
            return await _UserStore.CreateAsync(user);
        }

        #endregion
    }
}
