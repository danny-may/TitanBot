using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Downloader
{
    public interface IDownloader
    {
        ValueTask<byte[]> GetBytes(Uri url, int freshness = 3600, int timeout = 5000);
        ValueTask<Bitmap> GetImage(Uri url, int freshness = 86400, int timeout = 5000);
        ValueTask<string> GetString(Uri url, Encoding encoding = null, int freshness = 3600, int timeout = 5000);
        void HardReset(Uri url);
        void WipeCache();
    }
}