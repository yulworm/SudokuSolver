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

            string puzzle = "000000120240010000901004000400003650000090000036400001000100506000050043072000000";
            //string puzzle = "003700050070050800100006004502000000800904006000000902300500007004090060020007400";
            SudokuGrid grid = new SudokuGrid(puzzle);
            //Console.WriteLine(grid.ToStringFormatted());
            grid.set_possible_values_of_all_cells();
            //grid.display_all_possible_values();
            SudokuHelper.solve_grid(grid);
            //Console.WriteLine(grid.ToStringFormatted());

            //Console.WriteLine(grid.ToStringFormatted());
            //SudokuGrid grid = new SudokuGrid("000000000|000000000|000000002||000000000|560000430|000000000||000000000|002000000|000000000");


            //SudokuGrid grid = new SudokuGrid("003000000|000425000|000001000||000904000|030000000|000076000||000000000|000000000|000000000");
            //SudokuHelper.solve_grid(grid);

            //grid.set_possible_values_of_all_cells();
            //Console.WriteLine(grid.ToStringFormatted());

            //HashSet<(int, int, int)> results = SudokuHelper.find_block_block_interactions(grid._grid_cells);
            //Console.WriteLine($"# results= {results.Count}, results={SudokuHelper.format_coord_and_value_hashset(results)};");
            //Console.WriteLine(grid.ToStringFormatted());
            //grid.display_all_possible_values();
            //grid.set_possible_values_of_all_cells();
            //HashSet<(int, int, int)> results = SudokuHelper.find_block_block_interactions(grid._grid_cells);
            //foreach((int x, int y, int val) in results)
            //{
            //    Console.WriteLine($"excluded {val} at ({x},{y})");
            //}
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
