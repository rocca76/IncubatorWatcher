using System.ComponentModel;
using System.Linq;
using System.Windows;
using MahApps.Metro;
using Hardcodet.Wpf.TaskbarNotification;
using System.Drawing;
using System.Windows.Media;
using System;
using System.Windows.Media.Imaging;
using System.IO;


namespace IncubatorWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }
        private TaskbarIcon _taskbarIcon = null;

        static MainWindow()
        {
          Instance = new MainWindow();
        }

        private MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Netw"), Theme.Light);
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