using System;
using ReportsDataAndBusiness.DataEntities;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;
using Common;

namespace ReportsDataAndBusiness
{
    public class ReportWebServicesBL
    {

        public string GetHTMLReports(String PatientName, String AccessionNumber, string PatientID = "")
        {
            RecordLog("GetReports started. Patient Name: " + PatientName + "\t  Accession Number: " + AccessionNumber + "\t  PatientID: " + PatientID);
            string reportResult = string.Empty;
            DataAccess da = new DataAccess();
            List<Report> reports = da.GetReports(PatientName, AccessionNumber);

            try
            {
                if (reports.Count < 1)
                {///if no reports found in DB then query PCCI server
                    RecordLog("No reports found in DB, looking out in PCCI. Patient Name: " + PatientName + "\t  Accession Number: " + AccessionNumber + "\t  PatientID: " + PatientID);
                    if (PatientID != string.Empty)
                    {
                        RecordLog("Is PCCI online?");
                        if (IsPCCIAvailable())
                        {
                            RecordLog("Yes. Fetching PCCI reports \t  Accession Number: " + AccessionNumber + "\t  PatientID: " + PatientID);
                            reports.Add(FetchReportFromPCCI(AccessionNumber, PatientID));
                        }
                        else
                        {
                            RecordLog("PCCI reports not available.");
                        }
                    }
                    else
                    {
                        RecordLog("PatientID cannot be blank for PCCI report.");
                    }
                }

                if (reports != null && reports.Count > 0)
                {
                    //reportResult += "<Reports> ";
                    foreach (Report r in reports)
                    {
                        ApplyTemplateToReport(r);
                        reportResult += (r.ReportText);
                    }
                    //reportResult += " </Reports> ";
                }
            }
            catch (Exception ee)
            {
                RecordLog("Exception in GetReports Message:" + ee.Message);
                throw ee;
            }
            RecordLog("GetReports completed with number of reports: " + reports.Count.ToString());
            return reportResult;
        }

        public string GetReports(String PatientName, String AccessionNumber, List<string> QueryStrings, string PatientID = "")
        {
            RecordLog("GetReports started. Patient Name: " + PatientName + "\t  Accession Number: " + AccessionNumber + "\t  PatientID: " + PatientID);
            string reportResult = string.Empty;
            DataAccess da = new DataAccess();
            List<Report> reports = da.GetReports(PatientName, AccessionNumber);
            try
            {
                if (reports.Count < 1)
                {///if no reports found in DB then query PCCI server
                    RecordLog("No reports found in DB, looking out in PCCI. Patient Name: " + PatientName + "\t  Accession Number: " + AccessionNumber + "\t  PatientID: " + PatientID);
                    if (PatientID != string.Empty)
                    {
                        RecordLog("Is PCCI online?");
                        if (IsPCCIAvailable())
                        {
                            RecordLog("Yes. Fetching PCCI reports \t  Accession Number: " + AccessionNumber + "\t  PatientID: " + PatientID);
                            reports.Add(FetchReportFromPCCI(AccessionNumber, PatientID));
                        }
                        else
                        {
                            RecordLog("PCCI reports not available.");
                        }
                    }
                    else
                    {
                        RecordLog("PatientID cannot be blank for PCCI report.");
                    }
                }

                if (reports != null && reports.Count > 0)
                {
                    reportResult += "<Reports> ";
                    foreach (Report r in reports)
                    {
                        reportResult += "<Report> ";

                        if (QueryStrings == null || QueryStrings.Count < 1)
                        {
                            reportResult += Common.CommonFunctions.ConvertEntityToXML(r) + "\r\n";
                        }
                        else
                        {
                            XmlDocument reportXMl = new XmlDocument();
                            reportXMl = r.ReportXML;
                            foreach (string query in QueryStrings)
                            {
                                string tq = string.Empty;

                                if (!query.StartsWith("//"))
                                {
                                    tq = "//" + query;
                                }
                                else
                                {
                                    tq = query;
                                }

                                XmlNodeList nodes = reportXMl.SelectNodes(tq);
                                if (nodes != null)
                                {
                                    foreach (XmlNode node in nodes)
                                    {
                                        reportResult += node.ParentNode.OuterXml;
                                    }
                                }
                            }
                            //    reportResult += reportXMl.OuterXml;
                        }
                        reportResult += " </Report> ";
                    }
                    reportResult += " </Reports> ";
                }
            }
            catch (Exception ee)
            {
                RecordLog("Exception in GetReports Message:" + ee.Message);
                throw ee;
            }
            RecordLog("GetReports completed with number of reports: " + reports.Count.ToString());
            return reportResult;
        }


        static void ApplyTemplateToReport(Report report)
        {
            String reportFormat = string.Empty;
            try
            {

                StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.ReportsSetting));
                reportFormat = sr.ReadToEnd();
                sr.Close();

                // reportFormat  = reportFormat.Replace("", report.AccessionNumber);
                reportFormat = reportFormat.Replace("{{study.patientid}}", report.PatientID);
                reportFormat = reportFormat.Replace("{{study.patient_name}}", report.Patientname);
                reportFormat = reportFormat.Replace("{{study.study_date}}", report.ReportDateTime.ToString());

