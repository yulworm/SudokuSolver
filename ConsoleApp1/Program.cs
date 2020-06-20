using System;
using SudokuSolver;
using System.Collections.Generic;
using System.Linq;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using NLog.Extensions.Logging;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var services = new ServiceCollection();
            //ConfigureServices(services);

            //using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            //{
            //    MyApplication app = serviceProvider.GetService<MyApplication>();
            //    app.Run();
            //}

            //SudokuGrid grid = new SudokuGrid("010809300000506028007003001300601804004200500070090060700048000620130000081000950");
            //Console.WriteLine(grid.ToStringFormatted());
            //SudokuHelper.solve_grid(grid);
            //Console.WriteLine(grid.ToStringFormatted());
            SudokuGrid grid = new SudokuGrid("000000000|000000000|000000002||000000000|560000430|000000000||000000000|002000000|000000000");
            grid.set_possible_values_of_all_cells();
            HashSet<(int, int, int)> results = SudokuHelper.find_block_block_interactions(grid._grid_cells);
            foreach((int x, int y, int val) in results)
            {
                Console.WriteLine($"excluded {val} at ({x},{y})");
            }
                //"000000000|000000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000",
        }

        //private static void ConfigureServices(ServiceCollection services)
        //{
        //    services.AddTransient<MyApplication>();

        //    services.AddLogging(builder =>
        //    {
        //        builder.SetMinimumLevel(LogLevel.Information);
        //        builder.AddNLog("nlog.config");
        //    });

        //}
    }

        
}
