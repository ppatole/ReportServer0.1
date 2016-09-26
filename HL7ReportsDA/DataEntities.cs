using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReportsDataAndBusiness.DataEntities
{
    [Serializable]
    public class Report
    {
        String reportID = string .Empty;
        DateTime reportDateTime = new DateTime();
        DateTime importedOn = new DateTime();
        DateTime dOB = new DateTime();
        String refPhysician = string.Empty;
        String procedure = string.Empty;
        System.Xml.XmlDocument reportXML = new System.Xml.XmlDocument();
        string hL7 = string.Empty;

        String patientID = string.Empty;
        String patientname = String.Empty;
        String accessionNumber = String.Empty;
        List<Field> reportFields = new List<Field>();
        string reportText = string.Empty;

        XmlDocument observations = new XmlDocument();

        public string ReportID
        {
            get
            {
                return reportID;
            }

            set
            {
                reportID = value;
            }
        }

        public DateTime ReportDateTime
        {
            get
            {
                return reportDateTime;
            }

            set
            {
                reportDateTime = value;
            }
        }

        public DateTime ImportedOn
        {
            get
            {
                return importedOn;
            }

            set
            {
                importedOn = value;
            }
        }

        public List<Field> ReportFields
        {
            get
            {
                return reportFields;
            }

            set
            {
                reportFields = value;
            }
        }

        public string ReportText
        {
            get
            {
                return reportText;
            }

            set
            {
                reportText = value;
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

        public string Patientname
        {
            get
            {
                return patientname;
            }

            set
            {
                patientname = value;
            }
        }

        public string AccessionNumber
        {
            get
            {
                return accessionNumber;
            }

            set
            {
                accessionNumber = value;
            }
        }

        public string Procedure
        {
            get
            {
                return procedure;
            }

            set
            {
                procedure = value;
            }
        }

        public XmlDocument Observations
        {
            get
            {
                return observations;
            }

            set
            {
                observations = value;
            }
        }

        public DateTime DOB
        {
            get
            {
                return dOB;
            }

            set
            {
                dOB = value;
            }
        }

        public string RefPhysician
        {
            get
            {
                return refPhysician;
            }

            set
            {
                refPhysician = value;
            }
        }

        public string HL7
        {
            get
            {
                return hL7;
            }

            set
            {
                hL7 = value;
            }
        }

        public System.Xml.XmlDocument ReportXML
        {
            get
            {
                return reportXML;
            }

            set
            {
                reportXML = value;
            }
        }
    }

    [Serializable]
    public class Field
    {
        String fieldID;
        int sequence;
        string fieldNameID;
        String reportID;
        string value;
        FieldName fieldName;

        public string FieldID
        {
            get
            {
                return fieldID;
            }

            set
            {
                fieldID = value;
            }
        }

        public int Sequence
        {
            get
            {
                return sequence;
            }

            set
            {
                sequence = value;
            }
        }

        public string FieldNameID
        {
            get
            {
                return fieldNameID;
            }

            set
            {
                fieldNameID = value;
            }
        }

        public string ReportID
        {
            get
            {
                return reportID;
            }

            set
            {
                reportID = value;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public FieldName FieldName
        {
            get
            {
                return fieldName;
            }

            set
            {
                fieldName = value;
            }
        }
    }

    [Serializable]
    public class FieldName
    {
        String fieldNameID;
        String fieldNameDes;
        String parentFieldNameID;
        FieldName parentFieldName;

        public string FieldNameID
        {
            get
            {
                return fieldNameID;
            }

            set
            {
                fieldNameID = value;
            }
        }

        public string FieldNameDes
        {
            get
            {
                return fieldNameDes;
            }

            set
            {
                fieldNameDes = value;
            }
        }

        public string ParentFieldNameID
        {
            get
            {
                return parentFieldNameID;
            }

            set
            {
                parentFieldNameID = value;
            }
        }

        public FieldName ParentFieldName
        {
            get
            {
                return parentFieldName;
            }

            set
            {
                parentFieldName = value;
            }
        }
    }
}
