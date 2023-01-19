using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Excel
{
    

    public class DollarFormatter : IValueFormat
    {
        public string Pattern { get; set; }
        public string Format(object value)
        {
            return value.ToSafeValue<decimal>().ToString(Pattern);
        }

        public DollarFormatter()
        {
            Pattern = "$###,##0.000";
        }
    }

    public class DateFormatter : IValueFormat
    {
        public string Pattern { get; set; }
        public string Format(object value)
        {
            return value.ToSafeValue<DateTime>().ToString(Pattern);
        }

        public DateFormatter()
        {
            Pattern = "yyyy.MM.dd";
        }
    }
}
