using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Excel
{
    public interface IValueFormat
    {
        string Pattern { get; set; }
        string Format(object value);
    }
}
