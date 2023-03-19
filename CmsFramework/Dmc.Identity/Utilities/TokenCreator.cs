using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity.Utilities
{
    internal static class TokenCreator
    {
        const string Identity = "DmcApp";

        public static string Create<T, TKey>(T user, string purpose) 
            where T : IUser<TKey> 
            where TKey : IEquatable<TKey>
        {
            string dateTimeAsString = Convert.ToInt64(TimeStampUtilities.DateTimeToUnixTimestamp(DateTime.UtcNow)).ToString();
            string plainString = GenerateRawDataStringToEncrypt<T, TKey>(user, purpose, dateTimeAsString);
            string base64SignatureString = GenerateSignature(user.UniqueId, user.SecurityStamp, purpose, plainString);

            string finalStringToEncrypt = string.Format("{0}|{1}|{2}"
                , Identity
                , plainString
                , base64SignatureString);

            byte[] finalBytes = Encoding.UTF8.GetBytes(finalStringToEncrypt);
            byte[] encryptedBytes = ProtectedData.Protect(finalBytes, null, DataProtectionScope.LocalMachine);

            return EncodingUtilities.Base64UrlEncode(encryptedBytes);
        }

        internal static bool IsTokenValid<T, TKey>(T user, ParseTokenResult parsedToken, string purpose)
            where T : IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            if (string.IsNullOrWhiteSpace(parsedToken.SecurityStamp) || 
                !user.SecurityStamp.Equals(parsedToken.SecurityStamp, StringComparison.Ordinal))
            {
                return false;
            }

            if (parsedToken.UniqueId != user.UniqueId)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(parsedToken.Purpose) || 
                !purpose.Equals(parsedToken.Purpose, StringComparison.Ordinal))
            {
                return false;
            }

            // at the end validate signature
            string dateTimeAsString = Convert.ToInt64(TimeStampUtilities.DateTimeToUnixTimestamp(parsedToken.Created)).ToString();

            // ok to do this as we verified if it matches the token
            string plainString = GenerateRawDataStringToEncrypt<T, TKey>(user, parsedToken.Purpose, dateTimeAsString); 
            string serverGeneratedSignature = GenerateSignature(user.UniqueId, user.SecurityStamp, parsedToken.Purpose, plainString);

            string tokenSignature = Convert.ToBase64String(parsedToken.Signature);

            return !string.IsNullOrWhiteSpace(tokenSignature)
                && !string.IsNullOrWhiteSpace(serverGeneratedSignature)
                && tokenSignature.Equals(serverGeneratedSignature, StringComparison.Ordinal);
        }

        internal static bool TryParseToken(string input, out ParseTokenResult result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string identityPlainAndSignature = TryDecryptInputAndGetPlainText(input);

            if (string.IsNullOrWhiteSpace(identityPlainAndSignature))
            {
                return false;
            }

            string[] basicParts = identityPlainAndSignature.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (basicParts == null || basicParts.Length != 3) // identity, plainstring, signature
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(basicParts[0]) ||
                !Identity.Equals(basicParts[0], StringComparison.Ordinal)) // incorrect app.
            {
                return false;
            }

            return TryFillFromPartsInner(basicParts, out result);
        }

        #region Private Create Methods

        private static string GenerateRawDataStringToEncrypt<T, TKey>(T user, string purpose, string dateTimeAsString)
            where T : IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return string.Format("{0}.{1}.{2}.{3}"
                            , dateTimeAsString
                            , purpose
                            , user.UniqueId.ToString()
                            , user.SecurityStamp.ToString());
        }

        private static string GenerateSignature(Guid id, string securityStamp, string purpose, string finalString)
        {
            string text = string.Format("{0}{1}{2}{0}"
                , securityStamp.ToUpper()
                , id.ToString().ToUpper()
                , purpose);

            string stringToHash = string.Format("{0}.{1}"
                , text
                , finalString);

            byte[] bytes = Encoding.UTF8.GetBytes(stringToHash);
            using (SHA256Managed hashstring = new SHA256Managed())
            {
                byte[] hash = hashstring.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }                
        }

        #endregion

        #region Private Parse Methods

        private static bool TryFillFromPartsInner(string[] basicParts, out ParseTokenResult result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(basicParts[1]) || string.IsNullOrWhiteSpace(basicParts[2]))
            {
                return false;
            }

            byte[] signature = TryGetSignature(basicParts);
            if (signature == null)
            {
                return false;
            }

            string[] plainTextParts = basicParts[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (plainTextParts == null || plainTextParts.Length != 4) // datetime, purpose, uniqueid, security stamp
            {
                return false;
            }

            //DateTimeOffset created;
            //if (!DateTimeOffset.TryParse(plainTextParts[0], out created))
            //{
            //    return false;
            //}
            string timeStampAsString = plainTextParts[0];
            long timeStamp;
            if (!long.TryParse(timeStampAsString, out timeStamp))
            {
                return false;
            }

            DateTime created = TimeStampUtilities.UnixTimeStampToDateTimeUtc(timeStamp);

            Guid uniqueId;
            if (!Guid.TryParse(plainTextParts[2], out uniqueId))
            {
                return false;
            }

            // here we know that parts are correct.
            result = new ParseTokenResult();

            result.Created = created;
            result.UniqueId = uniqueId;
            result.Purpose = plainTextParts[1];
            result.SecurityStamp = plainTextParts[3];
            result.Signature = signature;

            return true;
        }

        private static byte[] TryGetSignature(string[] basicParts)
        {
            try
            {
                byte[] signature = Convert.FromBase64String(basicParts[2]);
                return signature;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string TryDecryptInputAndGetPlainText(string input)
        {
            try
            {
                byte[] encryptedBytes = EncodingUtilities.Base64UrlDecode(input);
                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.LocalMachine);
                string identityPlainAndSignature = Encoding.UTF8.GetString(decryptedBytes);
                return identityPlainAndSignature;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
