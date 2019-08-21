using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [Route("file")]
    [ApiController]
    public class FileController : Controller
    {
        const string FILE_NAME_REGEX = @"^(?<date>\d{14})(?<protect>\d{1})(?<other>\d{3})\d{4}(?<ext>\.[0-9a-zA-Z]+)$";

        private readonly AuthModule authModule;
        private readonly FileModule fileModule;
        private readonly ThumbnailModule thumbnailModule;
        private readonly MimeModule mimeModule;
        private readonly CacheModule cacheModule;
        private readonly ILogger logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authModule"></param>
        /// <param name="fileModule"></param>
        /// <param name="thumbnailModule"></param>
        /// <param name="mimeModule"></param>
        /// <param name="cacheModule"></param>
        /// <param name="loggerFactory"></param>
        public FileController(AuthModule authModule, FileModule fileModule, ThumbnailModule thumbnailModule, MimeModule mimeModule, CacheModule cacheModule, ILoggerFactory loggerFactory)
        {
            this.authModule = authModule;
            this.fileModule = fileModule;
            this.thumbnailModule = thumbnailModule;
            this.mimeModule = mimeModule;
            this.cacheModule = cacheModule;
            this.logger = loggerFactory.CreateLogger("File");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="user"></param>
        /// <param name="thumbnail"></param>
        /// <param name="expired"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{filename}")]
        public async Task<ActionResult> GetFileAsync(string filename, [FromQuery(Name = "u")]string user, [FromQuery(Name = "t")]string thumbnail, [FromQuery(Name = "e")]long? expired, [FromQuery(Name = "token")]string token)
        {
            var regex = new Regex(FILE_NAME_REGEX, RegexOptions.Compiled);
            var match = regex.Match(filename);
            if (!match.Success) {
                throw new RequestException("The file name is error");
            }
            var protect = match.Groups["protect"].Value == "1";
            var ext = match.Groups["ext"].Value;
            if (protect) {
                if (expired == null || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(user)) {
                    throw new AuthException("The validation variable error");
                }
                if (!authModule.ValidAccessToken(user, filename, expired.Value, token)) {
                    throw new AuthException("The validation is failed");
                }
            }
            if (!fileModule.AllowExt(ext)) {
                throw new RequestException("The file type is not allowed");
            }

            var file = fileModule.LoadFile(filename);
            if (!file.Exists) {
                throw new NotFoundException("The file does not exist");
            }
            if (!string.IsNullOrEmpty(thumbnail)) {
                if (thumbnailModule.TryGetAndSetThumbnailFileInfo(file, thumbnail.ToUpper(), out FileInfo thumbnailInfo)) {
                    file = thumbnailInfo;
                }
            }
            var contentType = mimeModule.GetMime(file.FullName);
            var buffer = await cacheModule.GetAndSetFileCache(file);
            logger.LogDebug($"access|{user}|{filename}|{file.Length}|{file.Name}");
            if (buffer != null) {
                return File(buffer, contentType);
            }
            return File(file.OpenRead(), contentType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="user"></param>
        /// <param name="timestamp"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete("{filename}")]
        public ActionResult<GeneralResult> DelateFile(string filename, [FromQuery(Name = "u")]string user, [FromQuery(Name = "t")]long timestamp, [FromQuery(Name = "token")]string token)
        {
            var regex = new Regex(FILE_NAME_REGEX, RegexOptions.Compiled);
            var match = regex.Match(filename);
            if (!match.Success) {
                throw new RequestException("The file name is error");
            }
            if (!authModule.ValidDeleteToken(user, filename, timestamp, token)) {
                throw new AuthException("The validation is failed");
            }
            var file = fileModule.LoadFile(filename);
            if (file.Exists) {
                file.Delete();
                cacheModule.RemoveFileCache(file.Name);
                var files = thumbnailModule.GetFiles(filename);
                foreach (var item in files) {
                    item.Delete();
                    cacheModule.RemoveFileCache(item.Name);
                }
                logger.LogInformation($"delete|{user}|{filename}|{file.Length}|");
                return new GeneralResult() { Message = "ok" };
            }
            else {
                throw new NotFoundException("The file does not exist");
            }
        }
    }
}
