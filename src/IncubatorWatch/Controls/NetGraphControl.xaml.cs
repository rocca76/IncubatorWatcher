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
        private readonly IncubatorDataCollection _incubatorData = new IncubatorDataCollection(60);

        public NetGraphControl()
        {
            InitializeComponent();

            InitializePlotter();

            for (int i = 0; i < 60; i++)
            {
                DateTime dateNow = DateTime.Now;
                dateNow = dateNow.Subtract(TimeSpan.FromSeconds(60 - i));
                _incubatorData.Add(new IncubatorData(dateNow, 0, 0));
            }
            
            try
            {
                //Removing the Legend (pen description)
                plotter.Children.RemoveAll(typeof(Legend));
            }
            catch (Exception)
            {
            }
        }

        private void InitializePlotter()
        {
            var receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorData);
            receivedGraph.SetXMapping(x => timeAxis.ConvertToDouble(x.Time));
            receivedGraph.SetYMapping(y => y.Temperature);
            plotter.AddLineGraph(receivedGraph, Color.FromArgb(255, 0, 177, 255), 1, "Température");

            var brush = new SolidColorBrush(Color.FromArgb(255, 0, 177, 255));
            recLineTemperature.Stroke = brush;

            /*var sentGraph = new EnumerableDataSource<NetworkUsageData>(_networkUsage);
            sentGraph.SetXMapping(x => timeAxis.ConvertToDouble(x.Time));
            sentGraph.SetYMapping(y => y.ByteSent);
            plotter.AddLineGraph(sentGraph, Colors.Red, 1, "SentBytes");
            recLineSent.Stroke = Brushes.Red;*/
        }
    }
}