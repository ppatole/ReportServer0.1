using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using ReportsDataAndBusiness;
using System.Globalization;
using System.Xml;
using Common;

// Reference on HL7ToXmlConverter.dll

namespace HL7Parser
{

    public class HL7Parser2X
    {

        private ReportsDataAndBusiness.DataEntities.Report report = new ReportsDataAndBusiness.DataEntities.Report();
        public bool IsHL7Valid
        {
            get
            {
                return true;// isHL7Valid;
            }
        }
        public ReportsDataAndBusiness.DataEntities.Report Report
        {
            get
            {
                return report;
            }
        }

        private HL7Parser2X()
        {
        }

        public HL7Parser2X(string HL7Message)
        {
            NHapi.Base.Parser.XMLParser xmlp = new NHapi.Base.Parser.DefaultXMLParser();
            //NHapi.Base.Model.IMessage imess =  xmlp.Parse(HL7Message);

            NHapi.Base.Parser.PipeParser pp = new NHapi.Base.Parser.PipeParser();
            NHapi.Base.Model.IMessage imess = pp.Parse(HL7Message,"2.3") ;

            NHapi.Model.V23.Message.ORU_R01 orm;
            orm = imess as NHapi.Model.V23.Message.ORU_R01;

            XmlDocument MessageXML = new XmlDocument();            
            if(orm != null)
            {
                MessageXML = xmlp.EncodeDocument(orm);
            }
            NHapi.Base.Model.IStructure[] seg = imess.GetAll("MSH");
            report.ReportXML = MessageXML;

            //testFunction(MessageXML);


            report.PatientID = GetPatientID(report.ReportXML);
            report.Patientname = GetPatientName(report.ReportXML);
            string date = string.Empty;
            report.ReportDateTime = GetDateTime(GetStudyDate(report.ReportXML), "yyyyMMddHHmm", out  date);
            report.DOB = GetDateTime(GetPatientDOB(report.ReportXML), "yyyyMMdd", out date);
            report.AccessionNumber = GetAccessioNnumber(report.ReportXML);
        }

        /// <summary>
        /// this is just to re-encode xml format message into hl7 to ceck if the XML is correct or not.
        /// </summary>
        /// <param name="xmlMEssage"></param>
        void testFunction(XmlDocument xmlMEssage)
        {
            NHapi.Model.V23.Message.ORU_R01 orm;
            //  orm = imess as NHapi.Model.V23.Message.ORU_R01;

            NHapi.Base.Parser.DefaultXMLParser dfx = new NHapi.Base.Parser.DefaultXMLParser();
            orm = dfx.ParseDocument(xmlMEssage, "2.3") as NHapi.Model.V23.Message.ORU_R01;

            NHapi.Base.Parser.PipeParser pp = new NHapi.Base.Parser.PipeParser();
           string s =  pp.Encode(orm);            
        }

        //public HL7Parser2X(string HL7Message)
        //{
        //    GetSeparator(HL7Message);
        //    PatientID = GetPatientID(HL7Message);
        //    PatientName = GetPatientName(HL7Message);
        //    StudyDate = GetStudyDate(HL7Message);
        //    PatientDOB = GetPatientDOB(HL7Message);
        //    AccessionNumber = GetAccessioNnumber(HL7Message);
        //    Observations = GetObservation(HL7Message);

        //    report = new ReportsDataAndBusiness.DataEntities.Report();
        //    report.PatientID = PatientID;
        //    report.Patientname = PatientName;
        //    report.AccessionNumber = AccessionNumber;
        //    report.Observations = Observations;
        //    report.RefPhysician = GetRefphys(HL7Message);
        //    report.Procedure = Procedure;
        //    report.HL7 = Regex.Replace(HL7Message, @"\x1c", "");
        //    report.ReportDateTime = GetDateTime(StudyDate, "yyyyMMddHHmm", out StudyDate);
        //    report.DOB = GetDateTime(PatientDOB, "yyyyMMdd", out PatientDOB);
        //}

