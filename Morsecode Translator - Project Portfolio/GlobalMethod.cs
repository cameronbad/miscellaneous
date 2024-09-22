using JollyWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSDD_Project_Portfolio
{
    public static class GlobalMethod
    {
        public static int ChooseInt(this int maxLength)
        {
            bool valid;
            int choice;

            //Validation loop
            do
            {
                //Request choice
                DarkGray("[Choice?] ");

                string uInput = Console.ReadLine() ?? String.Empty;
                valid = int.TryParse(uInput, out choice);

                //Check if input is a valid integer
                if (!valid)
                {
                    DarkRed("[Error] ");
                    Console.WriteLine("Input was not a number, please enter a digit\n");
                    continue;
                }

                //Check if input is within the correct range
                if (choice < 0 || choice > maxLength)
                {
                    DarkRed("[Error] ");
                    Console.WriteLine("Invalid number entered, please input a valid number!\n");
                    valid = false;
                }

            } while (!valid);

            Console.WriteLine();

            //Return choice
            return choice;
        }

        public static int ChooseInt(this List<int> range)
        {
            bool valid;
            int choice;

            //Validation loop
            do
            {
                //Request choice
                DarkGray("[Choice?] ");

                string uInput = Console.ReadLine() ?? String.Empty;
                valid = int.TryParse(uInput, out choice);

                //Check if input is a valid integer
                if (!valid)
                {
                    DarkRed("[Error] ");
                    Console.WriteLine("Input was not a number, please enter a digit\n");
                    continue;
                }

                //Check if input is within the correct range
                if (!(range.Contains(choice)) )
                {
                    DarkRed("[Error] ");
                    Console.WriteLine("Invalid number entered, please input a valid number!\n");
                    valid = false;
                }

            } while (!valid);

            Console.WriteLine();

            //Return choice
            return choice;
        }

        //Validated string input
        public static String textInput(String validChar) 
        {
            //validChar is the list of valid characters for string
            bool valid;
            String text;

            do
            {
                //Get Input
                GlobalMethod.DarkGray("[Translate]"); 
                Console.WriteLine(" Please enter text to translate.");
                GlobalMethod.DarkGray("[Input?] "); //Request choice
                text = Console.ReadLine() ?? String.Empty;

                //Validate
                String letters = text.ToLower();
                valid = letters.All(letter => validChar.Contains(letter));

                //Error if invalid
                if(!valid)
                {
                    DarkRed("[Error] ");
                    Console.WriteLine("Invalid character entered into string, please only enter letters and numbers!\n");
                }
            } while (!valid);

            return text; //Return the validated text input
        }

        public static String SingleTextInput(String validChar)
        {
            //validChar is the list of valid characters for string
            bool valid;
            String text;

            do
            {
                GlobalMethod.DarkGray("[Input?] "); //Request choice
                text = Console.ReadLine() ?? String.Empty;

                //Validate
                String letters = text.ToLower();
                valid = letters.All(letter => validChar.Contains(letter));

                //Error if invalid
                if (!valid)
                {
                    DarkRed("[Error] ");
                    Console.WriteLine("Invalid character entered into string, please only enter letters and numbers!\n");
                }

                //Additional validate
                if (letters.Length != 1)
                {
                    valid = false;
                    DarkRed("[Error] ");
                    Console.WriteLine("Please only enter a single character!\n");
                }

            } while (!valid);

            return text; //Return the validated text input
        }

        public static void Connect()
        {
            //Connect to database
            Database.Init("plesk.remote.ac", "ws350074_oop_dev", "0C~88una55003Jimz~", "ws350074_oop_dev", "SSLMode=None");
        }

        public static void DarkGray(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void DarkRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
