using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Filesystem;
using MediaBrowser.LibraryManagement;
using FileInfo=System.IO.FileInfo;

namespace SubtitleProvider.ExtensionMethods
{
    public static class DirectoryInfoExtensions
    {
        public static FileInfo[] GetFiles(this DirectoryInfo dirInfo, DirectoryInfo dir, string searchPatterns, params char[] separator)
        {
            var files = new List<FileInfo>();
            string[] patterns = searchPatterns.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pattern in patterns)
            {
                files.AddRange(dir.GetFiles(pattern));
            }

            return files.ToArray();
        }
    }

    public static class VideoExtensions
    {
        public static List<string> GetPossibleReleaseNames(this Video video)
        {
            var possibleReleaseNames = new List<string>();

            // Release name from file name
            var fileName = Path.GetFileNameWithoutExtension(video.GetVideoFileName());
            possibleReleaseNames.Add(fileName);

            // File name with and without dots
            possibleReleaseNames.Add(fileName.Replace(".", " "));
            possibleReleaseNames.Add(fileName.Replace(" ", "."));

            // Directory name
            var directoryName = video.MediaLocation.Name;
            possibleReleaseNames.Add(directoryName);

            // Directory with and without dots
            possibleReleaseNames.Add(directoryName.Replace(".", " "));
            possibleReleaseNames.Add(directoryName.Replace(" ", "."));

            return possibleReleaseNames;
        }

        public static string GetVideoFileName(this Video video)
        {
            foreach (var videoFile in video.VideoFiles)
            {
                return videoFile;
            }

            return "";

        }

        public static string GetMediaFolder(this Video video)
        {
            if (Helper.IsFolder(video.MediaLocation.Path))
            {
                return video.MediaLocation.Path;
            }

            //if (video.MediaLocation is VirtualFolderMediaLocation)
            //{
                
            //}

            return Path.GetDirectoryName(video.Path);
        }
    }
}
