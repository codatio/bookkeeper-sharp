using Codat.Bookkeeper.DataAccess;
using Codat.Bookkeeper.Middleware;
using Codat.Bookkeeper.Models;
using Codat.Bookkeeper.Validator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace Codat.Bookkeeper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddAuthorization();

            services.AddControllers();

            services.AddSingleton<RequestLogger>();

            var dbDir = Path.Combine(Environment.CurrentDirectory, "db");
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
            var dbPath = Path.Combine(dbDir, "bookkeeperapi.db");
            services.AddDbContext<BookkeeperDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

            services.AddSingleton<IValidator<Invoice>>(new InvoiceValidator());
            services.AddSingleton<IValidator<(Invoice,Payment)>>(new PaymentValidator());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bookkeeper", Version = "v1" });
            });

            services.AddCors(o => o.AddDefaultPolicy(builder => { builder.AllowAnyOrigin(); }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseMiddleware<RequestLogger>();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookkeeper"); });
        }
    }
}