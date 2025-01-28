﻿using Database.DI;
using DynamicForms;
using Entities.Contracts;
using Entities.Entities;
using Events.Handlers;
using GeoDatabase.ORM.DependencyInjection;
using Interfaces.Database.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Events;
using WindowsFormsApp4.Events.Handlers;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.ShapeConverters;
using GeoDatabase.ORM;

namespace WindowsFormsApp4
{
    public static class Program
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
            BattimetryInterpolator.ShapesPath = Configuration.GetValue<string>("MapsPath");
            MapDesigner.ServiceProvider = host.Services;

            //ServiceProvider.GetRequiredService<IDbManager>()
            //    .ReCreateAsync().GetAwaiter().GetResult();
            //ServiceProvider.GetRequiredService<GeoDbContext>()
            //    .DeleteAllShapes();
            ServiceProvider.GetRequiredService<GeoDbContext>()
                .EnsureShapefilesStructure();


            MainForm = ServiceProvider.GetRequiredService<Form1>();

            var cancellationTokenSource = new CancellationTokenSource();
            try
            {
                InitDispatchEventsScheduler(cancellationTokenSource.Token);
                Application.Run(MainForm);
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        public static Form1 MainForm { get; private set; }
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
                    services.AddMemoryCache();
                    services.AddEventBusWithHandlers();
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

        public static IServiceCollection AddEventBusWithHandlers(this IServiceCollection services)
        {
            services.AddTransient<IEventBus, EventBus>();
            services.AddSingleton<EventsDispather>();
            services.AddTransient<IEventHandler<SceneCreated>, InterpolateBattimetryHandler>();

            return services;
        }

        public static void InitDispatchEventsScheduler(CancellationToken cancellationToken) =>
           Task.Run(async () =>
           {
               var eventsDispatcher = ServiceProvider.GetRequiredService<EventsDispather>();
               await eventsDispatcher.ExecuteAsync(cancellationToken);
           }, cancellationToken);
    }
}
