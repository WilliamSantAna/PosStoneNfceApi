using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PosStoneNfce.API.Portal.App.Common.Utils.Criptografia
{
    public static class CriptografiaMide
    {
        private static readonly int _maxLengthPasswordDb = 256;

        public static string GeneratePassword()
        {
            int tamanhoSenha = 10;
            string caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@";
            StringBuilder builder = new StringBuilder(tamanhoSenha);
            Random random = new Random();
            
            while (1 < tamanhoSenha--)
                builder.Append(caracteresPermitidos[random.Next(caracteresPermitidos.Length)]);

            builder.Append("@");
            return builder.ToString();
        }

        public static string Criptografa(string password, string salt)
        {
            byte[] bIn = Encoding.Unicode.GetBytes(password);
            byte[] bSalt = Encoding.Unicode.GetBytes(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];            

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);

            HashAlgorithm s = HashAlgorithm.Create("SHA1");

            var bRet = s.ComputeHash(bAll);
            string newHash = Convert.ToBase64String(bRet);
            return newHash;
        }

        public static string GetTruncatedPassword(string encryptedPassword)
        {
            return encryptedPassword.Substring(
                0, (encryptedPassword.Length > _maxLengthPasswordDb ? 
                _maxLengthPasswordDb : encryptedPassword.Length));
        }

        public static string CriptografaSalt(string salt)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(salt));
        }

        public static string SignAC_SHA256(X509Certificate2 cert, string sAssinatura)
        {
            try
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sAssinaturaByte = enc.GetBytes(sAssinatura);

                RSA privateKey = cert.GetRSAPrivateKey();

                RSACryptoServiceProvider privateKey1 = new RSACryptoServiceProvider();
                privateKey1.ImportParameters(privateKey.ExportParameters(true));

                byte[] signature = privateKey1.SignData(sAssinaturaByte, "SHA256");
                bool isValid = privateKey1.VerifyData(sAssinaturaByte, "SHA256", signature);

                if (isValid)
                    return Convert.ToBase64String(signature);
                else
                    return "";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public static string Descriptografa(string valor)
        {
            try
            {
                if (string.IsNullOrEmpty(valor))
                    throw new Exception("O valor a ser descriptografado está nulo ou em branco.");

                string passPhrase = "Pas5pr@se";        // can be any string
                string saltValue = "s@1tValue";        // can be any string
                string hashAlgorithm = "SHA1";             // can be "MD5"
                int passwordIterations = 2;                  // can be any number
                string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
                int keySize = 256;                // can be 192 or 128

                return RijndaelSimple.Decrypt(
                    valor,
                    passPhrase,
                    saltValue,
                    hashAlgorithm,
                    passwordIterations,
                    initVector,
                    keySize);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha na hora de criptografar o valor informado.", ex);
            }
        }
    }
}