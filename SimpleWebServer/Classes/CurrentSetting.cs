using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebServer.Classes
{
    public sealed class CurrentSetting : Setting
    {
        private static volatile CurrentSetting instance;
        private static object syncRoot = new Object();

        private CurrentSetting() { }

        public static CurrentSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CurrentSetting();
                    }
                }

                return instance;
            }
        }
    }
}
