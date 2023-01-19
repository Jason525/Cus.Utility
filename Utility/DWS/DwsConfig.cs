using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Utility.DWS
{
    public abstract class DwsConfig
    {
        public virtual Dictionary<string, object> Revises { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }

        public DwsConfig()
        {
            Revises = new Dictionary<string, object>();
            Revises.Add("Timeout", 180000);
        }

        public abstract Stream GetWSDL();

        public virtual Services CreateService()
        {
            return new Services(this);
        }
    }

    public class RemoteConfig : DwsConfig
    {
        public string WsdlUrl { get; set; }

        public override Stream GetWSDL()
        {
            return new WebClient().OpenRead(WsdlUrl);
        }
    }

    public class LocalConfig : DwsConfig
    {
        public string FilePath { get; set; }

        public override Stream GetWSDL()
        {
            return new FileStream(FilePath, FileMode.Open);
        }
    }
}
