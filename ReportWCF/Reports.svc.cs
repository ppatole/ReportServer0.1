using System;
using System.Collections.Generic;

namespace ReportWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Reports : IReports
    {
        public string GetReports(String PatientName, String AccessionNumber, List<string> QueryStrings, string PatientID)
        {
            ReportsDataAndBusiness.ReportWebServicesBL rbl = new ReportsDataAndBusiness.ReportWebServicesBL();
            RecordLog("WCF Get Reports called");
            string result = string.Empty;
            try
            {
                result = rbl.GetReports(PatientName, AccessionNumber, QueryStrings, PatientID);
            }
            catch (Exception ee)
            {
                RecordLog("Exception in WCF Get Reports. Message:" + ee.Message);
            }
            RecordLog("WCF Get Reports complete");
            return result;           
        }
        public string GetHTMLReports(string PatientName, string AccessionNumber, string PatientID)
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

        public List<string> GetAllQueryStrings()
        {
            ReportsDataAndBusiness.ReportWebServicesBL rbl = new ReportsDataAndBusiness.ReportWebServicesBL();
            RecordLog("WCF Get GetAllQueryStrings called");
            List<string> result = new List<string>();
            try
            {
                result = rbl.GetAllQueryStrings();
            }
            catch (Exception ee)
            {
                RecordLog("Exception inGetAllQueryStrings. Message:" + ee.Message);
            }
            RecordLog("WCF Get GetAllQueryStrings complete");
            return result;
        }


        private static void RecordLog(string logMessage)
        {
            Common.Log performanceLog = new Common.Log("Sender", Properties.Settings.Default.EnablePerformanceLogging);
            performanceLog.WriteFilelog("ReportWCF", "GetReports", logMessage);
        }


    }
}
