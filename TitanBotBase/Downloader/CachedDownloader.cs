using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Logger;

namespace TitanBotBase.Downloader
{
    public class CachedDownloader : IDownloader
    {
        private ConcurrentDictionary<Uri, CacheObject> CacheStore
            = new ConcurrentDictionary<Uri, CacheObject>();
        private ILogger Logger { get; }

        internal CachedDownloader(ILogger logger)
        {
            Logger = logger;
        }

        public void WipeCache()
            => CacheStore.Clear();
        
        public void HardReset(Uri url)
        {
            if (CacheStore.ContainsKey(url))
                CacheStore.TryRemove(url, out CacheObject removed);
        }

        public async Task<string> GetString(Uri url, Encoding encoding = null, int freshness = 3600, int timeout = 5000)
        {
            var data = await GetBytes(url, freshness, timeout);

            if (data == null)
                return null;
            return (encoding ?? Encoding.Default).GetString(data);
        }

        public async Task<Bitmap> GetImage(Uri url, int freshness = 86400, int timeout = 5000)
        {
            var data = await GetBytes(url, freshness, timeout);

            if (data == null)
                return null;
            return new Bitmap(new MemoryStream(data));
        }
        
        public async Task<byte[]> GetBytes(Uri url, int freshness = 3600, int timeout = 5000)
        {
            if (!url.IsWellFormedOriginalString())
                throw new UriFormatException($"{url} is not a well formatted URI");
            var cache = CacheStore.GetOrAdd(url, x => new CacheObject(x, Logger));
            try
            {
                return await cache.Get(freshness, timeout);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "CachedDownloader");
                return null;
            }
        }

        internal class CacheObject
        {
            private Uri Location { get; }
            private DateTime LastUpdate { get; set; } = DateTime.MinValue;
            private byte[] Data { get; set; }
            private Task WebQuery { get; set; }
            private ILogger Logger { get; }
            
            public CacheObject(Uri url, ILogger logger)
            {
                Location = url;
                Logger = logger;
            }

            public async Task<byte[]> Get(int freshness, int timeout)
            {
                bool queryOwner = false;
                try
                {
                    if (LastUpdate.AddSeconds(freshness) < DateTime.Now)
                    {
                        Logger.Log(LogSeverity.Info, LogType.Downloader, "Updating WebCache", "CachedDownloader");
                        
                        if (WebQuery == null)
                        {
                            queryOwner = true;

                            Data = null;

                            WebQuery = new Task(() =>
                            {
                                using (var wc = new WebClient())
                                {
                                    Task.Run(async () =>
                                    {
                                        await Task.Delay(timeout);
                                        if (wc != null)
                                            wc.CancelAsync();
                                    });
                                    Data = wc.DownloadData(Location);
                                    Logger.Log(LogSeverity.Info, LogType.Downloader, "Download Complete", "CachedDownloader");
                                }
                                LastUpdate = DateTime.Now;

                            });
                            WebQuery.Start();
                        }

                        Logger.Log(LogSeverity.Info, LogType.Downloader, "Waiting for download", "CachedDownloader");
                        await WebQuery;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status != WebExceptionStatus.RequestCanceled)
                        Logger.Log(ex, "CachedDownloader");
                    else
                        Logger.Log(LogSeverity.Info, LogType.Downloader, "Download cancelled", "CachedDownloader");
                }
                finally
                {
                    if (queryOwner)
                        WebQuery = null;
                }

                return Data;
            }
        }
    }
}
