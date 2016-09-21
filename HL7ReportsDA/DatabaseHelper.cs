using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ReportsDataAndBusiness
{
    public static class DatabaseHelper
    {
        private static void RecordLog(string logMessage)
        {
            //  Common.Log.WriteFilelog("DatabaseHelper", "", logMessage);
        }

        public static SqlConnection GetConnection()
        {
            RecordLog("start GetConnection");
            SqlConnection conn = new SqlConnection();
            try
            {
                Properties.Settings.Default.Reload();
                conn.ConnectionString = "";
                conn.ConnectionString = Properties.Settings.Default.ConnectionString;
            }
            catch (Exception ex)
            {
                RecordLog("Exception in GetConnection. Message:" + ex.Message);
                throw ex;
            }
            RecordLog("complete GetConnection");
            return conn;
        }

        public static SqlDataReader ExecuteSQLReader(String SPName, List<SqlParameter> prms = null, System.Data.CommandType cmdType = System.Data.CommandType.StoredProcedure)
        {
            SqlDataAdapter da = new SqlDataAdapter();
            SqlConnection conn = GetConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            try
            {
                SqlCommand cmd = new SqlCommand(SPName, conn);
                cmd.CommandType = cmdType;
                if (prms != null && prms.Count > 0)
                {
                    cmd.Parameters.AddRange(prms.ToArray());
                }
                return cmd.ExecuteReader();
            }
            catch (Exception exx)
            {
                RecordLog("Exception in ExecuteSQLReader. Message:" + exx.Message);
                throw exx;
            }
            finally
            {
            }
        }

        public static int ExecuteNonQuery(String SPName, List<SqlParameter> prms = null, System.Data.CommandType cmdType = System.Data.CommandType.StoredProcedure)
        {
            SqlDataAdapter da = new SqlDataAdapter();
            SqlConnection conn = GetConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            try
            {
                SqlCommand cmd = new SqlCommand(SPName, conn);
                if (prms != null && prms.Count > 0)
                {
                    cmd.Parameters.AddRange(prms.ToArray());
                }

                cmd.CommandType = cmdType;

                return cmd.ExecuteNonQuery();
            }
            catch (Exception exx)
            {
                RecordLog("Exception in ExecuteNonQuery. Message:" + exx.Message);
                throw exx;
            }
            finally
            {
            }
        }
    }
}
