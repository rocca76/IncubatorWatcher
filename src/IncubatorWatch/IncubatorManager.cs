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
        ActuatorMode _actuatorMode = ActuatorMode.Manual;
        ActuatorState _actuatorState = ActuatorState.Unknown;
        #endregion


        #region Events
        public event ReceivedEventHandler EventHandlerMessageReceived;
        #endregion


        #region Constructors
        public IncubatorManager()
        {
            CommunicationNetwork.EventHandlerMessageReceived += new MessageEventHandler(OnMessageReceived);
            CommunicationNetwork.Instance.Init();
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

        private bool GetBooleanData(String message, String variable)
        {
            bool value = false;

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(new StringReader(message)))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.IsStartElement(variable))
                        {
                            xmlReader.Read();
                            value = Convert.ToBoolean(xmlReader.Value);
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
        #endregion


        #region Public Methods
        public void OnNewData(String message)
        {
          try
          {
            double temperature = GetData(message, "temperature");
            double targetTemperature = GetData(message, "targettemperature");
            double limitMaxTemperature = GetData(message, "limitmaxtemperature");
            bool maxtemperaturereached = GetBooleanData(message, "maxtemperaturereached");
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
            CommunicationNetwork.Instance.Disconnect();
        }

        public void SetTargetTemperature(double target, double limitMax)
        {
            string targetTxt = string.Format("TARGET_TEMPERATURE {0} {1}", target, limitMax);

            CommunicationNetwork.Instance.Send(targetTxt);
        }    

        public void SetTargetRelativeHumidity(double target)
        {
          string targetTxt = string.Format("TARGET_RELATIVE_HUMIDITY {0}", target);

          CommunicationNetwork.Instance.Send(targetTxt);
        }

        public void SetTargetVentilation( int fanEnabled, int intervalTarget, int durationTarget, int co2Target)
        {
            string targetTxt = string.Format("TARGET_VENTILATION {0} {1} {2} {3}", fanEnabled, intervalTarget, durationTarget, co2Target);

            CommunicationNetwork.Instance.Send(targetTxt);
        }

        public void SendActuatorMode(ActuatorMode mode)
        {
            switch (mode)
            {
                case ActuatorMode.Manual:
                    CommunicationNetwork.Instance.Send("ACTUATOR_MODE MANUAL");
                    break;
                case ActuatorMode.ManualCentered:
                    CommunicationNetwork.Instance.Send("ACTUATOR_MODE MANUAL_CENTERED");
                    break;
                case ActuatorMode.Auto:
                    CommunicationNetwork.Instance.Send("ACTUATOR_MODE AUTO");
                    break;
            }            
        }

        public void SendActuatorClose(int close)
        {
            string actuatorTxt = string.Format("ACTUATOR_CLOSE {0}", close);
            CommunicationNetwork.Instance.Send(actuatorTxt);
        }

        public void SendActuatorOpen(int open)
        {
            string actuatorTxt = string.Format("ACTUATOR_OPEN {0}", open);
            CommunicationNetwork.Instance.Send(actuatorTxt);
        }
        #endregion
    }
}
