using System;


namespace PCCIReports
{
    public class PCCIReports : I_PCCIReports
    {
        private AxISITELib.AxiSiteNonVisual axiSiteNonVisual;

        string accessionNo = string.Empty, patientID = string.Empty, org = string.Empty;

        string patientName = string.Empty;

        public string AccessionNo
        {
            get
            {
                return accessionNo;
            }

            set
            {
                accessionNo = value;
            }
        }

        public string PatientID
        {
            get
            {
                return patientID;
            }

            set
            {
                patientID = value;
            }
        }

        public string Org
        {
            get
            {
                return org;
            }
        }

        public string PatientName
        {
            get
            {
                return patientName;
            }

            set
            {
                patientName = value;
            }
        }

        private bool InitializeNonVisualCtrl()
        {
            bool bRet = false;
            RecordLog("InitializeNonVisualCtrl started");
            if (axiSiteNonVisual.Initialized)
                bRet = true;
            else
            {
                axiSiteNonVisual.iSyntaxServerIP = Properties.Settings.Default.ServerIP;// serverIP.Text;
                axiSiteNonVisual.ImageSuiteURL = "http://" + Properties.Settings.Default.ServerIP + "/iSiteWeb/WorkList/PrimaryWorkList.ashx/";
                axiSiteNonVisual.ImageSuiteDSN = "iSite";
                axiSiteNonVisual.Options = "StentorBackEnd,HideLoginWindow";                 //DisableISSA

                bRet = axiSiteNonVisual.Initialize();

                if (!bRet)
                {
                    RecordLog("Error Initializing NonVisual Control : " + axiSiteNonVisual.GetLastErrorCode());
                }
            }
            RecordLog("InitializeNonVisualCtrl completed");
            return bRet;

        }

        private bool Login()
        {
            bool bRet = false;
            RecordLog("Login completed");
            if (axiSiteNonVisual.GetCurrentUser() == "")
            {
                bRet = axiSiteNonVisual.Login(Properties.Settings.Default.ServerUserName, Properties.Settings.Default.ServerPassword, "iSite", "", "");

                if (!bRet)
                    RecordLog("Error logging in " + axiSiteNonVisual.GetLastErrorCode());
            }
            else
                bRet = true;
            RecordLog("Login completed");
            return bRet;
        }

        public string getReport(string accessionNo, string patientID)
        {
            RecordLog("getReport completed");
            this.accessionNo = accessionNo;
            this.patientID = patientID;
            this.org = Properties.Settings.Default.OrgName;

            string message = string.Empty;
            if (InitializeNonVisualCtrl())
            {
                if (Login())
                {
                    String ExamID;
                    PatientName = axiSiteNonVisual.FindPatient(patientID, org);
                    ExamID = axiSiteNonVisual.FindExam(AccessionNo, PatientID, org);
                    System.GC.Collect();
                    RecordLog("getReport completed");
                    return axiSiteNonVisual.GetReportData(PatientID, ExamID);
                }
                else
                {
                    message = "getReport completed";
                    RecordLog(message);
                    return message;
                }
            }
            else
            {
                message = "Connection with Intellispace Server couldnot be established";
                RecordLog(message);
                return message;                
            }
        }
        
        private void RecordLog(String log)
        {
            Common.Log L = new Common.Log("", Properties.Settings.Default.EnableDebugLogging);
            L.WriteFilelog("PCCI Reports", "Reports pull", log);

        }
    }
}
