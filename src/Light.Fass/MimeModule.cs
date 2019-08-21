using Microsoft.AspNetCore.StaticFiles;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class MimeModule
    {
        private readonly FileExtensionContentTypeProvider contentTypeProvider;

        private readonly string defaultMime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mimeSetting"></param>
        public MimeModule(MimeSetting mimeSetting)
        {
            this.contentTypeProvider = new FileExtensionContentTypeProvider();
            if (mimeSetting.MimeInfos != null) {
                foreach(var item in mimeSetting.MimeInfos) {
                    contentTypeProvider.Mappings[item.Ext] = item.Mime;
                }
            }
            if (!string.IsNullOrEmpty(mimeSetting.DefaultMime)) {
                defaultMime = mimeSetting.DefaultMime;
            }
            else {
                defaultMime = "application/octet-stream";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetMime(string fileName)
        {
            if (!contentTypeProvider.TryGetContentType(fileName, out string contentType)) {
                contentType = defaultMime;
            }
            return contentType;
        }
    }
}
