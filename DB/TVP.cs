using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;

using Utility;

namespace Utility.DB
{
    #region Attrubues
    public class TvpItemAttribute : Attribute
    {
        
    }

    public class TvpFieldAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public int MaxLength { get; set; }

        internal PropertyInfo Field { get; set; }
        internal TVPSpecialHandler Handler { get; set; }

        public TvpFieldAttribute(int length = -1, string name = "")
        {
            ColumnName = name;
            MaxLength = length;
        }

        internal object GetValue(object instance)
        {
            var value = Field.GetValue(instance, null);

            if (Handler != null && Handler.Handle != null)
            {
                value = Handler.Handle(value);
            }

            if (MaxLength > 0 && Field.PropertyType == typeof(string))
            {
                value = LimitString(value);
            }

            return value;
        }

        internal object LimitString(object value)
        {
            var str = value.ToSafeValue();

            if (str.Length > MaxLength)
            {
                str = str.Trim().Substring(0, MaxLength);
            }

            return str;
        }
    }
    #endregion 

    #region Special Handler
    public class TVPSpecialHandler
    {
        public List<string> Fields { get; set; }
        public Func<object, object> Handle { get; protected set; }

        public TVPSpecialHandler(params string[] fields)
        {
            Fields = fields.ToList();
        }

        public TVPSpecialHandler SetHandle<T>(Func<T, T> handle) 
        {
            Handle = (o) => { return handle((T)o); };
            return this;
        }
    }

    public interface ITVPSpecialHandling
    {
        List<TVPSpecialHandler> Handlers { get; }
    }
    #endregion

    #region Table Value Parameter
    public class ITvpItem { }

    public class TVP
    {
        public string TypeName { get; set; }
        public DataTable Value { get; set; }
        protected List<ITvpItem> Items { get; set; }

        public TVP(string pName, string tName)
        {
            TypeName = tName;
            Items = new List<ITvpItem>();
        }

        public TVP(string pName, string tName, DataTable value)
        {
            TypeName = tName;
            Value = value;
        }

        public TVP SetValue<T>(List<T> items, ITVPSpecialHandling handling = null) where T : class, new()
        {
            Value = ToDataTable(items, handling);
            return this;
        }

        protected DataTable ToDataTable<T>(List<T> items, ITVPSpecialHandling handling) where T : class, new()
        {
            var dt = new DataTable();

            if (items.Any())
            {
                var type = items[0].GetType();
                var fields = type.GetProperties().ToList();
                var isSpecial = type.GetCustomAttributes(typeof(TvpItemAttribute), true).Any();
                var attributes = GetTvpFieldAttrubues(fields, isSpecial).ToList();

                if (handling != null)
                {
                    attributes.ForEach(attr =>
                    {
                        dt.Columns.Add(attr.ColumnName);
                        attr.Handler = handling.Handlers.FirstOrDefault(h => h.Fields.Contains(attr.ColumnName));
                    });
                }
                else
                {
                    attributes.ForEach(attr => { dt.Columns.Add(attr.ColumnName); });
                }

                items.ForEach(item =>
                {
                    var row = dt.NewRow();

                    attributes.ForEach(attr =>
                    {
                        row[attr.ColumnName] = attr.GetValue(item);
                    });

                    dt.Rows.Add(row);
                });
            }

            return dt;
        }

        protected TvpFieldAttribute GetTvpFieldAttribute(PropertyInfo p)
        {
            return p.GetCustomAttributes(typeof(TvpFieldAttribute), true).FirstOrDefault() as TvpFieldAttribute;
        }

        protected IEnumerable<TvpFieldAttribute> GetTvpFieldAttrubues(IEnumerable<PropertyInfo> fields, bool isSpecial)
        {
            var list = fields.Select(p => new { field = p, attr = GetTvpFieldAttribute(p) });

            //if class is TvpItemAttribute, all properties are valid no matter whether it is TvpFieldAttribute
            //if class is not TvpItemAttribute, only use the properties with TvpFieldAttribute.  
            if (!isSpecial) list = list.Where(p => p.attr != null); 

            return list.Select(i =>
            {
                var attr = i.attr ?? new TvpFieldAttribute();
                attr.Field = i.field;
                if (attr.ColumnName.IsEmpty()) attr.ColumnName = i.field.Name;
                return attr;
            });
        }
    }
    #endregion
}
