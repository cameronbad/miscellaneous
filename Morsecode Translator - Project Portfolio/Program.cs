using JollyWrapper;
using System;

namespace OOSDD_Project_Portfolio
{
    internal class Program
    {
        static void LoginLoop(User user)
        {
            //Validated login loop which forces user to login before starting program
            bool auth = false;

            do
            {
                //Request username
                GlobalMethod.DarkGray("[Login]");
                Console.WriteLine(" Please enter your username.");

                //Request choice
                GlobalMethod.DarkGray("[Username?] ");
                string Username = Console.ReadLine() ?? String.Empty;

                //Request password
                GlobalMethod.DarkGray("[Login]");
                Console.WriteLine(" Please enter your password.");

                //Request choice
                GlobalMethod.DarkGray("[Password?] ");
                string Password = Console.ReadLine() ?? String.Empty;

                //Clear console to maintain password privacy
                Console.Clear();

                //Attempt to login
                user.Create(Username, Password);
                if (user.Login())
                {
                    //End loop
                    auth = true;
                    
                    //Feeback
                    GlobalMethod.DarkGray("[Login]");
                    Console.WriteLine(" User login successful!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("[Error]");
                    Console.ResetColor();
                    Console.WriteLine(" Incorrect details entered, login unsuccessful.");
                }
            } while (!auth);
        }

        static void Menu()
        {
            //Stylised menu for users
            GlobalMethod.DarkGray("[1]");
            Console.WriteLine(" Convert a message to morse code.");
            GlobalMethod.DarkGray("[2]");
            Console.WriteLine(" Convert morse code to a message.");
            GlobalMethod.DarkGray("[3]");
            Console.WriteLine(" Training.");
            GlobalMethod.DarkGray("[4]");
            Console.WriteLine(" Create a new user.");
            GlobalMethod.DarkGray("[5]");
            Console.WriteLine(" Edit a user.");
            GlobalMethod.DarkGray("[6]");
            Console.WriteLine(" Delete a user.");
            GlobalMethod.DarkGray("[0]");
            Console.WriteLine(" Logout and exit program.");

            Console.WriteLine("\nPlease enter the number of the option you wish to select.");
        }

        static int SelectUserLoop()
        {
            //Add UID's to list, compare check against list to make sure UID is real
            //Comparison list
            List<int> uids = new List<int>();

            GlobalMethod.Connect();

            //Check for username
            string SQL = "SELECT `UID`, `username` FROM `users` ORDER BY `UID`";
            QueryData userData = Database.ExecuteQuery(SQL).Result;

            foreach (var user in userData)
            {
                uids.Add(Convert.ToInt16(user["UID"]));
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[{0}] ", user["UID"]);
                Console.ResetColor();
                Console.WriteLine(user["username"]);
            }

            Console.WriteLine("\nPlease enter the number of the user you wish to select.");
            return uids.ChooseInt();
        }

        static string GetTranslator()
        {
            //Stylised menu for users
            GlobalMethod.DarkGray("[1]");
            Console.WriteLine(" International");
            GlobalMethod.DarkGray("[2]");
            Console.WriteLine(" American");

            //Request user choice
            Console.WriteLine("\nPlease enter the number of the morse code translation standard you wish to use.");
            int choice = 2.ChooseInt();

            //Return standard
            switch (choice)
            {
                case 1:
                    return "international.txt";
                case 2:
                    return "american.txt";
                default:
                    //Returns international if an error occurs.
                    return "international.txt";
            }
        }

        static Translator chooseEncryption()
        {
            //Stylised menu for users
            GlobalMethod.DarkGray("[1]");
            Console.WriteLine(" Unencrypted");
            GlobalMethod.DarkGray("[2]");
            Console.WriteLine(" AES Encryption");

            //Request user choice
            Console.WriteLine("\nPlease enter the number of the encryption method you wish to use.");
            int choice = 2.ChooseInt();

            //Return encryption type
            switch(choice)
            {
                case 1:
                    return new Unencrypted();
                case 2:
                    return new AESEncrypt();
                default:
                    //Returns unencrypted if an error occurs.
                    return new Unencrypted();
            }
        }

        static void TrainingLoop(Translator message)
        {
            bool running = true;

            do
            {
                Console.Clear();
                message.GetRandomMorse();

                //Continue?
                GlobalMethod.DarkGray("\n[Training] ");
                Console.WriteLine("Would you like to go again?");
                GlobalMethod.DarkGray("[1]");
                Console.WriteLine(" Yes");
                GlobalMethod.DarkGray("[2]");
                Console.WriteLine(" No");

                int Choice = 2.ChooseInt();

                if (Choice == 2)
                {
                    running = false;
                }

            } while (running);


        }


        static void MainFlow(User user)
        {
            //Declare user management object
            User userManagement = new User();
            //Declare empty object for translator
            Translator message;

            bool active = true;
            do
            {
                //Visual menu
                Menu();
                //User choice and validation
                int Choice = 6.ChooseInt();

                switch (Choice)
                {
                    case 1: //Convert a message to morse code.
                        message = chooseEncryption(); //Use method to declare translators type.
                        message.GetStandard(GetTranslator()); //Standard.
                        message.MorseConvert(GlobalMethod.textInput("abcdefghijklmnopqrstuvwxyz123456789 "), user); //Get input and convert it
                        Console.ReadLine();
                        break;
                    case 2: //Convert morse code to a message
                        message = chooseEncryption(); //Use method to declare translators type.
                        message.GetStandard(GetTranslator()); //Standard.
                        message.TextConvert(GlobalMethod.textInput(".·-| "), user); //Get input and convert it
                        Console.ReadLine();
                        break;
                    case 3: //Training
                        message = new Unencrypted();
                        message.GetStandard(GetTranslator()); //Standard.
                        TrainingLoop(message);
                        break;
                    case 4: //Create a new user
                        userManagement.Create();
                        break;
                    case 5: //Edit a user
                        userManagement.Edit(SelectUserLoop());
                        break;
                    case 6: //Delete a user
                        userManagement.Delete(SelectUserLoop());
                        break;
                    case 0: //Logout and end program
                        active = false;
                        break;
                    default:
                        GlobalMethod.DarkRed("[Error] Invalid input!");
                        Console.WriteLine();
                        break;
                }
                //Clear console to maintain privacy and keep console clean
                Console.Clear();
            } while (active);
        }

        static void Main(string[] args)
        {
            //Declare current user
            User user = new User();

            //Login check validation loop
            LoginLoop(user);

            //Menu
            MainFlow(user);
        }
    }
}