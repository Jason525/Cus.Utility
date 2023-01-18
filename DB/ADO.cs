using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Data;

namespace Utility.DB
{
    public class ADO : DBClient
    {
        #region Properties
        public string ConnString { get; protected set; }
        #endregion 

        #region Creator
        public ADO(string conn, IDB db, int timeout = 180) : base(db, timeout)
        {
            ConnString = conn;
        }
        #endregion

        #region Advanced Query
        public override T Advanced<T>(T query)
        {
            var sets = query.Datasets();

            ExecuteReader(query, reader =>
            {
                var index = 0;

                do
                {
                    var p = sets[index];
                    var itemType = p.PropertyType.GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var list = Activator.CreateInstance(listType);
                    var addMethod = listType.GetMethod("Add");

                    while (reader.Read())
                    {
                        object item;
                        if (itemType.GetConstructors().Any(c => c.GetParameters().Count() == 0))
                        {
                            item = DBConvert.ToModel(reader, itemType);
                        }
                        else
                        {
                            item = reader.GetValue(0);
                        }
                        addMethod.Invoke(list, new object[] { item });
                    }

                    p.SetValue(query, list, null);
                    index += 1;
                }
                while (reader.NextResult() && index < sets.Count);
            });

            return query as T;
        }
        #endregion 

        #region Query List
        public override List<T> Query<T>(DBQuery query)
        {
            var list = new List<T>();

            ExecuteReader(query, reader => 
            {
                while (reader.Read())
                {
                    list.Add(DBConvert.ToModel<T>(reader));
                }
            });

            return list;
        }

        public override List<string> QueryStringList(DBQuery query)
        {
            var list = new List<string>();

            ExecuteReader(query, reader =>
            {
                while (reader.Read())
                {
                    list.Add(reader[0] as string);
                }
            });

            return list;
        }

        public override List<T> QuerySingleColumn<T>(DBQuery query)  
        {
            Func<IDataReader, T> strConvert = (reader) => { return (T)reader[0];  };
            Func<IDataReader, T> valConvert = (reader) => { return (T)SafeConversion.To(typeof(T), reader[0]); };
            var convert = typeof(T) == typeof(string) ? strConvert : valConvert;
            var list = new List<T>();

            ExecuteReader(query, reader =>
            {
                while (reader.Read())
                {
                    list.Add(convert(reader));
                }
            });

            return list;
        }
        #endregion

        #region Sacle
        public override T Sacle<T>(DBQuery query)  
        {
            Func<object, T> strConvert = (obj) => { return (T)obj; };
            Func<object, T> valConvert = (obj) => { return (T)SafeConversion.To(typeof(T), obj); };
            var convert = typeof(T) == typeof(string) ? strConvert : valConvert;
            var native = Sacle(query);

            return convert(native);
        }

        public override object Sacle(DBQuery query)
        {
            object result = null;

            if (!query.CommandText.IsEmpty())
            {
                Execute(query, cmd =>
                {
                    result = cmd.ExecuteScalar();
                    query.SetOutputs(cmd.Parameters);
                });
            }

            return result;
        }
        #endregion 

        #region NonQuery
        public override int NonQuery(DBQuery query)
        {
            var rows = 0;

            Execute(query, cmd =>
            {
                rows = cmd.ExecuteNonQuery();
                query.SetOutputs(cmd.Parameters);
            });

            return rows;
        }
        #endregion 

        #region Execute SQL
        public void ExecuteSQL(string sql, Action<IDataReader> invoke, object parameters = null)
        {
            ExecuteReader(new DBQuery { CommandText = sql, IsSql = true, Inputs = parameters }, invoke);
        }

        public List<T> ExecuteSQL<T>(string sql, object parameters = null) where T : class, new()
        {
            return Query<T>(sql, true, parameters);
        }
        #endregion 

        #region Query DataSet, DataTable  
        public virtual DataSet QueryDataSet(DBQuery query)
        {
            var ds = new DataSet();

            ExecuteReader(query, (reader) =>
            {
                while (!reader.IsClosed)
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                    ds.Tables.Add(dt);
                }
            });

            return ds;
        }

        public virtual DataSet QueryDataSet(string text, bool isSql, object parameters = null)
        {
            return QueryDataSet(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }

        public virtual DataTable QueryDataTable(DBQuery query)
        {
            var dt = new DataTable();

            ExecuteReader(query, (reader) =>
            {
                if (!reader.IsClosed)
                {
                    dt.Load(reader);
                }
            });

            return dt;
        }

        public virtual DataTable QueryDataTable(string text, bool isSql, object parameters = null)
        {
            return QueryDataTable(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }
        #endregion 

        #region Execute
        public void ExecuteReader(DBQuery query, Action<IDataReader> invoke)
        {
            Execute(query, cmd =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    invoke(reader);
                }

                query.SetOutputs(cmd.Parameters);
            });
        }

        public void Execute(DBQuery query, Action<IDbCommand> invoke)
        {
            var timeout = query.Timeout.HasValue ? query.Timeout.Value : this.CommandTimeout;

            using (var conn = Database.GetConnection(ConnString))
            {
                var command = Database.GetCommand(conn, query.IsSql ? CommandType.Text : CommandType.StoredProcedure, query.CommandText, timeout);
                var outputs = query.Outputs(Database);

                if (query.Inputs != null) ToParameters(query.Inputs).ToList().ForEach(p => command.Parameters.Add(p));
                if (outputs.Any()) outputs.ToList().ForEach(p => command.Parameters.Add(p));

                conn.Open();
                invoke(command);
                conn.Close();
            }
        }
        #endregion 
    }
}
