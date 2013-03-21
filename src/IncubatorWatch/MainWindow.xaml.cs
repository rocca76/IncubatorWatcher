using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using System;


namespace IncubatorWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static MainWindow Instance;

        private String _actuator;
        public String Actuator
        {
            get { return _actuator; }
            set { _actuator = value; this.OnPropertyChanged("Actuator"); }
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