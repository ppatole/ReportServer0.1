using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Common
{
    public class CommonFunctions
    {
        public static string StudyDateQueryString = "MSH.7/TS.1";
        public static string PatientIDQueryString = "PID.2/CX.1";
        public static string PatientFirstNameQueryString = "PID.5/XPN.1";
        public static string PatientLastNameQueryString = "PID.5/XPN.2";
        public static string PatientDOBQueryString = "PID.7/TS.1";
        public static string AccessionNumberQueryString = "PID.3/CX.1";
        public static string AcknowledgeMessageQueryString = "MSH.10";
        public static string MessageVersionQueryString = "MSH.12";
        public static string ReferingPhysicianQueryString = "ORC.12/XCN.1";
        public static string ProcedureQueryString = "OBR.4/CE.2";
        public static string ObservationQueryString = "ORU_R01.OBSERVATION/OBX";

        public static string ConvertEntityToXML(object objectToSerialize)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
                // Initializes a new instance of the XmlDocument class.          
                XmlSerializer xmlSerializer = new XmlSerializer(objectToSerialize.GetType());
                // Creates a stream whose backing store is memory. 
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, objectToSerialize);
                    xmlStream.Position = 0;
                    //Loads the XML document from the specified string.
                    xmlDoc.Load(xmlStream);
                    return xmlDoc.InnerXml;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        static string ReadHL7Field(string LineHeader, uint FieldNumber, string Message, bool IsMandate, out bool IsValid)
        {
            Regex r = new Regex(LineHeader + @"\|(?:[^|]*\|){" + FieldNumber.ToString() + @"}([^|\r]*)");
            if (r.Matches(Message).Count > 0 && r.Matches(Message)[0].Groups.Count == 2)
            {
                IsValid = true;
                return r.Matches(Message)[0].Groups[1].Value;
            }
            else
            {
                if (IsMandate)
                {
                    IsValid = false;
                }
                else
                {
                    IsValid = true;
                }
                return "";
            }
        }

        public static string getValFromXML(XmlDocument xml, string field, int number = -1)
        {
            if (number != -1)
            {
                field = field + "." + number.ToString();
            }

            if (string.IsNullOrEmpty(xml.InnerXml))
            {
                return string.Empty;
            }
            string result = string.Empty;
            try
            {
                result = xml.SelectSingleNode("//" + field).InnerText;
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public static XmlDocument getXMLFromXML(XmlDocument xml, string field, int number = -1)
        {

            if (number != -1)
            {
                field = field + "." + number.ToString();
            }

            if (string.IsNullOrEmpty(xml.InnerXml))
            {
                return new XmlDocument();
            }
            XmlDocument result = new XmlDocument();
            try
            {
                foreach (XmlNode node in xml.SelectNodes("//" + field))
                {
                    result.InnerText += (node.OuterXml);
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }
    }
}
