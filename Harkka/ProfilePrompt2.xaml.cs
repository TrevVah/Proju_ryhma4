using System.Windows;


namespace KilsatMassiks
{

    public partial class ProfilePrompt2 : Window
    {
        public ProfilePrompt2()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
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