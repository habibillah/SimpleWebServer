using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebServer.Classes
{
    public class Server : SimpleHttpServer
    {
        public static readonly string SERVICE_NAME = "SimpleWebServer";

        private string webroot;

        public Server(string webroot, int port) : base(port)
        {
            this.webroot = webroot;
        }

        public override void OnResponse(ref HttpRequest request, ref HttpResponse response)
        {
            string path = this.webroot + "\\" + request.URL.Replace("/", "\\");

            if (Directory.Exists(path))
            {
                if (File.Exists(path + "index.html"))
                {
                    path += "\\index.html";
                }
                else
                {
                    string[] dirs = Directory.GetDirectories(this.webroot);
                    string[] files = Directory.GetFiles(this.webroot);

                    string bodyStr = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\n";
                    bodyStr += "<HTML><HEAD>\n";
                    bodyStr += "<META http-equiv=Content-Type content=\"text/html; charset=windows-1252\">\n";
                    bodyStr += "</HEAD>\n";
                    bodyStr += "<BODY><p>Folder listing, add a 'index.html' file to hide this view\n<p>\n";

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        bodyStr += "<br><a href = \"" + request.URL + Path.GetFileName(dirs[i]) + "/\">[" + Path.GetFileName(dirs[i]) + "]</a>\n";
                    }

                    for (int i = 0; i < files.Length; i++)
                    {
                        bodyStr += "<br><a href = \"" + request.URL + Path.GetFileName(files[i]) + "\">" + Path.GetFileName(files[i]) + "</a>\n";
                    }

                    bodyStr += "</BODY></HTML>\n";

                    response.BodyData = Encoding.ASCII.GetBytes(bodyStr);
                    return;
                }
            }

            if (File.Exists(path))
            {
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(path), true);

                // Get the data from a specified item in the key.
                String s = (String)rk.GetValue("Content Type");

                // Open the stream and read it back.
                response.fs = File.Open(path, FileMode.Open);
                if (s != "")
                    response.Headers["Content-type"] = s;
            }
            else
            {
                response.status = (int)ResponseState.NOT_FOUND;

                string bodyStr = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\n";
                bodyStr += "<HTML><HEAD>\n";
                bodyStr += "<META http-equiv=Content-Type content=\"text/html; charset=windows-1252\">\n";
                bodyStr += "</HEAD>\n";
                bodyStr += "<BODY>File not found!!</BODY></HTML>\n";

                response.BodyData = Encoding.ASCII.GetBytes(bodyStr);

            }
        }
    }
}
