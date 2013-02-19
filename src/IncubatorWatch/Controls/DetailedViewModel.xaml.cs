using System;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay;
using IncubatorWatch.Manager;
using System.ComponentModel;
using IncubatorWatch.Info;
using System.Threading;


namespace IncubatorWatch.Controls
{
    delegate void SetMessageCallback(String message);

    /// <summary>
    /// Interaction logic for DetailedViewModel.xaml
    /// </summary>
    public partial class DetailedViewModel : INotifyPropertyChanged
    {
        #region Private Variables
        private IncubatorManager _incubatorMnager = new IncubatorManager();
        private BackgroundWorker bw = new BackgroundWorker();
        #endregion

        public static DetailedViewModel _instance;

        private double _targetTemperature;
        public double TargetTemperature
        {
          get { return _targetTemperature; }
          set { _targetTemperature = value; this.OnPropertyChanged("TargetTemperature"); }
        }

        private double _targetRelativeHumidity;
        public double TargetRelativeHumidity
        {
          get { return _targetRelativeHumidity; }
          set { _targetRelativeHumidity = value; this.OnPropertyChanged("TargetRelativeHumidity"); }
        }

        private double _targetCO2;
        public double TargetCO2
        {
          get { return _targetCO2; }
          set { _targetCO2 = value; this.OnPropertyChanged("TargetCO2"); }
        }

        private String _actuatorButtonText;
        public String ActuatorButtonText
        {
            get { return _actuatorButtonText; }
            set { _actuatorButtonText = value; this.OnPropertyChanged("ActuatorButtonText"); }
        }

        

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion

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

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
          Visibility visibilityState = labelTilt.Visibility;
          BackgroundWorker worker = sender as BackgroundWorker;

          while (_incubatorMnager.State == ActuatorState.Opening || _incubatorMnager.State == ActuatorState.Closing)
          {
              if (visibilityState == Visibility.Visible)
            {
                visibilityState = Visibility.Hidden;
            }
            else if (visibilityState == Visibility.Hidden)
            {
                visibilityState = Visibility.Visible;
            }

            this.Dispatcher.Invoke((Action)(() => { SetLabelVisibility(visibilityState); }));

            Thread.Sleep(500);
          }

          if (bw.WorkerSupportsCancellation == true)
          {
            bw.CancelAsync();
            this.Dispatcher.Invoke((Action)(() => { SetLabelVisibility(Visibility.Visible); }));
          }
        }

        private void SetLabelVisibility(Visibility visibilityState)
        {
            labelTilt.Visibility = visibilityState;
            labelTiltTitle.Visibility = visibilityState;
        }

