using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class FileModule
    {
        private readonly string fileDirectory;

        private readonly HashSet<string> allowExts;

        private readonly bool all;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSetting"></param>
        public FileModule(FileSetting fileSetting)
        {
            if (!string.IsNullOrEmpty(fileSetting.FileDirectory)) {
                fileDirectory = fileSetting.FileDirectory;
            }
            else {
                var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                fileDirectory = Path.Combine(basePath, "file");
            }
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public bool AllowExt(string ext)
        {
            if (all || allowExts.Count == 0) {
                return true;
            }
            return allowExts.Contains(ext);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task SaveFileAsync(IFormFile file, string fileName)
        {
            var path = Path.Combine(fileDirectory, fileName);
            using (var stream = new FileStream(path, FileMode.CreateNew)) {
                await file.CopyToAsync(stream);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
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
