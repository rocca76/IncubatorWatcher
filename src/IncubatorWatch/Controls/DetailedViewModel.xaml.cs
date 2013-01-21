using System;
using System.Windows;


namespace IncubatorWatch.Controls
{
    delegate void SetMessageCallback(String message);

    /// <summary>
    /// Interaction logic for DetailedViewModel.xaml
    /// </summary>
    public partial class DetailedViewModel
    {
        public static DetailedViewModel _instance;
       
        public static DetailedViewModel Instance
        {
            get { return _instance; }
        }

        public DetailedViewModel()
        {
            InitializeComponent();

            _instance = this;
        }

        public void OnUpdateData(double temperature, double relativeHumidity)
        {
            try
            {
                lbl_TotalRcvd.Content = temperature.ToString("F2") + " °C";
                lbl_TotalSent.Content = relativeHumidity.ToString("F2") + " %";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}