using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCCIReports
{
    public interface I_PCCIReports
    {
        string getReport(string accessionNo, string patientID);
        string AccessionNo { get; set; }
        string PatientID { get; set; }

        string Org { get; }

        string PatientName { get; set; }
    }
}
