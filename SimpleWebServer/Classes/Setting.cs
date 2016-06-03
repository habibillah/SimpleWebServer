using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;

namespace SimpleWebServer.Classes
{
    [Serializable]
    public sealed class Setting
    {
        private static volatile Setting instance;
        private static object syncRoot = new Object();

        public string directoryPath { get; set; }
        public int port { get; set; }

        private Setting() { }

        public static Setting Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Setting();
                    }
                }

                return instance;
            }
        }
    }
}
