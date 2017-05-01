using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services
{
    public class CachedWebClient
    {
        private IDictionary<Uri, CacheObject> _cache
            = new ConcurrentDictionary<Uri, CacheObject>();
        public int DefaultFreshness { get; set; } = 3600;
        public event Func<Exception, Task> DefaultExceptionHandler;
        public event Func<Uri, string, Task> LogWebRequest;

        public CachedWebClient()
        {
        }

        public Task<string> GetString(string url)
            => GetString(new Uri(url), DefaultFreshness);
        public Task<string> GetString(Uri url)
            => GetString(url, DefaultFreshness);
        public Task<string> GetString(string url, int freshness)
            => GetString(new Uri(url), freshness);
        public async Task<string> GetString(Uri url, int freshness)
        {
            if (!_cache.ContainsKey(url))
            {
                _cache.Add(url, new CacheObject(url));
                _cache[url].LogWebRequest += Log;
            }

            return Encoding.Default.GetString(await _cache[url].Get(freshness));
        }

        public Task<byte[]> GetBytes(string url)
            => GetBytes(new Uri(url), DefaultFreshness);
        public Task<byte[]> GetBytes(Uri url)
            => GetBytes(url, DefaultFreshness);
        public Task<byte[]> GetBytes(string url, int freshness)
            => GetBytes(new Uri(url), freshness);
        public Task<byte[]> GetBytes(Uri url, int freshness)
        {
            if (!_cache.ContainsKey(url))
            {
                _cache.Add(url, new CacheObject(url));
                _cache[url].LogWebRequest += Log;
            }

            return _cache[url].Get(freshness);
        }

        private async Task Log(Uri url, string message)
        {
            await (LogWebRequest?.Invoke(url, message) ?? Task.CompletedTask);
        }

        internal class CacheObject
        {
            private Uri _location { get; }
            private DateTime _lastUpdate { get; set; } = DateTime.MinValue;
            private byte[] _data { get; set; }
            private Task _webQueryTask { get; set; }
            public event Func<Uri, string, Task> LogWebRequest;

            public CacheObject(string url) : this(new Uri(url)) { }
            public CacheObject(Uri url)
            {
                _location = url;
            }

            private void Log(string message)
            {
                LogWebRequest?.Invoke(_location, message);
            }

            public async Task<byte[]> Get(int freshness)
            {
                if (_lastUpdate.AddSeconds(freshness) < DateTime.Now)
                {
                    Log("Updating WebCache");
                    bool queryOwner = false;
                    if (_webQueryTask == null)
                    {
                        queryOwner = true;
                        _webQueryTask = new Task(() =>
                        {
                            using (var wc = new WebClient())
                            {
                                _data = wc.DownloadData(_location);
                                Log("Download Complete");
                            }
                            _lastUpdate = DateTime.Now;

                        });
                        _webQueryTask.Start();
                    }

                    Log("Waiting for download");

                    await _webQueryTask;
                    if (queryOwner)
                        _webQueryTask = null;
                }

                return _data;
            }
        }
    }
}
