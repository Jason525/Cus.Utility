using System;
using System.Reflection;

namespace Utility.Excel
{
    public class ExcelCell
    {
        public string FieldName { get; set; }
        public CellStyle Style { get; set; }
        public IValueFormat Formatter { get; set; }

        public ExcelCell(string fieldName, IValueFormat formatter = null)
        {
            FieldName = fieldName;
            Formatter = formatter;
        }

        public object GetValue(IExcelRow item)
        {
            object result = "";
            PropertyInfo p = item.GetType().GetProperty(FieldName);

            if (p != null)
            {
                object value = p.GetValue(item, null);

                if (value != null)
                {
                    IValueFormat formatter = Formatter;

                    if (item is ExcelRow)
                    {
                        ExcelCell specificColumn = (item as ExcelRow).SpecificColumns.Find(c => c.FieldName == FieldName);

                        if (specificColumn != null && specificColumn.Formatter != null)
                        {
                            formatter = specificColumn.Formatter;
                        }
                    }

                    if (formatter != null)
                    {
                        result = formatter.Format(value);
                    }
                    else
                    {
                        result = p.PropertyType == typeof(DateTime?) ? value.ToSafeValue().ToDate() : value;
                    }
                }
            }

            return result;
        }
    }
}
