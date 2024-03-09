using System;
using System.Security;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Diagnostics.Metrics;

namespace KilsatMassiks
{
    public partial class Profile : UserControl
    {
        List<KeyValuePair<string, RoutedEventHandler>> buttonHandlers = new List<KeyValuePair<string, RoutedEventHandler>>();
        private UserDataHandler handler = MainWindow.Instance.GetHandler();
        List<TextBox> textBoxes = new List<TextBox>();
        List<PasswordBox> passwordBoxes = new List<PasswordBox>();
        public Profile()
        {
            InitializeComponent();
            UpdateInformation();
        }

        private void ChangeFirstName_Click(object sender, RoutedEventArgs e)
        {
            AddTextBoxNextToButton(ChangeFirstName_Click, sender as Button);
        }

        private void ChangeLastName_Click(object sender, RoutedEventArgs e)
        {
            AddTextBoxNextToButton(ChangeLastName_Click, sender as Button);
        }

        private void ChangeEmail_Click(object sender, RoutedEventArgs e)
        {
            AddTextBoxNextToButton(ChangeEmail_Click, sender as Button);
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            AddPasswordBoxNextToButton(ChangePassword_Click, sender as Button);
        }

        private void UpdateInformation()
        {
            etunimi.Text = handler.currentUser.first_name;
            sukunimi.Text = handler.currentUser.last_name;
            sahkoposti.Text = handler.currentUser.email;
            salasana.Text = "********";
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            textBoxes.Clear();
            passwordBoxes.Clear();
            textBoxes = FindTextBoxes();
            passwordBoxes = FindPasswordBoxes();

            User tempUser = handler.currentUser;

            foreach (TextBox txtBox in textBoxes)
            {
                Debug.WriteLine(txtBox.Name);
                switch (txtBox.Name)
                {
                    case "FirstName_Input":
                        if (txtBox.Text == "") goto emptyInput;
                        tempUser.first_name = txtBox.Text;
                        break;
                    case "LastName_Input":
                        if (txtBox.Text == "") goto emptyInput;
                        tempUser.last_name = txtBox.Text;
                        break;
                    case "Email_Input":
                        if (txtBox.Text == "") goto emptyInput;
                        tempUser.email = txtBox.Text;
                        break;

                    default:
                        break;
                }
            }

            Password tempPassword = null;
            IntPtr ptr1;
            string? plainPSW = null;

            foreach (PasswordBox pswBox in passwordBoxes)
            {
                Debug.WriteLine(pswBox.Name);
                switch (pswBox.Name)
                {
                    case "Password_Input":
                        ptr1 = Marshal.SecureStringToBSTR(pswBox.SecurePassword);
                        plainPSW = Marshal.PtrToStringBSTR(ptr1);
                        byte[] newSalt = Hasher.GenerateSalt();
                        byte[] newHashedPassword = Hasher.ComputeHash(plainPSW,newSalt);
                        tempPassword = new Password(handler.currentUser.getID(), newHashedPassword, newSalt);
                        break;

                    default:
                        break;
                }
            }
            if (passwordBoxes.Count > 0)
            {   if (plainPSW == "") goto emptyInput;
                if (OpenProfilePromptWindow(plainPSW, handler.currentUser.getID()))
                {
                    handler.UpdateUser(tempUser);
                    if (tempPassword != null) { handler.UpdatePassword(tempUser, tempPassword); }
                    UpdateInformation();
                }
            }
            else
            {
                if (OpenProfilePrompt2Window(handler.currentUser.getID()))
                {
                    handler.UpdateUser(tempUser);
                    UpdateInformation();
                }
            }

        emptyInput:
            MessageBox.Show("Syöttölaatikot evät saa olla tyhjiä. \n" +
                            "Joko paina 'Peru', jotta syöttölaatikko katoaa tai syötä halumasi muutos.");
        }

