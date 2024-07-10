using newTask.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace newTask.Packages
{
    public interface IPKG_ERROR_LOGS
    {
        public void SaveLogs(string ErrorText, int? UserId);
    }
    public class PKG_ERROR_LOGS:PKG_BASE, IPKG_ERROR_LOGS
    {
        IConfiguration config;

        public PKG_ERROR_LOGS(IConfiguration config) : base(config)
        {
            this.config = config;
        }

        public void SaveLogs(string ErrorText, int? UserId)
        {
            {
                using (OracleConnection conn = new OracleConnection(ConnStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("c##tat.pkg_error_logs.save_logs", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_errorText", OracleDbType.Varchar2).Value = ErrorText;                       
                        cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = UserId??0;

                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }

            }

        }
    }
}
