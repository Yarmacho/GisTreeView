﻿using Database.DI;
using Entities.Entities;
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
