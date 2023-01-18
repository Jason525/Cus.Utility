using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace Utility.DB
{
    public abstract class DBClient
    {
        #region Properties
        public virtual IDB Database { get; protected set; }
        public virtual int CommandTimeout { get; protected set; }
        #endregion 

        #region Creator
        public DBClient(IDB database, int timeout = 180)
        {
            Database = database;
            CommandTimeout = timeout;
        }
        #endregion 

        #region Advanced Query
        public abstract T Advanced<T>(T query) where T : DBQuery, new();
        #endregion 

        #region Query Models
        public virtual T First<T>(string text, bool isSql, object parameters = null) where T : class, new()
        {
            return Query<T>(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters }).FirstOrDefault();
        }

        public abstract List<T> Query<T>(DBQuery query) where T : class, new();
        public virtual List<T> Query<T>(string text, bool isSql, object parameters = null) where T : class, new()
        {
            return Query<T>(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }
        #endregion

        #region Query Single Column
        public abstract List<string> QueryStringList(DBQuery query);
        public virtual List<string> QueryStringList(string text, bool isSql, object parameters = null)
        {
            return QueryStringList(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }

        public abstract List<T> QuerySingleColumn<T>(DBQuery query);
        public virtual List<T> QuerySingleColumn<T>(string text, bool isSql, object parameters = null) 
        {
            return QuerySingleColumn<T>(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }
        #endregion 

        #region Query Single
        public abstract T Sacle<T>(DBQuery query);
        public virtual T Sacle<T>(string text, bool isSql, object parameters = null) 
        {
            return Sacle<T>(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }

        public abstract object Sacle(DBQuery query);
        public virtual object Sacle(string text, bool isSql, object parameters = null)
        {
            return Sacle(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }
        #endregion 

        #region NonQuery
        public abstract int NonQuery(DBQuery query);
        public virtual int NonQuery(string text, bool isSql, object parameters = null)
        {
            return NonQuery(new DBQuery { CommandText = text, IsSql = isSql, Inputs = parameters });
        }
        #endregion 

        #region Mothods : Common
        public virtual IEnumerable<IDbDataParameter> ToParameters(object paras)
        {
            var list = new List<IDbDataParameter>();

            if (paras != null)
            {
                var pt = paras.GetType();

                if (pt.Name == "ExpandoObject")
                {
                    (paras as IDictionary<string, object>).ToList().ForEach(pi =>
                    {
                        list.Add(Database.ToParameter(pi.Key, pi.Value ?? DBNull.Value));
                    });
                }
                else
                {
                    pt.GetProperties().ToList().ForEach(pi =>
                    {
                        list.Add(Database.ToParameter(pi.Name, pi.GetValue(paras, null) ?? DBNull.Value));
                    });
                }
            }

            return list;
        }
        #endregion
    }
}
