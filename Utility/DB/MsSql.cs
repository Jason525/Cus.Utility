using System.Data;
using System.Data.SqlClient;

namespace Utility.DB
{
    public class MsSql : IDB
    {
        public IDbDataParameter ToParameter(string name, object value)
        {
            if (value is TVP)
            {
                TVP tvp = value as TVP;
                SqlParameter tvpParam = new SqlParameter(string.Concat("@", name), SqlDbType.Structured)
                {
                    TypeName = tvp.TypeName, 
                    Value = tvp.Value
                };

                return tvpParam;
            }

            return new SqlParameter(string.Concat("@", name), value);
        }

        public IDbConnection GetConnection(string conn)
        {
            return new SqlConnection(conn);
        }

        public IDbCommand GetCommand(IDbConnection conn, CommandType commandType, string commandText, int commandTimeout)
        {
            return new SqlCommand() { Connection = conn as SqlConnection, CommandType = commandType, CommandText = commandText, CommandTimeout = commandTimeout };
        }

        public IDbDataAdapter GetAdapter()
        {
            return new SqlDataAdapter();
        }
    }
}
