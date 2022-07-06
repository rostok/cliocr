using System;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Storage.Streams;
using Windows.Foundation.Metadata;

namespace cliocr {
    public class Program {
        private static async void Go(string[] args) {
			SoftwareBitmap bmp = null;
            try {
            	if (args.Contains("-i")) {
            		var stdin = Console.OpenStandardInput();
            		System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    int read;
                    byte[] buffer = new byte[16*1024];
                    while ((read = stdin.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
       				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(System.IO.WindowsRuntimeStreamExtensions.AsRandomAccessStream(ms));
       				bmp = await decoder.GetSoftwareBitmapAsync();
            	}
            	else {
            		var dataPackage = Clipboard.GetContent();
            		if (dataPackage.Contains(StandardDataFormats.Bitmap)) {
						var image = await dataPackage.GetBitmapAsync();
                		var imageStream = await image.OpenReadAsync();
           				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
           				bmp = await decoder.GetSoftwareBitmapAsync();
            		} else {
                    	Console.Error.WriteLine("Not Bitmap");
                        Environment.Exit(-1);
                    }
            	}
				//Console.WriteLine("bmp {0}x{1}@{2}", bmp.PixelWidth, bmp.PixelHeight, bmp.BitmapPixelFormat);
            }
            catch (Exception ex)
            {
            	Console.Error.WriteLine("Something bad happened while reading image:\n"+ex);
                Environment.Exit(-1);
            }

            try {
                OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                String lang = args.SkipWhile (s => s != "-l").Skip(1).DefaultIfEmpty("").First().ToLower();
                if (lang!="") ocrEngine = OcrEngine.TryCreateFromLanguage(OcrEngine.AvailableRecognizerLanguages.FirstOrDefault(l => l.LanguageTag.ToLower().StartsWith(lang)));
                OcrResult ocrResult = await ocrEngine.RecognizeAsync(bmp);
                string extractedText = "FAILURE";
                extractedText = ocrResult.Text;
                if (args.Contains("-v")) {
                    Console.WriteLine("text:\t{0}", ocrResult.Text);
                    Console.WriteLine("angle:\t{0}",ocrResult.TextAngle);
                    Console.WriteLine("lines:\t{0}",ocrResult.Lines.Count);
                }
                if (args.Contains("-c")) {
                    Thread thread = new Thread(() => {
                        if (!String.IsNullOrWhiteSpace(extractedText)) {
                        	var dp = new DataPackage();
                        	dp.SetText(extractedText);
                        	Clipboard.SetContent(dp);
                        	Clipboard.Flush();
						}
                    });
                    thread.SetApartmentState(ApartmentState.STA); 
                    thread.Start(); 
                    thread.Join();
                } else {
                    Console.WriteLine(extractedText);
                }
            }
            catch (Exception ex) {
        		Console.Error.WriteLine("Something bad happened while running OCR:\n"+ex);
                Environment.Exit(-1);
            }
            resetEvent.Set();
        }

        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        [STAThreadAttribute]
        static void Main(string[] args) {
            if (args.Contains("--help")||args.Contains("-h")||args.Contains("/?")) {
                Console.WriteLine("cliocr [command]");
                Console.WriteLine(" -c              copy recognized text to clipboard");
                Console.WriteLine(" -s              show avaiable languages");
                Console.WriteLine(" -i              read image from stdin (bmp, jpg, ...)");
                Console.WriteLine(" -v              verbose OcrResult");
                Console.WriteLine(" -l LANG         set lanugage");
                Console.WriteLine(" -h --help /?    this help");
                Environment.Exit(0);
            }
            if (args.Contains("-s")) {
                foreach(var l in OcrEngine.AvailableRecognizerLanguages) Console.WriteLine(l.LanguageTag+"\t"+l.DisplayName);
                Environment.Exit(0);
            }
        	Go(args);
        	resetEvent.WaitOne(); 
        }
	}
}