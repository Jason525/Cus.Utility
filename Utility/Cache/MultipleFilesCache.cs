using System;
using System.Web;
using System.Web.Caching;
using System.Linq;

namespace Utility.Cache
{
    public class MultipleFilesCache<T> where T : class
    {
        public string CacheKey { get; protected set; }
        public string[] FilePath { get; protected set; }
        public bool IsNetworkFile { get; protected set; }
        public Func<string[], T> Generator { get; protected set; }

        public MultipleFilesCache(string key, Func<string[], T> generator, params string[] path)
        {
            CacheKey = key;
            FilePath = path;
            Generator = generator;
            IsNetworkFile = path.Any(p => p.StartsWith("\\\\"));
        }

        public T Instance
        {
            get
            {
                //if (IsNetworkFile)
                //{
                //    return Generator(FilePath);
                //}

                if (HttpRuntime.Cache[CacheKey] == null)
                {
                    Init();
                }

                return HttpRuntime.Cache[CacheKey] as T;
            }
        }

        public MultipleFilesCache<T> Init()
        {
            var cd = new CacheDependency(FilePath);
            var instance = Generator(FilePath);
            HttpRuntime.Cache.Insert(CacheKey, instance, cd);

            return this;
        }
    }
}
