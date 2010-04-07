SubtitleProvider for MediaBrowser

About:
SubtitleProvider for MediaBrowser is a plugin that automatically downloads the missing subtitle files for your movies and tv shows.

Current version:
v1.1.1.2

How to use:
http://community.mediabrowser.tv/permalinks/1311/how-do-i-configure-and-use-the-subtitleprovider-plugin

How it works:
This plugin works against http://www.opensubtitles.org and http://www.subtitlesource.org. There's no plan to add other subtitle sources at this point. The plugin's workflow contains two phases:
1. Checking of available subtitle file. If the video file is named "My.Movie(2009)-release.avi", the plugin tries to find either a file "My.Movie(2009)-release.sub" or "My.Movie(2009)-release.srt". If a subtitle file is found, processing ends. 
2. If there's no subtitle file available, the plugin tries to find and download one. OpenSubtitles and SubtitleSource are currently supported subtitle sources. Matching the video against the subtitles is done based on the video's file size and hash (OpenSubtitles) or against the video's file name (SubtitleSource). If there's a match, the subtitle file is downloaded and extracted to the video folder. If needed, the subtitle file is also renamed to match the video file.

Configuration:
MediaBrowser Configuration Tool can be used to set preferred languages. Currently supported languages are:
-English
-Swedish
-Danish
-Norwegian
-Finnish
-Icelandic
-Spanish
-French
-Dutch

For example to make finnish the top priority language and english the fallback, use this string: Finnish,English

Known issues:
-

Future releases and features:
1. New ways to interact with the plugin
2. Extended configuration settings

Installation location: Install through the MediaBrowser Configuration

Guide: http://community.mediabrowser.tv/permalinks/1311/how-do-i-configure-and-use-the-subtitleprovider-plugin

Source code: http://github.com/mikoskinen/SubtitleProvider/

SubtitleProvider for MediaBrowser uses ZipStorer-library: http://zipstorer.codeplex.com/
SubtitleProvider for MediaBrowser uses XML-RPC.NET -library: http://www.xml-rpc.net/

Subtitles service allowed by http://www.OpenSubtitles.org