using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for testRef.xaml
    /// </summary>
    public partial class testRef : Window
    {
        public testRef()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           // txtResult.Text = ReportsDataAndBusiness.ReportWebServicesBL.GetReports(txtpatient.Text, txtpatient.Text, txtPatientID.Text);
        }
    }
}
