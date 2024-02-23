using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Raportti()
        {
            InitializeComponent();
            tapahtuma = new ObservableCollection<ajoTapahtuma>();

            // kusiset tapahtumat
            tapahtuma.Add(new ajoTapahtuma { Date = DateTime.Today.AddDays(-1), kmDriven = 50, korvausMaara = 25.00, henkilo = "faijas perse" });
            tapahtuma.Add(new ajoTapahtuma { Date = DateTime.Today.AddDays(-2), kmDriven = 40, korvausMaara = 20.00, henkilo = "Rock Ari" });

            //KORVAA DateTime.Now/*startDate*/ .xaml datepicker ajansyötöllä
            //KORVAA DateTime.Now/*endDate*/ .xaml datepicker ajansyötöllä
            for (DateTime currentDate = DateTime.Now/*startDate*/; currentDate <= DateTime.Now/*edDate*/; currentDate = currentDate.AddDays(1))
            {
                Trip trip = handler.GetDailyTrip(currentDate);
                if(trip != null) 
                {
                    string name = handler.currentUser.first_name + handler.currentUser.last_name;

                    //korvaus määrälle funktio joka laskee trp.km saadusta matkan pituudesta korvattavan määrän
                    tapahtuma.Add(new ajoTapahtuma { Date = currentDate, kmDriven = trip.km, korvausMaara = 20.00, henkilo = name });
                }
            }



            mileageDataGrid.ItemsSource = tapahtuma;
        }

        private int recordCount = 1; // lisays

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            tapahtuma.Add(new ajoTapahtuma { henkilo = $"henkilo {recordCount}" }); //lisaanumerohenkilonjalkeen
            recordCount++; // +1
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