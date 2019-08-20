using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Light.Fass.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("upload")]
    [ApiController]
    public class UploadController : Controller
    {
        private readonly AuthModule authModule;
        private readonly FileModule fileModule;
        private readonly CacheModule cacheModule;
        private readonly ThumbnailModule thumbnailModule;
        private readonly ILogger logger;

        static readonly char[] ForbitChars = { '|', '\r', '\n' };

        public UploadController(AuthModule authModule, FileModule fileModule, CacheModule cacheModule, ThumbnailModule thumbnailModule, ILoggerFactory loggerFactory)
        {
            this.authModule = authModule;
            this.fileModule = fileModule;
            this.cacheModule = cacheModule;
            this.thumbnailModule = thumbnailModule;
            this.logger = loggerFactory.CreateLogger("File");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="timestamp"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UploadResult>> UploadAsync([Required]IFormFile file, [FromQuery(Name = "p")]int protect, [FromQuery(Name = "u")]string user, [FromQuery(Name = "t")]long timestamp, [FromQuery(Name = "token")][Required]string token)
        {
            if (file == null) {
                throw new RequestException("No any files");
            }
            if (file.Name.IndexOfAny(ForbitChars) > -1) {
                throw new RequestException("The file name error");
            }
            if (!authModule.ValidUploadToken(user, file.FileName, protect, timestamp, token)) {
                throw new AuthException("The validation is failed");
            }
            if (cacheModule.TryGetUploadName(token, out string name)) {
                throw new AuthException($"The token is used, filename is {name}");
            }

            var random = new Random();
            var protectFlag = protect == 1;

            var code = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmss"), protectFlag ? "1" : "0", "000", random.Next(1000, 9999));
            var fileName = file.FileName;
            var index = fileName.LastIndexOf('.');
            if (index <= 0 || fileName.Length < index + 1) {
                throw new RequestException("The file has no extension");
            }
            var ext = fileName.Substring(index).ToLower();
            if (!fileModule.AllowExt(ext)) {
                throw new RequestException("The file type is not allowed");
            }
            var newName = code + ext;
            await fileModule.SaveFileAsync(file, newName);
            cacheModule.SetUploadName(token, newName);
            logger.LogInformation($"upload|{user}|{newName}|{file.Length}|{file.Name}");
            return new UploadResult() { FileName = newName, Message = "ok" };
        }
    }
}