                reportFormat = reportFormat.Replace("{{study.patient_birth_date}}", report.DOB.ToShortDateString());
                reportFormat = reportFormat.Replace("{{study.referring_physician}}", report.RefPhysician);
                reportFormat = reportFormat.Replace("{{study.study_description}}", report.Procedure);

                string studyDescription = string.Empty;

                XmlDocument StudyDesc = new XmlDocument();
                StudyDesc.InnerXml = report.ReportText;
                foreach (XmlNode node in StudyDesc.SelectNodes("//Observations/string"))
                {
                    studyDescription += "<table width=100%>";
                    string obs = node.InnerText;
                    if (!string.IsNullOrEmpty(obs))
                    {
                        string[] items = obs.Split('|');
                        if (items.Length > 3)
                        {
                            studyDescription += "<tr><td width=20%>" + items[1] + "</td><td>" + items[2] + "</td></tr>";
                        }
                        if (items.Length > 4)
                        {
                            studyDescription += "<tr><td width=20%> Notes </td><td></td></tr>";

                            for (int i = 3; i <= items.Length - 1; i++)
                            {
                                studyDescription += "<tr><td width=20%></td><td>" + items[i] + "</td></tr>";
                            }
                            studyDescription += "<tr height=10px><td > </td><td></td></tr>";
                        }
                    }
                    studyDescription += "<tr height=10px><td> </td><td></td></tr>";
                    studyDescription += "</table>";
                }

                report.ReportText = reportFormat;

                report.ReportText = report.ReportText.Replace("{{{NTE-3-ALL}}}", studyDescription);

            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        Common.Log PerformanceLog = new Common.Log("Listner", Properties.Settings.Default.EnableDebugLogging);
        private Report FetchReportFromPCCI(string AccessionNumber, string PatientID)
        {
            RecordLog("FetchReportFromPCCI started");
            Report report = new Report();
            try
            {
                String ReportText = string.Empty;
                Assembly asm = Assembly.LoadFile("PCCIReports.dll");
                Type tyPCCI = asm.GetType("PCCIReports");
                Type InPCCI = asm.GetType("I_PCCIReports");
                I_PCCIReports pr;
                if (tyPCCI != null && InPCCI != null)
                {
                    pr = (I_PCCIReports)Activator.CreateInstance(tyPCCI);
                    ReportText = pr.getReport(AccessionNumber, PatientID);
                    if (ReportText != string.Empty)
                    {
                        report.PatientID = PatientID;
                        report.AccessionNumber = AccessionNumber;
                        report.ReportText = ReportText;
                        report.ReportDateTime = DateTime.Now;
                        report.Patientname = pr.PatientName;
                        DataAccess da = new DataAccess();
                        da.SaveNewReport(report, "ISITE");

                    }
                    RecordLog("FetchReportFromPCCI completed");
                }
                else
                {
                    RecordLog("PCCI reports not fetched.");
                }
            }
            catch (Exception e)
            {
                RecordLog("Exception in FetchReportFromPCCI. Message:" + e.Message);
                throw e;
            }
            return report;
        }

        private bool IsPCCIAvailable()
        {
            bool result = false;
            try
            {
                Assembly asm = Assembly.LoadFile("PCCIReports.dll");
                Type tyPCCI = asm.GetType("PCCIReports");
                Type InPCCI = asm.GetType("I_PCCIReports");
                if (tyPCCI != null && InPCCI != null)
                {
                    object o = Activator.CreateInstance(tyPCCI);
                }
                result = true;

            }
            catch (Exception e)
            {
                RecordLog("Exception in IsPCCIAvailable. Message:" + e.Message);
            }
            GC.Collect();
            return result;
        }

        private void RecordLog(string logMessage)
        {
            Common.Log L = new Common.Log("", Properties.Settings.Default.EnableDebugLogging);
            L.WriteFilelog("HL7ReportsDataAndBusiness", "Reports web service", logMessage);
        }

        public List<String> GetAllQueryStrings()
        {
            List<string> QueryStrings = new List<string>();
            try
            {
                QueryStrings.Add("PatientIDQueryString|" + CommonFunctions.PatientIDQueryString);
                QueryStrings.Add("PatientFirstNameQueryString|" + CommonFunctions.PatientFirstNameQueryString);
                QueryStrings.Add("PatientLastNameQueryString|" + CommonFunctions.PatientLastNameQueryString);
                QueryStrings.Add("PatientDOBQueryString|" + CommonFunctions.PatientDOBQueryString);
                QueryStrings.Add("StudyDateQueryString|" + CommonFunctions.StudyDateQueryString);
                QueryStrings.Add("AccessionNumberQueryString|" + CommonFunctions.AccessionNumberQueryString);
                QueryStrings.Add("AcknowledgeMessageQueryString|" + CommonFunctions.AcknowledgeMessageQueryString);
                QueryStrings.Add("MessageVersionQueryString|" + CommonFunctions.MessageVersionQueryString);
                QueryStrings.Add("ReferingPhysicianQueryString|" + CommonFunctions.ReferingPhysicianQueryString);

                QueryStrings.Add("ProcedureQueryString|" + CommonFunctions.ProcedureQueryString);
                QueryStrings.Add("ObservationQueryString|" + CommonFunctions.ObservationQueryString);
            }
            catch (Exception e)
            {

            }
            return QueryStrings;
        }
    }
}
