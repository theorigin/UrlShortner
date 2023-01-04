using System;

namespace UrlShortner.Core
{
    public class IdGenerator{
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateId()
        {
            var stringChars = new char[7];
            for (var i = 0; i < stringChars.Length; i++)
            {
                char randomChar;
                do
                {
                    randomChar = GetRandomChar();
                } while (GetCountOfCharInArray(stringChars, randomChar) >= 2);

                stringChars[i] = randomChar;
            }
            return new string(stringChars);
		}

        private static int GetCountOfCharInArray(char[] array, char c)
        {
            return Array.FindAll(array, x => x == c).Length;
        }

        private static char GetRandomChar()
        {
            var rng = new Random();
            return Chars[rng.Next(Chars.Length)];
        }
    }
}