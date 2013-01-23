using System.ComponentModel;
using System.Linq;
using System.Windows;
using MahApps.Metro;


namespace IncubatorWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance;

        public MainWindow()
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

        private void SwitchTheme(Theme color)
        {
            if (color == Theme.Dark)
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Netw"), Theme.Dark);
            }
            else if (color == Theme.Light)
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Netw"), Theme.Light);
            }
        }

        private void BtnSettingsClick(object sender, RoutedEventArgs e)
        {
            if (BtnSwitchColors.Content.ToString() == "Dark")
            {
                SwitchTheme(Theme.Dark);
                BtnSwitchColors.Content = "Light";
            }
            else
            {
                SwitchTheme(Theme.Light);
                BtnSwitchColors.Content = "Dark";
            }
        }
    }
}