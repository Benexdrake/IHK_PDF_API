namespace IHK_PDF_API;

public class Helpers
{
    // Loading Json File as Config to seperate the IHK PDFs
    public static async Task<List<IHKPDF>> GetConfig()
    {
        var result = File.ReadAllText("config.json");
        var list = JsonConvert.DeserializeObject<List<IHKPDF>>(result);
        if (list is not null)
        {
            return list;
        }
        return new();
    }

    public static async Task SaveConfig(List<IHKPDF> pdfs)
    {
        string jsonString = JsonConvert.SerializeObject(pdfs, Formatting.Indented);
        File.WriteAllText("config.json", jsonString);
    }

    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

}
