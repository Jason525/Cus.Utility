using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Excel
{
    public class ExcelColumn
    {
        #region Head Settings
        public ExcelCell HeadSettings { get; set; }
        public string HeadText { get { return HeadSettings.FieldName; } set { HeadSettings.FieldName = value; } }
        public CellStyle HeadStyle { get { return HeadSettings.Style; } set { HeadSettings.Style = value; } }
        #endregion 

        #region Cell Settings
        public ExcelCell CellSettings { get; set; }
        public string FieldName { get { return CellSettings.FieldName; } set { CellSettings.FieldName = value; } }
        public CellStyle ItemStyle { get { return CellSettings.Style; } set { CellSettings.Style = value; } }
        public IValueFormat ValueFormat { get { return CellSettings.Formatter; } set { CellSettings.Formatter = value; } }
        #endregion

        #region Creator
        public ExcelColumn(string fieldName, IValueFormat formatter = null)
        {
            HeadSettings = new ExcelCell(fieldName);
            CellSettings = new ExcelCell(fieldName, formatter);  
        }

        public ExcelColumn(string fieldName, string headText, IValueFormat formatter = null)
        {
            HeadSettings = new ExcelCell(headText);
            CellSettings = new ExcelCell(fieldName, formatter);
        }
        #endregion 
    }
}
