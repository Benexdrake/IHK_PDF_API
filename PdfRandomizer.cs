using System.IO;

namespace IHK_PDF_API;

public class PdfRandomizer
{
    private string _path;
    private string _outputPath;

    public PdfRandomizer()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }
    public async Task<string> Start(string type, string path, string outputPath)
    {

        string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        _path = exePath + "\\" + path;
        _outputPath = exePath + "\\" + outputPath;
        Helpers.CreateDirectory(_path);
        Helpers.CreateDirectory(_outputPath);
        var pdfs = Helpers.GetConfig().Result;
        if(pdfs.Count > 0)
        {
            //Helpers.CreateDirectory(_path + $"\\{type}");

            var newPdfs = pdfs.Where(x => x.Type.Equals(type)).ToList();

            var list = new List<IHKPDF>();

            for (int i = 0; i < newPdfs.Count; i++)
            {
                if (Directory.Exists(_path + "\\" + newPdfs[i].Type + "\\" + newPdfs[i].FileName))
                {
                    list.Add(newPdfs[i]);
                }
            }
            if (list.Count > 0)
            {
                var newList = await Randomize(list);

                var pdf = await PDFConverter(newList, type);
                return pdf;
            }
        }
        return "";
    }

    private async Task<List<string>> Randomize(List<IHKPDF> pdfs)
    {
        Random rand = new();
        int count = pdfs[0].Sites.Length;

        List<string> newFilePaths = new();

        for (int i = 1; i <= count; i++)
        {
            newFilePaths.Add(_path + "\\" + pdfs[0].Type + "\\" + pdfs[rand.Next(0, pdfs.Count)].FileName + $"\\{i}.pdf");
        }
        return newFilePaths;
    }

    private async Task<string> PDFConverter(List<string> pdfs, string type)
    {
        int count = pdfs.Count;

        string fileName = type + "_";

        // Unterteil den pdf datei Pfad und bekommt so den Namen des Ordners als Dateiname z.B. SO-15 für Sommer 2015
        foreach (var item in pdfs)
        {
            var split = item.Split('\\');
            fileName += split[split.Length - 2] + "_";
        }
        fileName = fileName.Substring(0, fileName.Length - 1);
        fileName += ".pdf";

        List<PdfDocument> pdfDocuments = new List<PdfDocument>();
        for (int i = 0; i < count; i++)
        {
            pdfDocuments.Add(PdfReader.Open(pdfs[i], PdfDocumentOpenMode.Import));
        }

        using (PdfDocument outPdf = new())
        {
            foreach (var pdf in pdfDocuments)
            {
                CopyPages(pdf, outPdf);
            }
            string file = $"{_outputPath}\\{fileName}";
            outPdf.Info.Title = fileName;
            outPdf.Save(file);

            return file;
        }
    }

    void CopyPages(PdfDocument from, PdfDocument to)
    {
        for (int i = 0; i < from.PageCount; i++)
        {
            to.AddPage(from.Pages[i]);
        }
    }
}


