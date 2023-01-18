using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility.SimpleExcel.Attributes
{
    /// <summary>
    /// This attribute can only be used on property with Array or List property type
    /// </summary>
    public class ExcelInfoWithMutipleHeaderAttribute : ExcelInfoAttribute
    {
        /// <summary>
        /// param: models
        /// return: headers
        /// </summary>
        public string HeaderNameFuncName { get; set; }

        public ExcelInfoWithMutipleHeaderAttribute(string cellFormat, string headerNameFuncName) : base("", cellFormat)
        {
            HeaderNameFuncName = headerNameFuncName;
        }

        public List<string> GetDynamicHeaders(IEnumerable<object> models)
        {
            if(!models.Any())
            {
                return new List<string>();
            }
            object model = models.First();
            Type t = model.GetType();
            MethodInfo m = t.GetMethod(HeaderNameFuncName);
            IEnumerable<string> ret = m.Invoke(model, new object[] { models }) as IEnumerable<string>;
            return ret.ToList();
        }
    }

    public class ExcelInfoAttribute:Attribute
    {
        public string HeaderName { get; set; }

        public string CellFormat { get; set; }

        Func<object, bool> IsColVisible { get; set; }

        public ExcelInfoAttribute(string headerName, string cellFormat, Func<object, bool> isColVisibleFunc=null)
        {
            HeaderName = headerName;
            CellFormat = cellFormat;
            IsColVisible = isColVisibleFunc;
        }

        public ExcelInfoAttribute(string headerName, string cellFormat)
        {
            HeaderName = headerName;
            CellFormat = cellFormat;
        }

    }
}
