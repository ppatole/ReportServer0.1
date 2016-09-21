using System;
using System.Threading.Tasks;
using System.Windows;


namespace HL7TestingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Window1 w = new Window1();
            w.Show();
        }



        string GotMessage(String message)
        {
            return "";
        }

        string Log(String message)
        {
            //txtLog.Text = message + "\n\r";
            return "";
        }
        HL7Listener.HL7Listener hll = new HL7Listener.HL7Listener();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Task t = new Task(() => hll.HL7StopListen());
            t.Start();
        }

       

        private void btnStartService_Click(object sender, RoutedEventArgs e)
        {

            //testRef tr = new testRef();
            //tr.Show();
            string s = "asdasd";
            // MessageBox.Show(s);

            Task t = new Task(() => hll.HL7Listen());
            t.Start();
        }
    }
}
