using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
namespace ReportWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Reports : IReports
    {
        public string GetReports(String PatientName, String AccessionNumber, string PatientID)
        {
            ReportsDataAndBusiness.ReportWebServicesBL rbl = new ReportsDataAndBusiness.ReportWebServicesBL();
            RecordLog("WCF Get Reports called");
            string result = string.Empty;
            try
            {
                result = rbl.GetReports(PatientName, AccessionNumber, PatientID);
            }
            catch (Exception ee)
            {
                RecordLog("Exception in WCF Get Reports. Message:" + ee.Message);
            }
            RecordLog("WCF Get Reports complete");
            return result;           
        }
        public string GetHTMLReports(String PatientName, String AccessionNumber, string PatientID)
        {
            ReportsDataAndBusiness.ReportWebServicesBL rbl = new ReportsDataAndBusiness.ReportWebServicesBL();
            RecordLog("WCF Get HTML Reports called");
            string result = string.Empty;
            try
            {
                result =rbl.GetHTMLReports(PatientName, AccessionNumber, PatientID);
            }
            catch (Exception ee)
            {
                RecordLog("Exception in WCF Get HTML Reports. Message:" + ee.Message);
            }
            RecordLog("WCF Get HTML Reports complete");
            return result;
        }

        private static void RecordLog(string logMessage)
        {
            Common.Log performanceLog = new Common.Log("Sender", Properties.Settings.Default.EnablePerformanceLogging);
            performanceLog.WriteFilelog("ReportWCF", "GetReports", logMessage);
        }
    }
}
