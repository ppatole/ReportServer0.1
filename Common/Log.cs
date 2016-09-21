using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Common
{
    public class Log : IDisposable
    {
        bool IsFileLogInit = false;
        String LogDirectory = Properties.Settings.Default.LogLocation;
        Queue<string> Logqu = new Queue<string>();
        bool IsLoggingEnabled = true;
        bool stopLog = false;
        String LogFileName = "";
        StreamWriter sw;
        string Logname = string.Empty;


        private Log()
        { }

        public Log(string LogName = "", bool EnableLogging = true)
        {
            IsLoggingEnabled = EnableLogging;
            Logname = LogName;
            LogFileName = LogDirectory + "\\" + LogName + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (!IsFileLogInit)
            {
                IsFileLogInit = true;
                InitFileLog();
            }
        }

        public void InitFileLog()
        {
            //IsLoggingEnabled = Properties.Settings.Default.EnableDebugLogging;
            if (IsLoggingEnabled)
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
                Logqu = new Queue<string>();
                Task t = new Task(() => LogWriter());
                t.Start();
            }
        }

        public void WriteFilelog(String AppName, String ModuleName, String Message)
        {
            if (IsLoggingEnabled)
            {
                Message = DateTime.Now.ToString("HH:mm:ss.fff") + " : " + ModuleName + " : " + Message;
                Logqu.Enqueue(Message);
            }
        }


        private void LogWriter()
        {
            while (true)
            {
                if (Logqu.Count > 0)
                {

                    lock (LogFileName)
                    {
                        string t = "";
                        while (Logqu.Count > 0)
                        {
                            t += (Logqu.Dequeue()) + "\r\n";
                        }

                        LogFileName = LogDirectory + "\\" + Logname + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        if (sw != null)
                        {
                            sw.Close();
                        }
                        sw = new StreamWriter(LogFileName, true);

                        sw.Write(t);
                        sw.Close();
                    }
                }
                else
                {
                    if (stopLog)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        public void Dispose()
        {
            Logqu.Enqueue("Stop Logging");
            stopLog = true;
        }
    }


}
