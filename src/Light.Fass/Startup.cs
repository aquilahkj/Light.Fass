using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace Light.Fass
{
    public class Startup
    {
        Settings settings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            settings = Settings.LoadSettings(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
             .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
             .ConfigureApiBehaviorOptions(options => {
                 options.UseInvalidModelStateException();
             });
            services.AddMvc(x => {
                x.Filters.Add<ExceptionFilter>();
            });
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            if (settings.UseSwagger) {
                services.AddSwaggerGen(c => {
                    c.SwaggerDoc("v1", new Info { Title = "LIGHT FASS MGT API", Version = "v1" });
                    // 为 Swagger JSON and UI设置xml文档注释路径
                    var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                    //获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                    var apiXmlPath = Path.Combine(basePath, "Light.Fass.xml");
                    c.IncludeXmlComments(apiXmlPath);

                });
            }
            services.AddSingleton(new FileModule(settings.FileSetting));
            services.AddSingleton(new AuthModule(settings.AuthSetting));
            services.AddSingleton(new ThumbnailModule(settings.ThumbnailSetting));
            services.AddSingleton(new MimeModule(settings.MimeSetting));
            services.AddSingleton(new CacheModule(settings.CacheSetting));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (settings.UseLog) {
                loggerFactory.AddNLog();
            }
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseMvc();
            //app.UseAuthorizePermissoion();
            if (settings.UseSwagger) {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.ShowExtensions();
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LIGHT FASS API V1");
                });
            }
        }
    }
}
