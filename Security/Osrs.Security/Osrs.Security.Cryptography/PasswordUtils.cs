//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Security.Cryptography;
using System.Text;

namespace Osrs.Security
{
    public static class PasswordUtils
    {
        private static SHA512 hasher;

        /// <summary>
        /// Returns a SaltPair containing the generated salt and the hashed salted password.
        /// Uses a SHA512 hasher for hashing operations
        /// </summary>
        /// <param name="shaker"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SaltPair Hash(SaltShaker shaker, string password)
        {
            SaltPair embedded = shaker.Salt(password);
            if (hasher==null)
                hasher = SHA512.Create();

            byte[] data = Encoding.UTF8.GetBytes(embedded.SaltedPayload);
            data = hasher.ComputeHash(data);
            return new SaltPair(embedded.Salt, new string(Encoding.UTF8.GetChars(data)));
        }

        public static string Hash(SaltShaker shaker, string password, string storedSalt)
        {
            string embedded = shaker.Embed(password, storedSalt);
            if (hasher == null)
                hasher = SHA512.Create();
            byte[] data = Encoding.UTF8.GetBytes(embedded);
            data = hasher.ComputeHash(data);
            return new string(Encoding.UTF8.GetChars(data));
        }

        /// <summary>
        /// Performs a salt/hash/compare of a cleartext password against a stored password hash using the provided shaker for salting.
        /// This assumes that the salting is properly specified in the shaker and expects that the salting is repeatable. And unrepeatable salt will always fail the compare, in this case the salt used for the storedcredential must also be passed.
        /// </summary>
        /// <param name="shaker"></param>
        /// <param name="password"></param>
        /// <param name="storedCredential"></param>
        /// <returns></returns>
        public static bool Matches(SaltShaker shaker, string password, string storedCredential)
        {
            SaltPair cur = Hash(shaker, password);
            return cur.SaltedPayload.Equals(storedCredential);
        }

        public static bool Matches(SaltShaker shaker, string password, string storedCredential, string storedSalt)
        {
            string cur = Hash(shaker, password, storedSalt);
            return cur.Equals(storedCredential);
        }
    }
}
