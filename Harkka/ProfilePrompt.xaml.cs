using System.Windows;

namespace KilsatMassiks
{
    public partial class ProfilePrompt : Window
    {
        public ProfilePrompt()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = leftMarginTextBox.Password;
            string oldPassword = topMarginTextBox.Password;


            DialogResult = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
                DialogResult = false;
        }
    }
}