using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.FixedWidthFontExtractionTool
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging;
    using ScrollStitch.V20200707.Imaging.FontExtraction;
    using ScrollStitch.V20200707.Imaging.IO;
    using ScrollStitch.V20200707.Text;

    public class Program
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void PrintHelp(string[] args)
        {
            Console.WriteLine(new string('-', 76));
            Console.WriteLine();
            Console.WriteLine("FixedWidthFontExtractionTool.exe");
            Console.WriteLine("===");
            Console.WriteLine();
            Console.WriteLine("Usage");
            Console.WriteLine("===");
            Console.WriteLine();
            Console.WriteLine("Usage 1: print a character map template (as text) to the console or saved to a file.");
            Console.WriteLine("=====");
            Console.WriteLine();
            Console.WriteLine("```");
            Console.WriteLine("    FixedWidthFontExtractionTool.exe PrintTemplate {text_template.txt}");
            Console.WriteLine("```");
            Console.WriteLine();
            Console.WriteLine("If the output text file is omitted, the text template is printed to the console.");
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine();
            Console.WriteLine("Usage 2: parse a screenshot of the text template and extract the fixed-width fonts as a resource.");
            Console.WriteLine("=====");
            Console.WriteLine();
            Console.WriteLine("```");
            Console.WriteLine("    FixedWidthFontExtractionTool.exe ProcessImage {screenshot.png}");
            Console.WriteLine("```");
            Console.WriteLine();
            Console.WriteLine("The processed output is saved to the same directory as the screenshot image.");
            Console.WriteLine("The following outputs are generated:");
            Console.WriteLine();
            Console.WriteLine("- {screenshot}_CharBitmap.png"); 
            Console.WriteLine("- {screenshot}_Base64.txt");
            Console.WriteLine("- {screenshot}_CodeFragment.txt");
            Console.WriteLine();
            Console.WriteLine(new string('-', 76));
        }

#if true
        public static readonly string[] InjectedArguments = null;
#elif false
        public static readonly string[] InjectedArguments = new string[]
        {
            "Help"
        };
#elif false
        public static readonly string[] InjectedArguments = new string[]
        {
            "PrintTemplate",
            @"text_template.txt"
        };
#elif false
        public static readonly string[] InjectedArguments = new string[]
        {
            "ProcessImage", 
            @"screenshot.png"
        };
