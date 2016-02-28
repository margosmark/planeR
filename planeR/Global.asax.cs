using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace planeR
{
    public class Global : System.Web.HttpApplication
    {
        // AIzaSyBpG_QIhaASBUX-oGxWvy96An8MoLX-iiY 
        protected void Application_Start(object sender, EventArgs e)
        {

            ThreadPool.QueueUserWorkItem(_ =>
            {
                IHubContext clients = GlobalHost.ConnectionManager.GetHubContext<PlaneHub>();
                TcpClient tcpClient = null;
                NetworkStream clientSockStream = null;

                try
                {
                    string host = "192.168.1.114";
                    int port = 30003;
                    planeInfo blah = null;
                    while (true)
                    {
                        if (tcpClient == null)
                        {
                            tcpClient = new TcpClient(host, port);
                            clientSockStream = tcpClient.GetStream();
                        }

                        if (clientSockStream.CanRead)
                        {
                            StreamReader rdr = new StreamReader(clientSockStream);
                            foreach (string line in helper.ReadLines(rdr))
                            {
                                
                                //writer.WriteLine(line);
                                //System.Console.WriteLine(line);
                                string[] response = line.Split(',');
                                if (response.Length == 22)
                                {
                                    blah = new planeInfo(response);
                                    //System.Console.WriteLine(blah.ToString());
                                }
                                else
                                    continue;
                                
                                if (!blah.messageType.Equals(MessageType.MSG))
                                    continue;

                                if (blah.transmissiontype.Equals(TransmissionType.ES_AIRBORNE_POS) || blah.transmissiontype.Equals(TransmissionType.ES_SURFACE_POS))
                                    clients.Clients.All.broadcastMessage(blah.hexIdent, blah.lat, blah.lon, null);
                                else if (blah.transmissiontype.Equals(TransmissionType.ES_IDENT_AND_CATEGORY))
                                    clients.Clients.All.broadcastMessage(blah.hexIdent, null, null, blah.callSign);
                                else
                                    continue;

                                //foreach (string a in response)
                                //{
                                //    plane.messageType
                                //    //System.Console.WriteLine(a);
                                //}

                            }

                            tcpClient.Close();
                            // Closing the client does not automatically close the stream
                            clientSockStream.Close();

                            tcpClient = null;
                            clientSockStream = null;

                            
                        }
                        Thread.Sleep(50);
                    }
                }
                catch (SocketException ex)
                {
                    // do something to handle the error
                }
                finally
                {
                    if (tcpClient != null)
                    {
                        tcpClient.Close();
                        clientSockStream.Close();
                    }
                }

            });
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}