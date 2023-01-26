using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Utility.DB
{
    #region Attrubues
    public class DatasetAttribute : Attribute
    {
        public int Order { get; protected set; }

        public DatasetAttribute(int order)
        {
            Order = order;
        }
    }

    public class OutputAttribute : Attribute
    {

    }
    #endregion 

    #region DBQuery
    public class DBQuery
    {
        public string CommandText { get; set; }

        public bool IsSql { get; set; }

        public object Inputs { get; set; }

        public int? Timeout { get; set; }

        public List<PropertyInfo> Datasets()
        {
            return GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.GetCustomAttributes(typeof(DatasetAttribute), false).Any())
                .OrderBy(p => ((DatasetAttribute)p.GetCustomAttributes(typeof(DatasetAttribute), false)[0]).Order).ToList();
        }

        public IEnumerable<IDbDataParameter> Outputs(IDB db)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();
            List<PropertyInfo> pList = GetType().GetProperties().Where(p => !p.PropertyType.IsGenericType && p.GetCustomAttributes(typeof(OutputAttribute), false).Any()).ToList();

            pList.ForEach(p =>
            {
                IDbDataParameter parameter = db.ToParameter(p.Name, p.GetValue(this, null));
                parameter.Direction = ParameterDirection.Output;
                list.Add(parameter);
            });

            return list;
        }

        public void SetOutput(IDbDataParameter parameter)
        {
            if (parameter.Direction == ParameterDirection.Output)
            {
                PropertyInfo p = GetType().GetProperty(parameter.ParameterName.TrimStart('@'));

                if (p != null && p.CanWrite)
                {
                    p.SetValue(this, SafeConversion.To(p.PropertyType, parameter.Value), null);
                }
            }
        }

        public void SetOutputs(IDataParameterCollection parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                SetOutput(parameters[i] as IDbDataParameter);
            }
        }

        public void SetOutputs(IEnumerable<IDbDataParameter> parameters)
        {
            parameters.ToList().ForEach(SetOutput);
        }
    }
    #endregion
}
