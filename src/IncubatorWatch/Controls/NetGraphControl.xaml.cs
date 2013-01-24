using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro;
using IncubatorWatch.Manager;
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
            try
            {
                //Removing the Legend (pen description)
                //plotter.Children.RemoveAll(typeof(Legend));
            }
            catch (Exception)
            {
            }

            EnumerableDataSource<IncubatorData> receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
            receivedGraph.SetXMapping(x => timeAxis.ConvertToDouble(x.Time));
            receivedGraph.SetYMapping(y => y.Temperature);
            plotter.AddLineGraph(receivedGraph, Color.FromArgb(255, 0, 0, 255), 2, "Température");

            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            restr.YRange = new DisplayRange(18.5, 21.5);
            plotter.Viewport.Restrictions.Add(restr);
        }
    }
}