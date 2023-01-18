using System.Collections.Generic;
using System.Linq;

namespace Utility.Excel
{
    public class SheetConfig
    {
        public string Name { get; set; }
        public bool AutoFit { get; set; }
        public List<ExcelColumn> Columns { get; set; }
        public List<IExcelRow> Items { get; set; }

        public SheetConfig()
        {
            Name = "sheet1";
            Columns = new List<ExcelColumn>();
            Items = new List<IExcelRow>();
        }

        public void SetColumns(IEnumerable<ExcelColumn> columns)
        {
            Columns = columns.ToList();
        }

        public void SetItems<T>(List<T> items) where T : IExcelRow
        {
            items.ForEach(item => Items.Add(item));
        }
    }
}
