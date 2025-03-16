# Youtube Downloader

This is a simple, user-friendly YouTube downloader app built using WPF for Windows. The app allows users to download YouTube videos locally by utilizing yt-dlp, a popular tool for downloading videos from YouTube and other video hosting sites. The app interacts with a native host to run yt-dlp in the background and download videos seamlessly.

![Image](https://i.postimg.cc/wxrt0NJq/image.png)

#### Tested only on Firefox browser on Windows 11.
#### May not be very stable or have some bugs.
#### This app can work standalone or with the addon. Just copy paste a link or right click and select `Download Video` option from the context menu.

## Install Guide
- Build both `sytd-host` and `yt-downloader` in Visual Studio.
- Create a folder `Youtube Downloader` and inside that create `app` folder.
- Copy the build files from sytd-host and yt-downloader inside the `app` folder.
- Copy `host.json` and `register-host.bat` file inside `Youtube Downloader` folder.
- Run the `register-host.bat` file to create the registry entry for the host app.
- Download and install the addon from release.


## Depends on the following
- [yt-dlp](https://github.com/yt-dlp/yt-dlp)
- [WPFUI](https://wpfui.lepo.co/)
- [YoutubeDLSharp](https://github.com/Bluegrams/YoutubeDLSharp).

## Disclaimer

This app is intended for personal use and educational purposes only. Make sure to comply with YouTube's terms of service when downloading content from the platform.