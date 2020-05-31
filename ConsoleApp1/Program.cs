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
            SudokuGrid grid = new SudokuGrid("019600005|607840910|840219307|328196070|076300098|001500623||053002080|060458731|704031050");
            grid.set_possible_values_of_all_cells();

            List<int> target_possibles = grid._grid_cells[8, 8]._possible_values;

            foreach (int i in target_possibles)
            {
                Console.WriteLine(i);
            }
        }
    }
}
