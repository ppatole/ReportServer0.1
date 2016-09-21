using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ReportsDataAndBusiness.DataEntities;
using System.Threading;
using NHapi.Base.Model;
using NHapi.Base.Util;
using NHapi.Base.Parser;
using NHapi.Model.V23.Message;
using System.Linq;
using ReportsDataAndBusiness;

namespace HL7Listener
{
    public class HL7Listener
    {
        static TcpListener server = default(TcpListener);

        static bool Listening = false;

        public void HL7StopListen()
        {
            IsListening = false;
        }

        public static bool IsListening
        {
            get
            {
                return Listening;
            }
            set
            {
                Listening = value;
                Thread.Sleep(100);
                if (!Listening)
                {
                    if (server != null)
                    {
                        server.Stop();
                    }
                }

            }
        }

        public void HL7Listen()
        {
            server = null;
            UInt64 clientNo = 0;
            try
            {

                WriteToHL7LogFile("Inside HL7 Server Listener");

                IsListening = true;

                Int32 port = Properties.Settings.Default.port;

                //  IPAddress localAddr = IPAddress.Any;

                IPAddress localAddr = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                server = new TcpListener(localAddr, port);
                WriteToHL7LogFile("Inside HL7 Server Listener IP:" + localAddr + " and port: " + port);

            }
            catch (SocketException e)
            {
                WriteToHL7LogFile("SocketException:" + e.Message);
            }

            // Enter the listening loop.
            if (server == null)
            {
                WriteToHL7LogFile("Server Listener couldnot be initialised");
            }
            else
            {
                while (IsListening)
                {
                    try
                    {
                        // Start listening for client requests.
                        server.Start();

                        // Perform a blocking call to accept requests.
                        // You could also user server.AcceptSocket() here.
                        WriteToHL7LogFile("Waiting for a connection...");
                        TcpClient client = server.AcceptTcpClient();

                        WriteToHL7LogFile("Connected! :" + clientNo);
                        HandleClinet hc = new HandleClinet();
                        hc.startClient(client, (clientNo++).ToString());

                        if (clientNo > UInt64.MaxValue - 500)
                        {
                            clientNo = 0;
                        }
                    }
                    catch (Exception ee)
                    {
                        WriteToHL7LogFile("Exception: {0}" + ee.Message);
                        //throw;
                    }
                }
            }

            //WriteToHL7LogFile("HL7 Server listening stopped");
            //WriteToHL7LogFile("Outside HL7 Server Listener");
        }

        private void WriteToHL7LogFile(string v)
        {
            Common.Log L = new Common.Log("", Properties.Settings.Default.EnableDebugLogging);
            L.WriteFilelog(ReportsDataAndBusiness.Constant.APP_NAME, ReportsDataAndBusiness.Constant.HL7_Listener_ModuleName, v);

        }
    }


    public class HandleClinet
    {
        TcpClient clientSocket;
        string clNo;
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(ReadMessage);
            ctThread.Start();
        }
        private void ReadMessage()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[1002500];
            requestCount = 0;

            bool IsMessageCleaned = false;
            string data = string.Empty;

