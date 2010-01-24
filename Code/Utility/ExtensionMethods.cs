using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Filesystem;
using MediaBrowser.LibraryManagement;
using FileInfo = System.IO.FileInfo;

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
        public static int VideoFileCount(this Video video)
        {

            var videoFileCount = 0;

            foreach (var videoFile in video.VideoFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(videoFile);
                if (fileName.ToLower() != "sample" && fileName.ToLower() != "trailer")
                    videoFileCount++;
            }

            return videoFileCount;

        }

        public static List<string> GetPossibleReleaseNames(this Video video)
        {
            var possibleReleaseNames = new List<string>();

            // Release name from file name
            var fileName = Path.GetFileNameWithoutExtension(video.GetVideoFileName());
            possibleReleaseNames.Add(fileName);

            // File name with and without dots
            var fileNameWithoutDots = fileName.Replace(".", " ");
            if (!possibleReleaseNames.Contains(fileNameWithoutDots))
                possibleReleaseNames.Add(fileNameWithoutDots);

            var fileNameWithDots = fileName.Replace(" ", ".");
            if (!possibleReleaseNames.Contains(fileNameWithDots))
                possibleReleaseNames.Add(fileNameWithDots);

            // Directory name
            var directoryName = video.MediaLocation.Name;
            if (!possibleReleaseNames.Contains(directoryName))
                possibleReleaseNames.Add(directoryName);

            // Directory with and without dots
            var dirNameWithoutDots = directoryName.Replace(".", " ");
            if (!possibleReleaseNames.Contains(dirNameWithoutDots))
                possibleReleaseNames.Add(dirNameWithoutDots);

            var dirNameWithDots = directoryName.Replace(" ", ".");
            if (!possibleReleaseNames.Contains(dirNameWithDots))
                possibleReleaseNames.Add(dirNameWithDots);

            // CD-number removed from the filename
            var fileNameInLowerCase = fileName.ToLower();
            if (fileNameInLowerCase.IndexOf("cd1") > 0)
            {
                var fileNameWithoutCdNumber = fileNameInLowerCase.Substring(0, fileNameInLowerCase.IndexOf("cd1") - 1);
                if (!possibleReleaseNames.Contains(fileNameWithoutCdNumber))
                    possibleReleaseNames.Add(fileNameWithoutCdNumber);
            }

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

        public static List<string> GetVideoFileNames(this Video video)
        {
            var fileNames = new List<string>();
            foreach (var videoFile in video.VideoFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(videoFile);
                if (fileName.ToLower() != "sample" && fileName.ToLower() != "trailer")
                    fileNames.Add(videoFile);
            }

            return fileNames;
        }

        public static string GetMediaFolder(this Video video)
        {
            if (Helper.IsFolder(video.MediaLocation.Path))
            {
                return video.MediaLocation.Path;
            }

            return Path.GetDirectoryName(video.Path);
        }

        public static List<string> GetMediaFolders(this Video video)
        {

            var folders = new List<string>();

            foreach (var videoFile in video.VideoFiles)
            {
                var folder = Path.GetDirectoryName(videoFile);

                folders.Add(folder);
            }

            return folders;
        }

        public static string GetVideoHashString(this Video video)
        {
            var fileName = video.GetVideoFileName();

            byte[] result;
            using (Stream input = File.OpenRead(fileName))
            {
                result = ComputeMovieHash(input);
            }
            return result.ToHex();
        }

        public static long GetVideoSize(this Video video)
        {
            var fileName = video.GetVideoFileName();

            var fi = new FileInfo(fileName);

            return fi.Length;

        }

        public static string ToHex(this byte[] byteArray)
        {

            char[] c = new char[byteArray.Length * 2];
            byte b;
            for (int i = 0; i < byteArray.Length; ++i)
            {
                b = ((byte)(byteArray[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(byteArray[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }

            return new string(c);

        }

        public static string BuildString<T>(this IEnumerable<T> self, string delim)
        {
            return string.Join(",", self.Select(x => x.ToString()).ToArray());
        }

        #region Hash calculation helper methods

        private static byte[] ComputeMovieHash(Stream input)
        {
            long streamsize = input.Length;
            long lhash = streamsize;

            long i = 0;
            var buffer = new byte[sizeof(long)];
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }

            input.Position = Math.Max(0, streamsize - 65536);
            i = 0;
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }
            input.Close();
            var result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }

        private static string ToHexadecimal(byte[] bytes)
        {
            var hexBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return hexBuilder.ToString();
        }
        #endregion
    }
}
