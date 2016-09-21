using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HL7TestingApp
{
    /// <summary>
    /// Interaction logic for HL7Sender.xaml
    /// </summary>
    public partial class HL7Sender : Window
    {
        private const string ACCESS_REPLACEMENT = "[$ACCNO$]";
        private const int MILL_SEC_PER_MIN = 60000;

        UInt64 messgSentCounter = 0;

        public HL7Sender()
        {
            InitializeComponent();
            txtSeedMessage.Text = @"MSH|^~\&|LCS|LCA|LIS|TEST9999|199807311532||ORU^R01|3629|P|2.3
PID | 2 | 2161348462 | [$ACCNO$] | 1614614 | [$ACCNO$] ^ TESTPAT || 19760924 | M |||^^^^
00000 - 0000 ||||||| 86427531 ^^^ 03 | SSN# HERE
ORC | NW | 8642753100012 ^ LIS | [$ACCNO$] ^ LCS |||||| 19980727000000 ||| HAVILAND
OBR | 1 | 8642753100012 ^ LIS | [$ACCNO$] ^ LCS | 008342 ^ UPPER RESPIRATORY
CULTURE ^ L ||| 19980727175800 |||||| SS#634748641 CH14885 SRC:THROA
SRC: PENI | 19980727000000 |||||| [$ACCNO$] || 19980730041800 || BN | F
OBX | 1 | ST | 008342 ^ UPPER RESPIRATORY CULTURE^ L || FINALREPORT ||||| N | F ||| 19980729160500 | BN
ORC | NW | 8642753100012 ^ LIS | [$ACCNO$] ^ LCS |||||| 19980727000000 ||| HAVILAND
OBR | 2 | 8642753100012 ^ LIS | [$ACCNO$] ^ LCS | 997602 ^.^ L ||| 19980727175800 |||| G |||
19980727000000 |||||| [$ACCNO$] || 19980730041800 ||| F | 997602 ||| 008342
OBX | 2 | CE | 997231 ^ RESULT 1 ^ L || M415 ||||| N | F ||| 19980729160500 | BN
NTE | 1 | L | MORAXELLA(BRANHAMELLA) CATARRHALIS
NTE | 2 | L | HEAVY GROWTH
NTE | 3 | L | BETA LACTAMASE POSITIVE
OBX | 3 | CE | 997232 ^ RESULT 2 ^ L || MR105 ||||| N | F ||| 19980729160500 | BN
NTE | 1 | L | ROUTINE RESPIRATORY FLORA";
        }


        private static bool SendHL7(string server, int port, string hl7message)
        {
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
                    return false;

                // Send message to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                // Receive the response back
                int bytes = 0;

                s.ReceiveTimeout = 3000;
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                string page = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                s.Close();

                // Check to see if it was successful
                if (page.Contains("MSA|AA"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
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
                int interval = 1000;
                int port = 0;
                Stop = false;

                if (int.TryParse(txtPort.Text, out port))
                {
                    if (cmbMsgPerMin.Text == "0")
                    {
                        SendHL7(txtIPaddress.Text, int.Parse(txtPort.Text), txtSeedMessage.Text);
                    }
                    else
                    {
                        if (int.TryParse(cmbMsgPerMin.Text, out interval))
                        {
                            int waitMilSec = MILL_SEC_PER_MIN / interval;

                            while (!Stop)
                            {
                                SendHL7(txtIPaddress.Text, int.Parse(txtPort.Text), getMessage());

                                Random random = new Random();
                                Thread.Sleep(random.Next((int)(waitMilSec * 0.7), (int)(waitMilSec * 1.3)));
                                messgSentCounter++;
                            }
                        }
                    }
                }
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
            return txtAccessionNumberSeed.Text + (count++).ToString();
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
