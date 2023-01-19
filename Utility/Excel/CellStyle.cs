using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Utility.Excel
{
    #region CommonFormats
    public static class CommonFormats
    {
        /// <summary>
        /// $#,##0
        /// </summary>
        public const string DOLLAR = "$#,##0";
        /// <summary>
        /// $#,##0.00
        /// </summary>
        public const string DOLLAR_CENTS = "$#,##0.00";
        /// <summary>
        /// MM/DD/YYYY
        /// </summary>
        public const string DATE = "MM/DD/YYYY";
        /// <summary>
        /// DATETIME
        /// </summary>
        public const string DATETIME = "MM/dd/yy hh:mm PT";
        /// <summary>
        /// MM/DD/YY
        /// </summary>
        public const string SHORTDATE = "MM/DD/YY";
        /// <summary>
        /// ##0.000
        /// </summary>
        public const string PERCENT = "##0.000";
        /// <summary>
        /// 0.000%
        /// </summary>
        public const string PERCENT_SIGN = "0.000%";
    }
    #endregion 

    public class CellStyle
    {
        public bool Bold { get; set; }
        /// <summary>
        /// 0 ~ 12
        /// </summary>
        public int BorderBottom { get; set; }
        /// <summary>
        /// Suggest using CommonFormats firstly 
        /// </summary>
        public string Format { get; set; }
        public Action<ExcelStyle> DetailedSettings { get; set; }

        public void SetCell(ExcelRange cell)
        {
            if (!Format.IsEmpty()) cell.Style.Numberformat.Format = Format;
            cell.Style.Font.Bold = Bold;
            cell.Style.Border.Bottom.Style = (ExcelBorderStyle)BorderBottom;

            if (DetailedSettings != null) DetailedSettings(cell.Style);
        }
    }
}
