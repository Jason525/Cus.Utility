using System.Collections.Generic;

namespace Utility.Excel
{
    public interface IExcelRow { }

    public abstract class ExcelRow : IExcelRow
    {
        private List<ExcelCell> _specificColumns;

        public virtual List<ExcelCell> SpecificColumns
        {
            get => _specificColumns ?? (_specificColumns = new List<ExcelCell>());
            set => _specificColumns = value;
        }

        public IValueFormat this[string fieldName]
        {
            get
            {
                ExcelCell col = SpecificColumns.Find(c => c.FieldName == fieldName);

                return col?.Formatter;
            }
            set
            {
                ExcelCell col = SpecificColumns.Find(c => c.FieldName == fieldName);

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
