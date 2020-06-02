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
            SudokuGrid grid = new SudokuGrid("003700050070050800100006004502000000800904006000000902300500007004090060020007400");
            Console.WriteLine(grid.ToStringFormatted());
        }
    }
}
