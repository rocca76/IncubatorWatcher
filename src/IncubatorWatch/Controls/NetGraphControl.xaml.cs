using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace IncubatorWatch.Controls
{
    /// <summary>
    /// Interaction logic for NetGraphControl.xaml
    /// </summary>
    public partial class NetGraphControl
    {
        public long GlobalReceives = 0;
        public long GlobalSent = 0;

        public NetGraphControl()
        {
            InitializeComponent();

            InitializePlotter();

        }

        private void InitializePlotter()
        {
            /*var receivedGraph = new EnumerableDataSource<NetworkUsageData>(_networkUsage);
            receivedGraph.SetXMapping(x => timeAxis.ConvertToDouble(x.Time));
            receivedGraph.SetYMapping(y => y.ByteReceived);
            plotter.AddLineGraph(receivedGraph, Color.FromArgb(255, 0, 177, 255), 1, "ReceivedBytes");

            var brush = new SolidColorBrush(Color.FromArgb(255, 0, 177, 255));
            recLineReceived.Stroke = brush;

            var sentGraph = new EnumerableDataSource<NetworkUsageData>(_networkUsage);
            sentGraph.SetXMapping(x => timeAxis.ConvertToDouble(x.Time));
            sentGraph.SetYMapping(y => y.ByteSent);
            plotter.AddLineGraph(sentGraph, Colors.Red, 1, "SentBytes");
            recLineSent.Stroke = Brushes.Red;*/
        }

        public void SwitchTheme(Theme color)
        {
            if (color == Theme.Dark)
            {
                var brush = new SolidColorBrush(Color.FromRgb(37, 37, 37));
                Background = brush;
                plotter.Background = brush;
                lbl_Download.Foreground = Brushes.White;
                lbl_MaxMarker.Foreground = Brushes.White;
                lbl_Upload.Foreground = Brushes.White;
            }
            else if (color == Theme.Light)
            {
                Background = Brushes.White;
                plotter.Background = Brushes.White;

                lbl_Download.Foreground = Brushes.Black;
                lbl_MaxMarker.Foreground = Brushes.Black;
                lbl_Upload.Foreground = Brushes.Black;
            }
        }
    }
}