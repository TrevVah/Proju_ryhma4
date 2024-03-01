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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace KilsatMassiks
{
    /// <summary>
    /// Interaction logic for calendar.xaml
    /// </summary>
    public partial class calendar : UserControl
    {
        private const string FilePath = "";

        private UserDataHandler handler = MainWindow.Instance.GetHandler();

        public calendar()
        {
            InitializeComponent();
        }

        private void SyotaButtonClick(object sender, RoutedEventArgs e)
        {
            int kilometrit = 0;
            float matkaAika = 0;
            string osoiteTiedot = OsoiteTiedotTextBox.Text;
            try
            {
                 kilometrit = Int32.Parse(KilometritTextBox.Text);
                 matkaAika = ParseFloat(MatkaAikaTextBox.Text);
            }
            catch (FormatException)
            { 
                Console.WriteLine($"Unable to parse '{KilometritTextBox.Text}'"); 
            }

            DateTime selectedDate = PVMDatePicker.SelectedDate ?? DateTime.Now;


            handler.UpdateTrip(selectedDate, kilometrit, matkaAika, osoiteTiedot, 0);


            MessageBox.Show("Data saved to JSON file.");
        }

        static float ParseFloat(string input)
        {
            input = input.Replace(',', '.');

            if (float.TryParse(input, out float result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Type your input here...")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black; // or any other color you want for the entered text
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Gray;
            }
        }


    }



}


