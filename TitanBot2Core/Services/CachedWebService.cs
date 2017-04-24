using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services
{
    public class CachedWebService
    {
        private IDictionary<Uri, CacheObject> _cache
            = new ConcurrentDictionary<Uri, CacheObject>();
        private WebClient _client;
        public int DefaultCacheLifespan { get; set; }

        public CachedWebService()
        {
            _client = new WebClient();
        }

        public Task<string> Get(string url)
            => Get(new Uri(url), DefaultCacheLifespan);
        public Task<string> Get(Uri url)
            => Get(url, DefaultCacheLifespan);
        public Task<string> Get(string url, int cacheLifeSpan)
            => Get(new Uri(url), cacheLifeSpan);
        public Task<string> Get(Uri url, int cacheLifespan)
        {
            if (!_cache.ContainsKey(url))
                _cache.Add(url, new CacheObject(_client, url));

            return _cache[url].Get(cacheLifespan);
        }

        internal class CacheObject
        {
            private Uri _location { get; }
            private DateTime _lastUpdate { get; set; } = DateTime.MinValue;
            private string _data { get; set; }
            private WebClient _client { get; }
            private Task _webQueryTask { get; set; }

            public CacheObject(WebClient client, string url) : this(client, new Uri(url)) { }
            public CacheObject(WebClient client, Uri url)
            {
                _location = url;
                _client = client;
            }

            public async Task<string> Get(int invalidationSeconds)
            {
                if (_lastUpdate.AddSeconds(invalidationSeconds) < DateTime.Now)
                {
                    bool queryOwner = false;
                    if (_webQueryTask == null)
                    {
                        queryOwner = true;
                        _webQueryTask = new Task(async () =>
                        {
                            _data = await _client.DownloadStringTaskAsync(_location);
                            _lastUpdate = DateTime.Now;
                        });
                    }

                    await _webQueryTask;
                    if (queryOwner)
                        _webQueryTask = null;
                }

                return _data;
            }
        }
    }
}
