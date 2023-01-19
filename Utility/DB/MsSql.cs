using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                var tvp = value as TVP;
                var tvpParam = new SqlParameter(string.Concat("@", name), SqlDbType.Structured);
                tvpParam.TypeName = tvp.TypeName;
                tvpParam.Value = tvp.Value;

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
