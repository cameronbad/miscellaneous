using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JollyWrapper;
using Org.BouncyCastle.Cmp;
using BC = BCrypt.Net.BCrypt;

namespace OOSDD_Project_Portfolio
{
    //The currently logged in user
    internal class User
    {
        private int _uid { get; set; }

        private string _firstName { get; set;}

        private string _lastName { get; set; }

        public string _username { get; set; }

        public string _password { get; set; }

        //Uploads user to database upon creation
        public void Create()
        {
            //User input
            GlobalMethod.DarkGray("[Edit]"); //Request first name
            Console.WriteLine(" Please enter the user's first name.");
            GlobalMethod.DarkGray("[First Name?] "); //Request choice
            _firstName = Console.ReadLine() ?? String.Empty;

            GlobalMethod.DarkGray("[Edit]"); //Request last name
            Console.WriteLine(" Please enter the user's last name.");
            GlobalMethod.DarkGray("[Last Name?] "); //Request choice
            _lastName = Console.ReadLine() ?? String.Empty;

            GlobalMethod.DarkGray("[Edit]"); //Request username
            Console.WriteLine(" Please enter the user's username.");
            GlobalMethod.DarkGray("[Username?] "); //Request choice
            _username = Console.ReadLine() ?? String.Empty;

            GlobalMethod.DarkGray("[Edit]"); //Request username
            Console.WriteLine(" Please enter the user's password.");
            GlobalMethod.DarkGray("[Password?] "); //Request choice
            _password = BC.HashPassword(Console.ReadLine() ?? String.Empty);

            GlobalMethod.Connect();

            //Insert to database
            string SQL = "INSERT INTO `users` (`UID`, `firstName`, `lastName`, `username`, `password`, `TIMESTAMP`) VALUES (NULL, @val, @val, @val, @val, current_timestamp())";
            int createCheck = Database.ExecuteNonQuery(SQL, _firstName, _lastName, _username, _password).Result;
            Console.WriteLine(createCheck == 1 ? "User has been made!" : "Error: User creation unsuccessful");
            Get(_username);
        }

        //Creates temporary user for login check
        public void Create(string username, string password)
        {
            //Assign values
            _username = username;
            _password = password;
        }

        //Get from database
        private void Get(int UID)
        {
            GlobalMethod.Connect();

            //Check for username
            string SQL = "SELECT * FROM `users` WHERE `UID` = @val";
            QueryData userData = Database.ExecuteQuery(SQL, UID).Result;

            foreach (var user in userData)
            {
                _uid = Convert.ToInt16(user["UID"]);
                _firstName = user["firstName"];
                _lastName = user["lastName"];
                _username = user["username"];
                _password = user["password"];
            }
        }
        private void Get(string Username)
        {
            //Connect to database
            Database.Init("plesk.remote.ac", "ws350074_oop_dev", "0C~88una55003Jimz~", "ws350074_oop_dev", "SSLMode=None");

            //Check for username
            string SQL = "SELECT * FROM `users` WHERE `username` = @val";
            QueryData userData = Database.ExecuteQuery(SQL, Username).Result;

            foreach (var user in userData)
            {
                _uid = Convert.ToInt16(user["UID"]);
                _firstName = user["firstName"];
                _lastName = user["lastName"];
                _username = user["username"];
                _password = user["password"]; 
            }
        }

        //Save to database
        private void Save()
        {
            GlobalMethod.Connect();

            //Update database
            string SQL = "UPDATE `users` SET `firstName` = @val, `lastName` = @val, `username` = @val, `password` = @val WHERE `users`.`UID` = @val";
            int updateCheck = Database.ExecuteNonQuery(SQL, _firstName, _lastName, _username, _password, _uid).Result;
            Console.WriteLine(updateCheck == 1 ? "User has been updated!" : "Error: User update unsuccessful");
        }

        //Edit user
        public void Edit(int UID)
        {
            //Get user to edit.
            Get(UID);

            //User input
            GlobalMethod.DarkGray("[Edit]"); //Request first name
            Console.WriteLine(" Please enter the user's first name.");
            GlobalMethod.DarkGray("[First Name?] "); //Request choice
            _firstName = Console.ReadLine() ?? String.Empty;

            GlobalMethod.DarkGray("[Edit]"); //Request last name
            Console.WriteLine(" Please enter the user's last name.");
            GlobalMethod.DarkGray("[Last Name?] "); //Request choice
            _lastName = Console.ReadLine() ?? String.Empty;

            GlobalMethod.DarkGray("[Edit]"); //Request username
            Console.WriteLine(" Please enter the user's username.");
            GlobalMethod.DarkGray("[Username?] "); //Request choice
            _username = Console.ReadLine() ?? String.Empty;

            GlobalMethod.DarkGray("[Edit]"); //Request username
            Console.WriteLine(" Please enter the user's password.");
            GlobalMethod.DarkGray("[Password?] "); //Request choice
            _password = BC.HashPassword(Console.ReadLine() ?? String.Empty);

            //Save changes to user.
            Save();
        }

        //Delete user
        public void Delete(int UID)
        {
            //Get user details
            Get(UID);

            //Confirmation Warning
            GlobalMethod.DarkRed("[WARNING] ");
            Console.WriteLine("You are about to delete the user {0}, are you sure you wish to do this?", _username);
            GlobalMethod.DarkGray("[1]");
            Console.WriteLine(" Yes");
            GlobalMethod.DarkGray("[2]");
            Console.WriteLine(" No");

            int Choice = 2.ChooseInt();

            if(Choice == 1) //Run delete SQL
            {
                GlobalMethod.Connect();

                //Update database
                string SQL = "DELETE FROM `users` WHERE `users`.`UID` = @val";
                Database.ExecuteNonQuery(SQL, UID);
            }
        }

        //Login current user
        public bool Login()
        {
            GlobalMethod.Connect();

            //Check for username
            string SQL = "SELECT * FROM `users` WHERE `username` = @val";
            QueryData loginCheck = Database.ExecuteQuery(SQL, _username).Result;

            if(loginCheck.Count() == 1) //Check only 1 instance of username exists
            {
                foreach (var login in loginCheck)
                {
                    if(BC.Verify(_password, login["password"])) //Verify hashes
                    {
                        Get(_username);
                        return true;
                    } 
                }
            }
            return false;
        }

        //Log conversions to database
        public void LogConversion(String input, String output)
        {
            GlobalMethod.Connect();

            //Create log in database
            string SQL = "INSERT INTO `conversions` (`ID`, `UID`, `input`, `output`, `TIMESTAMP`) VALUES (NULL, @val, @val, @val, current_timestamp())";
            Database.ExecuteNonQuery(SQL, _uid, input, output);
        }
    }
}
