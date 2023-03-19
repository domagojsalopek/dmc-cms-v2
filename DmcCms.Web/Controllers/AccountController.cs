using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Identity;
using Dmc.Cms.Web.Attributes;
using Dmc.Cms.Web.Mappers;
using Dmc.Cms.Web.ViewModels;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace Dmc.Cms.Web.Controllers
{
    [Authorize]
    [NoCache]
    public class AccountController : ControllerBase
    {
        #region Fields

        private readonly IPostService _PostService;
        private readonly int _SendConfirmEmailCodeEveryXMinutes = 20;

        #endregion

        #region Constructors

        public AccountController(IPostService postService, ApplicationUserManager manager) : base(manager)
        {
            _PostService = postService ?? throw new ArgumentNullException(nameof(postService));
        }

        #endregion

        #region Submenu Helper

        public async Task<PartialViewResult> UserMenu()
        {
            Guid? userId = GetLoggedInUserId();
            if (!userId.HasValue)
            {
                return PartialView("Partials/_NoAdsPartial");
            }

            string cacheKey = string.Format("CustomerInfo_{0}", userId.Value);
            AccountDetailsViewModel accountDetailsViewModelFromCache = CacheHelper.GetFromCache<AccountDetailsViewModel>(cacheKey);
            if (accountDetailsViewModelFromCache != null)
            {
                return PartialView("Partials/_AccountPageSubmenuPartial", accountDetailsViewModelFromCache);
            }

            var user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return PartialView("Partials/_NoAdsPartial");
            }

            var viewModel = new AccountDetailsViewModel();
            TransferBasicUserInfoToViewModel(user, viewModel);
            CacheHelper.AddToCache(cacheKey, viewModel, DateTimeOffset.UtcNow.AddMinutes(60));

            return PartialView("Partials/_AccountPageSubmenuPartial", viewModel);
        }

        private void RemoveUserInfoFromCache(Guid? userId)
        {
            if (!userId.HasValue)
            {
                return;
            }

            string cacheKey = string.Format("CustomerInfo_{0}", userId.Value);
            CacheHelper.RemoveFromCache(cacheKey);
        }

        #endregion

        #region Account Details

        [AllowAnonymous] // it could be used after seesion expired
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> ResendConfirmationEmail()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "You must be logged in.", redirect = Url.Action(nameof(Login), "Account") });
            }

            User user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return Json(new { success = false, message = "You must be logged in.", redirect = Url.Action(nameof(Login), "Account") });
            }

            if (user.EmailConfirmed) // nothing to do
            {
                return Json(new { success = true, message = "E-mail already confirmed." });
            }

            if (!SendEmailConfirmationCode(user))
            {
                return Json(new { success = false, message = "There was an error sending e-mail. Please try again later." });
            }

            return Json(new { success = true, message = string.Format("Confirmation e-mail was sent. Please check your inbox and Spam folders. Link will expire in {0} hours.", UserManager.EmailTokenDuration.TotalHours) });
        }

        public async Task<ActionResult> Index(UserAccountMessageType? messageCode = null)
        {
            Guid? userId = GetLoggedInUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction(nameof(Login));
            }

            User loggedInUser = await UserManager.GetUserWithAllDetailsAsync(userId.Value);
            if (loggedInUser == null)
            {
                return RedirectToAction(nameof(Login));
            }

            int[] favourteIds = loggedInUser.FavouritePosts.Select(o => o.Id).Distinct().ToArray();
            int[] ratingIds = loggedInUser.Ratings.Select(o => o.PostId).Distinct().ToArray();
            int[] commentPostIds = loggedInUser.Comments.Select(o => o.PostId).Distinct().ToArray();

            int[] allIds = favourteIds
                .Concat(commentPostIds)
                .Concat(ratingIds)
                .Distinct()
                .ToArray();

            IEnumerable<Post> allPostsForAllIds = await _PostService.GetAllByPostIdsAsync(allIds);

            var viewModel = new AccountDetailsViewModel();
            TransferInfoToAccountDetailsViewModel(loggedInUser, allPostsForAllIds, viewModel, favourteIds, ratingIds, commentPostIds);

            AppendUserAccountMessageIfNeeded(messageCode);

            return View(viewModel);
        }

        private void AppendUserAccountMessageIfNeeded(UserAccountMessageType? errorCode)
        {
            if (!errorCode.HasValue)
            {
                return;
            }

            switch (errorCode.Value)
            {
                case UserAccountMessageType.ExternalLoginIdAlreadyUsed:
                    AddMessageToViewData(MessageType.Error, "Cannot associate this Social Media login.");
                    AddMessageToViewData(MessageType.Info, "This account is already associated with a different user. You can login with your other account and remove this Social Media login or delete the other account.");
                    break;

                case UserAccountMessageType.ExternalLoginAdded:
                    AddMessageToViewData(MessageType.Success, "External Login is now associated. You may use it to login in the future.");
                    break;

                default:
                    break;
            }
        }

        //TODO: with mapper
        private void TransferInfoToAccountDetailsViewModel(User loggedInUser, IEnumerable<Post> allPostsForAllIds, AccountDetailsViewModel viewModel, int[] favouteIds, int[] ratingIds, int[] commentIds)
        {
            // basic stuff
            TransferBasicUserInfoToViewModel(loggedInUser, viewModel);

            // Crete lists from all ...
            IEnumerable<Post> favourites = allPostsForAllIds.Where(o => favouteIds.Contains(o.Id));
            IEnumerable<Post> ratedPosts = allPostsForAllIds.Where(o => ratingIds.Contains(o.Id));
            IEnumerable<Post> commentPosts = allPostsForAllIds.Where(o => commentIds.Contains(o.Id));

            viewModel.FavouritePosts = PreparePostsViewModelList(viewModel, favourites, loggedInUser.Ratings, favourites); // for favourite every will be, for rated, every will have ... 
            viewModel.RatedPosts = PreparePostsViewModelList(viewModel, ratedPosts, loggedInUser.Ratings, favourites);
            viewModel.Comments = PrepareCommentsViewModel(viewModel, loggedInUser.Comments, commentPosts);
        }

        private List<UserCommentViewModel> PrepareCommentsViewModel(AccountDetailsViewModel viewModel, ICollection<Comment> comments, IEnumerable<Post> commentPosts)
        {
            List<UserCommentViewModel> userCommentViewModels = new List<UserCommentViewModel>();

            foreach (Comment comment in comments)
            {
                UserCommentViewModel userComment = CreateUserCommentViewModel(
                    comment,
                    commentPosts.FirstOrDefault(o => o.Id == comment.PostId));

                if (userComment != null)
                {
                    userCommentViewModels.Add(userComment);
                }
            }

            return userCommentViewModels
                .OrderByDescending(o => o.DateCreated)
                .ToList();
        }

        private UserCommentViewModel CreateUserCommentViewModel(Comment comment, Post post)
        {
            return new UserCommentViewModel
            {
                Author = comment.Author,
                CommentId = comment.Id,
                DateCreated = comment.Created,
                PostSlug = post?.Slug,
                ReplyToAuthor = comment.Parent?.Author,
                ReplyToId = comment.ParentId,
                ReplyToText = comment.Parent?.Text, // we need this?
                Text = comment.Text
            };
        }

        private static void TransferBasicUserInfoToViewModel(User loggedInUser, AccountDetailsViewModel viewModel)
        {
            // basic things ... 
            viewModel.Email = loggedInUser.Email;
            viewModel.FirstName = loggedInUser.FirstName;
            viewModel.LastName = loggedInUser.LastName;
            viewModel.NickName = loggedInUser.NickName;
            viewModel.MemberSince = loggedInUser.Created;

            // external login & membership things
            viewModel.EmailConfirmed = loggedInUser.EmailConfirmed;
            viewModel.HasPassword = loggedInUser.HasPassword;
            viewModel.HasExternalLogins = loggedInUser.Logins.Count > 0;
            viewModel.ExternalLogins = loggedInUser.Logins
                .Select(o => o.LoginProvider)
                .ToList();
        }

        private List<PostViewModel> PreparePostsViewModelList(AccountDetailsViewModel viewModel, IEnumerable<Post> posts, IEnumerable<Rating> ratings, IEnumerable<Post> favourites)
        {
            List<PostViewModel> favouritePostViewModel = new List<PostViewModel>();
            foreach (var item in posts)
            {
                PostViewModel postViewModel = new PostViewModel();

                PostMapper.TransferToViewModel(item, postViewModel);
                AppendRatingAndFavouriteInfo(postViewModel, ratings, favourites);

                favouritePostViewModel.Add(postViewModel);
            }
            return favouritePostViewModel;
        }

        private void AppendRatingAndFavouriteInfo(PostViewModel item, IEnumerable<Rating> ratingsForCurrentUserForThesePosts, IEnumerable<Post> favouritePosts)
        {
            Rating userRatingForTost = ratingsForCurrentUserForThesePosts.FirstOrDefault(o => o.PostId == item.Id);

            if (userRatingForTost != null)
            {
                item.HasRating = true;
                item.Liked = userRatingForTost.IsLike;
            }

            item.IsFavourite = favouritePosts.Any(o => o.Id == item.Id);
        }

        #endregion

        #region Delete Account

        public async Task<ActionResult> Delete()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await GetLoggedInUserAsync();
            
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var viewModel = new AccountDeleteViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MemberSince = user.Created,
                NickName = user.NickName,
                Token = UserManager.CreateDeleteAccountCode(user)
            };

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(AccountDeleteViewModel model)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return View(model);
            }

            var user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await UserManager.DeleteUserAsync(user, model.Token);

            if (!result.Succeeded)
            {
                // regenerate code
                model.Token = UserManager.CreateDeleteAccountCode(user);

                // add error messages
                AddErrorMessagesToViewData(result.Errors);

                // retry
                return View(model);
            }

            RemoveUserInfoFromCache(user.UniqueId);
            IdentitySignout();
            AddMessageToTempData(MessageType.Success, "Your account has been removed successfully. We are sad to see you go. Please remember that you can always create a new account.");
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region Login

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl, UserAccountMessageType? errorCode = null)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            // TODO: Special merge FB and local account permision page

            AppendLoginMessageBasedOnCodeIfNeeded(errorCode);
            ViewBag.ReturnUrl = ReturnUrl;
            return View(new UserLoginViewModel());
        }

        private void AppendLoginMessageBasedOnCodeIfNeeded(UserAccountMessageType? errorCode)
        {
            if (!errorCode.HasValue)
            {
                return;
            }

            switch (errorCode.Value)
            {
                case UserAccountMessageType.ExternalLoginEmailAlreadyUsed:
                    AddMessageToViewData(MessageType.Warning, "Looks like you already have an account here.");
                    AddMessageToViewData(MessageType.Info, "If you wish, you can merge an existing account with your social media account. If you forgot your password, you can request a new one.");
                    break;

                default:
                    AddMessageToViewData(MessageType.Error, "An unknown error occured.");
                    break;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserLoginViewModel model, string ReturnUrl)
        {
            if (Request.IsAuthenticated)
            {
                IdentitySignout();
                AddMessageToTempData(MessageType.Warning, "Session has expired. Please login again.");
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return View(model);
            }

            User user = await UserManager.FindUserByUserNameAsync(model.Email);

            if (user == null || !user.HasPassword)
            {
                AddMessageToViewData(MessageType.Error, "Incorrect E-mail or Password. Please Try again.");
                return View(model);
            }

            // TODO: Helper method to verify using user. that way we don't have to go to DB twice
            var result = await UserManager.VerifyUserCredentialsAsync(model.Email, model.Password);

            if (!result.Succeeded)
            {
                AddErrorMessagesToViewData(result.Errors);
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                SendEmailConfirmationCode(user);

                AddMessageToViewData(MessageType.Warning, "You need to verify your e-mail address before you can log in.");
                AddMessageToViewData(MessageType.Info, "We have sent a confirmation e-mail to you, please follow the link to activate your account.");

                return View(model);
            }

            ClaimsIdentity identity = await UserManager.CreateIdentityAsync(user, IdentityAuthenticationTypes.ApplicationCookie);
            IdentitySignin(identity, model.RememberMe);
            RemoveUserInfoFromCache(user.UniqueId);

            return RedirectToLocal(ReturnUrl ?? "/");
        }

        #endregion

        #region Forgot Password

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (Request.IsAuthenticated) // should not come here if already logged in
            {
                IdentitySignout();
                return RedirectToAction(nameof(ForgotPassword));
            }
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return View(model);
            }

            var user = await UserManager.FindUserByEmailAsync(model.Email);

            if (user == null || !user.EmailConfirmed || UserManager.IsLockedOut(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Send
            SendEmailResetCode(user);

            // If we got this far, something failed, redisplay form
            return View(nameof(ForgotPasswordConfirmation));
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            if (Request.IsAuthenticated)
            {
                IdentitySignout();
                AddMessageToTempData(MessageType.Warning, "Session has expired. Please login again.");
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        #endregion

        #region Reset Password

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(Guid? userId, string code)
        {
            if (!userId.HasValue || string.IsNullOrWhiteSpace(code))
            {
                AddErrorMessagesToTempData(new List<string> { "An error occured.", "Please verify that you clicked the link in the E-mail you received.", "You can also copy/paste the reset link in your browser Address bar." });
                return Error();
            }

            if (!(await UserManager.IsResetPasswordCodeCorrectAsync(userId.Value, code)))
            {
                AddErrorMessagesToTempData(new List<string> { "An error occured.", "Please verify that you clicked the link in the E-mail you received.", "You can also copy/paste the reset link in your browser Address bar." });
                return Error();
            }

            if (Request.IsAuthenticated)
            {
                IdentitySignout();
                RemoveUserInfoFromCache(userId);
            }

            ResetPasswordViewModel viewModel = new ResetPasswordViewModel
            {
                Code = code
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (Request.IsAuthenticated)
            {
                IdentitySignout();
                AddMessageToTempData(MessageType.Warning, "Session has expired. Please login again.");
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return View(model);
            }

            var user = await UserManager.FindUserByEmailAsync(model.Email);

            if (user == null || !user.EmailConfirmed)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                LoggedInUserInfo userInfo = new LoggedInUserInfo();
                userInfo.FromUser(user);
                IdentitySignin(userInfo, false);
                AddMessageToTempData(MessageType.Success, "Your password has been changed. Please keep your password safe.");
                RemoveUserInfoFromCache(user.UniqueId);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            AddErrorMessagesToTempData(result.Errors);
            return View(model);
        }

        #endregion

        #region Change Nickname

        public async Task<ActionResult> ChangeNickname()
        {
            User user = await GetLoggedInUserAsync();
            if (user == null)
            {
                return Error();
            }

            return View(new ChangeNicknameViewModel
            {
                Nickname = user.NickName,
                CurrentNickname = user.NickName
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeNickname(ChangeNicknameViewModel viewModel)
        {
            User user = await GetLoggedInUserAsync();
            if (user == null)
            {
                return Error();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // on this site it doesn't have to be unique. it's simply a display name
            user.NickName = viewModel.Nickname;

            MembershipResult updateNicknameResult = await UserManager.UserStore.UpdateAsync(user);

            if (!updateNicknameResult.Succeeded)
            {
                AddMessageToViewData(MessageType.Error, "Oops. It looks like we're having some issues. There was nothing wrong with your input. Please try again.");
                return View(viewModel);
            }

            AddMessageToTempData(MessageType.Success, "Your nickname was saved successfully.");
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Add Password

        public async Task<ActionResult> AddPassword()
        {
            Guid? userId = GetLoggedInUserId();

            if (!userId.HasValue)
            {
                return Error();
            }

            User user = await UserManager.FindUserByUniqueIdAsync(userId.Value);
            if (user == null)
            {
                return Error();
            }

            if (user.HasPassword)
            {
                AddMessageToTempData(MessageType.Info, "You already have a password. You may change it using the form below.");
                return RedirectToAction(nameof(ChangePassword));
            }

            if (!user.EmailConfirmed)
            {
                AddMessageToViewData(MessageType.Warning, "You will need to verify your e-mail address before you can log in with your new password.");
            }

            return View(new AddPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return View(model);
            }

            User user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return Error();
            }

            if (user.HasPassword)
            {
                AddMessageToTempData(MessageType.Info, "You already have a password. You may change it using the form below.");
                return RedirectToAction(nameof(ChangePassword));
            }

            MembershipResult result = await UserManager.AddPasswordToUserAsync(user, model.Password);

            // reset info cache because it's possible something changed
            RemoveUserInfoFromCache(user.UniqueId);

            if (!result.Succeeded)
            {
                AddErrorMessagesToViewData(result.Errors);
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                SendEmailConfirmationCode(user);
                AddMessageToTempData(MessageType.Success, "You created a password.");
                AddMessageToViewData(MessageType.Info, "You will need to verify your e-mail address before you can log in with your new password.");

                return RedirectToAction(nameof(Index));
            }

            // currently we'll never be here as social logins are treated as not confirmed by default.
            AddMessageToTempData(MessageType.Success, "You have created a password. You will be able to use your e-mail and this password to login.");
            IdentitySignout();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Change Password

        public async Task<ActionResult> ChangePassword()
        {
            Guid? userId = GetLoggedInUserId();

            if (!userId.HasValue)
            {
                return Error();
            }

            User user = await GetLoggedInUserAsync();

            if (!user.HasPassword)
            {
                AddMessageToTempData(MessageType.Info, "You do not have a password. You can add a password to your account by using the form below.");
                return RedirectToAction(nameof(AddPassword));
            }

            if (!user.EmailConfirmed)
            {
                SendEmailConfirmationCode(user);

                AddMessageToViewData(MessageType.Warning, "You need to verify your e-mail address before you can log in.");
                AddMessageToViewData(MessageType.Info, "We have sent an activation link to you. Please verify your e-mail.");

                return Error();
            }
            
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return View(model);
            }

            Guid? userId = GetLoggedInUserId();

            if (!userId.HasValue)
            {
                return Error();
            }

            if (!await UserManager.DoesUserHaveAPasswordAsync(userId.Value))
            {
                AddMessageToTempData(MessageType.Info, "You do not have a password. You can add a password to your account by using the form below.");
                return RedirectToAction(nameof(AddPassword));
            }

            MembershipResult result = await UserManager.ChangePasswordAsync(userId.Value, model.OldPassword, model.NewPassword);

            // reset info cache because it's possible something changed
            RemoveUserInfoFromCache(userId);

            if (!result.Succeeded)
            {
                AddErrorMessagesToViewData(result.Errors);
                return View(model);
            }

            IdentitySignout();
            AddMessageToTempData(MessageType.Success, "You have changed your password. Please Login using your new password.");
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new UserRegisterViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UserRegisterViewModel model)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                ModelState.AddModelError("", "Validation Has failed. Please correct the errors and try again.");
                return View(model);
            }

            User user = new User
            {
                Email = model.Email,
                UserName = model.Email
            };

            MembershipResult result = await UserManager.CreateUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                SendEmailConfirmationCode(user);
                return RedirectToAction(nameof(RegisterSuccess));
            }

            AddToModelStateMessagesFromMembershipResult(result);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterSuccess()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        #endregion

        #region Confirm E-mail

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(Guid? userId, string code, string ReturnUrl)
        {
            if (Request.IsAuthenticated && !SuppliedUserIdMatchesLoggedInUser(userId))
            {
                return RedirectToAction(nameof(Index));
            }

            if (!userId.HasValue || string.IsNullOrWhiteSpace(code))
            {
                AddErrorMessagesToTempData(new List<string> { "An error occured.", "Please verify that you clicked the link in the E-mail you received." });
                return RedirectToAction(nameof(Login));
            }

            var result = await UserManager.VerifyEmailAsync(userId.Value, code);

            // reset info cache because it's possible something changed
            RemoveUserInfoFromCache(userId);

            if (result.Succeeded)
            {
                AddMessageToTempData(MessageType.Success, "Thank you for confirming your e-mail address.");

                if (!string.IsNullOrWhiteSpace(ReturnUrl))
                {
                    return RedirectToLocal(ReturnUrl);
                }

                return RedirectToAction(nameof(Login));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View("Error");
        }

        private bool SuppliedUserIdMatchesLoggedInUser(Guid? userId)
        {
            if (!userId.HasValue)
            {
                return false;
            }

            return userId.Value == GetLoggedInUserId();
        }

        #endregion

        #region Logoff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LogOff()
        {
            IdentitySignout();
            return Json(new { success = true });
        }

        #endregion

        #region External Login Specific Methods

        [AllowAnonymous]
        public ActionResult PermissionsMissing(string provider, string ReturnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            AddMessageToViewData(MessageType.Warning, "Ooops. It looks like you did not give us all the necessary permissions.");
            AddMessageToViewData(MessageType.Info, "We need your E-mail address in order to be able to register you for the site.");

            return View();
        }

        public async Task<ActionResult> ManageExternalLogins(string ReturnUrl)
        {
            User user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return Error();
            }

            AssociateExternalLoginViewModel viewModel = CreateAssociateViewModel(ReturnUrl, user);
            return View(viewModel);
        }

        private static AssociateExternalLoginViewModel CreateAssociateViewModel(string ReturnUrl, User user)
        {
            List<string> alreadyAssociated = user.Logins
                            .Select(s => s.LoginProvider)
                            .ToList();

            List<string> supportedIds = new List<string>(ExternalLoginSettings.SupportedExternalLoginIds)
                .Except(alreadyAssociated, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var viewModel = new AssociateExternalLoginViewModel
            {
                ReturnUrl = ReturnUrl,
                AvailableProviders = supportedIds,
                AlreadyAssociatedProviders = alreadyAssociated,
                EmailConfirmed = user.EmailConfirmed,
                HasPassword = user.HasPassword
            };
            return viewModel;
        }

        public async Task<ActionResult> AssociateExternalLogin(string ReturnUrl)
        {
            User user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return Error();
            }

            AssociateExternalLoginViewModel viewModel = CreateAssociateViewModel(ReturnUrl, user);
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AssociateExternalLogin(ExternalLoginViewModel externalLogin)
        {
            if (!Request.IsAjaxRequest())
            {
                return Json(new { success = false, message = "Ajax, Please." });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Not all required data was supplied. Please try again." });
            }

            User currentUser = await GetLoggedInUserAsync();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "An error occured. Please try again." });
            }

            if (IsProviderAlreadyAssociated(currentUser, externalLogin.ExternalLoginProvider, externalLogin.Id))
            {
                return Json(new { success = true, redirect = Url.Action(nameof(Index), "Account", new { messageCode = UserAccountMessageType.ExternalLoginAdded }) });
            }

            User existing = await UserManager.GetByExternalLoginAsync(externalLogin.ExternalLoginProvider, externalLogin.Id);
            if (existing != null)
            {
                return Json(new
                {
                    success = false,
                    message = "External login was already associated.",
                    redirect = Url.Action(nameof(Index), "Account", new
                    {
                        messageCode = UserAccountMessageType.ExternalLoginIdAlreadyUsed
                    }),
                });
            }

            return await AddExternalLoginToCurrentUser(currentUser, externalLogin);
        }

        private async Task<ActionResult> AddExternalLoginToCurrentUser(User currentUser, ExternalLoginViewModel externalLogin)
        {
            var login = new IdentityLogin
            {
                LoginProvider = externalLogin.ExternalLoginProvider.ToLowerInvariant(),
                ProviderUniqueId = externalLogin.Id,
                ScreenName = externalLogin.ScreenName
            };

            // must use this overload where no password is demanded.
            MembershipResult result = await UserManager.AddLoginToUserAsync(currentUser, login);

            // reset info cache because it's possible something changed
            RemoveUserInfoFromCache(currentUser.UniqueId);

            if (!result.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occured. Please try again."
                });
            }

            return Json(new
            {
                allowed = true,
                success = true,
                redirect = Url.Action(nameof(Index), "Account", new { messageCode = UserAccountMessageType.ExternalLoginAdded })
            });
        }

        private bool IsProviderSupported(string provider)
        {
            return ExternalLoginSettings.SupportedExternalLoginIds.Any(s => s.Equals(provider, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsProviderAlreadyAssociated(User user, string provider, string providerId = null)
        {
            if (!string.IsNullOrWhiteSpace(providerId))
            {
                return user.Logins.Any(o => 
                    o.LoginProvider.Equals(provider, StringComparison.OrdinalIgnoreCase)
                    &&
                    o.ProviderUniqueId.Equals(providerId, StringComparison.OrdinalIgnoreCase)
                );
            }

            return user.Logins.Any(o => o.LoginProvider.Equals(provider, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ActionResult> DisassociateExternalLogin(string provider)
        {
            bool providerSent = !string.IsNullOrWhiteSpace(provider);
            User user = await GetLoggedInUserAsync();

            if (user == null || !providerSent)
            {
                return Error();
            }

            if (!IsProviderSupported(provider))
            {
                return Error();
            }

            if (!user.HasPassword || !user.EmailConfirmed) // will not see button anyway
            {
                AddMessageToTempData(MessageType.Error, "You cannot disassociate external login at this time.");
                AddMessageToTempData(MessageType.Info, "You need to create a password and have a confirmed e-mail address to be able to disassociate an external account, otherwise you would have no means of logging in.");
                return RedirectToAction(nameof(AddPassword));
            }

            IdentityLogin identityLogin = user.Logins
                .FirstOrDefault(o => o.LoginProvider == provider);

            if (identityLogin == null)
            {
                AddMessageToTempData(MessageType.Warning, "External login was already removed earlier. You can still use your credentials to login. If you wish you can associate your account again.");
                return RedirectToAction(nameof(Index));
            }

            ExternalLoginViewModel model = new ExternalLoginViewModel
            {
                Id = identityLogin.ProviderUniqueId,
                ExternalLoginProvider = identityLogin.LoginProvider,
                ScreenName = identityLogin.ScreenName
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> DisassociateExternalLogin(ExternalLoginViewModel model)
        {
            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                ModelState.AddModelError("", "Validation Has failed. Please correct the errors and try again.");
                return View(model);
            }

            User user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return Error();
            }

            if (!user.HasPassword || !user.EmailConfirmed) // will not see button anyway
            {
                AddMessageToTempData(MessageType.Error, "You cannot disassociate external login at this time.");
                AddMessageToTempData(MessageType.Info, "You need to create a password and have a confirmed e-mail address to be able to disassociate an external account, otherwise you would have no means of logging in.");
                return RedirectToAction(nameof(AddPassword));
            }

            // reset info cache because it's possible something changed
            RemoveUserInfoFromCache(user.UniqueId);

            if (!await RemoveExternalLoginFromCurrentUser(user, model))
            {
                AddMessageToViewData(MessageType.Error, "An error occured. Please try again.");
                return View(model);
            }

            AddMessageToTempData(MessageType.Success, "External login removed. You can still use your credentials to login. If you wish you can associate your account again.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RemoveExternalLoginFromCurrentUser(User user, ExternalLoginViewModel model)
        {
            MembershipResult membershipResult = await UserManager.RemoveLoginFromUserAsync(user.UniqueId, model.ExternalLoginProvider, model.Id);
            return membershipResult.Succeeded;
        }

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> ExternalLogin(ExternalLoginViewModel externalLogin)
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }

            // this is not the endpoint for connecting your account to fb
            if (Request.IsAuthenticated) 
            {
                return Json(new { success = true });
            }

            if (!ModelState.IsValid) 
            {
                // we don't explicitly say what's wrong as we expect the frontend to supply everything correctly.
                return Json(new { success = false, message = "Not all required data was supplied. Please try again." });
            }

            // maybe was logged in-before
            User userToLogin = await UserManager.GetByExternalLoginAsync(externalLogin.ExternalLoginProvider, externalLogin.Id);
            if (userToLogin != null)
            {
                return await LoginAndReturnSuccess(userToLogin, externalLogin.ReturnUrl);
            }

            // here no user was found. need to create a new user. now we definitely need email.
            if (string.IsNullOrWhiteSpace(externalLogin.Email))
            {
                return Json(new
                {
                    success = false,
                    message = "E-mail address is required.",
                    redirect = Url.Action(nameof(PermissionsMissing), "Account", new
                    {
                        externalLogin.ReturnUrl,
                        provider = externalLogin.ExternalLoginProvider
                    }),
                });
            }

            // if email is already used we redirect to login with a message they can associate their account.
            User userByEmail = await UserManager.FindUserByEmailAsync(externalLogin.Email);
            if (userByEmail != null)
            {
                return Json(new {
                    success = false,
                    message = "E-mail address is already used.",
                    redirect = Url.Action(nameof(Login), "Account", new {
                        ReturnUrl = Url.Action(nameof(ManageExternalLogins), "Account", new { externalLogin.ReturnUrl }),
                        errorCode = UserAccountMessageType.ExternalLoginEmailAlreadyUsed
                    }),
                });
            }

            return await CreateAndSaveNewExternalLogin(externalLogin);
        }

        private async Task<JsonResult> CreateAndSaveNewExternalLogin(ExternalLoginViewModel externalLogin)
        {
            User user = new User
            {
                FirstName = externalLogin.FirstName,
                LastName = externalLogin.LastName,
                Email = externalLogin.Email,
                UserName = externalLogin.Email, 

                // facebook says to require additional verification
                EmailConfirmed = false

                //EmailConfirmed = true // we can do this because external provider confirmed it for us. FB requires valid email
            };

            user.Logins.Add(new IdentityLogin
            {
                LoginProvider = externalLogin.ExternalLoginProvider.ToLowerInvariant(),
                ProviderUniqueId = externalLogin.Id,
                ScreenName = externalLogin.ScreenName // not exactly it
            });

            // must use this overload where no password is demanded.
            MembershipResult result = await UserManager.CreateUserAsync(user);

            // reset info cache because it's possible something changed
            RemoveUserInfoFromCache(user.UniqueId);

            if (!result.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occured. Please try again."
                });
            }

            // should we do this here?
            SendEmailConfirmationCode(user, externalLogin.ReturnUrl);
            return await LoginAndReturnSuccess(user, externalLogin.ReturnUrl, true);
        }

        private async Task<JsonResult> LoginAndReturnSuccess(User userToLogin, string returnUrl, bool mailWasSent = false)
        {
            ClaimsIdentity identity = await UserManager.CreateIdentityAsync(userToLogin, IdentityAuthenticationTypes.ApplicationCookie);
            IdentitySignin(identity, true);

            return Json(new
            {
                allowed = true,
                success = true,
                redirect = !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null,
                message = mailWasSent 
                    ? string.Format("Please check your e-mail {0} to confirm your account.", userToLogin.Email)
                    : null
            });
        }

        #endregion

        #region Child Actions

        [AllowAnonymous]
        [ChildActionOnly]
        public PartialViewResult HeaderLogin()
        {
            return PartialView("Partials/_HeaderLoginPartial", new UserLoginViewModel());
        }

        #endregion

        #region Private Methods

        private bool SendEmailResetCode(User user)
        {
            string token = UserManager.CreatePasswordResetCode(user);
            string callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.UniqueId, code = token }, protocol: Request.Url.Scheme);
            ConfirmEmailViewModel viewModel = new ConfirmEmailViewModel
            {
                Link = callbackUrl
            };

            try
            {
                string html = this.RenderViewToString("~/Views/Email/ResetPassword.cshtml", viewModel);
                EmailClient client = CreateEmailClient();
                client.Send(user.Email, "Reset your password", html);

                return true;
            }
            catch (Exception) //TODO: Log
            {
                return false;
            }
        }

        private bool SendEmailConfirmationCode(User user, string returnUrl = null)
        {
            string cacheKey = string.Format("SendEmailConfirmationCode_{0}", user.Email);
            if (!string.IsNullOrWhiteSpace(CacheHelper.GetFromCache<string>(cacheKey)))
            {
                return true; // was already sent X minutes ago. no need to send it again.
            }

            string code = UserManager.CreateEmailVerificationCode(user);
            string callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.UniqueId, code = code, ReturnUrl = returnUrl }, protocol: Request.Url.Scheme);
            ConfirmEmailViewModel viewModel = new ConfirmEmailViewModel
            {
                Link = callbackUrl
            };

            try
            {
                string html = this.RenderViewToString("~/Views/Email/ConfirmEmail.cshtml", viewModel);
                EmailClient client = CreateEmailClient();
                client.Send(user.Email, "Confirm your account", html);

                // if we're here add to cache that we sent
                CacheHelper.AddToCache(cacheKey, user.UniqueId, DateTimeOffset.UtcNow.AddMinutes(_SendConfirmEmailCodeEveryXMinutes));

                // everything is ok
                return true;
            }
            catch (Exception) //TODO: Log
            {
                return false;
            }
        }

        #endregion

        #region Helpers

        // copied from identity ... 
        private const string XsrfKey = "XsrfId";

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}