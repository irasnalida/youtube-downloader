using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace YoutubeDownloader;
public partial class MainWindow
{
    private readonly string[] _cmdArgs = Environment.GetCommandLineArgs();
    private YoutubeDL YTDL { get; }
    
    IProgress<DownloadProgress> _progress;

    //Dictionary<string, SubtitleData[]> subs;
    
    OptionSet _options = new OptionSet();
    
    List<FormatData> _formatVideoList = new List<FormatData>();
    List<FormatData> _formatAudioList = new List<FormatData>();

    string OUTOUT_TEMPLATE = "%(title)s - %(id)s [%(height)sp].%(ext)s";
    string OUTPUT_PATH = "E:\\Downloads\\Video";
    string URL = "";

    public MainWindow()
    {
        this.YTDL = new YoutubeDL() { YoutubeDLPath = "yt-dlp.exe", FFmpegPath = "ffmpeg.exe", OutputFolder = "E:\\Downloads\\Video", OutputFileTemplate = "%(title)s - %(id)s [%(height)sp].%(ext)s" };
        
        InitializeComponent();
        
        if (_cmdArgs.Length == 2)
        {
            Debug.WriteLine(_cmdArgs[1]);

            URL = _cmdArgs[1];
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"cookies.txt")))
            {
                _options.Cookies = Path.Combine(Directory.GetCurrentDirectory(), @"cookies.txt");
            }
            FetchAndLogData();
        }
    }
    
    private async void FetchAndLogData()
    {

        CustomeTitle.Text = "Fetching all formats...";
        //BtnFetch.IsEnabled = false;

        //FormatListBox.ItemsSource = null;

        var res = await YTDL.RunVideoDataFetch(URL, overrideOptions:_options);

        if (!res.Success)
        {
            foreach (string item in res.ErrorOutput)
            {
                Debug.WriteLine(item);
            }
            return;
        }

        VideoData video = res.Data;
        string title = video.Title;
        string uploader = video.Uploader;
        long? views = video.ViewCount;

        TextBlock_VideoTitle.Text = title;
        TextBox_OutputTemplate.Text = OUTOUT_TEMPLATE;
        TextBox_SaveToPath.Text = OUTPUT_PATH;

        //TbVidTitle.Text = title;

        //GridVidInfo.Visibility = Visibility.Visible;

        // all available download formats
        FormatData[] formats = video.Formats;
        Debug.WriteLine($"Title: {title}\nUploader: {uploader}\nViews: {views}");
        
        //_formatList.Clear();
        //_formatList.Add("Best");

        for (int i = formats.Length -1; i>=0; i--)
        {
            Debug.WriteLine($"Id: {formats[i].FormatId}, Res: {formats[i].Resolution}, FPS: {formats[i].FrameRate}, ApproxSize: {formats[i].ApproximateFileSize}, Size: {formats[i].FileSize}, Ext: {formats[i].Extension}, Format: {formats[i].Format}, {formats[i].Bitrate} {formats[i].AudioBitrate}");

            if (formats[i].FrameRate != null)
            {
                //string f = ($"{formats[i].Resolution}, {FormatFileSize(formats[i].ApproximateFileSize ?? 0)} [{formats[i].FormatId}]");
                _formatVideoList.Add(formats[i]);
            }
            else if (formats[i].Resolution == "audio only"){
                _formatAudioList.Add(formats[i]);
            }
        }
        //FormatListBox.ItemsSource = _formatList;
        //Debug.WriteLine(FormatListBox.SelectedIndex);
        //Debug.WriteLine(FormatListBox);

        //BtnFetch.IsEnabled = true;
        //BtnStartDownload.IsEnabled = true;
        //FormatListBox.IsEnabled = true;
        CustomeTitle.Text = "Fetching complete!";

        VideoListView.ItemsSource = _formatVideoList;
        VideoExpander.IsEnabled = true;
        VideoProgessBar.Visibility = Visibility.Collapsed;
        AudioListView.ItemsSource = _formatAudioList;
        AudioExpander.IsEnabled = true;
        AudioProgessBar.Visibility = Visibility.Collapsed;
    }
    
    private void ShowProgress(DownloadProgress p)
    {
        CustomeTitle.Text = p.State.ToString();
        ProgressDownload.Value = Math.Round(p.Progress * 100);
    }
    
    public static string FormatFileSize(long bytes)
    {
        var unit = 1024;
        if (bytes < unit) { return $"{bytes} B"; }

        var exp = (int)(Math.Log(bytes) / Math.Log(unit));
        return $"{bytes / Math.Pow(unit, exp):F2} {("KMGTPE")[exp - 1]}B";
    }

    private void Grid_HoldAndDrag(object sender, MouseButtonEventArgs e)
    {
        this.DragMove();
    }

    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        if( VideoListView.SelectedIndex < 0 ||
            AudioListView.SelectedIndex < 0 ||
            TextBox_OutputTemplate.Text.Length == 0 ||
            !Directory.Exists(TextBox_SaveToPath.Text) )
        {
            //show tooltip
            return;
        }


        FormatData v = _formatVideoList[VideoListView.SelectedIndex];
        FormatData a = _formatAudioList[AudioListView.SelectedIndex];

        Debug.WriteLine(v.FormatId);
        Debug.WriteLine(a.FormatId);

        _options.Format = $"{v.FormatId}+{a.FormatId}";
        //_options.Output = TextBox_OutputTemplate.Text;
        //this.YTDL.OutputFolder = TextBox_SaveToPath.Text;
        //this.YTDL.OutputFileTemplate = TextBox_OutputTemplate.Text;

        _progress = new Progress<DownloadProgress>(p => ShowProgress(p));

        RunResult<string> result;
        result = await YTDL.RunVideoDownload(_cmdArgs[1], progress: _progress, overrideOptions: _options);

        if (result.Success)
        {
            CustomeTitle.Text = "Download Completed";
        }
        else
        {
            CustomeTitle.Text = "Error";
            foreach (string err in result.ErrorOutput)
            {
                Debug.WriteLine(err);
            }
        }
    }

    private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"cookies.txt")))
        {
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), @"cookies.txt"), "");
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"cookies.txt"));
        }
    }
}