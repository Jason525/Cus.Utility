using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Utility.DB
{
    public static class DBConvert
    {
        #region IDataReader
        public static T ToModel<T>(IDataReader dr) where T : class
        {
            return ToModel(dr, typeof(T)) as T;
        }

        public static object ToModel(IDataReader dr, Type target)
        {
            object model = Activator.CreateInstance(target);

            for (int i = 0; i < dr.FieldCount; i++)
            {
                string name = dr.GetName(i);
                object value = dr.GetValue(i);
                PropertyInfo p = target.GetProperty(name, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

                if (p != null && p.CanWrite)
                {
                    p.SetValue(model, SafeConversion.To(p.PropertyType, value), null);
                }
            }

            return model;
        }
        #endregion 

        #region DataTable / DataRow
        public static List<T> ToModels<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(ToModel<T>(row));
                }
            }

            return list;
        }

        public static T ToModel<T>(DataRow dr) where T : new()
        {
            T obj = new T();

            UpdateModel(obj, dr);

            return obj;
        }

        public static void UpdateModel<T>(T obj, DataRow dr) where T : new()
        {
            foreach (PropertyInfo p in typeof(T).GetProperties())
            {
                if (p.CanWrite && dr.Table.Columns.Contains(p.Name))
                {
                    object value = "";

                    if (dr[p.Name] != DBNull.Value && dr[p.Name] != null)
                    {
                        if (p.PropertyType.FullName == dr[p.Name].GetType().FullName)
                        {
                            value = dr[p.Name];
                            p.SetValue(obj, value, null);
                            continue;
                        }
                        else
                        {
                            value = dr[p.Name].ToString();
                        }
                    }

                    p.SetValue(obj, SafeConversion.To(p.PropertyType, value), null);
                }
            }
        }
        #endregion 
    }
}
