﻿using JITECEntity;
using Newtonsoft.Json;
using PDFiumSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using Tesseract;

namespace JITECMondaiFormatter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // TODO パラメータ化など
            var examId = "2021r03a_ap";
            var examPartId = "2021r03a_ap_am_qs";
            var inputFilePath = @"C:\Users\koudenpa\source\repos\JITECMondaiFormatter\mondai\2021\2021r03a_ap_am_qs.pdf";
            var examRefName = "令和3年度 秋期 応用情報技術者試験 午前 問題";

            var input = new Input(new List<InputItem> {
                new InputItem(
                    examId,
                    examPartId,
                    examRefName,
                    inputFilePath,
                    ""
                    )
            });

            var inputeItem = input.Items.First();
            var outputDirPath = Path.Combine(examId, examPartId);
            Directory.CreateDirectory(outputDirPath);

            var questions = await ReadQuestion(
                inputeItem,
                outputDirPath
                );

            var output = new ExamPart(inputeItem.ExamId, inputeItem.ExamPartId, examRefName, questions.ToList(), 1);

            var outputFilePath = Path.Combine(examId, examPartId + ".json");
            await File.WriteAllTextAsync(outputFilePath, JsonConvert.SerializeObject(output));
        }

        public record OcrLine(
            string Text,
            Rect Bounds
            )
        { }

        public static async Task<IEnumerable<Question>> ReadQuestion(
            InputItem inputItem, string outputDirPath)
        {
            var pngEnc = new PngEncoder();
            var rawEnc = new BmpEncoder();
            // https://github.com/tesseract-ocr/tessdata
            using var ocr = new TesseractEngine(@"./tessdata", "eng+jpn", EngineMode.Default);

            using var doc = new PdfDocument(inputItem.QuestionFilePath);
            var pageNumber = 0;
            var qNo = 0;
            var qList = new List<Question>();
            foreach (var page in doc.Pages)
            {
                pageNumber++;

                var width = 1280;
                var height = (int)(width * page.Height / page.Width);

                using var pageBitmap = new PDFiumBitmap(width, height, true);
                page.Render(pageBitmap);

                var pageImagePath = $"p{pageNumber.ToString("000")}.png";
                var pageTextPath = $"p{pageNumber.ToString("000")}.txt";

                using var rawPageImage = Image.Load(pageBitmap.AsBmpStream());
                // Set the background to white, otherwise it's black. https://github.com/SixLabors/ImageSharp/issues/355#issuecomment-333133991
                rawPageImage.Mutate(x => x.BackgroundColor(Color.White));
                //await rawPageImage.SaveAsync(pageImagePath, pngEnc);

                using var normalizedPageImage = await NormalizePageImage(ocr, rawPageImage);
                await rawPageImage.SaveAsync(pageImagePath, pngEnc);

                // OCR
                using var bmpStream = new MemoryStream();
                await normalizedPageImage.SaveAsync(bmpStream, pngEnc);
                bmpStream.Position = 0;
                using var ocrImage = Pix.LoadFromMemory(bmpStream.ToArray());
                using var ocrRes = ocr.Process(ocrImage);
                await File.WriteAllTextAsync(pageTextPath, ocrRes.GetText());

                var lines = new List<OcrLine>();
                using var ite = ocrRes.GetIterator();
                do
                {
                    var text = ite.GetText(PageIteratorLevel.TextLine);
                    Rect bounds;
                    ite.TryGetBoundingBox(PageIteratorLevel.TextLine, out bounds);
                    lines.Add(new OcrLine(text, bounds));
                }
                while (ite.Next(PageIteratorLevel.TextLine));

                //// Detect Question
                //var qLines = new List<OcrLine>();
                //int detectedQNo = -1;
                //foreach (var ocrLine in lines)
                //{
                //    // XXX テキストが斜めってるおかげで「問」より先に次の設問の本文が来てしまう
                //    var isNewQ = ocrLine.Text.StartsWith("問");
                //    if (isNewQ)
                //    {
                //        var q = await WriteQuestion(inputItem, pngEnc, qNo, detectedQNo, normalizedPageImage, qLines, outputDirPath);
                //        if (q != null) { qList.Add(q); }
                //        qLines.Clear();
                //        int.TryParse(ocrLine.Words.Skip(1).FirstOrDefault()?.Text, out detectedQNo);
                //        if (detectedQNo > 0)
                //        {
                //            qNo++;
                //        }
                //    }
                //    qLines.Add(ocrLine);
                //}
                //{
                //    var q = await WriteQuestion(inputItem, pngEnc, qNo, detectedQNo, normalizedPageImage, qLines, outputDirPath);
                //    if (q != null) { qList.Add(q); }
                //}
            }

            return qList;
        }

        private static async Task<Image> NormalizePageImage(TesseractEngine ocr, Image pageImage)
        {
            using var bmpStream = new MemoryStream();
            await pageImage.SaveAsBmpAsync(bmpStream);
            bmpStream.Position = 0;
            using var ocrImage = Pix.LoadFromMemory(bmpStream.ToArray());
            using var ocrRes = ocr.Process(ocrImage);

            int orientation;
            float confidence;
            ocrRes.DetectBestOrientation(out orientation, out confidence);
            Console.WriteLine($"orientation:{orientation}, confidence:{confidence}");

            using var ite = ocrRes.GetIterator();
            while (ite.Next(PageIteratorLevel.TextLine))
            {
                Console.WriteLine(ite.GetText(PageIteratorLevel.TextLine));
                Rect bounds;
                ite.TryGetBoundingBox(PageIteratorLevel.TextLine, out bounds);
                Console.WriteLine(bounds);
            }

            return pageImage.Clone(x => { });// (x => x.Rotate((float)(normalizeRes.TextAngle ?? 0d)));
        }

        //// TODO パラメータ化など
        //private static FontCollection collection = new();
        //private static FontFamily family = collection.Add("Font/ipaexg.ttf");
        //private static Font font = family.CreateFont(14, FontStyle.Italic);
        //private static async Task<Question> WriteQuestion(
        //    InputItem inputItem,
        //    PngEncoder enc, int qNo, int detectedQNo, Image pageImage, IEnumerable<OcrLine> qLines, string outputDirPath)
        //{
        //    if (!qLines.Any() || detectedQNo < 1)
        //    {
        //        return null;
        //    }

        //    var qImagePath = Path.Combine(outputDirPath, $"q{qNo.ToString("000")}.png");
        //    var qTextPath = Path.Combine(outputDirPath, $"q{qNo.ToString("000")}.txt");

        //    // ページ番号行を消し飛ばす
        //    var pageNoReg = new Regex(@"^[ -~]+$");
        //    var normalizedLines = qLines.Reverse().SkipWhile(x => pageNoReg.IsMatch(x.Text)).Reverse().ToList();

        //    // XXX 幅ビミョーなので固定しておく
        //    //var qLeft = (int)Math.Max(0, normalizedLines.Min(x => x.Words.Min(y => y.BoundingRect.Left)) - 8);
        //    //var qRight = (int)Math.Min(pageImage.Width, normalizedLines.Max(x => x.Words.Max(y => y.BoundingRect.Left)) + 8);
        //    var qLeft = (int)(pageImage.Width * 0.05);
        //    var qRight = (int)(pageImage.Width * 0.95);
        //    var qTop = (int)Math.Max(0, normalizedLines.Min(x => x.Words.Min(y => y.BoundingRect.Top)) - 16);
        //    var qBottom = (int)Math.Min(pageImage.Height, normalizedLines.Max(x => x.Words.Max(y => y.BoundingRect.Top)) + 64);
        //    var refText = $"出典: {inputItem.ExamRefName} 問 {string.Format("{0,2}", qNo)}";

        //    using var qImage = pageImage
        //        .Clone(x => x.Crop(new Rectangle(qLeft, qTop, qRight - qLeft, qBottom - qTop)));
        //    // TODO 右寄せ面倒臭いけれど右寄せのほうがよさそう
        //    qImage.Mutate(x => x.DrawText(refText, font, Color.Black, new PointF(8, qImage.Height - 24)));
        //    await qImage.SaveAsync(qImagePath, enc);

        //    var qText = string.Join(Environment.NewLine, normalizedLines.Select(x => x.Text));
        //    //await File.WriteAllTextAsync(qTextPath, qText);
        //    // Log
        //    Console.WriteLine(qText);

        //    // XXX 回答ここで処理できるわけねーだろ
        //    return new Question(qNo, qText, qImagePath.Replace("\\", "/"), "-");
        //}
    }
}
