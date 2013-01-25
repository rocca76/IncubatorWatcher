﻿using System;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay;
using IncubatorWatch.Manager;


namespace IncubatorWatch.Controls
{
    delegate void SetMessageCallback(String message);

    /// <summary>
    /// Interaction logic for DetailedViewModel.xaml
    /// </summary>
    public partial class DetailedViewModel
    {
        #region Private Variables
        private IncubatorManager _incubatorMnager = new IncubatorManager();
        #endregion

        public static DetailedViewModel _instance;
       
        public static DetailedViewModel Instance
        {
            get { return _instance; }
        }

        public DetailedViewModel()
        {
            InitializeComponent();
            InitializePlotter();

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            _incubatorMnager.EventHandlerMessageReceived += new ReceivedEventHandler(OnMessageReceived);

            _instance = this;
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

            /*EnumerableDataSource<IncubatorData> receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
            receivedGraph.SetXMapping(x => temperatureTimeAxis.ConvertToDouble(x.Time));
            receivedGraph.SetYMapping(y => y.Temperature);
            plotterTemperature.AddLineGraph(receivedGraph, Color.FromArgb(255, 0, 0, 255), 2, "Température");

            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            restr.YRange = new DisplayRange(18.5, 21.5);
            plotterTemperature.Viewport.Restrictions.Add(restr);

            ///////////////////////////

            EnumerableDataSource<IncubatorData> receivedGraphRH = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
            receivedGraphRH.SetXMapping(x => temperatureTimeAxis.ConvertToDouble(x.Time));
            receivedGraphRH.SetYMapping(y => y.RelativeHumidity);
            plotterRelativeHumidity.AddLineGraph(receivedGraphRH, Color.FromArgb(255, 0, 0, 255), 2, "Humitidé Relative");

            ViewportAxesRangeRestriction restrRH = new ViewportAxesRangeRestriction();
            restrRH.YRange = new DisplayRange(29, 61);
            plotterRelativeHumidity.Viewport.Restrictions.Add(restrRH);*/
        }

        public void OnUpdateData(double temperature, double relativeHumidity)
        {
            try
            {
                /*lbl_TotalRcvd.Content = temperature.ToString("F2") + " °C";
                lbl_TotalSent.Content = relativeHumidity.ToString("F2") + " %";*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
    }
}