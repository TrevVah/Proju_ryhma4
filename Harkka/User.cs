using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KilsatMassiks
{
    public class User
    {
        public int userID { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        //string _password { get; set; }
        //byte[] _salt { get; set; }

        public User(int userID, string first_name, string last_name, string email)
        {
            this.userID = userID;
            this.first_name = first_name;
            this.last_name = last_name;
            this.email = email;
            //this._password = password;
            //this._salt = salt;
        }

        public int getID()
        {
            return userID;
        }

        public string getEmail()
        { 
            return email; 
        }
    }
}
