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

        //stdin.Close();

        File.AppendAllText("log.txt", $"{jsonString}\r\n");

        string path = Path.Combine(Directory.GetCurrentDirectory(), @"YoutubeDownloader.exe");
        string arguments = jsonString;

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