using Database.DI;
using DynamicForms;
using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.DependencyInjection;
using Interfaces.Database.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.ShapeConverters;

namespace WindowsFormsApp4
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;
            MapInitializer.ShapesPath = Configuration.GetValue<string>("MapsPath");
            MapDesigner.ServiceProvider = host.Services;

            ServiceProvider.GetRequiredService<IDbManager>()
                .CreateAsync().GetAwaiter().GetResult();
            //ServiceProvider.GetRequiredService<GeoDbContext>()
            //    .DeleteAllShapes();

            Application.Run(ServiceProvider.GetRequiredService<Form1>());
        }

        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        static IHostBuilder CreateHostBuilder()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => 
                {
                    var mapPath = Configuration.GetValue<string>("MapsPath");

                    services.AddTransient<Form1>();
                    services.AddSingleton(Configuration);
                    services.AddShapeConverters();
                    services.AddDataBase(Configuration);
                    services.AddMappings(typeof(Program).Assembly, mapPath);
                    services.AddGeoDataBase(mapPath);
                });
        }

        public static IServiceCollection AddShapeConverters(this IServiceCollection services)
        {
            services.AddTransient<IShapeEntityConverter<Gas>, ShapeToGasConverter>();
            services.AddTransient<IShapeEntityConverter<Scene>, ShapeToSceneConverter>();
            services.AddTransient<IShapeEntityConverter<Ship>, ShapeToShipConverter>();
            services.AddTransient<IShapeEntityConverter<Route>, ShapeToRouteConverter>();

            return services;
        }
    }
}
