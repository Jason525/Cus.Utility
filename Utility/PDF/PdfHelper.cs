using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utility.PDF.PdfAttributes;

namespace Utility.PDF
{
    public class PdfHelper
    {
        public static void CreatePdf(string templatePath, string outfilePath, object model)
        {
            PdfReader reader = new PdfReader(templatePath);

            Stream output = new MemoryStream();

            var stamper = new PdfStamper(reader, output);
            try
            {
                AcroFields fields = stamper.AcroFields;
                var dic = GetFieldDic(model);
                var fontDic = GetColorDic(model);
                foreach (var item in dic)
                {
                    fields.SetField(item.Key, item.Value);
                    if(fontDic.ContainsKey(item.Key))
                    {
                        fields.SetFieldProperty(item.Key, "textcolor", fontDic[item.Key], null);
                        fields.RegenerateField(item.Key);
                    }
                }
                stamper.FormFlattening = true;
                stamper.Close();
                reader.Close();
                stamper.Dispose();
                File.WriteAllBytes(outfilePath, ((MemoryStream)output).ToArray());
            }
            catch
            {
                throw;
            }
            finally
            {
                output.Close();
            }
        }

        private static Dictionary<string,string> GetFieldDic(object model)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            Type modelType = model.GetType();
            var properties = modelType.GetProperties();
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(PdfFieldAttribute), true).FirstOrDefault();
                if (attr == null)
                {
                    continue;
                }
                string fieldName = (attr as PdfFieldAttribute).FieldName;
                object value = prop.GetValue(model, null);
                string valStr = value.ToSafeValue();
                ret.Add(fieldName, valStr);
            }
            return ret;
        }

        private static Dictionary<string, BaseColor> GetColorDic(object model)
        {
            Dictionary<string, BaseColor> ret = new Dictionary<string, BaseColor>();
            Type modelType = model.GetType();
            var properties = modelType.GetProperties();
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(PdfFieldAttribute), true).FirstOrDefault();
                if (attr == null)
                {
                    continue;
                }
                string fieldName = (attr as PdfFieldAttribute).FieldName;
                BaseColor color = (attr as PdfFieldAttribute).FontColor;
                if (color != null)
                {
                    ret.Add(fieldName, color);
                }
            }
            return ret;
        }
    }
}
