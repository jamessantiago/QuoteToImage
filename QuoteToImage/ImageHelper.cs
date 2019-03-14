using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using CommandLine;
using System.Windows.Forms;


namespace QuoteToImage
{
    public static class ImageHelper
    {
        private static Font GetAdjustedFont(Graphics g, string graphicString, Font originalFont, int containerWidth, int containerHeight, int maxFontSize, int minFontSize, bool smallestOnFail)
        {
            Font testFont = null;
            for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);
                SizeF adjustedSizeNew = g.MeasureString(graphicString, testFont, containerWidth);

                if (containerHeight >= Convert.ToInt32(adjustedSizeNew.Height) && containerWidth >= Convert.ToInt32(adjustedSizeNew.Width))
                    return testFont;
            }

            if (smallestOnFail)
                return testFont;
            else
                return originalFont;
        }

        public static void CreateImage(ParserResult<Options> args)
        {
            string quote, leftBanner, rightBanner, bannerPos, size, layout, output, emphasis;
            quote = leftBanner = rightBanner = bannerPos = size = layout = output =  emphasis = string.Empty;

            args.MapResult(options =>
            {
                quote = options.Quote;
                leftBanner = options.LeftBanner;
                rightBanner = options.RightBanner;
                emphasis = options.Emphasis;
                bannerPos =
                    Regex.IsMatch(options.BannerPos ?? "", "top|bottom", RegexOptions.IgnoreCase)
                        ? options.BannerPos.ToLower()
                        : "bottom";
                size = Regex.IsMatch(options.Size, @"\d+x|X\d+")
                    ? options.Size.ToLower()
                    : throw new Exception("Size is not in the correct format");
                layout = Regex.IsMatch(options.Layout ?? "", "portrait|landscape", RegexOptions.IgnoreCase)
                    ? options.Layout.ToLower()
                    : "portrait";
                output = options.Output;
                //output = Uri.IsWellFormedUriString(options.Output, UriKind.RelativeOrAbsolute)
                //    ? options.Output
                //    : throw new Exception("");
                return 0;
            }, errs =>
            {
                Environment.Exit(1);
                return 1;
            });

            Font defaultFont = new Font("Linux Libertine G", 24);
            int x = int.Parse(size.Split('x')[0]);
            int y = int.Parse(size.Split('x')[1]);
            if (layout == "portrait")
            {
                int oldX = x;
                int oldY = y;
                x = oldY;
                y = oldX;
            }

            using (Bitmap b = new Bitmap(x, y))
            {
                var bodySize = b.Size;
                bodySize.Width -= 20;
                bodySize.Height -= 20;
                var bannerSize = b.Size;
                bannerSize.Width -= 20;
                bool hasBanner = !string.IsNullOrEmpty(leftBanner + rightBanner);
                if (hasBanner)
                {
                    bannerSize.Height = (int) ((double) b.Size.Height * 0.1) + 10;
                    bannerSize.Width /= 2;
                    bodySize.Height -= bannerSize.Height;
                }

                using (Graphics g = Graphics.FromImage(b))
                using (Font f = GetAdjustedFont(g, quote, defaultFont, bodySize.Width - 20, bodySize.Height - 20, 110, 32, true))
                using (Font bf = new Font(f, FontStyle.Bold))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.Clear(Color.White);

                    g.DrawRtfText(quote, emphasis, f, bf, new Rectangle(10, 10, bodySize.Width, bodySize.Height));
                    //g.DrawString(quote, f, new SolidBrush(Color.Black), new RectangleF(10, 10, bodySize.Width, bodySize.Height));
                    if (!string.IsNullOrEmpty(leftBanner))
                    {
                        Font lf = new Font(GetAdjustedFont(g, leftBanner, defaultFont, bannerSize.Width - 50, bannerSize.Height - 10, 32, 12, true), FontStyle.Bold);
                        g.DrawString(leftBanner, lf, new SolidBrush(Color.Black), 10, bodySize.Height + 10);
                    }
                    if (!string.IsNullOrEmpty(rightBanner))
                    {
                        Font rf = GetAdjustedFont(g, leftBanner, defaultFont, bannerSize.Width - 50, bannerSize.Height - 10, 24, 12, true);
                        var rsize = g.MeasureString(rightBanner, rf, new SizeF(bannerSize.Width, bannerSize.Height));
                        g.DrawString(rightBanner, rf, new SolidBrush(Color.Black), new RectangleF(b.Width - 10 - rsize.Width, bodySize.Height + 10, bannerSize.Width, bannerSize.Height));
                    }
                }

                b.Save(output, ImageFormat.Png);
            }
        }
    }
}
