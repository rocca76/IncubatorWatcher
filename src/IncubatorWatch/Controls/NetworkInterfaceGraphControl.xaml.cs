using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;


namespace IncubatorWatch.Controls
{
    /// <summary>
    /// Interaction logic for NetworkInterfaceGraphControl.xaml
    /// </summary>
    public partial class NetworkInterfaceGraphControl
    {
        public NetworkInterfaceGraphControl(List<NetworkInterface> selectednetworkInterface)
        {
            InitializeComponent();
        }

        public NetworkInterfaceGraphControl()
        {
            InitializeComponent();
        }

        public void ToggleVisibility(object obj)
        {
            var uiElement = obj as UIElement;
            if (uiElement != null)
                uiElement.Visibility = uiElement.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public void ToggleVisibilityOflblSent()
        {
            lblUpload.Visibility = lblUpload.Visibility == Visibility.Visible
                                         ? Visibility.Hidden
                                         : Visibility.Visible;

            recLineSent.Visibility = recLineSent.Visibility == Visibility.Visible
                                         ? Visibility.Hidden
                                         : Visibility.Visible;
        }

        public void ToggleVisibilityOfMaxVolume()
        {
            lblMaxVolumeSpeed.Visibility = lblMaxVolumeSpeed.Visibility == Visibility.Visible
                                         ? Visibility.Hidden
                                         : Visibility.Visible;
        }

        public void ToggleVisibilityOflblRcvd()
        {
            lblDownload.Visibility = lblDownload.Visibility == Visibility.Visible
                                          ? Visibility.Hidden
                                          : Visibility.Visible;

            recLineReceived.Visibility = recLineReceived.Visibility == Visibility.Visible
                                             ? Visibility.Hidden
                                             : Visibility.Visible;
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
    }
}