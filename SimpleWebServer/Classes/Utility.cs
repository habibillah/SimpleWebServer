using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebServer.Classes
{
    public class Utility
    {
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="filePath"></param>
        public static void SerializeObject<T>(T serializableObject, string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, serializableObject);
            }
        }


        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        public static string GetLocalAppDirectory()
        {
            string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDir = Path.Combine(rootDir, "SimpleWebServer");
            if (!Directory.Exists(appDir))
            {
                Directory.CreateDirectory(appDir);
            }

            return appDir;
        }

        public static string GetConfigurationFile()
        {
            string fileName = "config.bin";
            return Path.Combine(Utility.GetLocalAppDirectory(), fileName);
        }

        public static string GetLogFile()
        {
            string fileName = "log.txt";
            return Path.Combine(Utility.GetLocalAppDirectory(), fileName);
        }

        public static void LoadSetting()
        {
            var configFile = Utility.GetConfigurationFile();
            if (File.Exists(configFile))
            {
                Setting setting = Utility.DeSerializeObject<Setting>(configFile);
                CurrentSetting.Instance.directoryPath = setting.directoryPath;
                CurrentSetting.Instance.port = setting.port;
            }
            else
            {
                CurrentSetting.Instance.directoryPath = @"C:\";
                CurrentSetting.Instance.port = 8080;
            }

        }

        public static void SaveSetting(Setting setting)
        {
            var configFile = Utility.GetConfigurationFile();
            Utility.SerializeObject<Setting>(setting, configFile);
            Utility.LoadSetting();
        }

        public static IPAddress GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            throw new Exception("Local IP Address Not Found!");
        }
    }
}
