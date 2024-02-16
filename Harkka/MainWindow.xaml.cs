using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace KilsatMassiks
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TabItem> _tabs;
        public ObservableCollection<TabItem> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                OnPropertyChanged("Tabs");
            }
        }

        private TabItem _selectedTab;
        public TabItem SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                OnPropertyChanged("SelectedTab");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Tabs = new ObservableCollection<TabItem>();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new TabItem();
            newTab.Header = $"Tab {Tabs.Count + 1}";
            newTab.Content = new calendar();
            Tabs.Add(newTab);
        }

    }
}