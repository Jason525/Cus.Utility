using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Utility.SimpleExcel.Attributes;

namespace Utility.SimpleExcel
{
    public class ExcelHelper
    {

        #region Create New Excel
        /// <summary>
        /// Creates the new excel.
        /// </summary>
        /// <param name="headers">list of column header text</param>
        /// <param name="filename">full path to the XLSX file to create; will append to this</param>
        public static void CreateNewExcel(string[] headers, string filename)
        {
            CreateNewExcel(headers, filename, "Sheet1");
        }
        /// <summary>
        ///  Creates a XLSX file (2007 and later) with the passed-in headers.
        ///  The result is an Excel file with column headers only.
        /// </summary>
        /// <param name="headers">list of column header text</param>
        /// <param name="filename">full path to the XLSX file to create; will overwrite if exists.</param>
        /// <param name="sheet">The sheet.</param>
        public static void CreateNewExcel(string[] headers, string filename, string sheet)
        {
            string dir = Path.GetDirectoryName(filename);

            if (File.Exists(filename)) File.Delete(filename);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);//Steve #1041 2014.09.10

            int colIdx = 1;

            FileInfo newFile = new FileInfo(filename);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add(sheet);

                foreach (string col in headers)
                {
                    worksheet.Cells[1, colIdx].Value = col;
                    worksheet.Cells[1, colIdx].Style.Font.Bold = true;
                    colIdx++;
                }

                for (int i = 1; i < colIdx + 10; i++)
                {
                    worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                for (int j = 1; j < colIdx; j++)
                {
                    worksheet.Column(j).AutoFit();
                }

                xlPackage.Save();
            }
        }
        #endregion

        #region Add the eccel row
        /// <summary>
        /// Add the excel row.
        /// </summary>
        /// <param name="rowList">The rows.</param>
        /// <param name="filename">full path to the XLSX file to create; will append to this</param>
        public static void AddExcelRow(List<ExcelRowModel> rowList, string filename)
        {
            AddExcelRow(rowList, filename, "Sheet1");
        }

