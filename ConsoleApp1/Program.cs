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
            SudokuGrid grid = new SudokuGrid("000000000|000000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000");
            grid._grid_cells[0, 0]._possible_values = new List<int> { 1, 2, 3, 4, 5, 6 };
            grid._grid_cells[0, 1]._possible_values = new List<int> { 2 };
            grid._grid_cells[0, 2]._possible_values = new List<int> { 2, 5 };
            grid._grid_cells[1, 1]._possible_values = new List<int> { 3, 9 };
            grid._grid_cells[2, 2]._possible_values = new List<int> { 2, 3, 9 };

            HashSet<int> search_vals = new HashSet<int> { 2 };
            CoordinateList search_coords = new CoordinateList(new int[] { 0, 0, 0, 1, 1, 1, 0, 2 });

            CoordinateList coords_found = SudokuGrid.get_coordinates_where_values_are_possible(grid._grid_cells, search_vals, search_coords);

            search_vals = new HashSet<int> { 2, 5 };

            coords_found = SudokuGrid.get_coordinates_where_values_are_possible(grid._grid_cells, search_vals, search_coords);
        }
    }
}
