using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Light.Fass
{
    public class FileModule
    {
        private readonly string fileDirectory;

        private readonly HashSet<string> allowExts;

        private readonly bool all;

        public FileModule(FileSetting fileSetting)
        {
            fileDirectory = string.IsNullOrEmpty(fileSetting.FileDirectory) ? "file" : fileSetting.FileDirectory;
            var dir = new DirectoryInfo(fileDirectory);
            if (!dir.Exists) {
                dir.Create();
            }
            allowExts = new HashSet<string>();
            foreach (var item in fileSetting.AllowExts) {
                if (item == "*") {
                    all = true;
                }
                else {
                    allowExts.Add(item.Trim().ToLower());
                }
            }
        }

        public bool AllowExt(string ext)
        {
            if (all || allowExts.Count == 0) {
                return true;
            }
            return allowExts.Contains(ext);
        }

        public async Task SaveFileAsync(IFormFile file, string fileName)
        {
            var path = Path.Combine(fileDirectory, fileName);
            using (var stream = new FileStream(path, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }
        }

        public FileInfo LoadFile(string fileName)
        {
            var path = Path.Combine(fileDirectory, fileName);
            return new FileInfo(path);
        }

        //public bool DeleteFile(string fileName)
        //{
        //    var path = Path.Combine(fileDirectory, fileName);
        //    var fileInfo = new FileInfo(path);
        //    if (fileInfo.Exists) {
        //        fileInfo.Delete();
        //        return true;
        //    }
        //    return false;
        //}
    }
}
