using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleWebServer.Classes
{
    public class SimpleHttpRequest
    {
        private TcpClient client;

        private RequestState ParserState;

        private HttpRequest HttpRequest;

        private HttpResponse HttpResponse;

        byte[] myReadBuffer;

        SimpleHttpServer Parent;

        public SimpleHttpRequest(TcpClient client, SimpleHttpServer Parent)
        {
            this.client = client;
            this.Parent = Parent;

            this.HttpResponse.BodySize = 0;
        }

        public void Process()
        {
            myReadBuffer = new byte[client.ReceiveBufferSize];
            string myCompleteMessage = string.Empty;
            int numberOfBytesRead = 0;

            Parent.WriteLog("Connection accepted. Buffer: " + client.ReceiveBufferSize.ToString());

            NetworkStream ns = client.GetStream();

            string hValue = string.Empty;
            string hKey = string.Empty;

            try
            {
                // binary data buffer index
                int bufferIndex = 0;

                // Incoming message may be larger than the buffer size.
                do
                {
                    numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
                    myCompleteMessage = String.Concat(myCompleteMessage, Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                    // read buffer index
                    int ndx = 0;
                    do
                    {
                        switch (ParserState)
                        {
                            case RequestState.METHOD:
                                if (myReadBuffer[ndx] != ' ')
                                    HttpRequest.Method += (char)myReadBuffer[ndx++];
                                else
                                {
                                    ndx++;
                                    ParserState = RequestState.URL;
                                }
                                break;
                            case RequestState.URL:
                                if (myReadBuffer[ndx] == '?')
                                {
                                    ndx++;
                                    hKey = "";
                                    HttpRequest.Execute = true;
                                    HttpRequest.Args = new Hashtable();
                                    ParserState = RequestState.URLPARM;
                                }
                                else if (myReadBuffer[ndx] != ' ')
                                    HttpRequest.URL += (char)myReadBuffer[ndx++];
                                else
                                {
                                    ndx++;
                                    HttpRequest.URL
                                                = HttpUtility.UrlDecode(HttpRequest.URL);
                                    ParserState = RequestState.VERSION;
                                }
                                break;
                            case RequestState.URLPARM:
                                if (myReadBuffer[ndx] == '=')
                                {
                                    ndx++;
                                    hValue = "";
                                    ParserState = RequestState.URLVALUE;
                                }
                                else if (myReadBuffer[ndx] == ' ')
                                {
                                    ndx++;

                                    HttpRequest.URL
                                                = HttpUtility.UrlDecode(HttpRequest.URL);
                                    ParserState = RequestState.VERSION;
                                }
                                else
                                {
                                    hKey += (char)myReadBuffer[ndx++];
                                }
                                break;
                            case RequestState.URLVALUE:
                                if (myReadBuffer[ndx] == '&')
                                {
                                    ndx++;
                                    hKey = HttpUtility.UrlDecode(hKey);
                                    hValue = HttpUtility.UrlDecode(hValue);
                                    HttpRequest.Args[hKey] =
                                            HttpRequest.Args[hKey] != null ?
                                             HttpRequest.Args[hKey] + ", " + hValue :
                                                hValue;
                                    hKey = "";
                                    ParserState = RequestState.URLPARM;
                                }
                                else if (myReadBuffer[ndx] == ' ')
                                {
                                    ndx++;
                                    hKey = HttpUtility.UrlDecode(hKey);
                                    hValue = HttpUtility.UrlDecode(hValue);
                                    HttpRequest.Args[hKey] =
                                            HttpRequest.Args[hKey] != null ?
                                            HttpRequest.Args[hKey] + ", " + hValue :
                                               hValue;

                                    HttpRequest.URL
                                               = HttpUtility.UrlDecode(HttpRequest.URL);
                                    ParserState = RequestState.VERSION;
                                }
                                else
                                {
                                    hValue += (char)myReadBuffer[ndx++];
                                }
                                break;
                            case RequestState.VERSION:
                                if (myReadBuffer[ndx] == '\r')
                                    ndx++;
                                else if (myReadBuffer[ndx] != '\n')
                                    HttpRequest.Version += (char)myReadBuffer[ndx++];
                                else
                                {
                                    ndx++;
                                    hKey = "";
                                    HttpRequest.Headers = new Hashtable();
                                    ParserState = RequestState.HEADERKEY;
                                }
                                break;
                            case RequestState.HEADERKEY:
                                if (myReadBuffer[ndx] == '\r')
                                    ndx++;
                                else if (myReadBuffer[ndx] == '\n')
                                {
                                    ndx++;
                                    if (HttpRequest.Headers["Content-Length"] != null)
                                    {
                                        HttpRequest.BodySize =
                                 Convert.ToInt32(HttpRequest.Headers["Content-Length"]);
                                        this.HttpRequest.BodyData
                                                    = new byte[this.HttpRequest.BodySize];
                                        ParserState = RequestState.BODY;
                                    }
                                    else
                                        ParserState = RequestState.OK;

                                }
                                else if (myReadBuffer[ndx] == ':')
                                    ndx++;
                                else if (myReadBuffer[ndx] != ' ')
                                    hKey += (char)myReadBuffer[ndx++];
                                else
                                {
                                    ndx++;
                                    hValue = "";
                                    ParserState = RequestState.HEADERVALUE;
                                }
                                break;
                            case RequestState.HEADERVALUE:
                                if (myReadBuffer[ndx] == '\r')
                                    ndx++;
                                else if (myReadBuffer[ndx] != '\n')
                                    hValue += (char)myReadBuffer[ndx++];
                                else
                                {
                                    ndx++;
                                    HttpRequest.Headers.Add(hKey, hValue);
                                    hKey = "";
                                    ParserState = RequestState.HEADERKEY;
                                }
                                break;
                            case RequestState.BODY:
                                // Append to request BodyData
                                Array.Copy(myReadBuffer, ndx,
                                      this.HttpRequest.BodyData,
                                   bufferIndex, numberOfBytesRead - ndx);
                                bufferIndex += numberOfBytesRead - ndx;
                                ndx = numberOfBytesRead;
                                if (this.HttpRequest.BodySize <= bufferIndex)
                                {
                                    ParserState = RequestState.OK;
                                }
                                break;
                                //default:
                                //   ndx++;
                                //   break;

                        }
                    }
                    while (ndx < numberOfBytesRead);

                } while (ns.DataAvailable);

                Parent.WriteLog("You received the following message : \n" + myCompleteMessage);
                
                HttpResponse.version = "HTTP/1.1";

                if (ParserState != RequestState.OK)
                    HttpResponse.status = (int)ResponseState.BAD_REQUEST;
                else
                    HttpResponse.status = (int)ResponseState.OK;

                this.HttpResponse.Headers = new Hashtable();
                this.HttpResponse.Headers.Add("Server", Parent.Name);
                this.HttpResponse.Headers.Add("Date", DateTime.Now.ToString("r"));

                this.Parent.OnResponse(ref this.HttpRequest,
                                          ref this.HttpResponse);

                string HeadersString = this.HttpResponse.version + " " + this.Parent.responseStatus[this.HttpResponse.status] + "\n";

                foreach (DictionaryEntry Header in this.HttpResponse.Headers)
                {
                    HeadersString += Header.Key + ": " + Header.Value + "\n";
                }

                HeadersString += "\n";
                byte[] bHeadersString = Encoding.ASCII.GetBytes(HeadersString);

                // Send headers   
                ns.Write(bHeadersString, 0, bHeadersString.Length);

                // Send body
                if (this.HttpResponse.BodyData != null)
                    ns.Write(this.HttpResponse.BodyData, 0,
                                this.HttpResponse.BodyData.Length);

                if (this.HttpResponse.fs != null)
                    using (this.HttpResponse.fs)
                    {
                        byte[] b = new byte[client.SendBufferSize];
                        int bytesRead;
                        while ((bytesRead
                                       = this.HttpResponse.fs.Read(b, 0, b.Length)) > 0)
                        {
                            ns.Write(b, 0, bytesRead);
                        }

                        this.HttpResponse.fs.Close();
                    }

            }
            catch (Exception e)
            {
                Parent.WriteLog(e.ToString());
            }
            finally
            {
                ns.Close();
                client.Close();
                if (this.HttpResponse.fs != null)
                    this.HttpResponse.fs.Close();

                Thread.CurrentThread.Abort();
            }
        }
    }
}
