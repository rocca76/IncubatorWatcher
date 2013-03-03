using System;
using IncubatorWatch.Communication;
using System.Xml;
using System.IO;
using System.Windows;
using IncubatorWatch.Controls;
using IncubatorWatch.Info;

namespace IncubatorWatch.Manager
{
    public delegate void ReceivedEventHandler(String message);

    class IncubatorManager
    {
        #region Private Variables
        private readonly IncubatorDataCollection _incubatorDataCollection = new IncubatorDataCollection();
        private static AsynchronousSocketListener _asyncSocketListener = new AsynchronousSocketListener();
        ActuatorMode _actuatorMode = ActuatorMode.Manual;
        ActuatorState _actuatorState = ActuatorState.Unknown;
        #endregion


        #region Events
        public event ReceivedEventHandler EventHandlerMessageReceived;
        #endregion


        #region Constructors
        public IncubatorManager()
        {
            AsynchronousSocketListener.EventHandlerMessageReceived += new MessageEventHandler(OnMessageReceived);

            Init();
        }
        #endregion


        #region Public Properties
        public IncubatorDataCollection IncubatorData
        {
            get { return _incubatorDataCollection; }
        }

        public ActuatorMode Mode
        {
            get { return _actuatorMode; }
            set { _actuatorMode = value; }
        }

        public ActuatorState State
        {
          get { return _actuatorState; }
          set { _actuatorState = value; }
        }
        #endregion

        #region Private Methods
        private void OnMessageReceived(String message)
        {
            if (EventHandlerMessageReceived != null)
            {
                EventHandlerMessageReceived(message);
            }
        }

        private double GetData(String message, String variable)
        {
            double value = double.MaxValue;

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(new StringReader(message)))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.IsStartElement(variable))
                        {
                            xmlReader.Read();
                            value = Convert.ToDouble(xmlReader.Value);
                        }
                    }
                    xmlReader.Close();
                }
            }
            catch (Exception ex)
            {
              Console.Write(ex.ToString());
            }

            return value;
        }

        private String GetStringData(String message, String variable)
        {
            String value = "";

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(new StringReader(message)))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.IsStartElement(variable))
                        {
                            xmlReader.Read();
                            value = xmlReader.Value;
                        }
                    }
                    xmlReader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return value;
        }

        private void Init()
        {
          string presentTime = string.Format("TIME {0} {1} {2} {3} {4} {5} {6}",
                                    DateTime.Now.Year,
                                    DateTime.Now.Month,
                                    DateTime.Now.Day,
                                    DateTime.Now.Hour,
                                    DateTime.Now.Minute,
                                    DateTime.Now.Second,
                                    DateTime.Now.Millisecond);

          _asyncSocketListener.SendMessage(presentTime);
        }
        #endregion


        #region Public Methods
        public void OnNewData(String message)
        {
          try
          {
            double temperature = GetData(message, "temperature");
            double targetTemperature = GetData(message, "targettemperature");
            double limitMaxTemperature = GetData(message, "limitmaxtemperature");
            int maxtemperaturereached = (int)GetData(message, "maxtemperaturereached");
            int heatPower = (int)GetData(message, "heatpower");

            double relativeHumidity = GetData(message, "relativehumidity");
            double targetRelativeHumidity = GetData(message, "targetrelativehumidity");
            PumpStateEnum pumpState = (PumpStateEnum)GetData(message, "pumpstate");
            String pumpDuration = GetStringData(message, "pumpduration");

            double co2 = GetData(message, "co2");
            double targetCO2 = GetData(message, "targetco2");

            FanStateEnum fanState = (FanStateEnum)GetData(message, "fanstate");
            TrapStateEnum trapState = (TrapStateEnum)GetData(message, "trapstate");
            String ventilationDuration = GetStringData(message, "ventilationduration");

            int fanEnabled = (int)GetData(message, "ventilationfanenabled");
            double ventilationIntervalTarget = GetData(message, "ventilationIntervaltarget");
            double ventilationDurationTarget = GetData(message, "ventilationdurationtarget");
            VentilationState ventilationState = (VentilationState)GetData(message, "ventilationdstate");
            

            ActuatorMode actuatorMode = (ActuatorMode)GetData(message, "actuatormode");
            ActuatorState actuatorState = (ActuatorState)GetData(message, "actuatorstate");
            String actuatorDuration = GetStringData(message, "actuatorduration");

            this.IncubatorData.Add(new IncubatorData(DateTime.Now, temperature, relativeHumidity, (int)co2));

            DetailedViewModel.Instance.OnUpdateTemperatureData(temperature, targetTemperature, limitMaxTemperature, maxtemperaturereached, heatPower);

            DetailedViewModel.Instance.OnUpdateRelativeHumidityData(relativeHumidity, targetRelativeHumidity, pumpState, pumpDuration);

            DetailedViewModel.Instance.OnUpdateCO2Data(co2, targetCO2);

            DetailedViewModel.Instance.OnUpdateVentilationData(fanState, trapState, ventilationDuration, fanEnabled, 
                                                               ventilationIntervalTarget, ventilationDurationTarget, ventilationState);

            DetailedViewModel.Instance.OnUpdateActuatorData(actuatorMode, actuatorState, actuatorDuration);
          }
          catch (Exception ex)
          {
            Console.Write(ex.ToString());
          }
        }

        public void Shutdown()
        {
          _asyncSocketListener.SendMessage("EXIT");
          _asyncSocketListener.StopListening();
        }

        public void SetTargetTemperature(double target)
        {
            string targetTxt = string.Format("TARGET_TEMPERATURE {0}", target);

            _asyncSocketListener.SendMessage(targetTxt);
        }

        public void SetLimitMaxTemperature(double limitMax)
        {
            string limitMaxTxt = string.Format("LIMIT_MAX_TEMPERATURE {0}", limitMax);

            _asyncSocketListener.SendMessage(limitMaxTxt);
        }        

        public void SetTargetRelativeHumidity(double target)
        {
          string targetTxt = string.Format("TARGET_RELATIVE_HUMIDITY {0}", target);

          _asyncSocketListener.SendMessage(targetTxt);
        }

        public void SetTargetVentilation( int fanEnabled, int intervalTarget, int durationTarget, int co2Target)
        {
            string targetTxt = string.Format("TARGET_VENTILATION {0} {1} {2} {3}", fanEnabled, intervalTarget, durationTarget, co2Target);

            _asyncSocketListener.SendMessage(targetTxt);
        }

        public void SendActuatorMode(ActuatorMode mode)
        {
            switch (mode)
            {
                case ActuatorMode.Manual:
                    _asyncSocketListener.SendMessage("ACTUATOR_MODE MANUAL");
                    break;
                case ActuatorMode.ManualCentered:
                    _asyncSocketListener.SendMessage("ACTUATOR_MODE MANUAL_CENTERED");
                    break;
                case ActuatorMode.Auto:
                    _asyncSocketListener.SendMessage("ACTUATOR_MODE AUTO");
                    break;
            }            
        }

        public void SendActuatorClose(int close)
        {
            string actuatorTxt = string.Format("ACTUATOR_CLOSE {0}", close);
            _asyncSocketListener.SendMessage(actuatorTxt);
        }

        public void SendActuatorOpen(int open)
        {
            string actuatorTxt = string.Format("ACTUATOR_OPEN {0}", open);
            _asyncSocketListener.SendMessage(actuatorTxt);
        }
        #endregion
    }
}
