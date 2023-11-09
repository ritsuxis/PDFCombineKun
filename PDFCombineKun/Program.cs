using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFCombineKun
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string defaultOutputPdfFile = "output.pdf";
            var outputPdfFile = defaultOutputPdfFile;
            var inputPdfFiles = new List<string>();

            for (var i = 0; i < args.Length; i++)
            {
                // -o で出力ファイルの名前を指定
                if (args[i].Equals("-o", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    outputPdfFile = args[i + 1];
                    i++; // 次の引数も処理済みとしてスキップ
                }
                else
                {
                    inputPdfFiles.Add(args[i]);
                }
            }

            if (inputPdfFiles.Count < 1)
            {
                Console.WriteLine("少なくとも1つの入力ファイルを指定してください。");
                return;
            }

            MergePdfFiles(inputPdfFiles, outputPdfFile);

            Console.WriteLine($"PDFファイルの結合が完了しました。出力ファイル: {outputPdfFile}");
        }

        private static void MergePdfFiles(IEnumerable<string> inputPdfFiles, string outputPdfFile)
        {
            var document = new Document();
            var copy = new PdfCopy(document, new FileStream(outputPdfFile, FileMode.Create));

            document.Open();

            foreach (var reader in inputPdfFiles.Select(inputFile => new PdfReader(inputFile)))
            {
                for (var page = 1; page <= reader.NumberOfPages; page++)
                {
                    var importedPage = copy.GetImportedPage(reader, page);
                    copy.AddPage(importedPage);
                }

                reader.Close();
            }

            document.Close();
        }
    }
}