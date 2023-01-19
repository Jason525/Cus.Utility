using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Objects;

namespace Utility.DB
{
    public class EF<DB> : DBClient where DB : ObjectContext,  new()
    {
        #region Creator
        public EF(IDB client, int timeout = 180) : base(client, timeout)
        {

        }
        #endregion 

        #region Advanced
        public override T Advanced<T>(T query)
        {
            throw new NotImplementedException();
        }
        #endregion 

        #region Query
        public override List<T> Query<T>(DBQuery query)
        {
            var list = new List<T>();

            Execute(query, db => 
            {
                list = Query<T>(db, query);
            });

            return list;
        }

        protected List<T> Query<T>(ObjectContext db, DBQuery query)
        {
            var sParas = ToParameters(query.Inputs).ToList();
            var outputs = query.Outputs(Database);
            var result = new List<T>();

            if (query.IsSql)
            {
                sParas.AddRange(outputs);
                result = db.ExecuteStoreQuery<T>(query.CommandText, sParas.ToArray()).ToList();
            }
            else
            {
                var sql = new StringBuilder();

                sql.AppendFormat(" EXEC [dbo].[{0}] ", query.CommandText);
                sParas.ToList().ForEach(p => sql.AppendFormat(" {0}={0},", p.ParameterName));

                if (outputs.Any())
                {
                    outputs.ToList().ForEach(op =>
                    {
                        sParas.Add(op);
                        sql.AppendFormat(" {0}={0} OUTPUT, ", op.ParameterName);
                    });
                }

                result = db.ExecuteStoreQuery<T>(sql.ToString().Trim().TrimEnd(',', ' '), sParas.ToArray()).ToList();
            }

            if (outputs.Any()) query.SetOutputs(outputs);

            return result;
        }

        public override List<string> QueryStringList(DBQuery query)
        {
            var list = new List<string>();

            Execute(query, db =>
            {
                list = Query<string>(db, query);
            });

            return list;
        }

        public override List<T> QuerySingleColumn<T>(DBQuery query)
        {
            var list = new List<T>();

            Execute(query, db =>
            {
                list = Query<T>(db, query);
            });

            return list;
        }
        #endregion 

        #region Sacle
        public override T Sacle<T>(DBQuery query)
        {
            var result = default(T);

            Execute(query, db =>
            {
                result = Query<T>(db, query).FirstOrDefault();
            });

            return result;
        }

        public override object Sacle(DBQuery query)
        {
            object result = null;

            Execute(query, db =>
            {
                result = (object)Query<string>(db, query).FirstOrDefault();
            });

            return result;
        }
        #endregion 

        #region NonQuery
        public override int NonQuery(DBQuery query)
        {
            var rows = 0;

            Execute(query, db => 
            {
                var paras = new List<IDbDataParameter>();
                var outputs = query.Outputs(Database);

                if (query.Inputs != null) paras.AddRange(ToParameters(query.Inputs));

                paras.AddRange(outputs);

                rows = db.ExecuteStoreCommand(query.CommandText, paras.ToArray());
                query.SetOutputs(outputs);
            });

            return rows;
        }
        #endregion 

        #region Execute
        public void Execute(DBQuery query, Action<DB> invoke)
        {
            var timeout = (query != null && query.Timeout.HasValue) ? query.Timeout.Value : this.CommandTimeout;

            using (var db = new DB())
            {
                db.CommandTimeout = timeout;
                invoke(db);
            }
        }

        public void Execute(Action<DB> invoke)
        {
            Execute(null, invoke);
        }
        #endregion 
    }
}
