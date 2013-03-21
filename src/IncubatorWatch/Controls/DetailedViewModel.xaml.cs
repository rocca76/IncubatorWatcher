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
        private IncubatorManager _incubatorManager = new IncubatorManager();
        private BackgroundWorker bw = new BackgroundWorker();
        #endregion

        public static DetailedViewModel _instance;

        private double _targetTemperature;
        public double TargetTemperature
        {
          get { return _targetTemperature; }
          set { _targetTemperature = value; this.OnPropertyChanged("TargetTemperature"); }
        }

        private double _limitMaxTemperature;
        public double LimitMaxTemperature
        {
            get { return _limitMaxTemperature; }
            set { _limitMaxTemperature = value; this.OnPropertyChanged("LimitMaxTemperature"); }
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
            _incubatorManager.EventHandlerMessageReceived += new ReceivedEventHandler(OnMessageReceived);

            _instance = this;

            TargetTemperature = 0.0;
            LimitMaxTemperature = 0.0;
            TargetRelativeHumidity = 0.0;
            TargetCO2 = 0;
        }

        private void InitializePlotter()
        {
            try
            {
                EnumerableDataSource<IncubatorData> receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorManager.IncubatorData);
                receivedGraph.SetXMapping(x => temperatureTimeAxis.ConvertToDouble(x.Time));
                receivedGraph.SetYMapping(y => y.Temperature);
                plotterTemperature.AddLineGraph(receivedGraph, (Color)ColorConverter.ConvertFromString("#FF40B0E0"), 2, "Température");

                ViewportAxesRangeRestriction resT = new ViewportAxesRangeRestriction();
                resT.YRange = new DisplayRange(19.5, 40.5);
                plotterTemperature.Viewport.Restrictions.Add(resT);
                plotterTemperature.HorizontalAxis.Remove();

                plotterTemperature.Children.RemoveAll(typeof(Legend));

                ///////////////////////////

                receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorManager.IncubatorData);
                receivedGraph.SetXMapping(x => relativeHumidityTimeAxis.ConvertToDouble(x.Time));
                receivedGraph.SetYMapping(y => y.RelativeHumidity);
                plotterRelativeHumidity.AddLineGraph(receivedGraph, (Color)ColorConverter.ConvertFromString("#FF40B0E0"), 2, "Humitidé Relative");
                
                ViewportAxesRangeRestriction restrRH = new ViewportAxesRangeRestriction();
                restrRH.YRange = new DisplayRange(-5, 105);
                plotterRelativeHumidity.Viewport.Restrictions.Add(restrRH);
                plotterRelativeHumidity.HorizontalAxis.Remove();

                plotterRelativeHumidity.Children.RemoveAll(typeof(Legend));

                ///////////////////////////

                receivedGraph = new EnumerableDataSource<IncubatorData>(_incubatorManager.IncubatorData);
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

        public void OnUpdateTemperatureData(double temperature, double targetTemperature, double limitMaxTemperature, bool maxtemperaturereached, int heatPower)
        {
            try
            {
                if (temperature != double.MaxValue)
                {
                    tempratureValue.Content = temperature.ToString("F2") + " °C";
                }

                if (targetTemperature != double.MaxValue)
                {
                    TargetTemperature = targetTemperature;

                    if (targetTemperatureValue.Text == "??.??")
                    {
                        targetTemperatureValue.Text = targetTemperature.ToString("F2");
                    }
                }

                if (limitMaxTemperature != double.MaxValue)
                {
                    LimitMaxTemperature = limitMaxTemperature;

                    if (limitMaxTemperatureValue.Text == "??.??")
                    {
                        limitMaxTemperatureValue.Text = limitMaxTemperature.ToString("F2");
                    }
                }

                if (maxtemperaturereached == false)
                {
                    tempratureValue.Foreground = Brushes.Black;
                }
                else
                {
                    tempratureValue.Foreground = Brushes.Red;
                }

                if (heatPower != int.MaxValue)
                {
                    if (heatPower == 0)
                    {
                        heaterWatts.Foreground = Brushes.Black;
                    }
                    else
                    {
                        heaterWatts.Foreground = Brushes.Green;
                    }

                    heaterWatts.Content = heatPower.ToString() + " Watts";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OnUpdateRelativeHumidityData(double relativeHumidity, double targetRelativeHumidity, FanStateEnum fanState,
                                                 TrapStateEnum trapState, PumpStateEnum pumpState, String pumpDuration, 
                                                 int pumpIntervalTarget, int pumpDurationTarget)
        {
          try
          {
            if (relativeHumidity != double.MaxValue)
            {
                relativeHumidityValue.Content = relativeHumidity.ToString("F2") + " %";
            }

            if (targetRelativeHumidity != double.MaxValue)
            {
                TargetRelativeHumidity = targetRelativeHumidity;

                if (targetRelativeHumidityValue.Text == "???.??")
                {
                    targetRelativeHumidityValue.Text = targetRelativeHumidity.ToString("F2");
                }
            }

            if (pumpIntervalTarget != double.MaxValue)
            {
              if (pumpIntervalTxtBox.Text == "????")
              {
                pumpIntervalTxtBox.Text = pumpIntervalTarget.ToString();
              }
            }

            if (pumpDurationTarget != double.MaxValue)
            {
              if (pumpDurationTxtBox.Text == "????")
              {
                pumpDurationTxtBox.Text = pumpDurationTarget.ToString();
              }
            }


            if (trapState == TrapStateEnum.Closed)
            {
                trapOnOff.Content = "Cheminée: Fermé";
                trapOnOff.Foreground = Brushes.Black;
            }
            else if (trapState == TrapStateEnum.Opened)
            {
                trapOnOff.Content = "Cheminée: Ouverte";
                trapOnOff.Foreground = Brushes.Green;
            }

            if (fanState == FanStateEnum.Stopped)
            {
                fanOnOff.Content = "Fan: OFF";
                fanOnOff.Foreground = Brushes.Black;
            }   
            else if (fanState == FanStateEnum.Running)
            {
                fanOnOff.Content = "Fan: ON";
                fanOnOff.Foreground = Brushes.Green;
            }

            if (pumpState == PumpStateEnum.Stopped)
            {
                pumpOnOff.Content = "Pompe: OFF";

                if (pumpDuration == "00:00:00")
                {
                    pumpOnOff.Foreground = Brushes.Black;
                }
                else
                {
                    pumpOnOff.Foreground = Brushes.OrangeRed;
                }
            }
            else if (pumpState == PumpStateEnum.Running)
            {
                pumpOnOff.Content = "Pompe: ON";
                pumpOnOff.Foreground = Brushes.Green;
            }
            
            pumpOnOff.Content += " [ " + pumpDuration + " ] ";
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString());
          }
        }

        public void OnUpdateCO2Data(double co2, double targetCO2)
        {
            try
            {
                if (co2 != double.MaxValue)
                {
                    double co2Percent = co2 / 10000;
                    co2Value.Content = co2.ToString() + " ppm" + " | " + co2Percent.ToString("F4") + " %";
                }

                if (targetCO2 != double.MaxValue)
                {
                    TargetCO2 = targetCO2;

                    if (targetCO2Value.Text == "????")
                    {
                        targetCO2Value.Text = targetCO2.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OnUpdateVentilationData(FanStateEnum fanState, TrapStateEnum trapState, VentilationState ventilationState)
        {
          try
          {
            String ventilationTxt = "Ventilation: ??";

            if (ventilationState == VentilationState.Stopped)
            {
                ventilationTxt = "Ventilation: OFF";
                ventilationOnOff.Foreground = Brushes.Black;
            }
            else if (ventilationState == VentilationState.Started)
            {
                ventilationTxt = "Ventilation: ON";
                ventilationOnOff.Foreground = Brushes.Green;
            }

            ventilationOnOff.Content = ventilationTxt;
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString());
          }
        }

        public void OnUpdateActuatorData(ActuatorState state, String actuatorDuration)
        {
            try
            {
                _incubatorManager.State = state;

                if (state == ActuatorState.Unknown || state == ActuatorState.Stopped)
                {
                    ActuatorButtonText = "Démarrer l'inclinaison";
                }
                else if (state == ActuatorState.Paused)
                {
                    ActuatorButtonText = "Continuer l'inclinaison";
                }
                else
                {
                    ActuatorButtonText = "Arrêter l'inclinaison";
                }

                labelTilt.Content = "[ " + actuatorDuration + " ] ";
                labelTilt.Foreground = Brushes.Black;
                buttonOpenActuator.IsEnabled = true;
                buttonCloseActuator.IsEnabled = true;

                switch (state)
                {
                    case ActuatorState.Open:
                    {
                        labelTilt.Foreground = Brushes.OrangeRed;
                        labelTilt.Content += "Incliné à gauche";
                    }
                    break;
                    case ActuatorState.Close:
                    {
                        labelTilt.Foreground = Brushes.OrangeRed;
                        labelTilt.Content += "Incliné à droite";
                    }
                    break;
                    case ActuatorState.Opening:
                    {
                      labelTilt.Foreground = Brushes.Green;
                      labelTilt.Content += "Inclinaison vers la gauche...";
                      buttonOpenActuator.IsEnabled = false;
                      buttonCloseActuator.IsEnabled = false;
                    }
                    break;
                    case ActuatorState.Closing:
                    {
                      labelTilt.Foreground = Brushes.Green;
                      labelTilt.Content += "Inclinaison vers la droite...";
                      buttonOpenActuator.IsEnabled = false;
                      buttonCloseActuator.IsEnabled = false;
                    }
                    break;
                    case ActuatorState.Stopped:
                        labelTilt.Content += "Inclinaison arrêté";
                    break;
                    case ActuatorState.Unknown:
                        labelTilt.Content += "Inclinaison inconnue";
                    break;
                    case ActuatorState.Paused:
                        labelTilt.Content += "Inclinaison pausé";
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
            this.Dispatcher.BeginInvoke((Action)(() => { _incubatorManager.OnNewData(message); }));
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _incubatorManager.Shutdown();
        }

        private void buttonApplyTargetTemperature_Click(object sender, RoutedEventArgs e)
        {
          try
          {
            double target = Convert.ToDouble(targetTemperatureValue.Text);
            double limitMax = Convert.ToDouble(limitMaxTemperatureValue.Text);

            if (ValideTargetLimit(target, 0, 50) == false)
            {
                MessageBox.Show("La cible doit être entre 0 et 50 degrés celcius");
            }
            else if (ValideTargetLimit(limitMax, 0, 50) == false)
            {
                MessageBox.Show("La limit max doit être entre 0 et 50 degrés celcius");
            }
            else
            {
                _incubatorManager.SetTargetTemperature(target, limitMax);
            }
          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.ToString());
          }
        }

        private void buttonApplyTargetRelativeHumidity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double target = Convert.ToDouble(targetRelativeHumidityValue.Text);

                if (ValideTargetLimit(target, 0, 90) == false)
                {
                    MessageBox.Show("La cible doit être entre 0% et 90%");
                    return;
                }

                int intervalTarget = Convert.ToInt32(pumpIntervalTxtBox.Text);
                int durationTarget = Convert.ToInt32(pumpDurationTxtBox.Text);

                if (intervalTarget > 0 && durationTarget > 0)
                {
                    _incubatorManager.SetTargetRelativeHumidity(target, intervalTarget, durationTarget);
                }
                else
                {
                    MessageBox.Show("L'interval et la durée doivent être plus grand que 0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonApplyVentilation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int co2Target = Convert.ToInt32(targetCO2Value.Text);

                if (ValideTargetLimit(co2Target, 0, 10000) == false)
                {
                    MessageBox.Show("Valeur invalide");
                    return;
                }

                _incubatorManager.SetTargetVentilation(co2Target);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool ValideTargetLimit(double target, double limitMin, double limitMax)
        {
            bool result = false;

            if (target >= limitMin && target <= limitMax)
            {
                result = true;
            }

            return result;
        }

        private void buttonStartStopTilt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_incubatorManager.State == ActuatorState.Unknown || _incubatorManager.State == ActuatorState.Stopped)
                {
                    MessageBoxResult result = MessageBox.Show("Voulez-vous démarrer l'inclinaison automatique ?", "Inclinaison", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _incubatorManager.SendActuatorCommand(ActuatorCommand.Start);
                    }
                }
                else if (_incubatorManager.State == ActuatorState.Paused)
                {
                    MessageBoxResult result = MessageBox.Show("Voulez-vous continuer l'inclinaison automatique ?", "Inclinaison", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _incubatorManager.SendActuatorCommand(ActuatorCommand.Start);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        _incubatorManager.SendActuatorCommand(ActuatorCommand.Stop);
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Voulez-vous arrêter ?\nNon mettera l'inclinaison en pause", "Inclinaison", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _incubatorManager.SendActuatorCommand(ActuatorCommand.Stop);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        _incubatorManager.SendActuatorCommand(ActuatorCommand.Pause);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonCloseActuator_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _incubatorManager.SendActuatorClose(1);
        }

        private void buttonCloseActuator_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _incubatorManager.SendActuatorClose(0);
        }

        private void buttonOpenActuator_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _incubatorManager.SendActuatorOpen(1);
        }

        private void buttonOpenActuator_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _incubatorManager.SendActuatorOpen(0);
        }

        private void buttonActivatePump_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _incubatorManager.SendPumpActivate(1);
        }

        private void buttonActivatePump_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _incubatorManager.SendPumpActivate(0);
        }
    }
}