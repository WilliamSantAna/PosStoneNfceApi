using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PosStoneNfce.API.Portal.App.Common.Services
{
    public static class SymmetricEncryptor
    {
        private static EncryptProperties BuildEncryptProperties(byte[] encryptedData, string password)
        {
            var encryptProperties = new EncryptProperties
            {
                CipherText = encryptedData
            };

            var authKeySalt = encryptedData.AsSpan(0, PasswordSaltByteSize).ToArray();

            var keySalt = encryptedData.AsSpan(PasswordSaltByteSize, PasswordSaltByteSize).ToArray();

            encryptProperties.IV = encryptedData.AsSpan(2 * PasswordSaltByteSize, AesBlockByteSize).ToArray();

            encryptProperties.SignatureTag = encryptedData.AsSpan(encryptedData.Length - SignatureByteSize, SignatureByteSize).ToArray();

            encryptProperties.CipherTextIndex = authKeySalt.Length + keySalt.Length + encryptProperties.IV.Length;
            encryptProperties.CipherTextLength = encryptedData.Length - encryptProperties.CipherTextIndex - encryptProperties.SignatureTag.Length;

            encryptProperties.AuthKey = GetKey(password, authKeySalt);
            encryptProperties.KeySalt = GetKey(password, keySalt);
            return encryptProperties;
        }

        private static Aes CreateAes()
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC; // Used CBC instead of ECB for better security
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }

        private static string DecryptData(EncryptProperties encryptProperties)
        {
            using var aes = CreateAes();
            using var encryptor = aes.CreateDecryptor(encryptProperties.KeySalt, encryptProperties.IV);

            var decryptedBytes = encryptor.TransformFinalBlock(
                encryptProperties.CipherText,
                encryptProperties.CipherTextIndex,
                encryptProperties.CipherTextLength);

            return StringEncoding.GetString(decryptedBytes);
        }

        private static EncryptProperties EncryptText(string toEncrypt, string password)
        {
            var encryptProperties = new EncryptProperties();

            encryptProperties.KeySalt = GenerateRandomBytes(PasswordSaltByteSize);
            var key = GetKey(password, encryptProperties.KeySalt);
            encryptProperties.IV = GenerateRandomBytes(AesBlockByteSize);

            using var aes = CreateAes();
            using var encryptor = aes.CreateEncryptor(key, encryptProperties.IV);

            var plainText = StringEncoding.GetBytes(toEncrypt);
            encryptProperties.CipherText = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

            return encryptProperties;
        }

        private static void EnsureDataIsValid(byte[] encryptedData)
        {
            var paramIsNull = encryptedData is null;
            var lengthMeetsSpecification = encryptedData.Length < MinimumEncryptedMessageByteSize;

            if (paramIsNull || lengthMeetsSpecification)
            {
                throw new ArgumentException("Invalid length of encrypted data");
            }
        }

        private static byte[] GenerateRandomBytes(int numberOfBytes)
        {
            var randomBytes = new byte[numberOfBytes];
            Random.GetBytes(randomBytes);
            return randomBytes;
        }

        private static byte[] GetKey(string password, byte[] passwordSalt)
        {
            var keyBytes = StringEncoding.GetBytes(password);

            using (var derivator = new Rfc2898DeriveBytes(
                keyBytes, passwordSalt,
                PasswordIterationCount, HashAlgorithmName.SHA256))
            {
                return derivator.GetBytes(PasswordByteSize);
            }
        }

        private static byte[] IntegritySign(string password, EncryptProperties encryptProperties)
        {
            var authKeySalt = GenerateRandomBytes(PasswordSaltByteSize);
            var authKey = GetKey(password, authKeySalt);

            var result = MergeArrays(
                additionalCapacity: SignatureByteSize,
                authKeySalt, encryptProperties.KeySalt, encryptProperties.IV, encryptProperties.CipherText);

            using var hmac = new HMACSHA256(authKey);

            var payloadToSignLength = result.Length - SignatureByteSize;
            var signatureTag = hmac.ComputeHash(result, 0, payloadToSignLength);
            signatureTag.CopyTo(result, payloadToSignLength);

            return result;
        }

        private static byte[] MergeArrays(int additionalCapacity = 0, params byte[][] arrays)
        {
            var merged = new byte[arrays.Sum(a => a.Length) + additionalCapacity];
            var mergeIndex = 0;
            for (int i = 0; i < arrays.GetLength(0); i++)
            {
                arrays[i].CopyTo(merged, mergeIndex);
                mergeIndex += arrays[i].Length;
            }

            return merged;
        }

        private static void VerifySignature(EncryptProperties encryptProperties)
        {
            using var hmac = new HMACSHA256(encryptProperties.AuthKey);

            var payloadToSignLength = encryptProperties.CipherText.Length - SignatureByteSize;
            var signatureTagExpected = hmac.ComputeHash(encryptProperties.CipherText, 0, payloadToSignLength);

            // constant time checking to prevent timing attacks
            var signatureVerificationResult = 0;

            for (int i = 0; i < encryptProperties.SignatureTag.Length; i++)
            {
                signatureVerificationResult |= encryptProperties.SignatureTag[i] ^ signatureTagExpected[i];
            }

            if (signatureVerificationResult != 0)
            {
                throw new CryptographicException("Invalid signature");
            }
        }

        private const int AesBlockByteSize = 128 / 8;

        private const int MinimumEncryptedMessageByteSize =
            PasswordSaltByteSize + // auth salt
            PasswordSaltByteSize + // key salt
            AesBlockByteSize + // IV
            AesBlockByteSize + // cipher text min length
            SignatureByteSize;

        private const int PasswordByteSize = 256 / 8;
        private const int PasswordIterationCount = 100_000;
        private const int PasswordSaltByteSize = 128 / 8;
        private const int SignatureByteSize = 256 / 8;

        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();
        private static readonly Encoding StringEncoding = Encoding.UTF8;

        public static string Decrypt(byte[] encryptedData, string password)
        {
            EnsureDataIsValid(encryptedData);

            var encryptProperties = BuildEncryptProperties(encryptedData, password);

            VerifySignature(encryptProperties);

            return DecryptData(encryptProperties);
        }

        public static byte[] Encrypt(string text, string password)
        {
            var encryptProperties = EncryptText(text, password);

            return IntegritySign(password, encryptProperties);
        }
    }

    public class EncryptProperties
    {
        public byte[] AuthKey { get; set; }
        public byte[] CipherText { get; set; }
        public int CipherTextIndex { get; set; }
        public int CipherTextLength { get; set; }
        public byte[] IV { get; set; }
        public byte[] KeySalt { get; set; }
        public byte[] SignatureTag { get; set; }
    }
}