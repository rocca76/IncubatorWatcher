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
          BackgroundWorker worker = sender as BackgroundWorker;

          while (_incubatorMnager.State == ActuatorState.Opening || _incubatorMnager.State == ActuatorState.Closing)
          {
            if (labelTilt.Visibility == Visibility.Visible)
            {
              labelTilt.Visibility = Visibility.Hidden;
            }
            else if (labelTilt.Visibility == Visibility.Hidden)
            {
              labelTilt.Visibility = Visibility.Visible;
            }
            
            Thread.Sleep(500);
          }

          if (bw.WorkerSupportsCancellation == true)
          {
            bw.CancelAsync();
          }
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
                labelTemprature.Content = temperature.ToString("F2") + " °C";
                TargetTemperature = targetTemperature;

                if (targetTemperatureEdit.Text == "??.??")
                {
                    targetTemperatureEdit.Text = targetTemperature.ToString("F2");
                }

                labelWatts.Content = heatPower.ToString() + " watts";
                //labelMinMaxTemperature.Content = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OnUpdateData(double relativeHumidity, int co2)
        {
            try
            {
                labelRelativeHumidity.Content = relativeHumidity.ToString("F2") + " %";
                labelCO2Value.Content = co2.ToString() + " ppm";
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
            double target = Convert.ToDouble(targetTemperatureEdit.Text);

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