using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public static class IdentityCodes
    {
        public const string SavingError = "SavingError";
        public const string UsernameExists = "UsernameExists";
        public const string EmailExists = "EmailExists";
        public const string EmailNotConfirmed = "EmailNotConfirmed";
        public const string UserLockedOut = "UserLockedOut";
        public const string IncorrectUsernameOrPassword = "IncorrectUsernameOrPassword";
        public const string ResetPasswordCodeIncorrect = "ResetPasswordCodeIncorrect";
        public const string ResetPasswordCodeExpired = "ResetPasswordCodeExpired";
        public const string VerifyEmailCodeIncorrect = "VerifyEmailCodeIncorrect";
        public const string VerifyEmailCodeExpired = "VerifyEmailCodeExpired";
        public const string UserNotFound = "UserNotFound";
        public const string NoPasswordSet = "NoPasswordSet";

        // generic. we use specific below in case somebody wants to localize specifically
        public const string TokenIncorrect = "TokenIncorrect";
        public const string TokenExpired = "TokenExpired";
    }
}