#endif

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Main(string[] args)
        {
            if (!(InjectedArguments is null))
            {
                Console.WriteLine(new string('-', 76));
                Console.WriteLine("Original command-line arguments are ignored:");
                Console.WriteLine("        " + string.Join(" ", args));
                Console.WriteLine();
                Console.WriteLine("Injected command-line arguments are used:");
                Console.WriteLine("        " + string.Join(" ", InjectedArguments));
                Console.WriteLine();
                Console.WriteLine(new string('-', 76));
                Console.WriteLine("Press enter key to continue execution.");
                Console.ReadLine();
                Console.WriteLine(new string('-', 76));
                args = InjectedArguments;
            }
            try
            {
                string firstArg = (args.Length >= 1) ? args[0] : string.Empty;
                if (firstArg.Equals("PrintTemplate"))
                {
                    PrintTemplate(args);
                }
                else if (firstArg.Equals("ProcessImage"))
                {
                    ProcessImage(args);
                }
                else
                {
                    PrintHelp(args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(new string('-', 76));
                Console.WriteLine("Exception.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(new string('-', 76));
                Console.WriteLine("Execution ended on exception. Press enter key to terminate.");
                Console.ReadLine();
                Console.WriteLine(new string('-', 76));
            }
        }

        public static Range CharRange { get; } = new Range(32, 128);

        public static int CoreRowCount => 6;

        public static int CoreColumnCount => 16;

        public static readonly Lazy<CoreCharArrayInfo> CoreInfoLazy =
            new Lazy<CoreCharArrayInfo>(() =>
            {
                return new CoreCharArrayInfo(
                    CharRange, 
                    rowCount: CoreRowCount, 
                    columnCount: CoreColumnCount);
            });

        public static CoreCharArrayInfo CoreInfo => CoreInfoLazy.Value;

        public static readonly Lazy<PaddedCharArrayInfo> PaddedInfoLazy =
            new Lazy<PaddedCharArrayInfo>(() =>
            {
                return new PaddedCharArrayInfo(CoreInfo);
            });

        public static PaddedCharArrayInfo PaddedInfo => PaddedInfoLazy.Value;

        public static readonly Lazy<DuplexCharArrayInfo> DuplexInfoLazy =
            new Lazy<DuplexCharArrayInfo>(() =>
            {
                return new DuplexCharArrayInfo(PaddedInfo);
            });

        public static DuplexCharArrayInfo DuplexInfo => DuplexInfoLazy.Value;

        public static readonly Lazy<SpacedCharArrayInfo> SpacedInfoLazy =
            new Lazy<SpacedCharArrayInfo>(() =>
            {
                return new SpacedCharArrayInfo(DuplexInfoLazy.Value);
            });

        public static SpacedCharArrayInfo SpacedInfo => SpacedInfoLazy.Value;

        public static readonly Lazy<CharArrayFormatter> CharArrayFormatterLazy =
            new Lazy<CharArrayFormatter>(() =>
            {
                return new CharArrayFormatter(SpacedInfoLazy.Value);
            });

        public static CharArrayFormatter CharArrayFormatter => CharArrayFormatterLazy.Value;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void PrintTemplate(string[] args)
        {
            var formatter = CharArrayFormatter;
            if (args.Length == 1)
            {
                formatter.PrintToConsole();
            }
            else if (args.Length >= 2)
            {
                string outputTextFilename = args[1];
                Console.WriteLine("Writing character template to: " + outputTextFilename);
                using (var strm = new FileInfo(outputTextFilename).OpenWrite())
                {
                    strm.SetLength(0);
                    using (var textWriter = new StreamWriter(strm, Encoding.ASCII))
                    {
                        formatter.PrintToFile(textWriter);
                    }
                }
                Console.WriteLine("Finished writing character template.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ProcessImage(string[] args)
        {
            string screenshotFilename = args[1];
            if (!File.Exists(screenshotFilename))
            {
                throw new FileNotFoundException(
                    fileName: screenshotFilename, 
                    message: "Argument must be a full path to a screenshot bitmap file.");
            }
            Console.WriteLine("Loading screenshot bitmap: " + screenshotFilename);
            IntBitmap screenshotImage;
            try
            {
                screenshotImage = BitmapIoUtility.LoadAsIntBitmap(screenshotFilename);
            }
            catch
            {
                Console.WriteLine("Failed to load screenshot bitmap.");
                throw;
            }
            var duplexInfo = DuplexInfo;
            string inputOutputFolder = Path.GetDirectoryName(screenshotFilename);
            string inputFriendlyName = Path.GetFileNameWithoutExtension(screenshotFilename);
            string outputBitmapFilename = Path.Combine(
                inputOutputFolder, inputFriendlyName + "_CharBitmap.png");
            string outputBase64Filename = Path.Combine(
                inputOutputFolder, inputFriendlyName + "_Base64.txt");
            string outputCodeFragmentFilename = Path.Combine(
                inputOutputFolder, inputFriendlyName + "_CodeFragment.txt");
            var extractor = new FixedWidthFontRectExtractor(duplexInfo, screenshotImage);
            extractor.Process();
            var emitter = new FixedWidthFontResourceEmitter(extractor);
            emitter.Process();
            Console.WriteLine("Writing single column char bitmap to: " + outputBitmapFilename);
            emitter.SingleColumnImage.SaveToFile(outputBitmapFilename);
            Console.WriteLine("Writing Base64-encoded string to: " + outputBase64Filename);
            File.WriteAllText(outputBase64Filename, emitter.EncodedBase64);
            Console.WriteLine("Writing source code fragment to: " + outputCodeFragmentFilename);
            File.WriteAllText(outputCodeFragmentFilename, emitter.CodeFragment);
        }
    }
}
