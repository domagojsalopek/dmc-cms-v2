using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Crypto
{
    public class AesCipher
    {
        #region Constants

        private const int BlockBitSize = 128;
        private const int KeyBitSize = 256;
        private const int Iterations = 10000;

        #endregion

        #region Fields

        private static readonly Encoding _Encoding = new UTF8Encoding(false, true);
        private readonly byte[] _Password;
        private readonly byte[] _Salt;

        #endregion

        #region Constructors

        public AesCipher(string password, string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null, empty or white-space only.", nameof(password));
            }

            if (string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentException("Salt cannot be null, empty or white-space only.", nameof(salt));
            }

            _Password = _Encoding.GetBytes(password);
            _Salt = _Encoding.GetBytes(salt);
        }

        #endregion

        #region Encrypt

        public string Encrypt(string message)
        {
            byte[] bytesToEncrypt = _Encoding.GetBytes(message);
            byte[] result = Encrypt(bytesToEncrypt);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string message)
        {
            byte[] toDecrypt = Convert.FromBase64String(message);
            byte[] result = Decrypt(toDecrypt);

            return _Encoding.GetString(result);
        }

        public byte[] Encrypt(byte[] message)
        {
            var key = GetKey();
            return Encrypt(message, key);
        }

        public byte[] Decrypt(byte[] message)
        {
            var key = GetKey();
            return Decrypt(message, key);
        }

        #endregion

        #region Private Methods

        private static byte[] Decrypt(byte[] message, byte[] key)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Length <= 0)
            {
                throw new ArgumentException("Message is required.", nameof(message));
            }

            if (key == null || key.Length != (KeyBitSize / 8))
            {
                throw new ArgumentException(string.Format("Key needs to be {0} bit!", KeyBitSize), nameof(key));
            }

            using (var aes = new AesManaged
            {
                KeySize = KeyBitSize,
                BlockSize = BlockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                // set the key
                aes.Key = key;

                // start
                using (MemoryStream memoryStream = new MemoryStream(message))
                {
                    byte[] iv = GetIVFromMemoryStream(memoryStream); // get IV from stream, as stream should start with IV. this method will also move stream to the necessary position.
                    aes.IV = iv; // set to provider

                    // Create a decrytor to perform the stream transform.
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        // next, read encrypted bytes to buffer
                        byte[] buffer = new byte[memoryStream.Length - memoryStream.Position];
                        memoryStream.Read(buffer, 0, buffer.Length);

                        byte[] transformedBytes = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                        return transformedBytes;
                    }
                }
            }
        }

        private static byte[] GetIVFromMemoryStream(MemoryStream memoryStream)
        {
            //TODO: Validate lengths

            byte[] sizeBuffer = new byte[sizeof(int)];
            memoryStream.Read(sizeBuffer, 0, sizeBuffer.Length);

            int IVLength = BitConverter.ToInt32(sizeBuffer, 0);
            byte[] IVBuffer = new byte[IVLength]; // should be 16

            memoryStream.Read(IVBuffer, 0, IVLength);
            return IVBuffer;
        }

        private static byte[] Encrypt(byte[] message, byte[] key)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Length <= 0)
            {
                throw new ArgumentException("Message is required.", nameof(message));
            }

            if (key == null || key.Length != (KeyBitSize / 8))
            {
                throw new ArgumentException(string.Format("Key needs to be {0} bit!", KeyBitSize), nameof(key));
            }

            byte[] cipherText;
            byte[] iv;

            using (var aes = new AesManaged
            {
                KeySize = KeyBitSize,
                BlockSize = BlockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                aes.Key = key;

                //Use random IV
                aes.GenerateIV();
                iv = aes.IV;

                // Create a encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(message, 0, message.Length);
                        csEncrypt.FlushFinalBlock();

                        // Read the memory stream and convert it back into byte array
                        msEncrypt.Position = 0;
                        cipherText = msEncrypt.ToArray();
                    }
                }
            }

            // we now have encrypted bytes. now we need to create new stream to append
            // IV first, and then encrypted bytes
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(BitConverter.GetBytes(iv.Length), 0, sizeof(int)); // append IV size & IV to stream. site should be 16 based on block size
                memoryStream.Write(iv, 0, iv.Length);

                // now also write encrypted stream
                memoryStream.Write(cipherText, 0, cipherText.Length);

                // reset position and return as array
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }

        private byte[] GetKey()
        {
            const int keySize = KeyBitSize / 8;
            using (var rfcDeriveBytes = new Rfc2898DeriveBytes(_Password, _Salt, Iterations))
            {
                return rfcDeriveBytes.GetBytes(keySize); // 32 bytes
            }
        }

        #endregion
    }
}
