using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TestWebApp.Application;
using TestWebApp.Application.Configurations;
using TestWebApp.Converters;
using TestWebApp.Filters;
using TestWebApp.Infrastructure;
using TestWebApp.Interfaces;
using TestWebApp.Middlewares;

namespace TestWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables();

            Serilog.Debugging.SelfLog.Enable(Console.Error);
            Configuration = builder.Build();
        }

        public IWebHostEnvironment HostingEnvironment;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // setting rolling interval to minute, because of the testing should not take much time
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .WriteTo.File(
                    @"../logs/",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Environment} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Minute,
                    flushToDiskInterval: TimeSpan.FromMinutes(1),
                    retainedFileCountLimit: 1000)
                .Enrich.WithProperty("Environment", HostingEnvironment.EnvironmentName)
                .CreateLogger();

            services.AddDbContext<TestWebAppDbContext>(
                x =>
                {
                    x.UseSqlServer(Configuration.GetConnectionString(nameof(TestWebAppDbContext)));
                    x.UseLazyLoadingProxies();
                });

            services.AddScoped(typeof(GenericRepository<>));
            services.AddScoped<UnitOfWork>();

            services.AddScoped<IFileService, FileService>();

            services.Configure<FileServiceOptions>(Configuration.GetSection(nameof(FileServiceOptions)));

            services.AddLocalization();

            services.AddFluentValidation(x =>
            {
                x.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.AddControllers(x =>
            {
                x.Filters.Add<CustomActionFilter>();
            })
            .AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                x.SerializerSettings.Converters.Add(new RelationTypeJsonConverter());
                x.SerializerSettings.Converters.Add(new NullableRelationTypeJsonConverter());
            });

            services.AddProblemDetails(x =>
            {
                x.IncludeExceptionDetails = (context, ex) =>
                {
                    return HostingEnvironment.IsDevelopment();
                };
            });

            InitEnvironment(services, true).Wait();

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseProblemDetails();
            app.UseStaticFiles();
            app.UseSwagger().UseSwaggerUI();

            app.UseRequestLocalization(x =>
            {
                x.DefaultRequestCulture = new RequestCulture("ka-GE");
                x.SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("ka-GE"),
                    new CultureInfo("en-US")
                };
                x.SupportedUICultures = x.SupportedCultures;
                x.SetDefaultCulture("ka-GE");
                x.AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider());
            });

            app.UseProblemDetails();

            app.UseMiddleware<LoggingMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        public async Task InitEnvironment(IServiceCollection service, bool seed = false)
        {
            //if (!HostingEnvironment.IsDevelopment())
            //{
            //    using (var scope = service.BuildServiceProvider().CreateScope())
            //    {
            //        var services = scope.ServiceProvider;
            //        var dbContext = services.GetRequiredService<TestWebAppDbContext>();
            //        var seeder = new DbDataSeeder();
            //        await seeder.SeedData(dbContext);
            //    }
            //}

            //if (seed && HostingEnvironment.IsDevelopment())
            //{
            //    using (var scope = service.BuildServiceProvider().CreateScope())
            //    {
            //        var services = scope.ServiceProvider;
            //        var dbContext = services.GetRequiredService<TestWebAppDbContext>();
            //        var seeder = new DbDataSeeder();
            //        await seeder.SeedData(dbContext);
            //    }
            //}
        }
    }
}
