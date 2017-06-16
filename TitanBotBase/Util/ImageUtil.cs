using System.Drawing;

namespace TitanBotBase.Util
{
    public static class ImageUtil
    {
        public static Color AverageColor(this Bitmap image, float minBrightness = 0, float minSaturation = 0)
        {
            var rTot = 0d;
            var gTot = 0d;
            var bTot = 0d;

            var pixTot = 0d;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var pix = image.GetPixel(x, y);

                    if (pix.GetBrightness() > minBrightness && pix.GetSaturation() > minSaturation)
                    {
                        rTot += pix.R;
                        gTot += pix.G;
                        bTot += pix.B;
                        pixTot++;
                    }
                }
            }
            if (pixTot == 0)
                return Color.FromArgb(0);
            return Color.FromArgb((int)(rTot / pixTot), (int)(gTot / pixTot), (int)(bTot / pixTot));
        }
    }
}
