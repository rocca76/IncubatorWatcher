using System.ComponentModel;
using System.Linq;
using System.Windows;
using MahApps.Metro;
using System.Collections.Generic;


namespace IncubatorWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static MainWindow Instance;

        private double _motor;
        public double Motor
        {
            get { return _motor; }
            set { _motor = value; this.OnPropertyChanged("Motor"); }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            //ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Netw"), Theme.Light);

            //detailedViewModelGadget.TargetTemperature = 11;
            Motor = 0.123;
        }

        public bool ProcessCommandLineArgs(IList<string> args)
        {
            ShowHideApplication();
            return true;
        }

        private void Button1Click(object sender, RoutedEventArgs e)
        {
          MessageBox.Show("Button1Click");
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void TaskbarIconTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ShowHideApplication();
        }

        private void MnuItemExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void MnuItemShowIncubatorWatcherClick(object sender, RoutedEventArgs e)
        {
            ShowHideApplication();
        }

        private void ShowHideApplication()
        {
            Show();
            Focus();
        }
    }
}