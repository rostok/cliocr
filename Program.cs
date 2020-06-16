using System;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

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
                        OcrResult ocrResult = await ocrEngine.RecognizeAsync(bmp);
                        string extractedText = "FAILURE";
                        extractedText = ocrResult.Text;
                        Console.WriteLine(extractedText);
                    }
                    catch (Exception ex)
                    {
                		Console.Error.WriteLine("Clipboard is empty or data is not an image"+ex);
                    }
            }
            else 
            {
            	Console.Error.WriteLine("Not Bitmap");
            }
            resetEvent.Set();
        }

        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        [STAThread]
        static void Main(string[] args)
        {
        	Go(args);
        	resetEvent.WaitOne(); 
        }
	}
}