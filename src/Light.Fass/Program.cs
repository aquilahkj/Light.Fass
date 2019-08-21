using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the web host builder.
        /// </summary>
        /// <returns>The web host builder.</returns>
        /// <param name="args">Arguments.</param>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            var builder = WebHost.CreateDefaultBuilder(args)
                          .UseConfiguration(config)
                          .UseStartup<Startup>()
                          .UseKestrel(options => {
                              var size = Environment.GetEnvironmentVariable("MAX_UPLOAD_SIZE");
                              if (string.IsNullOrEmpty(size)) {
                                  size = config["MaxUploadSize"];
                              }
                              if (size != null) {
                                  if (Utils.TryParseByteNumber(size, out long value)) {
                                      options.Limits.MaxRequestBodySize = value;
                                  }
                                  else {
                                      throw new ConfigurationException("MaxUploadSize Variable Error");
                                  }
                              }
                          });
            return builder;
        }
    }
}
