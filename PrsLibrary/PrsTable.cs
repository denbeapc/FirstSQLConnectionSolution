using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsLibrary {
    public class PrsTable {
        protected static string ConnStr = @"Server=STUDENT05;Database=prs;Trusted_Connection=true;";

        protected static SqlCommand CreateSQLConnection(string ConnStr, string Sql, string Message) {
            SqlConnection Conn = new SqlConnection(ConnStr);
            Conn.Open();

            if (Conn.State != ConnectionState.Open) {
                throw new ApplicationException(Message);
            }

            SqlCommand Command0 = new SqlCommand(Sql, Conn);
            return Command0;
        }

        protected static int GetLastIdGenerated(string ConnStr, string TableName) {
            string Sql = String.Format("SELECT IDENT_CURRENT('{0}')", TableName);
            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Failed to load last index ... ");
            object newId = Command0.ExecuteScalar();
            return int.Parse(newId.ToString());
        }

        protected static int ExecuteSQLInsUpdDelCommand(SqlCommand Command0, string Message) {
            int recsAffected = Command0.ExecuteNonQuery();
            if (recsAffected != 1) {
                throw new ApplicationException(Message);
            }
            return recsAffected;
        }
    }
}