            try
            {
                requestCount = requestCount + 1;
                NetworkStream stream = clientSocket.GetStream();
                Int32 i = default(Int32);

                bool IsMsgReadComplete = false;

                while (clientSocket.Connected)
                {
                    // Loop to receive all the data sent by the client
                    i = stream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    WriteToHL7LogFile("Reciving number of bytes : " + i);

                    while (!IsMsgReadComplete)
                    {
                        //Translate data bytes to a ASCII string.
                        data = data + System.Text.UTF8Encoding.ASCII.GetString(bytesFrom, 0, i);

                        if (i < bytesFrom.Length)
                        {
                            IsMsgReadComplete = true;
                        }
                       // WriteToHL7LogFile(data);
                    }


                    IsMessageCleaned = false;
                    try
                    {
                        if (!(data == null))
                        {
                            if (!IsMessageCleaned)
                            {///NHAPI Pipe parser cannot take vertical tab chars at bigning of message.
                                data = System.Text.RegularExpressions.Regex.Replace(data, "^\v", "");
                                IsMessageCleaned = true;
                            }

                            //PipeParser hl7parser = new PipeParser();
                            //ACK ackRes = new ACK();
                            //String ResMess = hl7parser.Encode(ackRes);
                            //string ackMess = MakeACK(HL7Parser.HL7Parser2X.GetAckMess(data), HL7Parser.HL7Parser2X.GetVer(data), "AA", ackRes);
                            //Byte[] msg = Encoding.ASCII.GetBytes(ackMess);
                            //WriteToHL7LogFile("Sending ack" + ackMess);
                            //stream.Write(msg, 0, msg.Length);
                            //WriteToHL7LogFile("ack sent.");

                            WriteToHL7LogFile("Message proces called");
                            Task t = new Task(() => ProcessMessage(data));
                            t.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToHL7LogFile("Exception in ReadMessage. HL7 message parsing. Message: " + ex.Message);
                    }

                    stream.Close();
                    clientSocket.Close();
                    WriteToHL7LogFile("connection closed.");
                }
            }
            catch (Exception ex)
            {
                WriteToHL7LogFile("Exception in ReadMessage. Message: " + ex.Message);
            }
        }

 

       private void ProcessMessage(String Message)
        {
            try
            {
                HL7Parser.HL7Parser2X hl72 = new HL7Parser.HL7Parser2X(Message);

                Report report;
                if (hl72.IsHL7Valid)
                {
                    report = hl72.Report;
                    DataAccess da = new DataAccess();
                    da.SaveNewReport(report, "HL7");
                }
                else
                {
                    WriteToHL7LogFile("Report not generated for message.");
                }
            }
            catch (Exception ee)
            {
                WriteToHL7LogFile("Message Ex:" + ee.Message);
            }
        }
            
        /// Create an Ack message based on a received message/// 
        /// received message/// 
        /// Acknowlegde code/// 
        /// Message to be created/// 
        /// Created message
        public string MakeACK(string AckHead, string version, string ackCode, IMessage ackMessage)
        {

            // Create a Terser instance for the outbound message (the ACK). 
            Terser terser = new Terser(ackMessage);

            // Populate outbound MSH fields using data from inbound message 


            // Now set the message type, HL7 version number, acknowledgement code 
            // and message control ID fields: 
            string sendingApp = terser.Get("/MSH-3");
            string sendingEnv = terser.Get("/MSH-4");
            string CommunicationNameOfThisApplication = string.Empty;
            string EnvironmentIdentifierOfThisApplication = string.Empty;

            terser.Set("/MSH-3", CommunicationNameOfThisApplication);
            terser.Set("/MSH-4", EnvironmentIdentifierOfThisApplication);
            terser.Set("/MSH-5", sendingApp);
            terser.Set("/MSH-6", sendingEnv);
            terser.Set("/MSH-7", DateTime.Now.ToString("yyyyMMddmmhh"));
            terser.Set("/MSH-9", "ACK");
            terser.Set("/MSH-12", version);
            // The ackCode should be "AA" if the message was correctly handled and "AE" if there was an error 
            terser.Set("/MSA-1", ackCode == null ? "AA" : ackCode);
            terser.Set("/MSA-2", AckHead);


            return PipeParser.Encode(ackMessage, new EncodingCharacters('|', '^', '~', '\\', '&'));
        }

        private void WriteToHL7LogFile(string v)
        {
            Common.Log L = new Common.Log("", Properties.Settings.Default.EnableDebugLogging);
            L.WriteFilelog(ReportsDataAndBusiness.Constant.APP_NAME, "ClNo " + clNo + "-" + ReportsDataAndBusiness.Constant.HL7_Listener_ModuleName, v);

        }
    }
}
