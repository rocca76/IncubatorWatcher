using System;
using IncubatorWatch.Communication;
using System.Xml;
using System.IO;
using System.Windows;
using IncubatorWatch.Controls;

namespace IncubatorWatch.Manager
{
    public delegate void ReceivedEventHandler(String message);

    class IncubatorManager
    {
        #region Private Variables
        private readonly IncubatorDataCollection _incubatorDataCollection = new IncubatorDataCollection();
        private static AsynchronousSocketListener _asyncSocketListener = new AsynchronousSocketListener();
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
            int heatPower = (int)GetData(message, "heatpower");
            double relativeHumidity = GetData(message, "relativehumidity");
            int co2 = (int)GetData(message, "co2");

            this.IncubatorData.Add(new IncubatorData(DateTime.Now, temperature, relativeHumidity, co2));

            DetailedViewModel.Instance.OnUpdateTemperatureData(temperature, targetTemperature, heatPower);

            DetailedViewModel.Instance.OnUpdateData(relativeHumidity, co2);
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

        public void SetTargetTemperature(double targetTemperature)
        {
            string targetTemperatureTxt = string.Format("TARGET_TEMPERATURE {0}", targetTemperature);

          _asyncSocketListener.SendMessage(targetTemperatureTxt);
        }
        #endregion
    }
}