        private bool OpenProfilePrompt2Window(int userID)
        {
            ProfilePrompt2 profilePrompt = new ProfilePrompt2();

            bool? result = profilePrompt.ShowDialog();

            if (result == true)
            {
                string oldPassword = ConvertToPlainText(profilePrompt.topMarginTextBox.SecurePassword);
                string jsonPasswordFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Passwords.json");
                string jsonPasswordsString = File.ReadAllText(jsonPasswordFilePath);
                List<Password> passwords = JsonSerializer.Deserialize<List<Password>>(jsonPasswordsString);
                byte[] storedPassword = passwords[userID - 1].password;
                byte[] givenHashedPassword = Hasher.ComputeHash(oldPassword, passwords[userID - 1].salt);

                if (!StructuralComparisons.StructuralEqualityComparer.Equals(storedPassword, givenHashedPassword))
                {
                    Debug.WriteLine("Authentication failed.");
                    MessageBox.Show("Vanha salasana ei vastaa tietokantaan tallenettua salasanaa.", "Message", MessageBoxButton.OK);
                    return false;
                }
                MessageBox.Show("Uudet tiedot on tallennettu tietokantaan.", "Message", MessageBoxButton.OK);
                Debug.WriteLine("Authentication successful.");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool OpenProfilePromptWindow(string _password, int userID)
        {
            ProfilePrompt profilePrompt = new ProfilePrompt();

            bool? result = profilePrompt.ShowDialog();

            if (result == true)
            {
                string newPassword = ConvertToPlainText(profilePrompt.leftMarginTextBox.SecurePassword);
                string oldPassword = ConvertToPlainText(profilePrompt.topMarginTextBox.SecurePassword);
                if (newPassword != _password) 
                {
                    Debug.WriteLine("Uuden salasanan uudelleen kirjoitus ei täsmännyt haluttuun salasanaan.");
                    MessageBox.Show("New password doesn't match the given in form.", "Message" ,MessageBoxButton.OK);
                    return false; 
                }
                Debug.WriteLine($"{_password} matches {newPassword}");

                string jsonPasswordFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Passwords.json");
                string jsonPasswordsString = File.ReadAllText(jsonPasswordFilePath);
                List<Password> passwords = JsonSerializer.Deserialize<List<Password>>(jsonPasswordsString);
                byte[] storedPassword = passwords[userID-1].password;
                byte[] givenHashedPassword = Hasher.ComputeHash(oldPassword, passwords[userID - 1].salt);

                if (!StructuralComparisons.StructuralEqualityComparer.Equals(storedPassword, givenHashedPassword))
                {
                    Debug.WriteLine("Authentication failed.");
                    MessageBox.Show("Vanha salasana ei vastaa tietokantaan tallenettua salasanaa.", "Message", MessageBoxButton.OK);
                    return false;
                }
                MessageBox.Show("Uudet tiedot on tallennettu tietokantaan.", "Message", MessageBoxButton.OK);
                Debug.WriteLine("Authentication successful.");
                return true;
            }
            else
            {
                return false;
            }
        }

        private string ConvertToPlainText(SecureString secureString)
        {
            if (secureString == null)
                return string.Empty;

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        private List<TextBox> FindTextBoxes()
        {
            var txtBoxes = new List<TextBox>();

            var txtBoxGrid = FindName("TheGrid") as Grid;
            if (txtBoxGrid != null)
            {
                textBoxes.AddRange(txtBoxGrid.Children.OfType<TextBox>());
            }

            return textBoxes;
        }

        private List<PasswordBox> FindPasswordBoxes()
        {
            var pswBoxes = new List<PasswordBox>();

            var pswBoxGrid = FindName("TheGrid") as Grid;
            if (pswBoxGrid != null)
            {
               pswBoxes.AddRange(pswBoxGrid.Children.OfType<PasswordBox>());
            }

            return pswBoxes;
        }

        private void CancelInfoInput(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            string textBoxName = button.Name.Substring(0, button.Name.LastIndexOf("_")) + "_Input";
            string passwordBoxName = button.Name.Substring(0, button.Name.LastIndexOf("_")) + "_Input";
            Debug.WriteLine($"Removing the input box named: {textBoxName}.");

            var textBoxToRemove = ((Grid)button.Parent).Children.OfType<TextBox>().FirstOrDefault(tb => tb.Name == textBoxName);
            var passwordBoxToRemove = ((Grid)button.Parent).Children.OfType<PasswordBox>().FirstOrDefault(pb => pb.Name == passwordBoxName);
            if (textBoxToRemove != null)
            {
                ((Grid)button.Parent).Children.Remove(textBoxToRemove);
            }
            if (passwordBoxToRemove != null)
            {
                ((Grid)button.Parent).Children.Remove(passwordBoxToRemove);
            }

            var handlerPair = buttonHandlers.FirstOrDefault(pair => pair.Key == button.Name);
            if (handlerPair.Key != null)
            {
                button.Click -= CancelInfoInput;
                button.Click += handlerPair.Value;
                buttonHandlers.Remove(handlerPair);
            }

            Debug.WriteLine($"{buttonHandlers.Count()}");

            int originalColumn = Grid.GetColumn(button);
            Grid.SetColumn(button, originalColumn - 1);
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Content = "Vaihda";
        }

        private void AddTextBoxNextToButton(RoutedEventHandler handler, Button _button)
        {
            Debug.WriteLine($"Name of the button: {_button.Name}.");
            TextBox newTextBox = new TextBox();
            string textBoxName = _button.Name.Substring(0, _button.Name.LastIndexOf("_")) + "_Input";
            Debug.WriteLine($"Adding textbox with name: {textBoxName}");

            newTextBox.Name = textBoxName;

            if ( textBoxName == "FirstName_Input") { newTextBox.Text = etunimi.Text; }
            if (textBoxName == "LastName_Input") { newTextBox.Text = sukunimi.Text; }
            if (textBoxName == "Email_Input") { newTextBox.Text = sahkoposti.Text; }

            newTextBox.Margin = new Thickness(0, 0, 30, 0);
            newTextBox.MinWidth = 50;
            newTextBox.VerticalAlignment = VerticalAlignment.Center;

            Button button = _button;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            Grid.SetColumn(newTextBox, column);
            Grid.SetRow(newTextBox, row);

            Grid.SetColumn(button, column + 1);
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.Content = "Peru";

            buttonHandlers.Add(new KeyValuePair<string, RoutedEventHandler>(_button.Name, handler));

            button.Click -= handler;
            button.Click += CancelInfoInput;

            ((Grid)button.Parent).Children.Insert(((Grid)button.Parent).Children.IndexOf(button), newTextBox);
        }

        private void AddPasswordBoxNextToButton(RoutedEventHandler handler, Button _button)
        {
            Debug.WriteLine($"Name of the button: {_button.Name}.");
            PasswordBox newPasswordBox = new PasswordBox();
            string textBoxName = _button.Name.Substring(0, _button.Name.LastIndexOf("_")) + "_Input";
            Debug.WriteLine($"Adding textbox with name: {textBoxName}");

            newPasswordBox.Name = textBoxName;
            newPasswordBox.Margin = new Thickness(0, 0, 25, 0);
            newPasswordBox.MinWidth = 50;
            newPasswordBox.VerticalAlignment = VerticalAlignment.Center;

            Button button = _button;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            Grid.SetColumn(newPasswordBox, column);
            Grid.SetRow(newPasswordBox, row);

            Grid.SetColumn(button, column + 1);
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.Content = "Peru";

            buttonHandlers.Add(new KeyValuePair<string, RoutedEventHandler>(_button.Name, handler));

            button.Click -= handler;
            button.Click += CancelInfoInput;

            ((Grid)button.Parent).Children.Insert(((Grid)button.Parent).Children.IndexOf(button), newPasswordBox);
        }
    }
}
