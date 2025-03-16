using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace YoutubeDownloader;

public partial class MainWindow
{
    private readonly string[] _cmdArgs = Environment.GetCommandLineArgs();
    private YoutubeDL YoutubeDL { get; }
    
    private IProgress<DownloadProgress> _progress;
    //private IProgress<string> output;
    
    OptionSet _options = new OptionSet();
    
    List<string> _formatList = new List<string>();
    
    public MainWindow()
    {
        this.YoutubeDL = new YoutubeDL() { YoutubeDLPath = "yt-dlp.exe", FFmpegPath = "ffmpeg.exe", OutputFolder = "E:\\Downloads\\Video", OutputFileTemplate = "%(title)s - %(id)s [%(height)sp].%(ext)s" };

        InitializeComponent();
        
        _progress = new Progress<DownloadProgress>((p) => ShowProgress(p));
        if (_cmdArgs.Length == 2)
        {
            Debug.WriteLine(_cmdArgs[1]);

            TbVideoUrl.Text = _cmdArgs[1];
            //FetchAndLogData();
        }
    }
    
    private async void FetchAndLogData()
    {
        if (TbVideoUrl.Text == string.Empty)
        {
            TbVidTitle.Text = "Enter a valid Youtube link!";
            return;
        }

        CustomeTitle.Text = "Fetching all formats...";
        BtnFetch.IsEnabled = false;

        FormatListBox.ItemsSource = null;

        var res = await YoutubeDL.RunVideoDataFetch(TbVideoUrl.Text);

        VideoData video = res.Data;
        string title = video.Title;
        string uploader = video.Uploader;
        long? views = video.ViewCount;

        TbVidTitle.Text = title;
    
        GridVidInfo.Visibility = Visibility.Visible;

        // all available download formats
        FormatData[] formats = video.Formats;
        Debug.WriteLine($"Title: {title}\nUploader: {uploader}\nViews: {views}");
        
        _formatList.Clear();
        _formatList.Add("Best");

        for (int i = formats.Length -1; i>=0; i--)
        {
            //Debug.WriteLine($"Id: {formats[i].FormatId}, Res: {formats[i].Resolution}, FPS: {formats[i].FrameRate}, ApproxSize: {formats[i].ApproximateFileSize}, Size: {formats[i].FileSize}, Ext: {formats[i].Extension}, Format: {formats[i].Format}, {formats[i].Bitrate}");

            if (formats[i].ApproximateFileSize != null)
            {
                string f = ($"{formats[i].Resolution}, {FormatFileSize(formats[i].ApproximateFileSize ?? 0)} [{formats[i].FormatId}]");
                _formatList.Add(f);
            }
        }
        FormatListBox.ItemsSource = _formatList;
        Debug.WriteLine(FormatListBox.SelectedIndex);

        BtnFetch.IsEnabled = true;
        BtnStartDownload.IsEnabled = true;
        FormatListBox.IsEnabled = true;
        CustomeTitle.Text = "Fetching complete!";
    }
    
    private void ShowProgress(DownloadProgress p)
    {
        CustomeTitle.Text = p.State.ToString();
        PrgbDownload.Value = p.Progress * 100;
        TbTextDownloaded.Text = $"Downloaded {Math.Round(p.Progress*100,2)}% of {p.TotalDownloadSize}";
        TextSpeed.Text = p.DownloadSpeed;
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

    private void BtnFetch_OnClick_Fetch(object sender, RoutedEventArgs e)
    {
        FetchAndLogData();
    }

    private async void BtnStartDownload_OnClick(object sender, RoutedEventArgs e)
    {
        BtnStartDownload.Visibility = Visibility.Collapsed;
        ProgressData.Opacity = 1;
        
        if (ChkBoxSponsorblock.IsChecked == true) _options.AddCustomOption<string>("--sponsorblock-mark", "all");
        if (ChkBoxEmbedSubs.IsChecked == true) _options.AddCustomOption<string>("--embed-subs", "");
        if (ChkBoxChapters.IsChecked == true) _options.AddCustomOption<string>("--embed-chapters", "");
        
        RunResult<string> result;
        result = await YoutubeDL.RunVideoDownload(TbVideoUrl.Text, progress: _progress, overrideOptions: _options);

        if (result.Success)
        {
            Title = "Download Completed";
        }
        else
        {
            Title = "Error";
            foreach(string err in result.ErrorOutput)
            {
                Debug.WriteLine(err);
            }
        }
        ProgressData.Opacity = 0;
        BtnStartDownload.Visibility= Visibility.Visible;
    }

    private void FormatListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FormatListBox.SelectedIndex < 0) return;
        string selectedItem = FormatListBox.SelectedItem.ToString();
        if (selectedItem == "Best") { _options.Format = "bestvideo*+bestaudio/best"; Debug.WriteLine("Set Best"); }
        else
        {
            int startIndex = selectedItem.IndexOf('[');
            int endIndex = selectedItem.IndexOf(']', startIndex);
            string fs = "";

            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            {
                _options.Format = "bestvideo*+bestaudio/best"; Debug.WriteLine("Set Best");
            }
            else
            {
                fs = selectedItem.Substring(startIndex + 1, endIndex - startIndex - 1);
                if (selectedItem.Contains("audio only"))
                {
                    _options.Format = fs; Debug.WriteLine($"Set {fs}");
                }
                else
                {
                    _options.Format = fs+"+bestaudio"; Debug.WriteLine($"Set {fs}+bestaudio");
                }
            }
        }
    }
}