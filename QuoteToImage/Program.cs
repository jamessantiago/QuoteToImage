using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;

namespace QuoteToImage
{
    class Program
    {
        static void Main(string[] args)
        {
            var results = CommandLine.Parser.Default.ParseArguments<Options>(args);
            ImageHelper.CreateImage(results);
        }
    }
}
