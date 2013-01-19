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
            /*if (this.statusStrip1.InvokeRequired)
            {

            }
            else
            {

            }*/
        }

        public void Shutdown()
        {
            _asyncSocketListener.SendToNetduino("EXIT");
            _asyncSocketListener.StopListening();
        }
    }
}