        private DateTime GetDateTime(string DateString, string Format, out string NewDateString)
        {
            DateTime dt = new DateTime();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            if (!(DateString == null || DateString == String.Empty))
            {
                if (DateString.Length == 14)
                {
                    myDTFI.ShortDatePattern = "yyyyMMddHHmmss";
                    myDTFI.FullDateTimePattern = "yyyyMMddHHmmss";
                }
                else
                {
                    myDTFI.ShortDatePattern = Format;
                    myDTFI.FullDateTimePattern = Format;
                }
                dt = DateTime.ParseExact(DateString, "d", myDTFI);
            }

            NewDateString = dt.ToString();
            return dt;
        }

        //void GetSeparator(string Message)
        //{
        //    Regex r = new Regex(@"MSH\|(.){4}\|");
        //    if (r.Matches(Message).Count == 1 && r.Matches(Message)[0].Groups.Count == 2 && r.Matches(Message)[0].Groups[1].Captures.Count == 4)
        //    {
        //        ComponentSeparator = r.Matches(Message)[0].Groups[1].Captures[0].Value;
        //        RepetitionSeparator = r.Matches(Message)[0].Groups[1].Captures[1].Value;
        //        EscapeSeparator = r.Matches(Message)[0].Groups[1].Captures[2].Value;
        //        SubcomponentSeparator = r.Matches(Message)[0].Groups[1].Captures[3].Value;
        //    }
        //    else
        //    {
        //        isHL7Valid = false;
        //    }
        //    //Regex.Matches(Message, )[0].Value;
        //}

        string GetStudyDate(XmlDocument Message)
        {
            return CommonFunctions.getValFromXML(Message, CommonFunctions.StudyDateQueryString);
        }

        string GetPatientID(XmlDocument Message)
        {
            return CommonFunctions.getValFromXML(Message, CommonFunctions.PatientIDQueryString);
        }

        string GetPatientName(XmlDocument Message)
        {
            return CommonFunctions.getValFromXML(Message, CommonFunctions.PatientFirstNameQueryString) + " " + CommonFunctions.getValFromXML(Message, CommonFunctions.PatientLastNameQueryString);
        }

        string GetPatientDOB(XmlDocument Message)
        {
            return CommonFunctions.getValFromXML(Message, CommonFunctions.PatientDOBQueryString );
        }

        string GetAccessioNnumber(XmlDocument Message)
        {           
            return CommonFunctions.getValFromXML(Message, CommonFunctions.AccessionNumberQueryString);
        }

        public static string GetAckMess(XmlDocument Message)
        {
            return CommonFunctions.getValFromXML(Message, CommonFunctions.AcknowledgeMessageQueryString );
        }

        public static string GetVer(XmlDocument Message)
        {
            return CommonFunctions.getValFromXML(Message, CommonFunctions.MessageVersionQueryString);
        }

        //string GetRefphys(String Message)
        //{
        //    return ReadHL7Field("ORC", 11, Message, false, out isHL7Valid);
        //}

        //List<string> GetObservation(String Message)
        //{
        //    Regex r = new Regex(@"OBX\|(?<seq>[^|]*)(?:[^|]*)\|(?:[^|]*)\|(?<obs>[^|]*)\|(?:[^|]*)\|(?<res>[^|]*)(.)+\s+(?:NTE(?:[^|]*\|){3}(?<Note>.*)\s+){0,100}");
        //    List<string> Observation = new List<string>();

        //    foreach (Match m in r.Matches(Message))
        //    {
        //        if (m.Groups.Count > 5)
        //        {
        //            string obsTit = m.Groups["obs"].Value;
        //            if (obsTit.Split(ComponentSeparator.ToCharArray()).Length > 1)
        //            {
        //                obsTit = obsTit.Split(ComponentSeparator.ToCharArray())[1];
        //            }

        //            if (string.IsNullOrEmpty(Procedure))
        //            {
        //                Procedure = obsTit;
        //            }

        //            string obs = m.Groups["seq"].Value + "|" + obsTit + "|" + m.Groups["res"].Value.Replace(ComponentSeparator, " ") + "|";
        //            foreach (Capture c in m.Groups[5].Captures)
        //            {
        //                obs += c.Value.Replace(ComponentSeparator, " ") + "|";
        //            }
        //            Observation.Add(obs);
        //        }
        //    }
        //    return Observation;
        //}

       
    }
}
