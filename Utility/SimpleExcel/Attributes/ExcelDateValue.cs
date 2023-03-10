using System;

namespace Utility.SimpleExcel.Attributes
{
    public class ExcelDateValueAttribute : Attribute
    {
        public string DateFormat { get; set; }

        public ExcelDateValueAttribute(string dateFmt)
        {
            DateFormat = dateFmt;
        }
    }
}
