using Database.DI;
using DynamicForms.DependencyInjection;
using DynamicForms.Factories;
using Interfaces.Database.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tools;

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

            //Application.Run(ServiceProvider.GetRequiredService<Form1>());

            var point1 = new MapWinGIS.Point();
            point1.Set(1, 0);

            var point2 = new MapWinGIS.Point();
            point2.Set(2, 1);

            Debug.WriteLine(string.Join("\n", LineTools.EnumeratePointsInLine(point2, point1).Select(p => $"({p.x}, {p.y})")));
            
            Debugger.Break();
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
