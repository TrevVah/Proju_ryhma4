using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            MainWindow.Instance.Show();
        }
    }
}
