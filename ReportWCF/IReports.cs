using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ReportWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    
    public interface IReports
    {
        [OperationContract]
        List<string> GetAllQueryStrings();
        
        [OperationContract]
        string GetReports(String PatientName, String AccessionNumber, List<string> QueryStrings, string PatientID);

        [OperationContract]
        string GetHTMLReports(String PatientName, String AccessionNumber, string PatientID);                 
    }
}
