using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using MahApps.Metro;


namespace IncubatorWatch.Controls
{
    /// <summary>
    /// Interaction logic for DetailedViewModel.xaml
    /// </summary>
    public partial class DetailedViewModel
    {
        public static DetailedViewModel Instance;

        public DetailedViewModel()
        {
            InitializeComponent();

            Instance = this;
        }

        private void MenuItemShowPropertiesDialogClick(object sender, RoutedEventArgs e)
        {
            if (LstViewData.SelectedIndex != -1)
            {
                try
                {
                    MessageBox.Show("MenuItemShowPropertiesDialogClick");
                }
                catch (Exception exp)
                {
                    Trace.WriteLine("Something Went Wrong! :" + exp.Message);
                }
            }
        }

        private void MnuItemSearchOnline(object sender, RoutedEventArgs e)
        {
            if (LstViewData.SelectedIndex != -1)
            {
                try
                {
                    MessageBox.Show("MnuItemSearchOnline");
                }
                catch (Exception exp)
                {
                    Trace.WriteLine("Something Went Wrong! :" + exp.Message);
                }
            }
        }

        private void MnuItemOpenFileLocation(object sender, RoutedEventArgs e)
        {
            if (LstViewData.SelectedIndex != -1)
            {
                try
                {
                    MessageBox.Show("MnuItemOpenFileLocation");
                }
                catch (Exception exp)
                {
                    Trace.WriteLine("Something Went Wrong! :" + exp.Message);
                }
            }
        }
    }
}