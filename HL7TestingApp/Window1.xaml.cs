using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        ReportsWebService.ReportsClient rc;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            txtResult.Text = "";
            EndpointAddress svcAddress = null;
            BasicHttpBinding svcbinding = null;

            svcbinding = new BasicHttpBinding();
            svcbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            svcbinding.MaxBufferSize = 2147483647;
            svcbinding.MaxReceivedMessageSize = 2147483647;
            svcbinding.MaxBufferPoolSize = 2147483647;
            svcbinding.ReaderQuotas.MaxArrayLength = 2147483647;
            svcbinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            svcbinding.ReaderQuotas.MaxDepth = 2147483647;
            svcbinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            svcbinding.ReaderQuotas.MaxStringContentLength = 2147483647; 
          
            if (this.rc == null)
            {
                svcAddress = new EndpointAddress(Properties.Settings.Default.ReportServiceAddress);
                this.rc = new ReportsWebService.ReportsClient(svcbinding, svcAddress);
                txtResult.Text =  rc.GetReports(txtpatient.Text, txtAccession.Text, txtPatientID.Text);
            }
        }
    }
}
