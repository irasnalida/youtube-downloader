using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        Stream stdin = Console.OpenStandardInput();

        byte[] lengthBytes = new byte[4];
        stdin.Read(lengthBytes, 0, 4);

        int length = BitConverter.ToInt32(lengthBytes, 0);

        char[] buffer = new char[length];

        using (StreamReader reader = new StreamReader(stdin))
        {
            if (reader.Peek() >= 0)
            {
                reader.Read(buffer, 0, buffer.Length);
            }
        }

        string jsonString = new string(buffer);
        jsonString = jsonString.Replace("\\n", "\n").Replace("\\t", "\t");
        jsonString = jsonString.Trim('"');
        //File.WriteAllText("js.txt", jsonString);

        //stdin.Close();

        string arguments = "";
        if (jsonString.IndexOf(',') != -1)
        {
            File.WriteAllText("cookies.txt", jsonString.Split(',')[1]);
            arguments = '"' + jsonString.Split(',')[0] + '"';
        }
        else
        {
            arguments = jsonString;
        }
        
        string path = Path.Combine(Directory.GetCurrentDirectory(), @"YoutubeDownloader.exe");

        Process process = new Process();

        // Set the process start info
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = path,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        process.StartInfo = startInfo;

        process.Start();

        process.WaitForExit();
    }
}