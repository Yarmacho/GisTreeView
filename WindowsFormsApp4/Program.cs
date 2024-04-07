using Database.DI;
using DynamicForms;
using DynamicForms.DependencyInjection;
using DynamicForms.Factories;
using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.DependencyInjection;
using GeoDatabase.ORM.Mapper;
using GeoDatabase.ORM.Set.Extensions;
using Interfaces.Database.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            FormFactory.ServiceProvider = ServiceProvider;
            MapDesigner.ServiceProvider = ServiceProvider;
            ServiceProvider.GetRequiredService<IDbManager>()
                .CreateAsync().GetAwaiter().GetResult();

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
                    services.AddTransient<Form1>();
                    services.AddSingleton(Configuration);
                    services.AddShapeConverters();
                    services.AddDataBase(Configuration);
                    services.AddGeoDataBase(Configuration.GetValue<string>("MapsPath"));
                    services.AddMappings(typeof(Program).Assembly, Configuration.GetValue<string>("MapsPath"));
                });
        }
    }
}
