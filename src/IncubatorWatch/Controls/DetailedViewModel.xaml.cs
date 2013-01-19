using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using MahApps.Metro;
using IncubatorWatch.Communication;
using System.Xml;
using System.IO;


namespace IncubatorWatch.Controls
{
    delegate void SetMessageCallback(String message);

    /// <summary>
    /// Interaction logic for DetailedViewModel.xaml
    /// </summary>
    public partial class DetailedViewModel
    {
        public static DetailedViewModel _instance;
        private static AsynchronousSocketListener _asyncSocketListener;


        public static DetailedViewModel Instance
        {
            get { return _instance; }
        }

        public DetailedViewModel()
        {
            InitializeComponent();

            _instance = this;

            AsynchronousSocketListener.EventHandlerMessageReceived += new MessageEventHandler(OnMessageReceived);
            _asyncSocketListener = new AsynchronousSocketListener();

            _asyncSocketListener.SetTimeOnNetdino();
        }

        private void OnMessageReceived(String message)
        {
             RefreshLables(message);

            /*if (this.statusStrip1.InvokeRequired)
            {

            }
            else
            {

            }*/
        }

        public void RefreshLables(String message)
        {
            string value = GetData(message, "temperature");

            if (value.Length > 0)
            {
                lbl_TotalRcvd.Content = "Température: " + value + "°C";
            }

            value = GetData(message, "relativehumidity");

            if (value.Length > 0)
            {
                lbl_TotalSent.Content = "Humidité Relative: " + value + "%";
            }
        }

        private string GetData(String message, String variable)
        {
            string value = "";

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
                MessageBox.Show(message);
            }

            return value;
        }

        public void Shutdown()
        {
            _asyncSocketListener.SendToNetduino("EXIT");
            _asyncSocketListener.StopListening();
        }
    }
}