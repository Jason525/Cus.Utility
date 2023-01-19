using System;

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
            var p = item.GetType().GetProperty(FieldName);

            if (p != null)
            {
                var value = p.GetValue(item, null);

                if (value != null)
                {
                    var formatter = Formatter;

                    if (item is ExcelRow)
                    {
                        var specificColumn = (item as ExcelRow).SpecificColumns.Find(c => c.FieldName == FieldName);

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
                        if (p.PropertyType == typeof(DateTime?) && value!=null)
                        {
                            result = value.ToSafeValue().ToDate();
                        }
                        else
                        {
                            result = value;
                        }
                    }
                }
            }

            return result;
        } 
    }
}