        /// <summary>
        /// Add the excel row.
        /// </summary>
        /// <param name="rowList">The rows.</param>
        /// <param name="filename">full path to the XLSX file to create; will append to this</param>
        /// <param name="sheet">The sheet.</param>
        public static void AddExcelRow(List<ExcelRowModel> rowList, string filename, string sheet)
        {
            FileInfo newFile = new FileInfo(filename);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[sheet];

                int rowIdx = worksheet.Dimension.End.Row + 1;
                foreach (ExcelRowModel excelRowModel in rowList)
                {
                    // Find the last row
                    #region insert data

                    int i = 1;
                    foreach (ExcelCellValue cv in excelRowModel.CellValues)
                    {
                        worksheet.Cells[rowIdx, i].Value = cv.CellValue;

                        //if (!string.IsNullOrEmpty(cv.CellValue.ToString()))
                        {
                            switch (cv.CellFormat)
                            {
                                case ExcelCellValue.FORMAT_DOLLAR:
                                    worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "$#,##0";
                                    break;
                                case ExcelCellValue.FORMAT_DOLLAR_CENTS:
                                    worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "$#,##0.00";
                                    break;
                                case ExcelCellValue.FORMAT_DATE:
                                    worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "MM/DD/YYYY";
                                    if (!cv.CellValue.ToSafeValue().IsEmpty())
                                    {
                                        worksheet.Cells[rowIdx, i].Value = cv.CellValue.ToSafeValue().ToDate();
                                    }
                                    break;
                                case ExcelCellValue.FORMAT_DATETIME:
                                    worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "MM/dd/yy hh:mm PT";
                                    if (!cv.CellValue.ToSafeValue().IsEmpty())
                                    {
                                        worksheet.Cells[rowIdx, i].Value = cv.CellValue.ToSafeValue().ToDate();
                                    }
                                    break;
                                case ExcelCellValue.FORMAT_PERCENT:
                                    worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "##0.000";
                                    break;
                                case ExcelCellValue.FORMAT_PERCENT_SIGN:
                                    double percent = 0;
                                    if (double.TryParse(cv.CellValue.ToString(), out percent))
                                        worksheet.Cells[rowIdx, i].Value = percent.ToString("##0.00") + "%";
                                    break;
                                case ExcelCellValue.FORMAT_ZIP:
                                    worksheet.Cells[rowIdx, i].Value = cv.CellValue.ToString().TrimEnd('-');
                                    break;
                                case ExcelCellValue.FORMAT_NUMBER:
                                    double num = 0;
                                    if (double.TryParse(cv.CellValue.ToString(), out num))
                                        worksheet.Cells[rowIdx, i].Value = num;
                                    break;
                                case ExcelCellValue.FORMAT_LONGTEXT:
                                    worksheet.Cells[rowIdx, i].Style.WrapText = true;
                                    worksheet.Cells[rowIdx, i].Style.ShrinkToFit = true;
                                    ExcelColumn col = worksheet.Column(i);
                                    col.Width = 150;
                                    break;
                                default:
                                    worksheet.Cells[rowIdx, i].Value = cv.CellValue;
                                    break;
                            }
                        }
                        worksheet.Cells[rowIdx, i].Style.Font.Size = 9;
                        worksheet.Cells[rowIdx, i].Style.Font.Name = "Calibri";
                        i++;
                    }
                    #endregion
                    rowIdx++;
                }
                for (int j = 1; j < rowList[0].CellValues.Count - 1; j++)
                {
                    worksheet.Column(j).AutoFit();
                }
                xlPackage.Save();
            }
        }

        private static void DoUpdateRowValues(ExcelPackage xlPackage, List<ExcelRowModel> rowList, string filename, string sheet, int startRowIndex)
        {
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[sheet];
            int rowIdx = startRowIndex;
            foreach (ExcelRowModel excelRowModel in rowList)
            {
                // Find the last row
                #region insert data

                int i = 1;
                foreach (ExcelCellValue cv in excelRowModel.CellValues)
                {
                    worksheet.Cells[rowIdx, i].Value = cv.CellValue;

                    //if (!string.IsNullOrEmpty(cv.CellValue.ToString()))
                    {
                        switch (cv.CellFormat)
                        {
                            case ExcelCellValue.FORMAT_DOLLAR:
                                worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "$#,##0";
                                break;
                            case ExcelCellValue.FORMAT_DOLLAR_CENTS:
                                worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "$#,##0.00";
                                break;
                            case ExcelCellValue.FORMAT_DATE:
                                worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "MM/DD/YYYY";
                                if (!cv.CellValue.ToSafeValue().IsEmpty())
                                {
                                    worksheet.Cells[rowIdx, i].Value = cv.CellValue.ToSafeValue().ToDate();
                                }
                                break;
                            case ExcelCellValue.FORMAT_DATETIME:
                                worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "MM/dd/yy hh:mm PT";
                                if (!cv.CellValue.ToSafeValue().IsEmpty())
                                {
                                    worksheet.Cells[rowIdx, i].Value = cv.CellValue.ToSafeValue().ToDate();
                                }
                                break;
                            case ExcelCellValue.FORMAT_PERCENT:
                                worksheet.Cells[rowIdx, i].Style.Numberformat.Format = "##0.000";
                                break;
                            case ExcelCellValue.FORMAT_PERCENT_SIGN:
                                double percent = 0;
                                if (double.TryParse(cv.CellValue.ToString(), out percent))
                                    worksheet.Cells[rowIdx, i].Value = percent.ToString("##0.00") + "%";
                                break;
                            case ExcelCellValue.FORMAT_ZIP:
                                worksheet.Cells[rowIdx, i].Value = cv.CellValue.ToString().TrimEnd('-');
                                break;
                            case ExcelCellValue.FORMAT_NUMBER:
                                double num = 0;
                                if (double.TryParse(cv.CellValue.ToString(), out num))
                                    worksheet.Cells[rowIdx, i].Value = num;
                                break;
                            case ExcelCellValue.FORMAT_LONGTEXT:
                                worksheet.Cells[rowIdx, i].Style.WrapText = true;
                                worksheet.Cells[rowIdx, i].Style.ShrinkToFit = true;
                                ExcelColumn col = worksheet.Column(i);
                                col.Width = 150;
                                break;
                            default:
                                worksheet.Cells[rowIdx, i].Value = cv.CellValue;
                                break;
                        }
                    }
                    worksheet.Cells[rowIdx, i].Style.Font.Size = 9;
                    worksheet.Cells[rowIdx, i].Style.Font.Name = "Calibri";
                    i++;
                }
                #endregion
                rowIdx++;
            }
            for (int j = 1; j < rowList[0].CellValues.Count - 1; j++)
            {
                worksheet.Column(j).AutoFit();
            }
        }

        public static void UpdateRows(List<ExcelRowModel> rowList, string filename, string sheet, int startRowIndex)
        {
            FileInfo newFile = new FileInfo(filename);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                DoUpdateRowValues(xlPackage, rowList, filename, sheet, startRowIndex);
                xlPackage.Save();
            }
        }
        #endregion

        #region Insert data to excel
        /// <summary>
        /// Inserts the data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="rowList">The rows.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        public static void InsertData(string fileName, List<ExcelRowModel> rowList, string sheetName)
        {
            if (rowList.Count > 0)
            {
                List<ExcelRowModel> tempList = new List<ExcelRowModel>();
                foreach (ExcelRowModel excelRowModel in rowList)
                {
                    List<ExcelCellValue> cellValues = new List<ExcelCellValue>();
                    foreach (ExcelCellModel cell in excelRowModel.CellModels)
                    {
                        ExcelCellValue cv = new ExcelCellValue
                        {
                            CellValue = cell.CellValue,
                            CellFormat = cell.CellFormat
                        };
                        cellValues.Add(cv);
                    }
                    excelRowModel.CellValues = cellValues;
                    tempList.Add(excelRowModel);
                }

                AddExcelRow(tempList, fileName, sheetName);
            }
        }
        #endregion

    }

    public class ExcelCellValue
    {
        public object CellValue { get; set; }
        public string CellFormat { get; set; } // use "Text", "Dollar", "DollarWithCents", "Date", "DateTime", "Percent", "PercentWithSign", "Number"
        public const string FORMAT_TEXT = "Text";
        public const string FORMAT_DOLLAR = "Dollar";
        public const string FORMAT_DOLLAR_CENTS = "DollarWithCents";
        public const string FORMAT_DATE = "Date";
        public const string FORMAT_DATETIME = "DateTime";
        public const string FORMAT_PERCENT = "Percent";
        public const string FORMAT_PERCENT_SIGN = "PercentWithSign";
        public const string FORMAT_NUMBER = "Number";
        public const string FORMAT_ZIP = "Zip";
        public const string FORMAT_LONGTEXT = "LongText";
    }

    #region Excel Row Model
    public class ExcelRowModel
    {
        public List<ExcelCellModel> CellModels { get; set; }

        public List<ExcelCellValue> CellValues { get; set; }
    }
    #endregion

    #region Excel Cell Model
    public class ExcelCellModel
    {
        public string CellValue { get; set; }

        public string CellFormat { get; set; }
    }
    #endregion

    public class ExcelUtility
    {

        public static void CreateNewByModels<T>(string fullName, string sheetName, List<T> models)
            where T : class, new()
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            props = props.OrderBy(p => p.MetadataToken).ToArray();
            List<string> headerList = GetHeaders(props, models);
            List<ExcelRowModel> excelRowList = new List<ExcelRowModel>();
            foreach (T item in models)
            {
                ExcelRowModel rowModel = new ExcelRowModel();
                List<ExcelCellModel> cells = new List<ExcelCellModel>();
                rowModel.CellModels = cells;
                excelRowList.Add(rowModel);
                foreach (PropertyInfo prop in props)
                {
                    ExcelInfoAttribute excelAttr = prop.GetCustomAttributes(typeof(ExcelInfoAttribute), true).FirstOrDefault() as ExcelInfoAttribute;
                    if (excelAttr == null)
                    {
                        continue;
                    }
                    if (excelAttr is ExcelInfoWithMutipleHeaderAttribute)
                    {
                        ExcelInfoWithMutipleHeaderAttribute a = excelAttr as ExcelInfoWithMutipleHeaderAttribute;
                        cells.AddRange(GetDynamicValues(models, item, a, prop));
                    }
                    else
                    {
                        ExcelCellModel cell = new ExcelCellModel();
                        cell.CellFormat = excelAttr.CellFormat;

                        object objVal = prop.GetValue(item, null);
                        string strVal;
                        if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            ExcelDateValueAttribute dateFmtAttr = prop.GetCustomAttributes(typeof(ExcelDateValueAttribute), true).FirstOrDefault() as ExcelDateValueAttribute;
                            string fmt = "MM/dd/yyyy";
                            if (dateFmtAttr != null)
                            {
                                fmt = dateFmtAttr.DateFormat;
                            }
                            strVal = objVal == null ? "" : ((DateTime)objVal).ToString(fmt);
                        }
                        else
                        {
                            strVal = objVal.ToSafeValue();
                        }

                        cell.CellValue = strVal;
                        cells.Add(cell);
                    }
                }
            }
            ExcelHelper.CreateNewExcel(headerList.ToArray(), fullName, sheetName);
            ExcelHelper.InsertData(fullName, excelRowList, sheetName);
        }

        private static List<ExcelCellModel> GetDynamicValues(IEnumerable<object> models, object model, ExcelInfoWithMutipleHeaderAttribute a, PropertyInfo prop)
        {
            List<ExcelCellModel> ret = new List<ExcelCellModel>();
            List<string> headers = a.GetDynamicHeaders(models);
            IEnumerable<string> values = prop.GetValue(model, null) as IEnumerable<string>;
            if (values == null)
            {
                values = new List<string>();
            }
            for (int i = 0; i < headers.Count; i++)
            {
                ExcelCellModel cell = new ExcelCellModel();
                cell.CellFormat = a.CellFormat;
                cell.CellValue = "";
                if (values.Count() > i)
                {
                    cell.CellValue = values.ElementAt(i);
                }
                ret.Add(cell);
            }
            return ret;
        }



        public static List<string> GetHeaders(PropertyInfo[] props, IEnumerable<object> models)
        {
            List<string> headerList = new List<string>();
            foreach (PropertyInfo prop in props)
            {
                ExcelInfoAttribute excelAttr = prop.GetCustomAttributes(typeof(ExcelInfoAttribute), true).FirstOrDefault() as ExcelInfoAttribute;
                if (excelAttr == null)
                {
                    continue;
                }
                if (excelAttr is ExcelInfoWithMutipleHeaderAttribute)
                {
                    ExcelInfoWithMutipleHeaderAttribute a = excelAttr as ExcelInfoWithMutipleHeaderAttribute;
                    headerList.AddRange(a.GetDynamicHeaders(models));
                }
                else
                {
                    string header = excelAttr.HeaderName;
                    headerList.Add(header);
                }
            }
            return headerList;
        }
    }
}
