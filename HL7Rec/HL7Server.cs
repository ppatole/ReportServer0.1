
using System.ServiceProcess;
using System.Threading.Tasks;
using System;

namespace HL7ReportListnerService
{
    public partial class HL7Server : ServiceBase
    {
        public HL7Server()
        {
            RecordLog("HL7 service init");
            InitializeComponent();
            RecordLog("HL7 service init complete");
        }
        HL7Listener.HL7Listener hll = new HL7Listener.HL7Listener();

        protected override void OnStart(string[] args)
        {
            RecordLog("Starting service");
            try
            {

                if (hll != null)
                {
                    Task t = new Task(() => hll.HL7Listen());
                    t.Start();
                }
                else
                {
                    RecordLog("Listener not initialised.:");
                }

            }
            catch (Exception ex)
            {
                RecordLog("Exception in service start. Message:" + ex.Message);
                RecordLog("Exception in service start. Base Message:" + ex.GetBaseException().Message);
            }
        }

        protected override void OnStop()
        {
            RecordLog("Starting service");
            try
            {
                if (hll != null)
                {
                    Task t = new Task(() => hll.HL7StopListen());
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                RecordLog("Exception in service stop. Message:" + ex.Message);
                RecordLog("Exception in service stop. Base Message:" + ex.GetBaseException().Message);
            }
        }

        private void RecordLog(string logMessage)
        {
            Common.Log L = new Common.Log("", Properties.Settings.Default.EnableDebugLogging);
            L.WriteFilelog("HL7ReportsService", "HL7 Reports windows service", logMessage);
        }
    }
}
