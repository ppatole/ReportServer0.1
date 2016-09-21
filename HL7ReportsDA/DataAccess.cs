using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportsDataAndBusiness.DataEntities;
using System.Data.SqlClient;
using System.Xml;

namespace ReportsDataAndBusiness
{
    public class DataAccess
    {
        Common.Log L = new Common.Log("", Properties.Settings.Default.EnableDebugLogging);
        private  void RecordLog(string logMessage)
        {
            L.WriteFilelog("HL7ReportsDataAndBusiness", "DataAccess", logMessage);
        }

        Common.Log PerformanceLog = new Common.Log("Listner");

        public void SaveNewReport(Report report, String source = "")
        {
           // RecordLog("Inside save report. Connction string : " + DatabaseHelper.GetConnection().ConnectionString);

            FieldName fieldName = new FieldName();
            List<SqlParameter> prms = new List<SqlParameter>();
            prms.Add(new SqlParameter("@XML", Common.CommonFunctions.ConvertEntityToXML(report)));
            DatabaseHelper.ExecuteNonQuery("SaveReport", prms);
            PerformanceLog.WriteFilelog(source, "HL7", report.AccessionNumber + ", " + report.PatientID + ", " + report.Patientname + ", " + report.ReportDateTime + ", " + report.ReportText);
        }

        /// <summary>
        /// this will return Field Name object.
        /// this will not fill in parent field name. then it might craete recurring calls
        /// whereever is required to get parent fieldName object, it must be called/codded saperatly. 
        /// </summary>
        /// <returns></returns>
        public  FieldName GetFieldNameByID(String FieldNameID)
        {
            FieldName fieldName = new FieldName();
            try
            {
                List<SqlParameter> prms = new List<SqlParameter>();
                prms.Add(new SqlParameter() { ParameterName = "@FieldNameID", Value = FieldNameID });
                SqlDataReader sdr = DatabaseHelper.ExecuteSQLReader("GetFieldNameByID");
                sdr.Read();
                fieldName = new FieldName()
                {
                    FieldNameID = sdr["FieldNameID"].ToString(),
                    FieldNameDes = sdr["FieldNameDes"].ToString(),
                    ParentFieldNameID = sdr["ParentFieldNameID"].ToString()
                };
            }
            catch (Exception ex)
            {
                RecordLog("Exception in GetFieldNameByID. Message:" + ex.Message);
                throw ex;
            }

            RecordLog("complete GetFieldNameByID");
            return fieldName;
        }


        public  List<Report> GetReports(String PatientName, String AccessionNumber)
        {
            List<Report> Reports = new List<Report>();
            Reports = new List<Report>();
            try
            {
                List<SqlParameter> prms = new List<SqlParameter>();
                prms.Add(new SqlParameter() { ParameterName = "@PatientName", Value = PatientName });
                prms.Add(new SqlParameter() { ParameterName = "@AccessionNumber", Value = AccessionNumber });
                SqlDataReader sdr = DatabaseHelper.ExecuteSQLReader("GetReport", prms);
                while (sdr.Read())
                {
                    Report r = new Report();

                    r.AccessionNumber = sdr["AccessionNumber"].ToString();
                    r.ImportedOn = (DateTime)sdr["ImportedOn"];
                    r.PatientID = sdr["PatientID"].ToString();
                    r.Patientname = sdr["Patientname"].ToString();
                    r.ReportDateTime = (DateTime)sdr["ReportDateTime"];
                    // r.ReportXML = sdr["ReportXML"].ToString();
                    string xml = string.Empty;
                    xml = sdr["ReportXML"].ToString();
                    r.RefPhysician = getValFromXML(xml, "RefPhysician");
                    r.Procedure = getValFromXML(xml, "Procedure");
                    r.DOB = DateTime.ParseExact(getValFromXML(xml, "DOB"), "yyyy-MM-ddTHH:mm:ss", null);
                    r.HL7 = getValFromXML(xml, "HL7");

                    XmlDocument xmlObs = new XmlDocument();
                    xmlObs.InnerXml = xml;
                    foreach (XmlNode xn in xmlObs.SelectNodes("/Report/Observations/string"))
                    {
                        r.Observations.Add(xn.InnerText);
                    }
                    Reports.Add(r);
                }
                sdr.Close();
            }
            catch (Exception ex)
            {
                RecordLog("Exception in GetReports. Message:" + ex.Message);
                throw ex;
            }

            RecordLog("complete GetReports");
            return Reports;
        }

        static string getValFromXML(String xml, string field)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return string.Empty;
            }
            string result = string.Empty;
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.InnerXml = xml;
                result = xd.SelectSingleNode("/Report/" + field).InnerText;
            }
            catch (Exception e)
            {
            }
            return result;
        }

        // static List<FieldName> fieldNames;

        //public static List<DataEntities.FieldName> GetAllFieldNames(bool Refresh = false)
        //{
        //    RecordLog("start GetAllFieldNames");
        //    try
        //    {
        //        if (Refresh || (fieldNames == null || fieldNames.Count < 1))
        //        {
        //            fieldNames = new List<FieldName>();
        //            SqlDataReader sdr = DatabaseHelper.ExecuteSQLReader("GetAllFieldNames");
        //            while (sdr.Read())
        //            {
        //                fieldNames.Add(new FieldName()
        //                {
        //                    FieldNameID = sdr["FieldNameID"].ToString(),
        //                    FieldNameDes = sdr["FieldNameDes"].ToString(),
        //                    ParentFieldNameID = sdr["ParentFieldNameID"].ToString()
        //                });
        //            }

        //            sdr.Close();

        //            //find parents
        //            foreach (FieldName fName in fieldNames)
        //            {
        //                //check cyclic ref
        //                /// TODO: check cyclic ref, yet to implement 

        //                fName.ParentFieldName = fieldNames.Where(x => x.FieldNameID == fName.ParentFieldNameID).FirstOrDefault();
        //            }


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RecordLog("Exception in GetAllFieldNames. Message:" + ex.Message);
        //        throw ex;
        //    }

        //    RecordLog("complete GetAllFieldNames");
        //    return fieldNames;
        //}


    }
}
