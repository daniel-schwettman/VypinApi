using System;
using System.Text;
using System.Security.Cryptography;

namespace VypinApi.Models
{
    public class Helper
    {

        /**
         * Private methods
         */

        // Password methods
        public String generateSalt(int length)
        {
            var bytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        //Generating a password hash using PBKDF:
        public String generateHash(String userPassword, String userSalt, int iterations, int length)
        {
            byte[] password = Encoding.ASCII.GetBytes(userPassword);
            byte[] salt = Encoding.ASCII.GetBytes(userSalt);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return System.Text.Encoding.ASCII.GetString(deriveBytes.GetBytes(length));
            }
        }
    }
}