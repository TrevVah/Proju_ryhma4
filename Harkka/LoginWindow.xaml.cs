using System;
using System.Security;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections;

namespace KilsatMassiks
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static LoginWindow Instance { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           Window windowsignup = new SignUpWindow();
            this.Hide();
            windowsignup.Show();
        }

        private void CloseUp(object sender, System.ComponentModel.CancelEventArgs e) 
        { 

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoginButton(object sender, RoutedEventArgs e)
        {
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(password_field.SecurePassword);
            User _user = AuthenticateUser(email_field.Text, System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr));
            if (_user != null)
            {
                this.Hide();
                MainWindow.Instance.SwarmApp(_user);
            }
            else
            {
                Debug.WriteLine("Authenticaton failed.");
            }

        }

        public User? AuthenticateUser(string email, string password)
        {
            string jsonUserFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Users.json");
            string jsonPasswordFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Passwords.json");

            if (!File.Exists(jsonUserFilePath))
            {
                Debug.WriteLine("Password file doesn't exist.\"");
                return null;
            }

            if (!File.Exists(jsonPasswordFilePath))
            {
                Debug.WriteLine("Password file doesn't exist.");
                return null;
            }

            string jsonUsersString = File.ReadAllText(jsonUserFilePath);
            string jsonPasswordsString = File.ReadAllText(jsonPasswordFilePath);
            List<User> users = JsonSerializer.Deserialize<List<User>>(jsonUsersString);
            List<Password> passwords = JsonSerializer.Deserialize<List<Password>>(jsonPasswordsString);
            int counter = 1;
            foreach (User user in users)
            {
                if (users[counter-1].getEmail() == email)
                {
                    byte[] storedPassword = passwords[counter - 1].password;
                    byte[] givenHashedPassword = Hasher.ComputeHash(password, passwords[counter - 1].salt);

                    if (StructuralComparisons.StructuralEqualityComparer.Equals(storedPassword, givenHashedPassword))
                    {
                        Debug.WriteLine("Authentication successful.");
                        return user;
                    }
                    
                    else
                    {
                        Debug.WriteLine(storedPassword + "  NOT   " + givenHashedPassword);
                        Debug.WriteLine("Password didn't match.");
                        return null;
                    }
                }
                counter++;
            }

            return null;
        }

    }
}
