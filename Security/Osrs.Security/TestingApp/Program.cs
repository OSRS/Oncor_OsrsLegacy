using Osrs.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random();
            int seed = r.Next();
            int size = 8192;
            string pass;

            pass = "Hello World";
            ShakeAndBake(pass, seed, size);
            Console.WriteLine();

            pass = "Pr0c@ss)r5_ !nthesh~d-\\";
            ShakeAndBake(pass, seed, size);
            Console.WriteLine();

            Console.WriteLine("ALL DONE - ENTER TO EXIT");
            Console.ReadLine();
        }

        static void ShakeAndBake(string password, int seed, int size)
        {
            SaltShaker shake;
            SaltPair saltFirst;
            SaltPair saltSecond;
            SaltPair hashFirst;
            SaltPair hashSecond;
            bool match;
            string salted;

            //Start with a pass
            shake = new SaltShaker(char.MinValue, char.MaxValue, size, SaltCreationModel.NonRepeatableStrong, SaltEmbeddingModel.Middle, seed);
            Console.WriteLine("==========================================");
            Console.WriteLine("Type A - nonRepeatable salts");
            Console.WriteLine("==========================================");
            Console.WriteLine("Password: " + password);

            saltFirst = shake.Salt(password);
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Salt: " + saltFirst.Salt.GetHashCode());
            Console.WriteLine("Salted: " + saltFirst.SaltedPayload.GetHashCode());
            Console.WriteLine("------------------------------------------");

            shake = new SaltShaker(char.MinValue, char.MaxValue, size, SaltCreationModel.NonRepeatableStrong, SaltEmbeddingModel.Middle, seed);
            saltSecond = shake.Salt(password);
            salted = shake.Embed(password, saltFirst.Salt);
            Console.WriteLine("Salt: " + saltSecond.Salt.GetHashCode());
            Console.WriteLine("Salted: " + saltSecond.SaltedPayload.GetHashCode());
            Console.WriteLine("------------------------------------------");
            match = saltFirst.SaltedPayload.Equals(saltSecond.SaltedPayload);
            Console.WriteLine("Matches: " + match);
            match = saltFirst.SaltedPayload.Equals(salted);
            Console.WriteLine("MatchesB: " + match);
            salted = shake.Embed(password, saltSecond.Salt);
            match = saltSecond.SaltedPayload.Equals(salted);
            Console.WriteLine("MatchesC: " + match);
            Console.WriteLine("==========================================");

            shake = new SaltShaker(char.MinValue, char.MaxValue, size, SaltCreationModel.Repeatable, SaltEmbeddingModel.Randomized, seed);
            Console.WriteLine("Type B");
            Console.WriteLine("==========================================");

            saltFirst = shake.Salt(password);
            hashFirst = PasswordUtils.Hash(shake, password);
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Salt: " + saltFirst.Salt.GetHashCode());
            Console.WriteLine("Salted: " + saltFirst.SaltedPayload.GetHashCode());
            Console.WriteLine("Hashed: " + hashFirst.SaltedPayload.GetHashCode());
            Console.WriteLine("UtilsMatch: " + PasswordUtils.Matches(shake, password, hashFirst.SaltedPayload));
            Console.WriteLine("------------------------------------------");

            shake = new SaltShaker(char.MinValue, char.MaxValue, size, SaltCreationModel.Repeatable, SaltEmbeddingModel.Randomized, seed);
            saltSecond = shake.Salt(password);
            hashSecond = PasswordUtils.Hash(shake, password);
            Console.WriteLine("Salt: " + saltSecond.Salt.GetHashCode());
            Console.WriteLine("Salted: " + saltSecond.SaltedPayload.GetHashCode());
            Console.WriteLine("Hashed: " + hashFirst.SaltedPayload.GetHashCode());
            Console.WriteLine("UtilsMatch: " + PasswordUtils.Matches(shake, password, hashSecond.SaltedPayload));
            Console.WriteLine("------------------------------------------");
            match = saltFirst.SaltedPayload.Equals(saltSecond.SaltedPayload);
            Console.WriteLine("Matches (Pass): " + match);
            match = hashFirst.SaltedPayload.Equals(hashSecond.SaltedPayload);
            Console.WriteLine("Matches (Hash): " + match);
            Console.WriteLine("==========================================");
        }
    }
}
