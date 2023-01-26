using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace Utility.DB
{
    public class EF<DB> : DBClient where DB : ObjectContext, new()
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
            List<T> list = new List<T>();

            Execute(query, db =>
            {
                list = Query<T>(db, query);
            });

            return list;
        }

        protected List<T> Query<T>(ObjectContext db, DBQuery query)
        {
            List<IDbDataParameter> sParas = ToParameters(query.Inputs).ToList();
            List<IDbDataParameter> outputs = query.Outputs(Database).ToList();
            List<T> result;

            if (query.IsSql)
            {
                sParas.AddRange(outputs);
                result = db.ExecuteStoreQuery<T>(query.CommandText, sParas.ToArray()).ToList();
            }
            else
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendFormat(" EXEC [dbo].[{0}] ", query.CommandText);
                sParas.ToList().ForEach(p => sql.AppendFormat(" {0}={0},", p.ParameterName));

                if (outputs.Any())
                {
                    outputs.ForEach(op =>
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
            List<string> list = new List<string>();

            Execute(query, db =>
            {
                list = Query<string>(db, query);
            });

            return list;
        }

        public override List<T> QuerySingleColumn<T>(DBQuery query)
        {
            List<T> list = new List<T>();

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
            T result = default(T);

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
                result = Query<string>(db, query).FirstOrDefault();
            });

            return result;
        }
        #endregion 

        #region NonQuery
        public override int NonQuery(DBQuery query)
        {
            int rows = 0;

            Execute(query, db =>
            {
                List<IDbDataParameter> paras = new List<IDbDataParameter>();
                List<IDbDataParameter> outputs = query.Outputs(Database).ToList();

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
            int timeout = query != null && query.Timeout.HasValue ? query.Timeout.Value : CommandTimeout;

            using (DB db = new DB())
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
