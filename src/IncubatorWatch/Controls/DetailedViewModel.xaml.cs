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
                EnumerableDataSource<IncubatorData> receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
                receivedGraph.SetXMapping(x => temperatureTimeAxis.ConvertToDouble(x.Time));
                receivedGraph.SetYMapping(y => y.Temperature);
                plotterTemperature.AddLineGraph(receivedGraph, (Color)ColorConverter.ConvertFromString("#FFCB3500"), 2, "Température");

                ViewportAxesRangeRestriction resT = new ViewportAxesRangeRestriction();
                resT.YRange = new DisplayRange(18.5, 21.5);
                plotterTemperature.Viewport.Restrictions.Add(resT);
                plotterTemperature.HorizontalAxis.Remove();

                plotterTemperature.Children.RemoveAll(typeof(Legend));

                ///////////////////////////

                EnumerableDataSource<IncubatorData> receivedGraphRH = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
                receivedGraphRH.SetXMapping(x => relativeHumidityTimeAxis.ConvertToDouble(x.Time));
                receivedGraphRH.SetYMapping(y => y.RelativeHumidity);
                plotterRelativeHumidity.AddLineGraph(receivedGraphRH, (Color)ColorConverter.ConvertFromString("#FFCB3500"), 2, "Humitidé Relative");
                
                ViewportAxesRangeRestriction restrRH = new ViewportAxesRangeRestriction();
                restrRH.YRange = new DisplayRange(29, 61);
                plotterRelativeHumidity.Viewport.Restrictions.Add(restrRH);
                plotterRelativeHumidity.HorizontalAxis.Remove();

                plotterRelativeHumidity.Children.RemoveAll(typeof(Legend));

                ///////////////////////////

                EnumerableDataSource<IncubatorData> receivedGraphCO2 = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
                receivedGraphCO2.SetXMapping(x => CO2TimeAxis.ConvertToDouble(x.Time));
                receivedGraphCO2.SetYMapping(y => y.CO2);
                plotterCO2.AddLineGraph(receivedGraphCO2, (Color)ColorConverter.ConvertFromString("#FFCB3500"), 2, "CO2");
                
                ViewportAxesRangeRestriction restrCO2 = new ViewportAxesRangeRestriction();
                restrCO2.YRange = new DisplayRange(400, 1400);
                plotterCO2.Viewport.Restrictions.Add(restrCO2);
                plotterCO2.HorizontalAxis.Remove();

                plotterCO2.Children.RemoveAll(typeof(Legend));
            }
            catch (Exception)
            {
            }
        }

        public void OnUpdateData(double temperature, double relativeHumidity, int co2)
        {
            try
            {
                labelTemprature.Content = "Température: " + temperature.ToString("F2") + " °C";
                labelRelativeHumidity.Content = "Humidité Relative: " + relativeHumidity.ToString("F2") + " %";
                labelCO2.Content = "CO2: " + co2.ToString() + " ppm"; 
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