using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWebServer.Classes
{
    public abstract class SimpleHttpServer
    {
        private int port;
        private TcpListener listener;
        private Thread Thread;

        public Hashtable responseStatus;
        public string Name = "SimpleHttpServer/1.0.*";

        public SimpleHttpServer(int port)
        {
            this.port = port;
            InitializeResponseStatus();
        }

        private void InitializeResponseStatus()
        {
            this.responseStatus = new Hashtable();

            responseStatus.Add(200, "200 Ok");
            responseStatus.Add(201, "201 Created");
            responseStatus.Add(202, "202 Accepted");
            responseStatus.Add(204, "204 No Content");

            responseStatus.Add(301, "301 Moved Permanently");
            responseStatus.Add(302, "302 Redirection");
            responseStatus.Add(304, "304 Not Modified");

            responseStatus.Add(400, "400 Bad Request");
            responseStatus.Add(401, "401 Unauthorized");
            responseStatus.Add(403, "403 Forbidden");
            responseStatus.Add(404, "404 Not Found");

            responseStatus.Add(500, "500 Internal Server Error");
            responseStatus.Add(501, "501 Not Implemented");
            responseStatus.Add(502, "502 Bad Gateway");
            responseStatus.Add(503, "503 Service Unavailable");
        }

        public void Listen()
        {
            bool done = false;

            listener = new TcpListener(Utility.GetLocalIP(), this.port);
            listener.Start();

            WriteLog("Listening On: " + port.ToString());

            while (!done)
            {
                SimpleHttpRequest request = new SimpleHttpRequest(listener.AcceptTcpClient(), this);
                Thread Thread = new Thread(new ThreadStart(request.Process));
                Thread.Name = "HTTP Request";
                Thread.Start();
            }
        }

        public void Start()
        {
            this.Thread = new Thread(new ThreadStart(this.Listen));
            this.Thread.Start();
        }

        public void Stop()
        {
            listener.Stop();
            this.Thread.Abort();
        }
        
        public bool IsAlive
        {
            get
            {
                return this.Thread.IsAlive;
            }
        }

        public void WriteLog(string eventMessage)
        {
            var logFile = Utility.GetLogFile();
            using (Stream stream = File.Open(logFile, FileMode.Append))
            {
                var writer = new StreamWriter(stream);
                writer.WriteLine(eventMessage);
            }
        }

        public abstract void OnResponse(ref HttpRequest request, ref HttpResponse response);
    }
}
