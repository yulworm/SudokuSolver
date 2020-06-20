using System;
using System.Collections.Generic;
using System.Text;
using SudokuSolver;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

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
            Dictionary < (int x, int y), List<int> > hidden = find_hidden_singles(cells);
            foreach ( var pair in hidden )
            {
                cells[pair.Key.x, pair.Key.y]._possible_values = pair.Value;
            }

            HashSet<(int, int, int)> block_and_r = find_pointing_pairs(cells);
            foreach((int x, int y, int val) in block_and_r)
            {
                cells[x, y]._possible_values.Remove(val);
            }

            return cells;
        }

        public static Dictionary<(int x, int y), List<int>> find_hidden_singles(Cell[,] cells)
        {
            Dictionary <(int x, int y),List<int>> results = new Dictionary<(int x, int y), List<int>>();

            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) coords = SudokuGrid.get_coordinates_for_all_shapes();

            for (int i=1; i<=9; i++)
            {
                foreach (CoordinateList col in coords.cols)
                {
                    CoordinateList found_spots = SudokuGrid.get_coordinates_where_value_is_possible(cells, i, col);

                    if (found_spots.Count() == 1)
                    {
                        (int x, int y) coord = found_spots.get_coordinate(0);
                        results.TryAdd((coord.x, coord.y), new List<int>{ i });
                    }
                }

                foreach (CoordinateList row in coords.rows)
                {
                    CoordinateList found_spots = SudokuGrid.get_coordinates_where_value_is_possible(cells, i, row);

                    if (found_spots.Count() == 1)
                    {
                        (int x, int y) coord = found_spots.get_coordinate(0);
                        results.TryAdd((coord.x, coord.y), new List<int> { i });
                    }
                }

                foreach (CoordinateList block in coords.blocks)
                {
                    CoordinateList found_spots = SudokuGrid.get_coordinates_where_value_is_possible(cells, i, block);

                    if (found_spots.Count() == 1)
                    {
                        (int x, int y) coord = found_spots.get_coordinate(0);
                        results.TryAdd((coord.x, coord.y), new List<int> { i });
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

        // return values are x,y coordinate and a value that should be removed from possible values
        public static HashSet<(int, int, int)> find_pointing_pairs(Cell[,] cells)
        {
            HashSet<(int, int, int)> possible_values_to_remove = new HashSet<(int, int, int)>();

            //Console.WriteLine("Starting find_pointing_pairs");
            //SudokuGrid g = new SudokuGrid(cells);
            //g.display_all_possible_values();

            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) coords = SudokuGrid.get_coordinates_for_all_shapes();

            foreach(CoordinateList block in coords.blocks)
            {
                CoordinateList[] g_rows = new CoordinateList[3]; // the coordinates in the row outside the block
                CoordinateList[] g_cols = new CoordinateList[3]; // the coordinates in the column outside the block
                CoordinateList[] block_rows = new CoordinateList[3];  // the coordinates in the row inside the block
                CoordinateList[] block_cols = new CoordinateList[3];  // the coordinates in the column inside the block
                HashSet<int> xs = new HashSet<int>();
                HashSet<int> ys = new HashSet<int>();
                foreach((int x, int y) in block)
                {
                    xs.Add(x);
                    ys.Add(y);
                }
                for(int i=0; i<3; i++)
                {
                    CoordinateList row = SudokuGrid.get_all_coordinates_for_row(xs.ElementAt(i), ys.ElementAt(i));
                    CoordinateList col = SudokuGrid.get_all_coordinates_for_column(xs.ElementAt(i), ys.ElementAt(i));
                    g_rows[i] = new CoordinateList(row.Except(block).ToList());
                    g_cols[i] = new CoordinateList(col.Except(block).ToList());
                    block_rows[i] = new CoordinateList( block.Intersect(row).ToList() );
                    block_cols[i] = new CoordinateList( block.Intersect(col).ToList());
                }

                for(int value = 1; value <=9; value++)
                {
                    int unique_to_row = -1; //-1 = initial value, 0-2 = that row is the only one with the value, -9 = the value is in multiple rows
                    int unique_to_col = -1;

                    for (int i = 0; i < 3; i++)
                    {
                        if (SudokuGrid.get_coordinates_where_value_is_possible(cells, value, block_rows[i]).Count() > 0)
                        {
                            if (unique_to_row == -1)
                            {
                                // this is the first row we have found the value in
                                unique_to_row = i;

                            } else if (unique_to_row > -1 )
                            {
                                // this is the second row that we have found the value in
                                unique_to_row = -9;

                            } else
                            {
                                // it will already be -9
                            }
                        }

                        if (SudokuGrid.get_coordinates_where_value_is_possible(cells, value, block_cols[i]).Count() > 0)
                        {
                            if (unique_to_col == -1)
                            {
                                // this is the first column we have found the value in
                                unique_to_col = i;

                            }
                            else if (unique_to_col > -1)
                            {
                                // this is the second column that we have found the value in
                                unique_to_col = -9;

                            }
                            else
                            {
                                // it will already be -9
                            }
                        }
                    }

                    if(unique_to_row > -1)
                    {
                        foreach((int x, int y) in g_rows[unique_to_row])
                        {
                            possible_values_to_remove.Add((x,y,value));
                        }
                        
                    }

                    if (unique_to_col > -1)
                    {
                        foreach ((int x, int y) in g_cols[unique_to_col])
                        {
                            possible_values_to_remove.Add((x, y, value));
                        }

                    }
                }
            }

            return possible_values_to_remove;
        }

        // finds a value that only appears twice in two block and they line up, so the value cannot be set for the third block
        // return values are x,y coordinate and a value that should be removed from possible values
        public static HashSet<(int, int, int)> find_block_block_interactions(Cell[,] cells)
        {
            Console.WriteLine("Start find_block_block_interactions");
            HashSet<(int, int, int)> possible_values_to_remove = new HashSet<(int, int, int)>();
            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) coords = SudokuGrid.get_coordinates_for_all_shapes();

            // find a possible value that only exists in 2 cells of more than 1 block
            for (int i=1; i <=9; i++)
            {
                List<CoordinateList> i_twice_in_block = new List<CoordinateList> ();
                foreach (CoordinateList block_coords in coords.blocks)
                {
                    CoordinateList spots_in_block = SudokuGrid.get_coordinates_where_value_is_possible(cells, i, block_coords);
                    Console.WriteLine($"Found {spots_in_block.Count()} instances for {i}");
                    if (spots_in_block.Count() == 2)
                    {
                        i_twice_in_block.Add(spots_in_block);
                    }
                }

                if (i_twice_in_block.Count() >= 2)
                {
                    // check if the values line up by row or column
                    foreach (CoordinateList ref_coords in i_twice_in_block) 
                    {
                        List<int> rows = new List<int>();
                        List<int> cols = new List<int>();
                        foreach((int x, int y) in ref_coords)
                        {
                            rows.Add(y);
                            cols.Add(x);
                        }
                        CoordinateList ref_block_coords = SudokuGrid.get_all_coordinates_for_block(cols[0], rows[0]);

                        foreach (CoordinateList other_coords in i_twice_in_block)
                        {
                            bool match_rows = true;
                            bool match_cols = true;

                            if (!ref_coords.Equals(other_coords))
                            {
                                CoordinateList other_block_coords = new CoordinateList();
                                foreach ((int x, int y) in other_coords)
                                {
                                    if (!rows.Contains(y))
                                    {
                                        match_rows = false;
                                    }

                                    if (!cols.Contains(x))
                                    {
                                        match_cols = false;
                                    }

                                    other_block_coords = SudokuGrid.get_all_coordinates_for_block(x, y);
                                }

                                if (match_cols || match_rows)
                                {
                                    Console.WriteLine($"find_block_block_interactions should exclude {i}");
                                }

                                // if the rows or columns matched, then we want the coordinate of the row or column that is not in either block
                                if ( match_rows )
                                {
                                    CoordinateList row_to_exclude;
                                    foreach(int row in rows)
                                    {
                                        CoordinateList full_row = SudokuGrid.get_all_coordinates_for_row(0, row);

                                        row_to_exclude = new CoordinateList(full_row.Except(ref_block_coords).Except(other_block_coords).ToList());
                                        foreach((int x, int y) in row_to_exclude)
                                        {
                                            possible_values_to_remove.Add((x, y, i));
                                        }
                                    }
                                }

                                if (match_cols)
                                {
                                    CoordinateList col_to_exclude;
                                    foreach (int col in cols)
                                    {
                                        CoordinateList full_col = SudokuGrid.get_all_coordinates_for_column(col, 0);

                                        col_to_exclude = new CoordinateList(full_col.Except(ref_block_coords).Except(other_block_coords).ToList());
                                        foreach ((int x, int y) in col_to_exclude)
                                        {
                                            possible_values_to_remove.Add((x, y, i));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            
            return possible_values_to_remove;
        }
        }
    }
