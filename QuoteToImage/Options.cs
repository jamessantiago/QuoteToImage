using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace QuoteToImage
{
    public class Options
    {
        [Option('q', "quote", Required = true, HelpText = "A quote to fit within the image.  Markdown formatting is supported.")]
        public string Quote { get; set; }

        [Option('e', "emphasis", Required = true, HelpText = "Text within the quote to give strong emphasis (bold)")]
        public string Emphasis { get; set; }

        [Option('l', "left-banner", Required = false, HelpText = "Text to fit within the left portion of the banner")]
        public string LeftBanner { get; set; }

        [Option('r', "right-banner", Required = false, HelpText = "Text to fit within the right portion of the banner")]
        public string RightBanner { get; set; }

        [Option('b', "banner-position", Required = false, HelpText = "Position (Top or Bottom) of the banner.  Default is Bottom.")]
        public string BannerPos { get; set; }

        [Option('s', "Size", Required = true, HelpText = "Size of the image in pixels (LengthXWidth => e.g. 800x600).  If left and/or right banner text is specified then a portion of the image will be taken up for this use.")]
        public string Size { get; set; }

        [Option('p', "Layout", Required = false, HelpText = "Layout of the image (Portrait or Landscape).  Default is Portrait.")]
        public string Layout { get; set; }

        [Option('o', "OutputFile", Required = true, HelpText = "File output (.png)")]
        public string Output { get; set; }
    }
}
