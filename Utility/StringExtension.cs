using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Utility
{
    public static class StringExtension
    {
        #region Is Methods
        public static bool IsEmpty(this string str) { return string.IsNullOrWhiteSpace(str); }
        public static bool IsInt(this string value) { return SafeConversion.Is<int>(value); }
        public static bool IsNumber(this string value) { return SafeConversion.Is<double>(value); }
        public static bool IsDate(this string value) { return SafeConversion.Is<DateTime>(value); }
        public static bool IsBool(this string value) { return SafeConversion.Is<bool>(value); }
        public static bool IsGuid(this string value) { return SafeConversion.Is<Guid>(value); }
        public static bool IsEmail(this string value) { return SafeConversion.IsEmail(value); }
        #endregion 

        #region To Methods
        public static T To<T>(this string value, T defaultValue = default(T)) where T : struct { return SafeConversion.To<T>(value, defaultValue); }
        public static int ToInt(this string value) { return SafeConversion.To<int>(value); }
        public static float ToFloat(this string value) { return SafeConversion.To<float>(value); }
        public static double ToDouble(this string value) { return SafeConversion.To<double>(value); }
        public static decimal ToDecimal(this string value) { return SafeConversion.To<decimal>(value); }
        public static DateTime ToDate(this string value) { return SafeConversion.To<DateTime>(value); }
        public static bool ToBool(this string value) { return SafeConversion.To<bool>(value); }
        public static Guid ToGuid(this string value) { return SafeConversion.To<Guid>(value); }

        /// <summary>
        /// if anyway == false, this method does not provide casing to convert a word that is entirely uppercase, such as an acronym.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="anyway"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string value, bool anyway = false)
        {
            if (value.IsEmpty()) return "";

            if (anyway)
            {
                return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
            }

            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value);
        }

        public static string ToUsDollar(this string txt)
        {
            return txt.ToAllDouble().ToString("c", new CultureInfo("en-US"));
        }

        /// <summary>
        /// "1", "Y", "YES" or "TRUE" will be ture by default 
        /// </summary>
        public static bool ToAllBoolen(this string txt, params string[] trueString)
        {
            return ToBool(txt, new[] { "1", "Y", "YES", "TRUE" }.Concat(trueString).ToArray());
        }

        public static bool ToBool(this string txt, params string[] trueString)
        {
            return !string.IsNullOrEmpty(txt) && trueString.Contains(txt, new CompareBoolString());
        }

        public class CompareBoolString : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }


        /// <summary>
        /// start with "$" or "-$", end with "%", includes ","
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToAllDouble(this string txt, double defaultValue = 0)
        {
            double d = defaultValue;

            if (!string.IsNullOrEmpty(txt))
            {
                txt = txt.Trim();

                if (!double.TryParse(txt, out d))
                {
                    if (!txt.StartsWith(",") && !txt.EndsWith(",")) txt = txt.Replace(",", "");
                    if (txt.StartsWith("-$")) txt = txt.Replace("$", "");
                    if (txt.StartsWith("($")) txt = txt.Replace("($", "-").Replace(")", "");
                    if (txt.StartsWith("$")) txt = txt.Substring(1);
                    if (txt.EndsWith("%")) txt = txt.Remove(txt.Length - 1);

                    if (double.TryParse(txt, out d))
                    {
                        return d;
                    }
                }
            }

            return d;
        }

        public static decimal ToAllDecimal(this string txt, decimal defaultValue = 0)
        {
            return txt.ToAllDouble().ToSafeValue().ToDecimal();
        }
        #endregion
    }
}
