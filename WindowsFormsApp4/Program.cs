using Database.DI;
using DynamicForms.DependencyInjection;
using DynamicForms.Factories;
using Interfaces.Database.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
            ServiceProvider.GetRequiredService<IDbManager>()
                .CreateAsync().GetAwaiter().GetResult();
            //ServiceProvider.GetRequiredService<IShapesManager>()
            //    .DeleteAllShapes(Configuration.GetValue<string>("MapsPath"));

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
    }
}
