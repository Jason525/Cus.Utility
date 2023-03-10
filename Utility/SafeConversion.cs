using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
namespace Utility
{
    public static class SafeConversion
    {
        #region Convert Methods

        public static string ConvertEnumDispalyAttrToString(object enumObj)
        {
            if (enumObj == null)
            {
                return string.Empty;
            }
            Type t = enumObj.GetType();
            FieldInfo field = t.GetField(enumObj.ToString());
            if (field == null)
            {
                return string.Empty;
            }
            DisplayAttribute displayAttr = field.GetCustomAttributes(false).FirstOrDefault(a => a is DisplayAttribute) as DisplayAttribute;
            if (displayAttr == null)
            {
                DisplayNameAttribute displayNameAttr = field.GetCustomAttributes(false).FirstOrDefault(a => a is DisplayNameAttribute) as DisplayNameAttribute;
                return displayNameAttr == null ? "" : displayNameAttr.DisplayName;
            }
            return displayAttr.Name;
        }

        public static T To<T>(string value, T defaultValue = default(T)) where T : struct
        {
            return (T)To(typeof(T), value, defaultValue);
        }

        public static Nullable<T> ToNullable<T>(string value, Nullable<T> defaultValue = null) where T : struct
        {
            if (string.IsNullOrEmpty(value) || !Is<T>(value)) return defaultValue;

            return To(typeof(Nullable<T>), value, defaultValue) as Nullable<T>;
        }

        public static object To(Type type, object value, object defaultValue = null)
        {
            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (type == typeof(string)) return value == null || value == DBNull.Value ? "" : value.ToString(); //type is string
            if (!type.IsValueType && !isNullable) return value; //type is calss
            if (defaultValue == null) defaultValue = isNullable ? null : Activator.CreateInstance(type);

            if (value != null && value != DBNull.Value)
            {
                if (type.FullName == value.GetType().FullName)
                {
                    return value;
                }
                Type realType = isNullable ? Nullable.GetUnderlyingType(type) : type;
                if (realType != null)
                {
                    if (realType.IsEnum)
                    {
                        return Enum.ToObject(realType, value.ToSafeValue().ToInt());
                    }
                    else if (realType == typeof(bool) || realType == typeof(bool?))
                    {
                        return value.ToSafeValue().ToAllBoolen();
                    }
                    else
                    {
                        MethodInfo tryParse = realType.GetMethod("TryParse",
                            BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                            new[] { typeof(string), realType.MakeByRefType() },
                            new[] { new ParameterModifier(2) });
                        object[] parameters = new[] { value.ToString(), Activator.CreateInstance(realType) };
                        bool success = tryParse != null && (bool)tryParse.Invoke(null, parameters);

                        if (success)
                        {
                            return parameters[1];
                        }
                    }
                }
            }

            return defaultValue;
        }
        #endregion 

        #region Match Methods
        public static bool Is<T>(string value) where T : struct
        {
            return Is(typeof(T), value);
        }

        public static bool Is(Type type, object value)
        {
            if (value != null)
            {
                MethodInfo tryParse = type.GetMethod("TryParse",
                                BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                                new[] { typeof(string), type.MakeByRefType() },
                                new[] { new ParameterModifier(2) });
                object[] parameters = new[] { value, Activator.CreateInstance(type) };
                return tryParse != null && (bool)tryParse.Invoke(null, parameters);
            }

            return false;
        }

        public static bool IsEmail(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;

            return Regex.IsMatch(str.Trim().ToLower(), "^([\\w-]+)(\\.[\\w-]+)*@([a-z0-9]+)([\\.-][a-z0-9]+)*(\\.[a-z]{2,})$") ||
                Regex.IsMatch(str.Trim().ToLower(), "^([\\w-]+)(\\.[\\w-]+)*@([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");
        }
        #endregion
    }
}
