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
        public SimpleWebServer()
        {
            InitializeComponent();
            this.ServiceName = "SimpleWebServer";
        }

        protected override void OnStart(string[] args)
        {

        }

        protected override void OnStop()
        {

        }
    }
}
