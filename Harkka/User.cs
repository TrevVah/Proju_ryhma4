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
    internal class User
    {
        int _userID { get; set; }
        string _first_name { get; set; }
        string _last_name { get; set; }
        string _email { get; set; }
        string _password { get; set; }
        //byte[] _salt { get; set; }

        public User(int userID, string first_name, string last_name, string email, string password)
        {
            this._userID = userID;
            this._first_name = first_name;
            this._last_name = last_name;
            this._email = email;
            this._password = password;
            //this._salt = salt;
        }

        public int getID()
        {
            return _userID;
        }
    }
}
