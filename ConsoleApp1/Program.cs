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
            SudokuGrid grid = new SudokuGrid("010809300000506028007003001300601804004200500070090060700048000620130000081000950");
            Console.WriteLine(grid.ToStringFormatted());
            SudokuHelper.solve_grid(grid);
            Console.WriteLine(grid.ToStringFormatted());
        }
    }
}
