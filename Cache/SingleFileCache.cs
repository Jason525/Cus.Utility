using System;
using System.Web;
using System.Web.Caching;

namespace Utility.Cache
{
    public class FileCache<T> where T : class
    {
        public string CacheKey { get; protected set; }

        public string FilePath { get; protected set; }

        public bool IsNetworkFile { get; protected set; }

        public Func<string, T> Generator { get; protected set; }

        public FileCache(string key, string path, Func<string, T> generator)
        {
            CacheKey = key;
            FilePath = path;
            Generator = generator;
            IsNetworkFile = path.StartsWith("\\\\");
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

        public FileCache<T> Init()
        {
            var cd = new CacheDependency(FilePath);
            var instance = Generator(FilePath);
            HttpRuntime.Cache.Insert(CacheKey, instance, cd);

            return this;
        }
    }
}
