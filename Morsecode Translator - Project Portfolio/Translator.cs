using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JollyWrapper;
using static System.Net.Mime.MediaTypeNames;

[assembly: InternalsVisibleTo("Unit Testing")]

namespace OOSDD_Project_Portfolio
{
    internal abstract class Translator
    {
        //Load morse code list depending on which standard to translate

        //Characters to be compared against (A,B,C, etc...)
        protected List<String> _CharSet { get; set; } = new List<String>();

        //Morse translation set
        protected List<String> _MorseSet { get; set; } = new List<String>();

        //Get _CharSet and _MorseSet from text file
        public void GetStandard(String file)
        {
            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                //Check for commented, skip if so
                if(line.Contains('#'))
                {
                    continue;
                }

                //Split line into parts
                string[] parts = line.Split(' ');

                if (parts.Length == 2) //Check for only two parts (character and morse)
                {
                    _CharSet.Add(parts[0]);
                    _MorseSet.Add(parts[1]);
                }
            }
        }

        public void GetRandomMorse()
        {
            //Used to make random numbers
            Random r = new Random();
            int character = r.Next(0, (_MorseSet.Count - 1));

            //Print randomly generated morse
            GlobalMethod.DarkGray("[Training] ");
            Console.WriteLine("What is the equivalent for {0}?", _MorseSet[character]);

            //Get Answer
            string Answer = GlobalMethod.SingleTextInput("abcdefghijklmnopqrstuvwxyz123456789");
            Answer = Answer.ToUpper();

            if (Answer == _CharSet[character])
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("[Correct] ");
                Console.ResetColor();
                Console.WriteLine("That was the correct answer!");
            }
            else
            {
                GlobalMethod.DarkRed("[Incorrect] ");
                Console.WriteLine("That was not the correct answer, the correct answer was: {0}!", _CharSet[character]);
            }
        }

        public abstract void MorseConvert(String text, User user);
        public abstract void TextConvert(String morse, User user);
    }

    class Unencrypted : Translator
    {
        public override void MorseConvert(String text, User user)
        {
            text = text.ToUpper();
            String morse = "";

            //Split text into parts
            char[] parts = text.ToCharArray();

            foreach (char part in parts)
            {
                String letter = part.ToString();

                if (letter == " ") //if text is a space, add a |
                {
                    morse = String.Concat(morse, "|");
                    continue;
                }
                else if (!morse.EndsWith("|") && !String.IsNullOrEmpty(morse)) //Check if previous character was a "|", if not add a space inbetween letters
                {
                    morse = String.Concat(morse, " "); 
                }

                //Finds the position of the letter in the CharSet and gets the equivalent more from MorseSet
                string mPart = _MorseSet[_CharSet.IndexOf(letter)];
                morse = String.Concat(morse, mPart);
            }
            //Return translation and prompt user to move on
            GlobalMethod.DarkGray("[Result] ");
            Console.WriteLine(morse);
            GlobalMethod.DarkGray("[Press enter to return]");

            //Log Conversion
            user.LogConversion(text, morse);
        }
        public override void TextConvert(String morse, User user)
        {
            String text = "";
            morse = morse.Replace(".", "·");

            //Split morse into words
            string[] words = morse.Split("|");

            foreach (string word in words)
            {
                //Split morse into letters
                string[] letters = word.Split(" ");

                if (!letters.All(letter => _MorseSet.Contains(letter))) //Error validation to check for incorrect morse code
                {
                    GlobalMethod.DarkRed("[Error] ");
                    Console.WriteLine("Inavlid morse code entered! Please enter valid morse code according to the given standard!");
                    return;
                }

                foreach (string letter in letters)
                {
                    //Finds the position of the letter in the MorseSet and gets the equivalent more from CharSet
                    string tPart = _CharSet[_MorseSet.IndexOf(letter)];
                    text = String.Concat(text, tPart);
                }

                if (!(word == words.Last()))
                {
                    text = String.Concat(text, " "); //When word ends, add a space
                }
            }

            //Make text more readable
            text = text.ToLower();

            //Return translation and prompt user to move on
            GlobalMethod.DarkGray("[Result] ");
            Console.WriteLine(text);
            GlobalMethod.DarkGray("[Press enter to return]");

            //Log Conversion
            user.LogConversion(morse, text);
        }
    }

    class AESEncrypt : Translator
    {
        public static string GetPassword()
        {
            //Request password
            GlobalMethod.DarkGray("[Encryption]");
            Console.WriteLine(" Please enter your password.");

            //Request choice
            GlobalMethod.DarkGray("[Password?] ");
            string Password = Console.ReadLine() ?? String.Empty;

            //Return password
            return Password;
        }

        public static string Encode(string morse, string password)
        {
            //Encryption
            byte[] encryptedBytes;
            byte[] pepper = Encoding.UTF8.GetBytes("SecurePepperMorse"); 

            using (Aes aes = Aes.Create())
            {
                aes.Key = new Rfc2898DeriveBytes(password, pepper ,1000).GetBytes(32);
                aes.IV = new Rfc2898DeriveBytes(password, pepper, 1000).GetBytes(16);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                var input = Encoding.UTF8.GetBytes(morse);

                encryptedBytes = encryptor.TransformFinalBlock(input, 0, input.Length);
            }

            return Convert.ToHexString(encryptedBytes);
        }
        public static string Decode(string morse, string password)
        {
            //Decryption
            byte[] pepper = Encoding.UTF8.GetBytes("SecurePepperMorse"); 
            byte[] decryptedBytes;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = new Rfc2898DeriveBytes(password, pepper, 1000).GetBytes(32);
                    aes.IV = new Rfc2898DeriveBytes(password, pepper, 1000).GetBytes(16);

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    var input = Convert.FromHexString(morse);

                    decryptedBytes = decryptor.TransformFinalBlock(input, 0, input.Length);
                }
            }
            catch (Exception e)
            {
                GlobalMethod.DarkRed("[Error] ");
                Console.WriteLine("Error occured while decoding message.");

                return "N/A";
            }


            return Encoding.UTF8.GetString(decryptedBytes);
        }


        public override void MorseConvert(String text, User user)
        {
            text = text.ToUpper();
            String morse = "";

            //Encryption
            string password = GetPassword();
            text = Encode(text, password);

            //Split text into parts
            char[] parts = text.ToCharArray();

            foreach (char part in parts)
            {
                String letter = part.ToString();

                if (letter == " ") //if text is a space, add a |
                {
                    morse = String.Concat(morse, "|");
                    continue;
                }
                else if (!morse.EndsWith("|") && !String.IsNullOrEmpty(morse)) //Check if previous character was a "|", if not add a space inbetween letters
                {
                    morse = String.Concat(morse, " ");
                }

                //Finds the position of the letter in the CharSet and gets the equivalent more from MorseSet
                string mPart = _MorseSet[_CharSet.IndexOf(letter)];
                morse = String.Concat(morse, mPart);
            }

            //Return translation and prompt user to move on
            GlobalMethod.DarkGray("[Result] ");
            Console.WriteLine(morse);
            GlobalMethod.DarkGray("[Press enter to return]");

            //Log Conversion
            user.LogConversion(text, morse);
        }
        public override void TextConvert(String morse, User user)
        {
            String text = "";
            morse = morse.Replace(".", "·");

            //Split morse into words
            string[] words = morse.Split("|");

            foreach (string word in words)
            {
                //Split morse into letters
                string[] letters = word.Split(" ");

                if (!letters.All(letter => _MorseSet.Contains(letter))) //Error validation to check for incorrect morse code
                {
                    GlobalMethod.DarkRed("[Error] ");
                    Console.WriteLine("Inavlid morse code entered! Please enter valid morse code according to the given standard!");
                    return;
                }

                foreach (string letter in letters)
                {
                    //Finds the position of the letter in the MorseSet and gets the equivalent more from CharSet
                    string tPart = _CharSet[_MorseSet.IndexOf(letter)];
                    text = String.Concat(text, tPart);
                }

                if(!(word == words.Last()))
                {
                    text = String.Concat(text, " "); //When word ends, add a space
                }
            }

            //Decode text
            string password = GetPassword();
            text = Decode(text, password);

            //Make text more readable
            text = text.ToLower();

            //Return translation and prompt user to move on
            GlobalMethod.DarkGray("[Result] ");
            Console.WriteLine(text);
            GlobalMethod.DarkGray("[Press enter to return]");

            //Log Conversion
            user.LogConversion(morse, text);
        }
    }
}
