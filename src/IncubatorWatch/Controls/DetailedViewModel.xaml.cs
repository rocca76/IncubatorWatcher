using System;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay;
using IncubatorWatch.Manager;
using System.ComponentModel;
using IncubatorWatch.Info;


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
        #endregion

        public static DetailedViewModel _instance;

        private double _targetTemperature;
        public double TargetTemperature
        {
          get { return _targetTemperature; }
          set { _targetTemperature = value; this.OnPropertyChanged("TargetTemperature"); }
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

        public void OnUpdateActuatorData(ActuatorMode mode, ActuatorState state)
        {
            try
            {
                FrameworkElement parent = (FrameworkElement)this.Parent;

                while (true)
                {
                    if (parent is MainWindow)
                    {
                        String actuatorTxt = "Actuateur: ";

                        switch (mode)
                        {
                            case ActuatorMode.Manual:
                                actuatorTxt += "Manuel";
                                break;
                            case ActuatorMode.Auto:
                                actuatorTxt += "Automatique";
                                break;
                        }

                        actuatorTxt += " - ";

                        switch (state)
                        {
                            case ActuatorState.Open:
                                actuatorTxt += "Ouvert";
                                break;
                            case ActuatorState.Close:
                                actuatorTxt += "Fermé";
                                break;
                            case ActuatorState.Opening:
                                actuatorTxt += "Ouvre...";
                                break;
                            case ActuatorState.Closing:
                                actuatorTxt += "Ferme...";
                                break;
                            case ActuatorState.Stopped:
                                actuatorTxt += "Arrêté";
                                break;
                        }


                        ((MainWindow)parent).Actuator = actuatorTxt;
                        break;
                    }

                    parent = (FrameworkElement)parent.Parent;
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

        private void buttonOpenActuator_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                _incubatorMnager.SendActuatorCommand(ActuatorCommand.Open);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonOpenActuator_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                _incubatorMnager.SendActuatorCommand(ActuatorCommand.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonCloseActuator_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                _incubatorMnager.SendActuatorCommand(ActuatorCommand.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonCloseActuator_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                _incubatorMnager.SendActuatorCommand(ActuatorCommand.Close);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void radioBtnManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Passage au mode manuel ?", "Changement de mode", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _incubatorMnager.SendActuatorMode(ActuatorMode.Manual);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void radioBtnAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Passage au mode automatique ?", "Changement de mode", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _incubatorMnager.SendActuatorMode(ActuatorMode.Auto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}