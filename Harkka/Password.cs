using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilsatMassiks
{
    public class Password
    {
        public int userID { get; set; }
        public byte[] password { get; set; }
        public byte[] salt { get; set; }

        public Password(int userID, byte[] password, byte[] salt)
        {
            this.userID = userID;
            this.password = password;
            this.salt = salt;
        }

        public int getID()
        {
            return userID;
        }

        public byte[] getPassword() 
        {
            return password;
        }
        public byte[] getSalt() 
        {
            return salt;
        }
    }
}

