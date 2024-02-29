using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
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

namespace KilsatMassiks
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        Action<string?> TryAddUser = result =>
        {
            if (result != null)
            {
                Debug.WriteLine(result);
            }
        };
        private void CloseUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoginWindow.Instance.Show();
        }

        public string? CheckBothPasswords(SecureString first_field, SecureString secnod_field)
        {
            IntPtr ptr1 = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(first_field);
            IntPtr ptr2 = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secnod_field);
            string plainPSW1 = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr1);
            string plainPSW2 = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr2);

            if (plainPSW1 == plainPSW2)
            {
                return plainPSW1;
            }
            else
            {
                return null;
            }
        }

        public string? AddUser(string _first_name, string _last_name, string _email, string _password)
        {
            Debug.WriteLine("Finding user database...");

            string jsonUserFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Users.json");
            string jsonPasswordFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Passwords.json");

            if (!File.Exists(jsonUserFilePath))
            {
                File.Create(jsonUserFilePath).Close();
                string jsonInitContent = "[\n]";
                File.WriteAllText(jsonUserFilePath, jsonInitContent);
                Debug.WriteLine("Users.json was not found and was created.");
            }

            if (!File.Exists(jsonPasswordFilePath))
            {
                File.Create(jsonPasswordFilePath).Close();
                string jsonInitContent = "[\n]";
                File.WriteAllText(jsonPasswordFilePath, jsonInitContent);
                Debug.WriteLine("Passwords.json was not found and was created.");
            }

            Debug.WriteLine("Adding an user...");
            {
                string jsonUsersString = File.ReadAllText(jsonUserFilePath);
                string jsonPasswordsString = File.ReadAllText(jsonPasswordFilePath);
                List<User> users = JsonSerializer.Deserialize<List<User>>(jsonUsersString);
                List<Password> passwords = JsonSerializer.Deserialize<List<Password>>(jsonPasswordsString);
                foreach (User user in users)
                {
                    if (user.getEmail() == _email)
                    {
                        return "User with that email already exist.";
                    }
                }

                User newUser = new User(users.Count + 1, _first_name, _last_name, _email);
                users.Add(newUser);

                jsonUsersString = JsonSerializer.Serialize(users);
                jsonUsersString = jsonUsersString.Replace("},{", "},\n{");
                jsonUsersString = jsonUsersString.Replace("[", "[\n");
                jsonUsersString = jsonUsersString.Replace("]", "\n]");

                Debug.WriteLine(JsonSerializer.Serialize(newUser));

                byte[] _salt = Hasher.GenerateSalt();
                byte[] _hashedPassword = Hasher.ComputeHash(_password, _salt);

                Password newPassword = new Password(users.Count, _hashedPassword, _salt);
                Debug.WriteLine(JsonSerializer.Serialize(newPassword));
                passwords.Add(newPassword);

                jsonPasswordsString = JsonSerializer.Serialize(passwords);
                jsonPasswordsString = jsonPasswordsString.Replace("},{", "},\n{");
                jsonPasswordsString = jsonPasswordsString.Replace("[", "[\n");
                jsonPasswordsString = jsonPasswordsString.Replace("]", "\n]");


                File.WriteAllText(jsonUserFilePath, jsonUsersString);
                File.WriteAllText(jsonPasswordFilePath, jsonPasswordsString);
                Debug.WriteLine("User added.");
            }

            return null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string givenPassword = CheckBothPasswords(password_field1.SecurePassword, password_field2.SecurePassword);
            if(givenPassword != null)
            {
                TryAddUser(AddUser(first_name_field.Text, last_name_field.Text, email_field.Text, givenPassword));
                MessageBox.Show($"Tunnukset luotu käyttäjälle {first_name_field.Text} {last_name_field.Text}.");
                this.Close();
            }
            else
            {
                Debug.WriteLine("Given passwords do not match.");
            }
           
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
