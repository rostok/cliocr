using System;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using System.Linq;
using Windows.ApplicationModel.Core;

namespace cliocr
{
    public class Program 
    {
        private static async void Go(string[] args)
        {
            var dataPackage = Clipboard.GetContent();
            if (dataPackage.Contains(StandardDataFormats.Bitmap))
            {
                    try
                    {
                        var image = await dataPackage.GetBitmapAsync();
                        var imageStream = await image.OpenReadAsync();

						SoftwareBitmap bmp;
						BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
						bmp = await decoder.GetSoftwareBitmapAsync();
						//Console.WriteLine("bmp {0}x{1}@{2}", bmp.PixelWidth, bmp.PixelHeight, bmp.BitmapPixelFormat);
                        OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                        String lang = args.SkipWhile (s => s != "-l").Skip(1).DefaultIfEmpty("").First().ToLower();
                        if (lang!="") ocrEngine = OcrEngine.TryCreateFromLanguage(OcrEngine.AvailableRecognizerLanguages.FirstOrDefault(l => l.LanguageTag.ToLower().StartsWith(lang)));
                        OcrResult ocrResult = await ocrEngine.RecognizeAsync(bmp);
                        string extractedText = "FAILURE";
                        extractedText = ocrResult.Text;
                        if (args.Contains("-c")) 
                        {
                            Thread thread = new Thread(() => {
                                var dp = new DataPackage();
                                dp.SetText(extractedText);
                                Clipboard.SetContent(dp);
                                Clipboard.Flush();
                            });
                            thread.SetApartmentState(ApartmentState.STA); 
                            thread.Start(); 
                            thread.Join();
                        }
                        else 
                        {
                            Console.WriteLine(extractedText);
                        }
                    }
                    catch (Exception ex)
                    {
                		Console.Error.WriteLine("Something bad happened:\n"+ex);
                        Environment.Exit(-1);
                    }
            }
            else 
            {
            	Console.Error.WriteLine("Not Bitmap");
                Environment.Exit(-1);
            }
            resetEvent.Set();
        }

        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            if (args.Contains("--help")||args.Contains("-h")||args.Contains("/?")) {
                Console.WriteLine("cliocr [command]");
                Console.WriteLine(" -c              copy recognized text to clipboard");
                Console.WriteLine(" -i              show avaiable languages");
                Console.WriteLine(" -l LANG         set lanugage");
                Console.WriteLine(" -h --help /?    this help");
                Environment.Exit(0);
            }
            if (args.Contains("-i")) {
                foreach(var l in OcrEngine.AvailableRecognizerLanguages) Console.WriteLine(l.LanguageTag+"\t"+l.DisplayName);
                Environment.Exit(0);
            }
        	Go(args);
        	resetEvent.WaitOne(); 
        }
	}
}