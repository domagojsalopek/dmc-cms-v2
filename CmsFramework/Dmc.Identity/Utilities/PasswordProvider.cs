using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class PasswordProvider : IPasswordProvider
    {
        #region Constants

        const int SaltSize = 128 / 8; // 16 -> 128 bits
        const int HashSize = 256 / 8; // 32 -> 256 bits
        const int HashIterations = 10000;

        #endregion

        #region Constructors

        public PasswordProvider()
        {

        }

        #endregion

        #region Methods

        public byte[] HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            // Prepare salt
            byte[] salt = GenerateSalt();

            // Generate hash using salt
            byte[] hash = ComputeHash(salt, password);

            // Prepare destination byte Array.
            // Length is one for marker + salt size + hash size
            byte[] destinationArray = new byte[1 + SaltSize + HashSize];

            // append marker
            destinationArray[0] = byte.MinValue;

            // copy salt to array
            Buffer.BlockCopy(salt, 0, destinationArray, 1, SaltSize);

            // copy hash to array
            Buffer.BlockCopy(hash, 0, destinationArray, SaltSize + 1, HashSize);

            // return destination array
            return destinationArray;
        }

        public bool VerifyPassword(byte[] password, string plainTextPassword)
        {
            const int requiredLength = 1 + SaltSize + HashSize;

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (plainTextPassword == null)
            {
                throw new ArgumentNullException(nameof(plainTextPassword));
            }

            if (password.Length != requiredLength) // length must match
            {
                return false;
            }

            if (password[0] != byte.MinValue) // when hashing this value is used as marker. for now we only have this one.
            {
                return false;
            }

            // get salt from password
            byte[] salt = GetSaltFromPassword(password);

            // generate hash using extracted salt and plain text password
            byte[] generatedHash = ComputeHash(salt, plainTextPassword);

            // extract hash from password to compare
            byte[] expectedHash = ExtractHashFromPassword(password);

            // compare byte arrays
            return AreByteArraysEqual(generatedHash, expectedHash);
        }

        #endregion

        #region Private Methods

        private bool AreByteArraysEqual(byte[] generatedHash, byte[] expectedHash)
        {
            if (expectedHash.Length != generatedHash.Length)
            {
                return false;
            }

            for (int i = 0; i < expectedHash.Length; i++)
            {
                byte valueToCheck = generatedHash[i];
                byte valueToCompare = expectedHash[i];

                if (valueToCheck != valueToCompare)
                {
                    return false;
                }
            }

            return true;
        }

        private byte[] ExtractHashFromPassword(byte[] password)
        {
            byte[] passwordOnlyBytes = new byte[HashSize];

            Buffer.BlockCopy(password, 1 + SaltSize, passwordOnlyBytes, 0, HashSize);

            return passwordOnlyBytes;
        }

        private static byte[] GetSaltFromPassword(byte[] password)
        {
            byte[] salt = new byte[SaltSize];

            // copy from password to salt
            Buffer.BlockCopy(password, 1, salt, 0, SaltSize);

            return salt;
        }

        private static byte[] ComputeHash(byte[] salt, string password)
        {
            using (var rfcDeriveBytes = new Rfc2898DeriveBytes(password, salt, HashIterations))
            {
                return rfcDeriveBytes.GetBytes(HashSize);
            }
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        #endregion
    }
}
