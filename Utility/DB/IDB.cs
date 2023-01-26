using System.Data;

namespace Utility.DB
{
    public interface IDB
    {
        IDbDataParameter ToParameter(string name, object value);
        IDbConnection GetConnection(string conn);
        IDbCommand GetCommand(IDbConnection conn, CommandType commandType, string commandText, int commandTimeout);
        IDbDataAdapter GetAdapter();
    }
}
