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

        private const string FilePath = "C:\\Users\\Trevor\\Desktop\\Laukku\\Proju_ryhma4-trevorin_oksa\\Proju_ryhma4-trevorin_oksa\\Harkka\\data.json";
        public calendar()
        {
            InitializeComponent();
        }

        private void SyotaButtonClick(object sender, RoutedEventArgs e)
        {


            string kilometrit = KilometritTextBox.Text;
            string matkaAika = MatkaAikaTextBox.Text;
            string osoiteTiedot = OsoiteTiedotTextBox.Text;
            DateTime selectedDate = PVMDatePicker.SelectedDate ?? DateTime.Now;


            var newData = new
            {
                Kilometrit = kilometrit,
                MatkaAika = matkaAika,
                OsoiteTiedot = osoiteTiedot,
                PVM = selectedDate
            };

            string jsonData;

            if (File.Exists(FilePath))
            {

                string existingData = File.ReadAllText(FilePath);

                try
                {

                    var existingList = JsonConvert.DeserializeObject<List<object>>(existingData);


                    existingList.Add(newData);


                    jsonData = JsonConvert.SerializeObject(existingList, Newtonsoft.Json.Formatting.Indented);
                }
                catch (JsonSerializationException)
                {
                    try
                    {

                        var existingObject = JObject.Parse(existingData);


                        var newList = new List<object> { existingObject.ToObject<object>(), newData };


                        jsonData = JsonConvert.SerializeObject(newList, Newtonsoft.Json.Formatting.Indented);
                    }
                    catch (JsonReaderException)
                    {

                        jsonData = JsonConvert.SerializeObject(new List<object> { newData }, Newtonsoft.Json.Formatting.Indented);
                    }
                }
            }
            else
            {
                jsonData = JsonConvert.SerializeObject(new List<object> { newData }, Newtonsoft.Json.Formatting.Indented);
            }


            File.WriteAllText(FilePath, jsonData);


            MessageBox.Show("Data saved to JSON file.");
        }
    }
}

//C:\\Users\\Trevor\\Desktop\\Laukku\\Proju_ryhma4-trevorin_oksa\\Proju_ryhma4-trevorin_oksa\\Harkka\\data.json
