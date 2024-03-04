using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KilsatMassiks
{
    /// <summary>
    /// Interaction logic for Raportti.xaml
    /// </summary>
    public partial class Raportti : UserControl
    {
        public ObservableCollection<ajoTapahtuma> tapahtuma { get; set; }

        //MainWindowissa luodun UserDataHandler classi, joka omistaa kirjautuneen käyttäjän currentUser muuttujassa.
        private UserDataHandler handler = MainWindow.Instance.GetHandler();
        DateTime mista1 = DateTime.Now;
        DateTime mihin1 = DateTime.Now;


        public Raportti()
        {
            InitializeComponent();
            tapahtuma = new ObservableCollection<ajoTapahtuma>();
        }

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            mista1 = mista.SelectedDate ?? DateTime.Now;
            mihin1 = mihin.SelectedDate ?? DateTime.Now;
            int randomNumber = new Random().Next(100, 1000);

            tapahtuma.Clear();
            Trip trip = null;
            for (DateTime currentDate = mista1; currentDate <= mihin1; currentDate = currentDate.AddDays(1))
            {
                trip = handler.GetDailyTrip(currentDate);
                if (trip != null)
                {
                    string name = handler.currentUser.first_name + handler.currentUser.last_name;
                    tapahtuma.Add(new ajoTapahtuma { Date = currentDate, kmDriven = trip.km, korvausMaara = randomNumber, henkilo = name });
                    Debug.WriteLine("Added: " + new ajoTapahtuma{ Date = currentDate, kmDriven = trip.km, korvausMaara = 20.00, henkilo = name });
                }
            }
            mileageDataGrid.ItemsSource = tapahtuma;
        }
        private void OnDataGridPrinting(object sender, RoutedEventArgs e)
        {
            string Title = handler.currentUser.first_name + handler.currentUser.last_name + handler.currentUser.userID;


            System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
            if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
            {
                Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
                // sizing of the element.
                mileageDataGrid.Measure(pageSize);
                mileageDataGrid.Arrange(new Rect(10, 10, pageSize.Width, pageSize.Height));
                Printdlg.PrintVisual(mileageDataGrid, Title);
            }
        }
    }

    public class ajoTapahtuma
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public int? kmDriven { get; set; } = 0;
        public double korvausMaara { get; set; } = 0;
        public string henkilo { get; set; } = "henkilo";

    }
}