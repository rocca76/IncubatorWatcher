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

        private bool _isQuickModeEnable = true;
        private double _prevDetailedSizeHeight = 480;
        private double _prevDetailedSizeWidth = 720;
        private double _prevWinSizeHeight;
        private double _prevWinSizeWidth;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Netw"), Theme.Light);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _prevWinSizeWidth = Width;
            _prevWinSizeHeight = Height;
        }

        private void Button1Click(object sender, RoutedEventArgs e)
        {
            if (_isQuickModeEnable)
            {
                ChangeVisibilityOfEndTaskBtn(Visibility.Visible);
            }
            else
            {
                //detailedViewModelGadget.KillSelectedTask();
                MessageBox.Show("Button1Click");
            }
        }

        private void ExpanMoreLessInfoExpanded(object sender, RoutedEventArgs e)
        {
            expan_MoreLessInfo.Header = "Fewer details";
            _prevWinSizeWidth = Width;
            _prevWinSizeHeight = Height;

            Height = _prevDetailedSizeHeight;
            Width = _prevDetailedSizeWidth;

            detailedViewModelGadget.Visibility = Visibility.Visible;
            _isQuickModeEnable = false;
        }

        private void ExpanMoreLessInfoCollapsed(object sender, RoutedEventArgs e)
        {
            expan_MoreLessInfo.Header = "More details";

            _prevDetailedSizeWidth = Width;
            _prevDetailedSizeHeight = Height;

            Width = _prevWinSizeWidth;
            Height = _prevWinSizeHeight;

            detailedViewModelGadget.Visibility = Visibility.Hidden;
            _isQuickModeEnable = true;

            btnEndTask.Visibility = Visibility.Visible;
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
                //detailedViewModelGadget.SwitchTheme(color);
            }
            else if (color == Theme.Light)
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Netw"), Theme.Light);
                //detailedViewModelGadget.SwitchTheme(color);
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

        public void ChangeVisibilityOfEndTaskBtn(Visibility value)
        {
            btnEndTask.Visibility = value;
        }
    }
}