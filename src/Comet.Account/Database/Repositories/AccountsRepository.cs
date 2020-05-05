using System;

namespace Comet.Account.Database.Repositories
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Comet.Account.Database.Models;
    using System.Security.Cryptography;
    using System.Text;
    using Org.BouncyCastle.Utilities.Encoders;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository for defining data access layer (DAL) logic for the account table. Allows
    /// the server to fetch account details for player authentication. Accounts are fetched
    /// on demand for each player authentication request.
    /// </summary>
    public static class AccountsRepository
    {
        /// <summary>
        /// Fetches an account record from the database using the player's username as a
        /// unique key for selecting a single record.
        /// </summary>
        /// <param name="username">Username to pull account info for</param>
        /// <returns>Returns account details from the database.</returns>
        public static async Task<DbAccount> FindAsync(string username)
        {
            using (var db = new ServerDbContext())
                return await db.Accounts.Include(x => x.Authority)
                    .Include(x => x.Status)
                    .Where(x => x.Username == username)
                    .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Validates the user's inputted password, which has been decrypted from the client
        /// request decode method, and is ready to be hashed and compared with the SHA-1
        /// hash in the database.
        /// </summary>
        /// <param name="input">Inputted password from the client's request</param>
        /// <param name="hash">Hashed password in the database</param>
        /// <param name="salt">Salt for the hashed password in the databse</param>
        /// <returns>Returns true if the password is correct.</returns>
        public static bool CheckPassword(string input, string hash, string salt)
        {
            byte[] inputHashed;
            using (var sha1 = SHA1.Create())
                inputHashed = sha1.ComputeHash(Encoding.ASCII.GetBytes(input + salt));
            var final = Hex.ToHexString(inputHashed);
            return final.Equals(hash);
        }

        public static string HashPassword(string password, string salt)
        {
            byte[] inputHashed;
            using (var sha256 = SHA256.Create())
                inputHashed = sha256.ComputeHash(Encoding.ASCII.GetBytes(password + salt));
            var final = Hex.ToHexString(inputHashed);
            return final;
        }

        public static string GenerateSalt()
        {
            const string UPPER_S = "QWERTYUIOPASDFGHJKLZXCVBNM";
            const string LOWER_S = "qwertyuioplkjhgfdsazxcvbnm";
            const string NUMBER_S = "1236547890";
            const string POOL_S = UPPER_S + LOWER_S + NUMBER_S;
            const int SIZE_I = 30;

            Random random = new Random();
            string output = "";
            for (int i = 0; i < SIZE_I; i++)
            {
                output += POOL_S[random.Next() % POOL_S.Length];
            }
            return output;
        }
    } 
}