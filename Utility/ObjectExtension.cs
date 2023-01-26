using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utility
{
    public static class ObjectExtension
    {
        #region Clone
        public static T Clone<T>(this T obj) where T : class
        {
            if (obj == null) return null;

            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;

            return formatter.Deserialize(memoryStream) as T;
        }
        #endregion

        public static bool Is<T>(this object value) where T : struct
        {
            string str = "";

            if (value != null && value != DBNull.Value)
            {
                str = value.ToString();
            }

            return SafeConversion.Is<T>(str);
        }

        #region To Safe Value
        public static T ToSafeValue<T>(this object value, T defaultValue = default(T)) where T : struct
        {
            string str = "";

            if (value != null && value != DBNull.Value)
            {
                str = value.ToString();
            }

            return SafeConversion.To(str, defaultValue);
        }

        public static string ToSafeValue(this object value, string defaultValue = "")
        {
            string str = defaultValue;

            if (value != null && value != DBNull.Value)
            {
                str = value.ToString();
            }

            return str;
        }
        #endregion 
    }

    public static class TypeExtension
    {
        public static bool IsAnonymousType(this Type type)
        {
            bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            bool nameContainsAnonymousType = type.FullName != null && type.FullName.Contains("AnonymousType");
            bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }
    }
}
