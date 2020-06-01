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
            string puzzle = "019600005|607840910|840219307|328196070|076300098|001500623||053002080|060458731|704031050";
            SudokuGrid grid = new SudokuGrid(puzzle);

            Console.WriteLine(grid.ToStringFormatted());

            Console.WriteLine(SudokuSolverClasses.SudokuHelper.solve_grid(grid).ToStringFormatted());

        }
    }
}
