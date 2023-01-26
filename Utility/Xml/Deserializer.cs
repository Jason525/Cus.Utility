using System.Linq;
using System.Xml.Linq;

namespace Utility.Xml
{
    public static class Deserializer
    {
        public static T FromAttribute<T>(XElement ele) where T : class, new()
        {
            T result = new T();

            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                XAttribute attr = ele.Attribute(p.Name);

                if (attr != null)
                {
                    p.SetValue(result, attr.Value, null);
                }
            });

            return result;
        }

        public static T FromSubElement<T>(XElement ele) where T : class, new()
        {
            T result = new T();

            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                XElement sub = ele.Element(p.Name);

                if (sub != null)
                {
                    p.SetValue(result, sub.Value, null);
                }
            });

            return result;
        }
    }
}
