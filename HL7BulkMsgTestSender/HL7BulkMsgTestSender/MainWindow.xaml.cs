using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Globalization;
using System.IO;

namespace HL7BulkMsgTestSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ACCESS_REPLACEMENT = "ACCNOXXX";
        private const int MILL_SEC_PER_MIN = 60000;

        UInt64 messgSentCounter = 0;
        int concurrentMessage = 0;
        public MainWindow()
        {
            InitializeComponent();
            string msg = string.Empty;
            msg = LoadMessage();
            txtSeedMessage.Text = msg;
        }

        private static string LoadMessage()
        {
            string msg = string.Empty;
            try
            {
                StreamReader sr = new StreamReader(@"C:\oruMessage.txt");
                while (!sr.EndOfStream)
                {
                    string ln = sr.ReadLine();
                    //all message line must begin with c char command.. else ignore the messageline
                    if (System.Text.RegularExpressions.Regex.IsMatch(ln, @"^\w{3}\|"))
                    {
                        msg = msg + ln + "\r\n";
                    }
                }
                sr.Close();
            }
            catch (Exception ee)
            { }

            return msg;
        }

        private string SendHL7(ref string server, ref int port, ref string hl7message)
        {
            string result = string.Empty;

            try
            {

                // Add the leading and trailing characters so it is LLP complaint.
                string llphl7message = Convert.ToChar(11).ToString() + hl7message + Convert.ToChar(28).ToString() + Convert.ToChar(13).ToString();

                // Get the size of the message that we have to send.
                Byte[] bytesSent = Encoding.ASCII.GetBytes(llphl7message);
                Byte[] bytesReceived = new Byte[256];

                // Create a socket connection with the specified server and port.
                Socket s = ConnectSocket(server, port);

                // If the socket could not get a connection, then return false.
                if (s == null)
                    return result;

                // Send message to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                // Receive the response back
                int bytes = 0;

                s.ReceiveTimeout = 3000;
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                string page = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                s.Close();

                if (concurrentMessage > 0)
                {
                    concurrentMessage--;
                }

                // Check to see if it was successful
                if (page.Contains("MSA|AA"))
                {
                    result = "Message sent";
                    return result;
                }
                else
                {

                    return result;
                }
            }
            catch (Exception ex)
            {
                result = "Exception in message sent. ex message: " + ex.Message;
                return result;
            }

        }

        private static Socket ConnectSocket(string server, int port)
        {
            //Socket s = null;
            //IPHostEntry hostEntry = null;

            //// Get host related information.
            //hostEntry = Dns.GetHostEntry(server);

            //foreach (IPAddress address in hostEntry.AddressList)
            //{
            IPEndPoint ipe = CreateIPEndPoint(server + ":" + port);
            Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.Connect(ipe);

            //if (tempSocket.Connected)
            //{
            //    s = tempSocket;
            //    break;
            //}
            //else
            //{
            //    continue;
            //}
            //}
            return tempSocket;
        }

        // Handles IPv4 and IPv6 notation.
        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (ep.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }
        Task t;
        bool Stop = false;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            t = new Task(() => start());
            t.Start();

        }

        private void start()
        {
            this.Dispatcher.Invoke((Action)(() =>
                       {
                           DateTime statTime = DateTime.Now;
                           txtResult.Text = "Message Sending started at:" + statTime.ToLongTimeString() + "\r\n";
                           int interval = 1000;
                           int port = 0;
                           Stop = false;
                           string ip = txtIPaddress.Text;
                           int pp = int.Parse(txtPort.Text);
                           string msg;

                           messgSentCounter= 0;
                           concurrentMessage= 0;

                           if (int.TryParse(txtPort.Text, out port))
                           {
                               if (cmbMsgPerMin.Text == "0")
                               {
                                   msg = getMessage();
                                   SendHL7(ref ip, ref pp, ref msg);
                               }
                               else
                               {
                                   if (int.TryParse(cmbMsgPerMin.Text, out interval))
                                   {
                                       int waitMilSec = MILL_SEC_PER_MIN / interval;

                                       while (!Stop && UInt64.Parse(txtNumberofMsg.Text)> messgSentCounter)
                                       {
                                           while (concurrentMessage > Int32.Parse(txtMaxNumMsg.Text))
                                           {
                                              // txtResult.Text = txtResult.Text + "Slowing down\r\n";
                                               System.Threading.Thread.Sleep(30);
                                           }
                                           //SendHL7(txtIPaddress.Text, int.Parse(txtPort.Text), getMessage());
                                           this.Dispatcher.Invoke((Action)(() =>
                                           {
                                               msg = getMessage();
                                               Task sm = new Task(() => SendHL7(ref ip, ref pp, ref msg));
                                               sm.Start();
                                           }));

                                           Random random = new Random();
                                           Thread.Sleep(random.Next((int)(waitMilSec * 0.7), (int)(waitMilSec * 1.3)));
                                           messgSentCounter++;
                                           concurrentMessage++;

                                       }
                                   }
                               }
                           }
                           DateTime endTime = DateTime.Now;
                           txtResult.Text = txtResult.Text + "Message Sending completed at:" + endTime.ToLongTimeString() + "\r\n";                           
                           txtResult.Text = txtResult.Text + "Message sent :"+  messgSentCounter + "\r\n";
                           TimeSpan span = endTime.Subtract(statTime);

                           txtResult.Text = txtResult.Text + "Time required :" + span.Duration().ToString() + "\r\n";
                       }));

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (t != null && t.Status == TaskStatus.Running)
            {
                Stop = true;
            }
        }

        private string getMessage()
        {
            return System.Text.RegularExpressions.Regex.Replace(txtSeedMessage.Text, ACCESS_REPLACEMENT, getNewAccessionNumber());
        }

        UInt64 count = 0;
        private string getNewAccessionNumber()
        {
            count = count + 1;
            return txtAccessionNumberSeed.Text + (count).ToString();
        }

        private void button_1_Click(object sender, RoutedEventArgs e)
        {
            if (t != null && t.Status == TaskStatus.Running)
            {
                Stop = true;
            }
        }
    }
}
