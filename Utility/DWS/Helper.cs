using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Services.Description;

namespace Utility.DWS
{
    public static class Helper
    {
        public static Assembly GetAssembly(string ns, Stream stream)
        {
            ServiceDescription sd = ServiceDescription.Read(stream);
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");

            CodeNamespace cn = new CodeNamespace(ns);
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);

            CSharpCodeProvider icc = new CSharpCodeProvider();
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");

            CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);

            if (true == cr.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
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
