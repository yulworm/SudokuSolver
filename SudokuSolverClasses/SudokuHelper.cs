using System;
using System.Collections.Generic;
using System.Text;
using SudokuSolver;
using System.Linq;

namespace SudokuSolver
{
    public class SudokuHelper
    {
        public static Cell[,] set_value_for_single_possible_value_cells(Cell[,] grid)
        {
            //Console.WriteLine("set_value_for_single_possible_value_cells begin");
            foreach ((int x, int y) in SudokuGrid.get_all_coordinates_for_grid() )
            {
                Cell c = grid[x, y];
                //Console.WriteLine($" ({x},{y}) {c._possible_values.Count} possible values");
                if (c._value == 0 && c._possible_values.Count == 1)
                {
                    //Console.WriteLine($" ({x},{y}) to {c._possible_values[0]}");
                    grid = SudokuGrid.set_cell_value_and_update_possible_values(grid, x, y, c._possible_values[0]);
                }
            }
            //Console.WriteLine("set_value_for_single_possible_value_cells end");
            return grid;
        }

        public static SudokuGrid solve_puzzle(string puzzle)
        {
            return solve_grid(new SudokuGrid(puzzle));
        }

        public static SudokuGrid solve_grid(SudokuGrid grid)
        {
            SudokuGrid prev_grid = (SudokuGrid)grid.Clone();
            bool grid_changed = true;

            grid.set_possible_values_of_all_cells();
            //grid.display_all_possible_values();

            while ( !grid.is_grid_solved() && grid_changed )
            {
                grid._grid_cells = set_value_for_single_possible_value_cells(grid._grid_cells);

                grid._grid_cells = narrow_down_possible_values(grid._grid_cells);

                grid_changed = !grid.Equals(prev_grid);
                prev_grid = (SudokuGrid) grid.Clone();
                //grid.display_all_possible_values();
                //Console.WriteLine($"End of while loop, not solved={!grid.is_grid_solved()} grid changed = {grid_changed}");
            }

            return grid;
        }

        public static Cell[,] narrow_down_possible_values(Cell[,] cells)
        {
            //cells = find_hidden_singles(cells);

            return cells;
        }

        public static HashSet<(int x, int y, int val)> find_hidden_singles(Cell[,] cells)
        {
            HashSet<(int x, int y, int val)> results = new HashSet<(int x, int y, int val)>();

            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) coords = SudokuGrid.get_coordinates_for_all_shapes();

            for (int i=1; i<=9; i++)
            {
                foreach (CoordinateList col in coords.cols)
                {
                    CoordinateList found_spots = SudokuGrid.get_coordinates_where_values_are_possible(cells, i, col);

                    if (found_spots.Count() == 1)
                    {
                        (int x, int y) coord = found_spots.get_coordinate(0);
                        results.Add((coord.x, coord.y, i));
                    }
                }
                
            }

            return results;
        }

        public static bool are_lists_equal(List<int> a, List<int> b)
        {
            var a_not_b = a.Except(b).ToList();
            var b_not_a = b.Except(a).ToList();

            return a.Count == b.Count && !a_not_b.Any() && !b_not_a.Any();
        }

    }
}
