using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Utility.Xml
{
    public static class Deserializer
    {
        public static T FromAttribute<T>(XElement ele) where T : class, new()
        {
            var result = new T();

            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                var attr = ele.Attribute(p.Name);

                if (attr != null && attr.Value != null)
                {
                    p.SetValue(result, attr.Value, null);
                }
            });

            return result;
        }

        public static T FromSubElement<T>(XElement ele) where T : class, new()
        {
            var result = new T();

            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                var sub = ele.Element(p.Name);

                if (sub != null && sub.Value != null)
                {
                    p.SetValue(result, sub.Value, null);
                }
            });

            return result;
        }
    }
}
