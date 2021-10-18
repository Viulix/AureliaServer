
using System;
using System.Drawing;
using System.Drawing.Text;
using ImageMagick;

namespace Aurelia
{
    public class ImageProcessor
    {
        
        public static void ImageBuilder(string filename, string idolname, string group, string rarity, int rar, string id, string pathDefiner)
        {
            PrivateFontCollection modernFont = new PrivateFontCollection();
            modernFont.AddFontFile("assets/fonts/walrus/Walrus-Bold.ttf");
            try
            {
                int fontsize;
                switch (idolname.Length)
                {
                    case >= 5 when idolname.Length <= 6:
                        fontsize = 120;
                        break;
                    case >= 6 when idolname.Length <= 7:
                        fontsize = 110;
                        break;
                    case >= 7 when idolname.Length <= 8:
                        fontsize = 100;
                        break;
                    case >= 9 when idolname.Length <= 10:
                        fontsize = 90;
                        break;
                    case >= 10:
                        fontsize = 80;
                        break;
                    default:
                        fontsize = 130;
                        break;
                }
                int width;
                int height;
                int frameHeight;
                int frameWidth;
                int textHeight = 0;
                int textWidth = 0;

                switch (rar)
                {
                    case 5:
                        width = 58;
                        height = 105;
                        frameWidth = 934;
                        frameHeight = 1565;
                        textWidth = 59;
                        textHeight = 978;
                        break;
                    case 4:
                        width = 65;
                        height = 65;
                        frameWidth = 934;
                        frameHeight = 1536;
                        textWidth = 67;
                        textHeight = 920;
                        break;
                    case 3:
                        width = 67;
                        height = 79;
                        frameWidth = 934;
                        frameHeight = 1541;
                        textWidth = 68;
                        textHeight = 950;
                        break;
                    case 2:
                        width = 64;
                        height = 77;
                        frameWidth = 934;
                        frameHeight = 1537;
                        textWidth = 65;
                        textHeight = 945;
                        break;
                    default:
                        width = 30;
                        height = 36;
                        frameWidth = 861;
                        frameHeight = 1464;
                        textWidth = 30;
                        textHeight = 905;
                        break;
                }
                using Bitmap cardTemp = new($"assets/groups/{group}/{idolname}/{filename}");
                using Bitmap card = new(cardTemp, 800, 1200);
                using Bitmap rareFrameTemp = new($"assets/frames/{rarity}Frame.png");
                using Bitmap rareFrame2 = new(rareFrameTemp, frameWidth, frameHeight);
                using Bitmap rareFrame = new(rareFrameTemp, frameWidth, frameHeight);
                {
                    using (Graphics g = Graphics.FromImage(rareFrame))
                        g.DrawImage(card, width, height);
                    using (Graphics g = Graphics.FromImage(rareFrame))
                        g.DrawImage(rareFrame2, 0, 0);
                    rareFrame.Save($"assets/{pathDefiner}/{id}card.png");
                }
                var readSettings = new MagickReadSettings()
                {
                    Font = "assets/fonts/walrus/Walrus-Bold.ttf",
                    FontFamily = "Walrus Bold",
                    FontPointsize = fontsize,
                    TextGravity = Gravity.Center,
                    StrokeColor = MagickColors.White,
                    StrokeWidth = 4,
                    BackgroundColor = MagickColors.Transparent,
                    Height = 150,
                    Width = 630
                };
                using var image = new MagickImage($"assets/{pathDefiner}/{id}card.png");
                using (var caption = new MagickImage($"caption:{idolname}", readSettings))
                {
                    Percentage per = new Percentage(55);
                    caption.Shadow(textWidth + 74, textHeight + 362, 7, per);
                    image.Composite(caption, textWidth + 77, textHeight + 362, CompositeOperator.Over);

                }
                using (var caption = new MagickImage($"caption:{idolname}", readSettings))
                {
                    image.Composite(caption, textWidth + 75, textHeight + 360, CompositeOperator.Over);

                    image.Write($"assets/{pathDefiner}/{id}1card.png");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}