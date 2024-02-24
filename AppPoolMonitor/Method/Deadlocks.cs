using System;
using System.Data;
using System.Data.SqlClient;

namespace AppPoolMonitor.Method
{
    internal class Deadlocks
    {
        internal string DataSource;
        internal string InitialCatalog;
        internal string UserId;
        internal string Password;

        public Deadlocks()
        {
            DataSource = null;
            InitialCatalog = null;
            UserId = null;
            Password = null;
        }

        public DataTable Execute() {

        DataTable results = new DataTable();

            string connString = "Data Source="+ DataSource + ";Initial Catalog="+ InitialCatalog + ";User Id="+ UserId + ";Password="+ Password;

            string query = " SELECT"
                           + " CASE DTL.REQUEST_SESSION_ID"
                           + " WHEN -2 THEN 'ORPHANED DISTRIBUTED TRANSACTION'"
                           + " WHEN -3 THEN 'DEFERRED RECOVERY TRANSACTION'"
                           + " ELSE DTL.REQUEST_SESSION_ID END AS SPID,"
                           + " DB_NAME(DTL.RESOURCE_DATABASE_ID) AS DATABASENAME,"
                           + " SO.NAME AS LOCKEDOBJECTNAME,"
                           + " DTL.RESOURCE_TYPE AS LOCKEDRESOURCE,"
                           + " DTL.REQUEST_MODE AS LOCKTYPE,"
                           + " ST.TEXT AS SQLSTATEMENTTEXT,"
                           + " ES.LOGIN_NAME AS LOGINNAME,"
                           + " ES.HOST_NAME AS HOSTNAME,"
                           + " CASE TST.IS_USER_TRANSACTION"
                           + " WHEN 0 THEN 'SYSTEM TRANSACTION'"
                           + " WHEN 1 THEN 'USER TRANSACTION' END AS USER_OR_SYSTEM_TRANSACTION,"
                           + " AT.NAME AS TRANSACTIONNAME,"
                           + " DTL.REQUEST_STATUS"
                           + " FROM"
                           + " SYS.DM_TRAN_LOCKS DTL"
                           + " JOIN SYS.PARTITIONS SP ON SP.HOBT_ID = DTL.RESOURCE_ASSOCIATED_ENTITY_ID"
                           + " JOIN SYS.OBJECTS SO ON SO.OBJECT_ID = SP.OBJECT_ID"
                           + " JOIN SYS.DM_EXEC_SESSIONS ES ON ES.SESSION_ID = DTL.REQUEST_SESSION_ID"
                           + " JOIN SYS.DM_TRAN_SESSION_TRANSACTIONS TST ON ES.SESSION_ID = TST.SESSION_ID"
                           + " JOIN SYS.DM_TRAN_ACTIVE_TRANSACTIONS AT ON TST.TRANSACTION_ID = AT.TRANSACTION_ID"
                           + " JOIN SYS.DM_EXEC_CONNECTIONS EC ON EC.SESSION_ID = ES.SESSION_ID"
                           + " CROSS APPLY SYS.DM_EXEC_SQL_TEXT(EC.MOST_RECENT_SQL_HANDLE) AS ST"
                           + " WHERE"
                           + " RESOURCE_DATABASE_ID = DB_ID()"
                           + " ORDER BY DTL.REQUEST_SESSION_ID;";

            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand command = new SqlCommand(query, conn))
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                dataAdapter.Fill(results);

            return results;
        }
    }
}
