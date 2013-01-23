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
            /*for (int i = 0; i < 60; i++)
            {
                DateTime dateNow = DateTime.Now;
                dateNow = dateNow.Subtract(TimeSpan.FromSeconds(60 - i));
                _incubatorDataCollection.Add(new IncubatorData(dateNow, 0, 0));
            }*/

            //this.IncubatorData.Add(new IncubatorData(DateTime.Now, 15, 20));
            //this.IncubatorData.Add(new IncubatorData(DateTime.Now, 30, 80));

            AsynchronousSocketListener.EventHandlerMessageReceived += new MessageEventHandler(OnMessageReceived);

            _asyncSocketListener.SetTimeOnNetdino();
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
        #endregion


        #region Public Methods
        public void OnNewData(String message)
        {
            try
            {
                double temperature = GetData(message, "temperature");
                double relativeHumidity = GetData(message, "relativehumidity");

                DetailedViewModel.Instance.OnUpdateData(temperature, relativeHumidity);

                this.IncubatorData.Add(new IncubatorData(DateTime.Now, temperature, relativeHumidity));
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        public void Shutdown()
        {
            _asyncSocketListener.SendToNetduino("EXIT");
            _asyncSocketListener.StopListening();
        }
        #endregion
    }
}
