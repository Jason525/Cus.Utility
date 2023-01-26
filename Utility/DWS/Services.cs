using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Utility.DWS
{
    public class Services
    {
        public DwsConfig Request { get; set; }
        public Assembly CurrentAssembly { get; protected set; }
        public Type CurrentClass { get; protected set; }
        public object CurrentObj { get; protected set; }

        public Services(DwsConfig request)
        {
            Request = request;

            using (Stream stream = Request.GetWSDL())
            {
                CurrentAssembly = Helper.GetAssembly(Request.Namespace, stream);
            }

            CurrentClass = CreateClass(Request.ClassName);
            CurrentObj = Activator.CreateInstance(CurrentClass);

            foreach (KeyValuePair<string, object> rev in Request.Revises)
            {
                PropertyInfo pTimeout = CurrentClass.GetProperty(rev.Key);
                if (pTimeout != null) pTimeout.SetValue(CurrentObj, rev.Value, null);
            }
        }

        public object Invoke(string methodName, params object[] args)
        {
            MethodInfo m = CurrentClass.GetMethod(methodName);

            return m?.Invoke(CurrentObj, args);
        }

        public Type CreateClass(string className)
        {
            return CurrentAssembly.GetType(string.Concat(Request.Namespace, ".", className), true, true);
        }

        public object CreateObject(string className)
        {
            return Activator.CreateInstance(CreateClass(className));
        }
    }
}
