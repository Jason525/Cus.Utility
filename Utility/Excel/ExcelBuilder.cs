using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utility.Excel
{
    public static class ExcelBuilder
    {
        #region Create Methods
        public static void CreateNew(string filePath, List<ExcelColumn> columns, List<IExcelRow> items)
        {
            CreateNew(filePath, new SheetConfig { Columns = columns, Items = items });
        }

        public static void CreateNew(string filePath, params SheetConfig[] sheets)
        {
            using (ExcelPackage xlPackage = Create(sheets))
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                xlPackage.SaveAs(new FileInfo(filePath));
            }
        }

        public static ExcelPackage Create(params SheetConfig[] sheets)
        {
            ExcelPackage xlPackage = new ExcelPackage();

            foreach (SheetConfig sheet in sheets)
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add(sheet.Name);

                FillHead(worksheet, sheet.Columns.Select(c => c.HeadSettings));
                FillItems(worksheet, 2, sheet.Columns.Select(c => c.CellSettings), sheet.Items);

                if (sheet.AutoFit)
                {
                    for (int i = 1; i <= sheet.Columns.Count; i++) worksheet.Column(i).AutoFit();
                }
            }

            return xlPackage;
        }
        #endregion 

        #region Fill Methods
        private static void FillHead(ExcelWorksheet sheet, IEnumerable<ExcelCell> cellSettings)
        {
            FillRow(sheet, 1, cellSettings);
        }

        private static void FillItems(ExcelWorksheet sheet, int startRowIndex, IEnumerable<ExcelCell> cellSettings, List<IExcelRow> items)
        {
            int rowIndex = startRowIndex;

            items.ForEach(item =>
            {
                FillRow(sheet, rowIndex, cellSettings, item);
                rowIndex += 1;
            });
        }

        private static void FillRow(ExcelWorksheet sheet, int rowIndex, IEnumerable<ExcelCell> cellSettings, IExcelRow item = null)
        {
            int colIndex = 1;

            foreach (ExcelCell setting in cellSettings)
            {
                FillCell(sheet.Cells[rowIndex, colIndex], setting, item);
                colIndex += 1;
            }
        }

        private static void FillCell(ExcelRange cell, ExcelCell settings, IExcelRow item = null)
        {
            if (item == null)
            {
                cell.Value = settings.FieldName;
            }
            else
            {
                cell.Value = settings.GetValue(item);
            }

            if (settings.Style != null) settings.Style.SetCell(cell);
        }
        #endregion 
    }
}
