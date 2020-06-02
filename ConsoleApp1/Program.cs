using System;
using SudokuSolver;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                MyApplication app = serviceProvider.GetService<MyApplication>();
                app.Run();
            }

            SudokuGrid grid = new SudokuGrid("010809300000506028007003001300601804004200500070090060700048000620130000081000950");
            Console.WriteLine(grid.ToStringFormatted());
            SudokuHelper.solve_grid(grid);
            Console.WriteLine(grid.ToStringFormatted());
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MyApplication>();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog("nlog.config");
            });

        }
    }

        
}
