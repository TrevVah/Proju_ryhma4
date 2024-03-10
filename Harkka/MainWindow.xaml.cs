using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KilsatMassiks
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<UserDataHandler> users = new List<UserDataHandler>();

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CloseTabCommand { get; }
        public static MainWindow Instance { get; private set; }

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
            Check_User_Login();
            Instance = this;

            CloseTabCommand = new RelayCommand(TabCloseCommandExecute);

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            var newTab = CreateNewTabItem($"Profiili", new Profile());
            Tabs.Add(newTab);
            SelectedTab = newTab;
        }

        private void Check_User_Login()
        {
            if (users.Count == 0)
            {
                Window windowlogin = new LoginWindow();
                this.Hide();
                windowlogin.Show();
            }
        }

        private void OpenCalendarTab(object sender, RoutedEventArgs e)
        {
            var newTab = CreateNewTabItem($"Matkan lisäys", new calendar());
            Tabs.Add(newTab);
            SelectedTab = newTab;

        }

        public void SwarmApp(User _user)
        {
            users.Add(new UserDataHandler(_user));
            this.Show();
        }

        private void OpenReportTab(object sender, RoutedEventArgs e)
        {
            var newTab = CreateNewTabItem($"Raportin luonti", new Raportti());
            Tabs.Add(newTab);
            SelectedTab = newTab;
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            Button closeButton = sender as Button;
            TabItem tabItem = closeButton.Tag as TabItem;

            if (tabItem != null)
            {
                Tabs.Remove(tabItem);
            }
        }

        private void TabCloseCommandExecute(object parameter)
        {
            TabItem tabItem = parameter as TabItem;

            if (tabItem != null)
            {
                Tabs.Remove(tabItem);
            }
        }

        private TabItem CreateNewTabItem(string header, object content)
        {
            var newTab = new TabItem();
            newTab.Header = header;

            // Create a StackPanel to hold the header content and the close button
            var headerPanel = new StackPanel();
            headerPanel.Orientation = Orientation.Horizontal;

            var headerTextBlock = new TextBlock();
            headerTextBlock.Text = header;

            var image = new Image();
            image.Source = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "x.png"))); // Set the image source
            image.Width = 10;
            image.Height = 10;
            image.Margin = new Thickness(5, 0, 0, 0);
            image.Cursor = Cursors.Hand; // Set cursor to indicate it's clickable

            // Handle click event
            image.MouseLeftButtonUp += (sender, e) => Tabs.Remove(newTab);

            headerPanel.Children.Add(headerTextBlock);
            headerPanel.Children.Add(image);

            newTab.Header = headerPanel;
            newTab.Content = content;

            return newTab;
        }

        public UserDataHandler GetHandler()
        {
            return users[0];
        }
    }


}