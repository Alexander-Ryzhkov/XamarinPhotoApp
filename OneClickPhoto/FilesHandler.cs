using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OneClickPhoto
{
    public static class FilesHandler
    {
        public static string appFolderName = "OneClickPhoto";
        public static string appFolderPath;
        public static string[] appDirectories;

        public static void UpdateAppFolderPath()
        {
            string externalStorageDirectory = GetExternalStorageDirectory();
            appFolderPath = System.IO.Path.Combine(externalStorageDirectory, appFolderName);
            if (!Directory.Exists(appFolderPath))
                Directory.CreateDirectory(appFolderPath);
        }

        public static void UpdateAppDirectories()
        {
            appDirectories = Directory.GetDirectories(appFolderPath);
        }

        private static string GetExternalStorageDirectory()
        {
            string externalStorageDirectory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            return externalStorageDirectory;
        }
    }
}