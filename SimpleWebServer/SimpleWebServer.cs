using SimpleWebServer.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebServer
{
    public partial class SimpleWebServer : ServiceBase
    {
        public static readonly string SERVICE_NAME = "SimpleWebServer";

        private SimpleHttpServer server;

        public SimpleWebServer()
        {
            InitializeComponent();
            this.ServiceName = SERVICE_NAME;

            Utility.LoadSetting();
        }

        protected override void OnStart(string[] args)
        {
            this.server = new Server(CurrentSetting.Instance.directoryPath, CurrentSetting.Instance.port);
            this.server.Start();
        }

        protected override void OnStop()
        {
            this.server.Stop();
        }
    }
}