        private void InitializePlotter()
        {
            try
            {
                EnumerableDataSource<IncubatorData> receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
                receivedGraph.SetXMapping(x => temperatureTimeAxis.ConvertToDouble(x.Time));
                receivedGraph.SetYMapping(y => y.Temperature);
                plotterTemperature.AddLineGraph(receivedGraph, (Color)ColorConverter.ConvertFromString("#FF40B0E0"), 2, "Température");

                ViewportAxesRangeRestriction resT = new ViewportAxesRangeRestriction();
                resT.YRange = new DisplayRange(13, 27);
                plotterTemperature.Viewport.Restrictions.Add(resT);
                plotterTemperature.HorizontalAxis.Remove();

                plotterTemperature.Children.RemoveAll(typeof(Legend));

                ///////////////////////////

                receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
                receivedGraph.SetXMapping(x => relativeHumidityTimeAxis.ConvertToDouble(x.Time));
                receivedGraph.SetYMapping(y => y.RelativeHumidity);
                plotterRelativeHumidity.AddLineGraph(receivedGraph, (Color)ColorConverter.ConvertFromString("#FF40B0E0"), 2, "Humitidé Relative");
                
                ViewportAxesRangeRestriction restrRH = new ViewportAxesRangeRestriction();
                restrRH.YRange = new DisplayRange(-5, 105);
                plotterRelativeHumidity.Viewport.Restrictions.Add(restrRH);
                plotterRelativeHumidity.HorizontalAxis.Remove();

                plotterRelativeHumidity.Children.RemoveAll(typeof(Legend));

                ///////////////////////////

                receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorMnager.IncubatorData);
                receivedGraph.SetXMapping(x => CO2TimeAxis.ConvertToDouble(x.Time));
                receivedGraph.SetYMapping(y => y.CO2);
                plotterCO2.AddLineGraph(receivedGraph, (Color)ColorConverter.ConvertFromString("#FF40B0E0"), 2, "CO2");
                
                ViewportAxesRangeRestriction restrCO2 = new ViewportAxesRangeRestriction();
                restrCO2.YRange = new DisplayRange(300, 2200);
                plotterCO2.Viewport.Restrictions.Add(restrCO2);
                plotterCO2.HorizontalAxis.Remove();

                plotterCO2.Children.RemoveAll(typeof(Legend));
            }
            catch (Exception ex)
            {
              Console.Write(ex.ToString());
            }
        }

        public void OnUpdateTemperatureData(double temperature, double  targetTemperature, int heatPower)
        {
            try
            {
                labelTempratureValue.Content = temperature.ToString("F2") + " °C";
                TargetTemperature = targetTemperature;

                if (TemperatureTargetValue.Text == "??.??")
                {
                  TemperatureTargetValue.Text = targetTemperature.ToString("F2");
                }

                labelWatts.Content = heatPower.ToString() + " watts";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OnUpdateRelativeHumidityData(double relativeHumidity, double targetRelativeHumidity, int pumpState)
        {
          try
          {
            labelRelativeHumidityValue.Content = relativeHumidity.ToString("F2") + " %";
            TargetRelativeHumidity = targetRelativeHumidity;

            if (HumidityRelativeTargetValue.Text == "??.??")
            {
              HumidityRelativeTargetValue.Text = targetRelativeHumidity.ToString("F2");
            }

            String pumpTxt = "Pompe: ";
            if (pumpState == 1)
            {
              pumpTxt = "ON";
            }
            else if (pumpState == 0)
            {
              pumpTxt = "OFF";
            }

            pumpOnOff.Content = pumpTxt;
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString());
          }
        }

        public void OnUpdateCO2Data(int co2, int targetCO2, int fanState)
        {
          try
          {
            labelCO2Value.Content = co2.ToString("F2") + " ppm";
            TargetCO2 = targetCO2;

            if (CO2TargetValue.Text == "????")
            {
              CO2TargetValue.Text = targetCO2.ToString("F2");
            }

            String fanTxt = "Ventillation: ";
            if (fanState == 1)
            {
              fanTxt = "ON";
            }
            else if (fanState == 0)
            {
              fanTxt = "OFF";
            }

            labelFan.Content = fanTxt;
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString());
          }
        }

        public void OnUpdateActuatorData(ActuatorMode mode, ActuatorState state, String actuatorDuration)
        {
            try
            {
                _incubatorMnager.Mode = mode;
                _incubatorMnager.State = state;

                if (mode == ActuatorMode.Manual || mode == ActuatorMode.ManualCentered)
                {
                    ActuatorButtonText = "Start inclinaison";
                }
                else if (mode == ActuatorMode.Auto)
                {
                    ActuatorButtonText = "Stop Inclinaison";
                }

                labelTilt.Content = "[ " + actuatorDuration + " ]  ";

                switch (state)
                {
                    case ActuatorState.Open:
                        labelTilt.Content += "Ouvert";
                    break;
                    case ActuatorState.Close:
                        labelTilt.Content += "Fermé";
                    break;
                    case ActuatorState.Opening:
                    {
                      if (bw.IsBusy != true)
                      {
                        bw.RunWorkerAsync();
                      }

                      labelTilt.Content += "Ouvre...";
                    }
                    break;
                    case ActuatorState.Closing:
                    {
                      if (bw.IsBusy != true)
                      {
                        bw.RunWorkerAsync();
                      }

                      labelTilt.Content += "Ferme...";
                    }
                    break;
                    case ActuatorState.Stopped:
                        labelTilt.Content += "Arrêté";
                    break;
                    case ActuatorState.Unknown:
                        labelTilt.Content += "Inconnue";
                    break;
                }
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

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
          try
          {
            double target = Convert.ToDouble(TemperatureTargetValue.Text);

            if (ValideTargetTemperature(target))
            {
                _incubatorMnager.SetTargetTemperature(target);
            }
            else
            {
                MessageBox.Show("Valeur invalide");
            }
          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.ToString());
          }
        }

        private bool ValideTargetTemperature(double target )
        {
            bool result = false;

            if (target > 0 && target < 50)
            {
                result = true;
            }

            return result;
        }

        private void buttonStartStopTilt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_incubatorMnager.Mode == ActuatorMode.Auto)
                {
                    MessageBoxResult result = MessageBox.Show("Voulez-vous centrer les plateaux en allant en mode manuel ?", "Inclinaison", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result  == MessageBoxResult.Yes)
                    {
                        _incubatorMnager.SendActuatorMode(ActuatorMode.ManualCentered);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        _incubatorMnager.SendActuatorMode(ActuatorMode.Manual);
                    }
                }
                else if (_incubatorMnager.Mode == ActuatorMode.Manual || _incubatorMnager.Mode == ActuatorMode.ManualCentered)
                {
                    MessageBoxResult result = MessageBox.Show("Voulez-vous passer en mode inclinaison automatique ?", "Inclinaison", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _incubatorMnager.SendActuatorMode(ActuatorMode.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}