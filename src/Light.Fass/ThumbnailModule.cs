using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class ThumbnailModule
    {
        class Options
        {
            public string Ext
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }

            public int Width
            {
                get;
                set;
            }

            public string Mode
            {
                get;
                set;
            }

            public int Flag
            {
                get;
                set;
            }
        }

        private readonly string fileDirectory;

        private readonly Dictionary<string, ImageCodecInfo> codecDict;

        private readonly Dictionary<string, Options> optionsDict;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thumbnailSetting"></param>
        public ThumbnailModule(ThumbnailSetting thumbnailSetting)
        {
            if (!string.IsNullOrEmpty(thumbnailSetting.ThumbnailDirectory)) {
                fileDirectory = thumbnailSetting.ThumbnailDirectory;
            }
            else {
                var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                fileDirectory = Path.Combine(basePath, "thumbnail");
            }
            var dir = new DirectoryInfo(fileDirectory);
            if (!dir.Exists) {
                dir.Create();
            }

            var codecInfos = ImageCodecInfo.GetImageEncoders();//获取图像编码器的信息
            codecDict = new Dictionary<string, ImageCodecInfo>();
            if (thumbnailSetting.ThumbnailExts != null) {
                foreach (var item in thumbnailSetting.ThumbnailExts) {
                    var ext = item.Trim().ToLower();
                    foreach (var info in codecInfos) {
                        var arr = info.FilenameExtension.Split(';');
                        var fe = "*" + ext.ToUpper();
                        if (arr.Contains(fe)) {
                            codecDict[ext] = info;
                            break;
                        }
                    }
                }
            }
            var op = new Options {
                Ext = "_s",
                Width = 320,
                Height = 240,
                Mode = "HW",
                Flag = 100
            };
            optionsDict = new Dictionary<string, Options>() {
                { "S", op }
            };
            if (thumbnailSetting.Types != null) {
                foreach (var item in thumbnailSetting.Types) {
                    var option = new Options();
                    if (string.IsNullOrWhiteSpace(item.Type)) {
                        continue;
                    }
                    var type = item.Type.ToUpper();
                    option.Ext = "_" + type;
                    if (item.Width != null && item.Width > 0) {
                        option.Width = item.Width.Value;
                    }
                    if (item.Height != null && item.Height > 0) {
                        option.Height = item.Height.Value;
                    }
                    if (option.Width <= 0 && option.Height <= 0) {
                        option.Width = 320;
                    }
                    if (string.IsNullOrEmpty(option.Mode)) {
                        option.Mode = "F";
                    }
                    if (item.Flag != null && item.Flag > 0 && item.Flag <= 100) {
                        option.Flag = item.Flag.Value;
                    }
                    else {
                        option.Flag = 100;
                    }
                    optionsDict[type] = option;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="type"></param>
        /// <param name="thumbnailInfo"></param>
        /// <returns></returns>
        public bool TryGetAndSetThumbnailFileInfo(FileInfo fileInfo, string type, out FileInfo thumbnailInfo)
        {
            thumbnailInfo = null;
            if (!optionsDict.TryGetValue(type, out Options options)) {
                return false;
            }
            if (!codecDict.TryGetValue(fileInfo.Extension, out ImageCodecInfo codecInfo)) {
                return false;
            }
            var fileName = fileInfo.Name;
            var targetName = fileName.Insert(fileName.LastIndexOf('.'), options.Ext);
            var targetPath = Path.Combine(fileDirectory, targetName);
            var targetInfo = new FileInfo(targetPath);

            if (!targetInfo.Exists) {
                var result = MakeThumbnail(fileInfo.FullName, targetInfo.FullName, codecInfo, options.Width, options.Height, options.Mode, options.Flag);
                if (!result) {
                    targetInfo.Create();
                    return false;
                }
                else {
                    targetInfo.Refresh();
                    thumbnailInfo = targetInfo;
                }
            }
            else {
                if (targetInfo.Length == 0) {
                    return false;
                }
                else {
                    thumbnailInfo = targetInfo;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public FileInfo[] GetFiles(string fileName)
        {
            var index = fileName.LastIndexOf('.');
            if (index > 0) {
                fileName = fileName.Substring(0, index);
            }
            var dir = new DirectoryInfo(fileDirectory);
            var files = dir.GetFiles(fileName + "_*");
            return files;
        }

        private bool MakeThumbnail(string sourcePath, string targetPath, ImageCodecInfo codecInfo, int width, int height, string mode, int flag)
        {
            Image sourceImage = null;
            Image bitmap = null;
            Graphics g = null;
            EncoderParameters ep = null;
            EncoderParameter eParam = null;
            try {
                sourceImage = Image.FromFile(sourcePath);
                bool land = sourceImage.Width >= sourceImage.Height;

                int toWidth = 0;
                int toHeight = 0;

                if (width > 0) {
                    toWidth = width;
                }
                else {
                    toWidth = sourceImage.Width;
                }

                if (height > 0) {
                    toHeight = height;
                }
                else {
                    toHeight = sourceImage.Height;
                }


                int x = 0;
                int y = 0;
                int ow = sourceImage.Width;
                int oh = sourceImage.Height;

                if (width > 0 && height > 0 && !string.IsNullOrWhiteSpace(mode)) {
                    switch (mode.ToUpper()) {
                        case "HW"://指定高宽缩放（不变形）
                            int tempheight = sourceImage.Height * width / sourceImage.Width;
                            if (tempheight > height) {
                                toWidth = sourceImage.Width * height / sourceImage.Height;
                            }
                            else {
                                toHeight = sourceImage.Height * width / sourceImage.Width;
                            }
                            break;
                        case "W"://指定宽，高按比例                   
                            toHeight = sourceImage.Height * width / sourceImage.Width;
                            break;
                        case "H"://指定高，宽按比例
                            toWidth = sourceImage.Width * height / sourceImage.Height;
                            break;
                        case "F"://指定高，宽按比例
                            if (land) {
                                if (width > sourceImage.Width) {
                                    return false;
                                }
                                toWidth = width;
                                toHeight = sourceImage.Height * width / sourceImage.Width;
                            }
                            else {
                                if (width > sourceImage.Height) {
                                    return false;
                                }
                                toHeight = width;
                                toWidth = sourceImage.Width * width / sourceImage.Height;
                            }
                            break;
                    }
                }

                //新建一个bmp图片
                bitmap = new Bitmap(toWidth, toHeight);

                //新建一个画板
                g = Graphics.FromImage(bitmap);

                g.CompositingQuality = CompositingQuality.HighQuality;

                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清空画布并以透明背景色填充
                g.Clear(Color.Transparent);

                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(sourceImage, new Rectangle(0, 0, toWidth, toHeight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);

                //以下代码为保存图片时，设置压缩质量
                ep = new EncoderParameters();
                eParam = new EncoderParameter(Encoder.Quality, flag);
                ep.Param[0] = eParam;
                bitmap.Save(targetPath, codecInfo, ep);
                return true;
            }
            catch {
                return false;
            }
            finally {
                if (sourceImage != null) {
                    sourceImage.Dispose();
                }
                if (bitmap != null) {
                    bitmap.Dispose();
                }
                if (g != null) {
                    g.Dispose();
                }
                if (ep != null) {
                    ep.Dispose();
                }
                if (eParam != null) {
                    eParam.Dispose();
                }
            }
        }
    }
}
