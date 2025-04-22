using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace PDFToolKit.Service
{
    public class PdfMergeService
    {
       
        public bool MergeFiles(string[] inputFiles, string outputFilePath)
        {
            try
            {
               
                using var outputDocument = new PdfDocument();

                foreach (var file in inputFiles)
                {
                    using var inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                    for (int i = 0; i < inputDocument.PageCount; i++)
                    {
                        var page = inputDocument.Pages[i];
                        outputDocument.AddPage(page);
                    }
                }

                outputDocument.Save(outputFilePath);
               
                return true;
            }
            catch (Exception ex)
            {
                // TODO: Add logging
                return false;
            }
        }
    }
}
