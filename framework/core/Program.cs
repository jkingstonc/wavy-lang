using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string text;
        //var fileStream = new FileStream(@"F:\OneDrive - Lancaster University\programming\c#\wavy~\wavy~\test.w~", FileMode.Open, FileAccess.Read);
        var fileStream = new FileStream(@"C:\Users\44778\OneDrive - Lancaster University\programming\c#\wavy~\wavy~\test.w~", FileMode.Open, FileAccess.Read);
        using (var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8))
        {
            text = streamReader.ReadToEnd();
        }
        WavyRuntime runtime = new WavyRuntime();
        runtime.compile(text);
        System.Console.ReadLine();
    }
}
