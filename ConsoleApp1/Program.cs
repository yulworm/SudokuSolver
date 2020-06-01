using System;
using SudokuSolver;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SudokuGrid grid = new SudokuGrid("000000120240010000901004000400003650000090000036400001000100506000050043072000000");
            grid = SudokuHelper.solve_grid(grid);
            Console.WriteLine(grid);
        }
    }
}
