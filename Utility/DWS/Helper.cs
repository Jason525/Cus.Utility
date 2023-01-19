using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Services.Description;

namespace Utility.DWS
{
    public static class Helper
    {
        public static Assembly GetAssembly(string ns, Stream stream)
        {
            var sd = ServiceDescription.Read(stream);
            var sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");

            var cn = new CodeNamespace(ns);
            var ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);

            var icc = new CSharpCodeProvider();
            var cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");

            var cr = icc.CompileAssemblyFromDom(cplist, ccu);

            if (true == cr.Errors.HasErrors)
            {
                var sb = new StringBuilder();
                foreach (CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(Environment.NewLine);
                }

                throw new Exception(sb.ToString());
            }

            return cr.CompiledAssembly;
        }
    }
}
