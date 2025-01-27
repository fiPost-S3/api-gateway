using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api_gateway.Controllers;
using api_gateway.Helper;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace api_gateway
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        private const string SECRET_KEY = "this is my custom Secret key for authnetication";
        public static readonly SymmetricSecurityKey SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.Configure<KestrelServerOptions>(
            Configuration.GetSection("Kestrel"));

            Constants.PackageApiUrl = Configuration.GetValue<string>("PackageApiUrl");
            Constants.LocationApiUrl = Configuration.GetValue<string>("LocationApiUrl");
            Constants.PersonApiUrl = Configuration.GetValue<string>("PersonApiUrl");

            services.AddControllers();

            FlurlHttp.Configure(settings => settings.AllowedHttpStatusRange = "400-504,6xx");

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("*")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                      });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
             .AddJwtBearer("JwtBearer", jwtOptions =>
             {
                 jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                 {
                     IssuerSigningKey = SIGNING_KEY,
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateIssuerSigningKey = false,
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.FromMinutes(5)
                 };
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                /*
                if (env.IsDevelopment())
                {
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                }
                else
                {*/
                    endpoints.MapControllers();
                //}
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API-Gateway");
                // Serve the swagger UI at the app's root
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
