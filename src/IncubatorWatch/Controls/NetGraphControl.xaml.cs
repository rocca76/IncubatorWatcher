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
using IncubatorWatch.Manager;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace IncubatorWatch.Controls
{
    /// <summary>
    /// Interaction logic for NetGraphControl.xaml
    /// </summary>
    public partial class NetGraphControl
    {
        #region Private Variables
        private IncubatorManager _incubatorMnager = new IncubatorManager();
        #endregion


        public NetGraphControl()
        {
            InitializeComponent();

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            _incubatorMnager.EventHandlerMessageReceived += new ReceivedEventHandler(OnMessageReceived);

            InitializePlotter();
            
            try
            {
                //Removing the Legend (pen description)
                plotter.Children.RemoveAll(typeof(Legend));
            }
            catch (Exception)
            {
            }
        }

        private void OnMessageReceived(String message)
        {
            this.Dispatcher.Invoke((Action)(() => { _incubatorMnager.OnNewData(message); }));
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _incubatorMnager.Shutdown();
        }

        private void InitializePlotter()
        {
            EnumerableDataSource<IncubatorData> receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
            receivedGraph.SetXMapping(x => timeAxis.ConvertToDouble(x.Time));
            receivedGraph.SetYMapping(y => y.Temperature);
            plotter.AddLineGraph(receivedGraph, Color.FromArgb(255, 0, 0, 255), 2, "Température");

            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            restr.YRange = new DisplayRange(15, 25);
            plotter.Viewport.Restrictions.Add(restr);
            
            //var axis = (DateTimeAxis)plotter.VerticalAxis;
            //double yMin = 0;
            //double yMax = 100;
            //Rect domainRect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
            //xMin and xMax are left to your discretion based on your DateTimeAxis

            /*double yMin = 1;
            double yMax = 10;
            double xMin = 1;
            double xMax = 10;
            plotter.Viewport.Visible = new DataRect(xMin, yMin, xMax - xMin, yMax - yMin);*/

            //plotter.Viewport.FitToView();

            //plotter.AddLineGraph(receivedGraph, Color.FromArgb(255, 0, 177, 255), 1, "Température");

            /*var sentGraph = new EnumerableDataSource<NetworkUsageData>(_networkUsage);
            sentGraph.SetXMapping(x => timeAxis.ConvertToDouble(x.Time));
            sentGraph.SetYMapping(y => y.ByteSent);
            plotter.AddLineGraph(sentGraph, Colors.Red, 1, "SentBytes");
            recLineSent.Stroke = Brushes.Red;*/
        }
    }
}