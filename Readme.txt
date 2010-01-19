SubtitleProvider for MediaBrowser

About:
SubtitleProvider for MediaBrowser is a plugin that automatically downloads the missing subtitle files for your movies and tv shows.

Notice:
The plugin is currently in BETA-stage. Please read the known issues. Use at your own risk (and submit bug reports if possible).

Current version:
BETA 2

How to use:
Use MediaBrowser's Configuration Wizard to add new plugin source and point it here: http://www.ohjelmistot.org/SubtitleProv ... n_info.xml 

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

For example to make finnish the top priority language and english the fallback, use this string: Finnish,English

Known issues:
-Doesn't work with multi-part video files Fixed in Beta 2
-Not optimized at all Somewhat fixed in Beta 2. More to come
-No plugin.xml available at the current time so the plugin must be manually downloaded Fixed, new installation link available in this post
-There's no gui-support whatsoever. Should there be? 

Future releases and features:
1. Multi-part video support Supported in Beta 2
2. More subtitle sources OpenSubtitles.org added in Beta 2
3. New ways to match the video files to subtitles (hashing?) Supported in Beta 2

Installation location: http://www.ohjelmistot.org/SubtitleProv ... n_info.xml

Source code: http://github.com/mikoskinen/SubtitleProvider/

SubtitleProvider for MediaBrowser uses ZipStorer-library: http://zipstorer.codeplex.com/
SubtitleProvider for MediaBrowser uses XML-RPC.NET -library: http://www.xml-rpc.net/

Subtitles service allowed by http://www.OpenSubtitles.org