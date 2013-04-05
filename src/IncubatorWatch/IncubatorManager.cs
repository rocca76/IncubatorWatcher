using System;
using System.Xml;
using System.IO;
using System.Windows;
using HatchWatch.Controls;
using HatchWatch.Info;

namespace HatchWatch.Manager
{
    public delegate void ReceivedEventHandler(String message);

    class IncubatorManager
    {
        #region Private Variables
        private readonly IncubatorDataCollection _incubatorDataCollection = new IncubatorDataCollection();
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
            int pumpIntervalTarget = (int)GetData(message, "pumpintervaltarget");
            int pumpDurationTarget = (int)GetData(message, "pumpdurationtarget");

            double co2 = GetData(message, "co2");
            double targetCO2 = GetData(message, "targetco2");
            FanStateEnum fanState = (FanStateEnum)GetData(message, "fanstate");
            TrapStateEnum trapState = (TrapStateEnum)GetData(message, "trapstate");
            VentilationState ventilationState = (VentilationState)GetData(message, "ventilationdstate");
            int ventilationIntervalTarget = (int)GetData(message, "ventilationintervaltarget");
            int ventilationDurationTarget = (int)GetData(message, "ventilationdurationtarget");
            String ventilationDuration = GetStringData(message, "ventilationduration");
            bool ventilationStandby = GetBooleanData(message, "ventilationstandby");

            ActuatorState actuatorState = (ActuatorState)GetData(message, "actuatorstate");
            String actuatorDuration = GetStringData(message, "actuatorduration");

            bool controlActivated = GetBooleanData(message, "controlactivated");

            this.IncubatorData.Add(new IncubatorData(DateTime.Now, temperature, relativeHumidity, (int)co2));

            DetailedViewModel.Instance.OnUpdateTemperatureData(temperature, targetTemperature, limitMaxTemperature, maxtemperaturereached, heatPower);

            DetailedViewModel.Instance.OnUpdateRelativeHumidityData(relativeHumidity, targetRelativeHumidity, fanState, trapState, pumpState, pumpDuration, pumpIntervalTarget, pumpDurationTarget);

            DetailedViewModel.Instance.OnUpdateCO2Data(co2, targetCO2);

            DetailedViewModel.Instance.OnUpdateVentilationData(fanState, trapState, ventilationState, ventilationDuration, ventilationIntervalTarget, ventilationDurationTarget, ventilationStandby);

            DetailedViewModel.Instance.OnUpdateActuatorData(actuatorState, actuatorDuration);

            DetailedViewModel.Instance.OnUpdateGeneral(controlActivated);
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
            string targetTxt = string.Format("TEMPERATURE_PARAMETERS {0} {1}", target, limitMax);

            CommunicationNetwork.Instance.Send(targetTxt);
        }

        public void SetTargetRelativeHumidity(double target, int intervalTarget, int durationTarget)
        {
          string targetTxt = string.Format("RELATIVE_HUMIDITY_PARAMETERS {0} {1} {2}", target, intervalTarget, durationTarget);

          CommunicationNetwork.Instance.Send(targetTxt);
        }

        public void SetTargetVentilation(int co2Target, int intervalTarget, int durationTarget)
        {
            string targetTxt = string.Format("VENTILATION_PARAMETERS {0} {1} {2}", co2Target, intervalTarget, durationTarget);

            CommunicationNetwork.Instance.Send(targetTxt);
        }

        public void SendActuatorCommand(ActuatorCommand command)
        {
            string commandTxt = string.Format("ACTUATOR_COMMAND {0}", Convert.ToInt32(command));

            CommunicationNetwork.Instance.Send(commandTxt);           
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

        public void SendPumpActivate(int activate)
        {
            string activateTxt = string.Format("PUMP_ACTIVATE {0}", activate);
            CommunicationNetwork.Instance.Send(activateTxt);
        }

        public void SendControlActivated(int activated)
        {
            string activateTxt = string.Format("CONTROL_ACTIVATED {0}", activated);
            CommunicationNetwork.Instance.Send(activateTxt);
        }   
        #endregion
    }
}
