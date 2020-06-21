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

        //public static Cell[,] set_value_for_single_possible_value_cells(Cell[,] grid)
        //{
        //    Console.WriteLine("set_value_for_single_possible_value_cells begin");
        //    CoordinateList singles = SudokuGrid.find_cells_with_a_quantity_of_possible_values(grid, 1, 1);
        //    foreach ((int x, int y) in singles) {
        //        //Console.WriteLine($" ({x},{y}) to {c._possible_values[0]}");
        //        grid = SudokuGrid.set_cell_value_and_update_possible_values(grid, x, y, grid[x,y]._possible_values[0]);
        //    }
        //    Console.WriteLine("set_value_for_single_possible_value_cells end");
        //    return grid;
        //}
        public static Cell[,] set_value_for_single_possible_value_cells(Cell[,] grid)
        {
            //Console.WriteLine("set_value_for_single_possible_value_cells begin");
            foreach ((int x, int y) in SudokuGrid.get_all_coordinates_for_grid())
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

                grid = narrow_down_possible_values(grid);

                grid_changed = !grid.Equals(prev_grid);
                prev_grid = (SudokuGrid) grid.Clone();
                //grid.display_all_possible_values();
                //Console.WriteLine($"End of while loop, not solved={!grid.is_grid_solved()} grid changed = {grid_changed}");
            }

            return grid;
        }


        public static SudokuGrid narrow_down_possible_values(SudokuGrid grid)
        {
            grid = narrowing_possible_values(grid, find_hidden_singles(grid._grid_cells), SudokuGrid.tech_hidden_single);

            grid = narrowing_possible_values(grid, find_pointing_pairs(grid._grid_cells), SudokuGrid.tech_pointing_pair);

            //grid = narrowing_possible_values(grid, find_block_block_interactions(grid._grid_cells), SudokuGrid.tech_block_block);

            return grid;
        }

        private static SudokuGrid narrowing_possible_values(SudokuGrid grid, HashSet<(int, int, int)> instructions, int narrowing_method)
        {
            foreach ((int x, int y, int val) in instructions)
            {
                grid._grid_cells[x, y]._possible_values.Remove(val);
            }

            if (instructions.Count > 0)
            {
                grid._techniques_used[narrowing_method] = true;
            }

            return grid;
        }

        // look for values that are only possible in a single cell within a shape
        public static HashSet<(int, int, int)> find_hidden_singles(Cell[,] cells)
        {
            HashSet<(int, int, int)> results = new HashSet<(int, int, int)>();

            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) coords = SudokuGrid.get_coordinates_for_all_shapes();

            for (int search_value=1; search_value<=9; search_value++)
            {
                foreach (CoordinateList col in coords.cols)
                {
                    CoordinateList found_spots = SudokuGrid.get_coordinates_where_value_is_possible(cells, search_value, col);

                    if (found_spots.Count() == 1)
                    {
                        (int x, int y) coord = found_spots.get_coordinate(0);
                        results.Add((coord.x, coord.y, search_value));
                    }
                }

                foreach (CoordinateList row in coords.rows)
                {
                    CoordinateList found_spots = SudokuGrid.get_coordinates_where_value_is_possible(cells, search_value, row);

                    if (found_spots.Count() == 1)
                    {
                        (int x, int y) coord = found_spots.get_coordinate(0);
                        results.Add((coord.x, coord.y, search_value));
                    }
                }

                foreach (CoordinateList block in coords.blocks)
                {
                    CoordinateList found_spots = SudokuGrid.get_coordinates_where_value_is_possible(cells, search_value, block);

                    if (found_spots.Count() == 1)
                    {
                        (int x, int y) coord = found_spots.get_coordinate(0);
                        results.Add((coord.x, coord.y, search_value));
                    }
                }
            }

            Console.Write($"end find_hidden_singles. Results=");
            foreach ((int x, int y, int val) in results)
            {
                Console.Write($" ({x},{y}) v={val}");
            }
            Console.WriteLine(";");

            return results;
        }

        public static bool are_int_lists_equal(List<int> a, List<int> b)
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

        public static HashSet<(int, int, int)> find_block_block_interactions(Cell[,] cells, List<CoordinateList> blocks, List<CoordinateList> lines, int value_to_check)
        {
            Console.WriteLine($"Start of generic find_block_block_interactions for {value_to_check}. # blocks={blocks.Count()} #lines={lines.Count()}");

            HashSet<(int, int, int)> possible_values_to_remove = new HashSet<(int, int, int)>();

            // find a line without the value in 2 blocks
            foreach(CoordinateList line in lines)
            {
                //Console.WriteLine($"checking line {line}");
                int value_found_index = -1; // -1=not found, 99=found more than once, otherwise the index of the block where it was found
                CoordinateList line_in_block;
                for (int block_index=0; block_index < blocks.Count; block_index++) 
                {
                    CoordinateList block = blocks[block_index];
                    line_in_block = new CoordinateList(line.Intersect(block).ToList());
                    CoordinateList cells_with_value = SudokuGrid.get_coordinates_where_value_is_possible(cells, value_to_check, line_in_block);
                    if (cells_with_value.Count()>0)
                    {
                        // it was already found
                        if (value_found_index > -1)
                        {
                            value_found_index = 99;
                        } else
                        {
                            value_found_index = block_index;
                        }
                    }
                    //Console.WriteLine($"checking block {block}");
                    //Console.WriteLine($"line in block={line_in_block}");
                    //Console.WriteLine($"value found index={value_found_index}");
                }

                if (value_found_index > -1 && value_found_index < 99)
                {
                    // the value is excluded from where the other two lines intersect with the block that had the value
                    CoordinateList other_lines_in_block = new CoordinateList( blocks[value_found_index].Except(line).ToList() );

                    foreach((int x, int y) in SudokuGrid.get_coordinates_where_value_is_possible(cells, value_to_check, other_lines_in_block))
                    {
                        possible_values_to_remove.Add((x, y, value_to_check));
                    }
                }
            }

            Console.WriteLine($"End of generic find_block_block_interactions, returning {possible_values_to_remove.Count} to remove");
            return possible_values_to_remove;
        }

        public static HashSet<(int, int, int)> find_block_block_interactions(Cell[,] cells)
        {
            Console.WriteLine("Start find_block_block_interactions");
            HashSet<(int, int, int)> possible_values_to_remove = new HashSet<(int, int, int)>();
            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) shapes = SudokuGrid.get_coordinates_for_all_shapes();

            // we check for all the possible values one at a time
            for (int i = 1; i <= 9; i++)
            {
                // we treat each group of 3 blocks the same way
                // there are 3 groups where the reference direction is row and 3 by columns

                // we need the 3 blocks and the 3 rows/columns that will be evaluated


                for (int a = 0; a < 3; a++)
                {
                    List<CoordinateList> row_blocks = new List<CoordinateList>();
                    List<CoordinateList> col_blocks = new List<CoordinateList>();
                    List<CoordinateList> rows = new List<CoordinateList>();
                    List<CoordinateList> cols = new List<CoordinateList>();

                    for (int b = 0; b < 3; b++)
                    {
                        rows.Add(shapes.rows[3 * a + b]);
                        row_blocks.Add(shapes.blocks[3 * a + b]);

                        cols.Add(shapes.cols[3 * a + b]);
                        col_blocks.Add(shapes.blocks[3 * b + a]);
                    }

                    HashSet<(int, int, int)> to_remove = find_block_block_interactions(cells, row_blocks, rows, i);
                    Console.WriteLine($"rows # to remove={to_remove.Count}");
                    possible_values_to_remove.UnionWith(to_remove);
                    Console.WriteLine($"total # to remove={possible_values_to_remove.Count}");
                    to_remove = find_block_block_interactions(cells, col_blocks, cols, i);
                    Console.WriteLine($"cols # to remove={to_remove.Count}");
                    possible_values_to_remove.UnionWith(to_remove);
                    Console.WriteLine($"total # to remove={possible_values_to_remove.Count}");
                }
            }

            //    List<CoordinateList> i_twice_in_block = new List<CoordinateList> ();
            //    foreach (CoordinateList block_coords in coords.blocks)
            //    {
            //        CoordinateList spots_in_block = SudokuGrid.get_coordinates_where_value_is_possible(cells, i, block_coords);
            //        Console.WriteLine($"Found {spots_in_block.Count()} instances for {i}");
            //        if (spots_in_block.Count() == 2)
            //        {
            //            i_twice_in_block.Add(spots_in_block);
            //        }
            //    }

            //    if (i_twice_in_block.Count() >= 2)
            //    {
            //        // check if the values line up by row or column
            //        foreach (CoordinateList ref_coords in i_twice_in_block) 
            //        {
            //            List<int> rows = new List<int>();
            //            List<int> cols = new List<int>();
            //            foreach((int x, int y) in ref_coords)
            //            {
            //                rows.Add(y);
            //                cols.Add(x);
            //            }
            //            CoordinateList ref_block_coords = SudokuGrid.get_all_coordinates_for_block(cols[0], rows[0]);

            //            foreach (CoordinateList other_coords in i_twice_in_block)
            //            {
            //                bool match_rows = true;
            //                bool match_cols = true;

            //                if (!ref_coords.Equals(other_coords))
            //                {
            //                    CoordinateList other_block_coords = new CoordinateList();
            //                    foreach ((int x, int y) in other_coords)
            //                    {
            //                        if (!rows.Contains(y))
            //                        {
            //                            match_rows = false;
            //                        }

            //                        if (!cols.Contains(x))
            //                        {
            //                            match_cols = false;
            //                        }

            //                        other_block_coords = SudokuGrid.get_all_coordinates_for_block(x, y);
            //                    }

            //                    if (match_cols || match_rows)
            //                    {
            //                        Console.WriteLine($"find_block_block_interactions should exclude {i}");
            //                    }

            //                    // if the rows or columns matched, then we want the coordinate of the row or column that is not in either block
            //                    if ( match_rows )
            //                    {
            //                        CoordinateList row_to_exclude;
            //                        foreach(int row in rows)
            //                        {
            //                            CoordinateList full_row = SudokuGrid.get_all_coordinates_for_row(0, row);

            //                            row_to_exclude = new CoordinateList(full_row.Except(ref_block_coords).Except(other_block_coords).ToList());
            //                            foreach((int x, int y) in row_to_exclude)
            //                            {
            //                                possible_values_to_remove.Add((x, y, i));
            //                            }
            //                        }
            //                    }

            //                    if (match_cols)
            //                    {
            //                        CoordinateList col_to_exclude;
            //                        foreach (int col in cols)
            //                        {
            //                            CoordinateList full_col = SudokuGrid.get_all_coordinates_for_column(col, 0);

            //                            col_to_exclude = new CoordinateList(full_col.Except(ref_block_coords).Except(other_block_coords).ToList());
            //                            foreach ((int x, int y) in col_to_exclude)
            //                            {
            //                                possible_values_to_remove.Add((x, y, i));
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //}

            Console.WriteLine($"End find_block_block_interactions, total # to remove={possible_values_to_remove.Count}");
            return possible_values_to_remove;
        }

        public static HashSet<(int, int, int)> find_x_y_wing(Cell[,] cells)
        {
            return new HashSet<(int, int, int)> { (6, 3, 7), (7, 3, 7), (8, 3, 7), (3, 5, 7), (4, 5, 7), (5, 5, 7) };
        }

        public static string format_coord_and_value_hashset(HashSet<(int, int, int)> set_to_format)
        {
            string output = "";
            foreach ((int x, int y, int val) in set_to_format)
            {
                output += $"({x},{y}) value={val},";
            }
            return output;
        }

    }
}
