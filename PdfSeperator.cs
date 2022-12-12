using System.IO;

namespace IHK_PDF_API;

public class PdfSeperator
{
    private string _path;
    public PdfSeperator()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    public async Task Start(string path)
    {
        string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        _path = exePath + "\\" + path;

        // Load Config
        var pdfs = Helpers.GetConfig().Result;
        if(pdfs.Count > 0)
        {
            Helpers.CreateDirectory(_path);
            foreach (var pdf in pdfs)
            {
                var file = _path + "\\" + pdf.Type + "\\" + pdf.FileName + ".pdf";
                if (File.Exists(file))
                {
                    await SeperatePDF(file, pdf);
                }
            }
        }
    }

    private async Task SeperatePDF(string file, IHKPDF ihkpdf)
    {
        var sites = ihkpdf.Sites;
        PdfDocument pdf = PdfReader.Open(file, PdfDocumentOpenMode.Import);
        int site = 1;
        for (int i = 0; i < sites.Length; i++)
        {
            PdfDocument result = new PdfDocument();
            int a = site;
            for (int j = a; j < a + sites[i]; j++)
            {
                result.AddPage(pdf.Pages[j]);
                site++;
            }
            var path = Path.GetFullPath(file).Replace(".pdf", "");

            Helpers.CreateDirectory(path);
            result.Save(path + $"\\{i + 1}.pdf");
        }
        pdf.Close();
    }
}
