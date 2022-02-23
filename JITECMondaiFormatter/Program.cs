using PDFiumSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace JITECMondaiFormatter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var input = new Input(new List<InputItem> {
                new InputItem(
                    @"C:\Users\koudenpa\source\repos\JITECMondaiFormatter\mondai\2021\2021r03a_ap_am_qs.pdf",
                    @"")
            });

            await ReadPdfAsync(input.Items.First().QuestionFilePath);
        }

        public static async Task ReadPdfAsync(string filename)
        {
            var enc = new PngEncoder();
            var ocr = OcrEngine.TryCreateFromLanguage(new Language("ja"));

            using var doc = new PdfDocument(filename);
            var pageNumber = 0;
            foreach (var page in doc.Pages)
            {
                pageNumber++;

                var width = 1024;
                var height = (int)(width * page.Height / page.Width);

                using var pageBitmap = new PDFiumBitmap(width, height, true);

                page.Render(pageBitmap);

                var imagePath = $"p{pageNumber.ToString("000")}.png";
                var textPath = $"p{pageNumber.ToString("000")}.txt";
                var image = Image.Load(pageBitmap.AsBmpStream());

                // Set the background to white, otherwise it's black. https://github.com/SixLabors/ImageSharp/issues/355#issuecomment-333133991
                image.Mutate(x => x.BackgroundColor(Color.White));

                image.Save(imagePath, enc);

                using var bmpStream = new InMemoryRandomAccessStream();
                image.Save(bmpStream.AsStream(), enc);
                var bmpDec = await BitmapDecoder.CreateAsync(bmpStream);
                var ocrRes = await ocr.RecognizeAsync(await bmpDec.GetSoftwareBitmapAsync());
                await File.WriteAllTextAsync(textPath, ocrRes.Text);
            }
        }
    }
}
