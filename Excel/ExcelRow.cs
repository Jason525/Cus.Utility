using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Excel
{
    public interface IExcelRow { }

    public abstract class ExcelRow : IExcelRow
    {
        private List<ExcelCell> _SpecificColumns;

        public virtual List<ExcelCell> SpecificColumns
        {
            get
            {
                if (_SpecificColumns == null) _SpecificColumns = new List<ExcelCell>();

                return _SpecificColumns;
            }
            set
            {
                _SpecificColumns = value;
            }
        }

        public IValueFormat this[string fieldName]
        {
            get
            {
                var col = SpecificColumns.Find(c => c.FieldName == fieldName);

                return col == null ? col.Formatter : null;
            }
            set
            {
                var col = SpecificColumns.Find(c => c.FieldName == fieldName);

                if (col != null)
                {
                    col.Formatter = value;
                }
                else
                {
                    SpecificColumns.Add(new ExcelCell(fieldName, value));
                }
                
            }
        }

        public virtual void AddSpecific(string fieldName, IValueFormat formatter)
        {
            SpecificColumns.Add(new ExcelCell(fieldName, formatter));
        }
    }
}
