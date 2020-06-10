using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RecordPRO.Models;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using RecordPRO.Config;
using RecordPRO.Utils;
using System.Text;
using RecordPRO.Services;

namespace RecordPRO
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //注册Swagger服务
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Record Pro API", Version = "v1", Description = "A RESTful API for RecordPro,                  Token For Testing:                      eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJSZWNvcmRQcm9BUEkiLCJpYXQiOiIyMDIwLzUvMjIgMjE6NDc6MjgiLCJuYW1lIjoieXloIiwiaWQiOjE4fQ.3ExJ8FmtYVRB1S3M9aPcVYq1tmiuVKpe1CYXeske_oI", Contact = new OpenApiContact { Name = "Contact Us On GitHub", Url = new Uri("https://github.com/StevenEzio/RecordPro") } });
                //c.OperationFilter<CustomHeaderSwaggerAttribute>();
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            //指定使用云数据库
            services.AddDbContext<RecordPROContext>(opt =>
                opt.UseMySql("Server=yuhyeung.mysql.rds.aliyuncs.com;User=yuh;Password=ezio0124;Database=RecordPro", mySqlOptions => mySqlOptions.ServerVersion(new Version(5,7),ServerType.MySql)));
            services.AddSingleton<IUtils, UtilsImpl>();
            services.AddSingleton<IServices, ServicesImpl>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
