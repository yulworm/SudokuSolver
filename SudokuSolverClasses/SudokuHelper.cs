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

        public static Cell[,] set_value_for_naked_singles(Cell[,] grid)
        {
            //Console.WriteLine("set_value_for_naked_singles begin");
            CoordinateList singles = SudokuGrid.find_cells_with_a_quantity_of_possible_values(grid, 1, 1);
            foreach ((int x, int y) in singles)
            {
                //Console.WriteLine($" ({x},{y}) to {c._possible_values[0]}");
                grid = SudokuGrid.set_cell_value_and_update_possible_values(grid, x, y, grid[x, y]._possible_values[0]);
            }
            //Console.WriteLine("set_value_for_naked_singles end");
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
                grid._grid_cells = set_value_for_naked_singles(grid._grid_cells);

                //Console.WriteLine(grid.ToStringFormatted());
                //grid.display_all_possible_values();

                // if naked singles didn't change anything, then try the techniques
                grid_changed = !grid.Equals(prev_grid);
                if (!grid_changed) {
                    grid = apply_solving_techniques(grid);
                } 
                
                grid_changed = !grid.Equals(prev_grid);
                prev_grid = (SudokuGrid) grid.Clone();
                //grid.display_all_possible_values();
                //Console.WriteLine($"End of while loop, not solved={!grid.is_grid_solved()} grid changed = {grid_changed}");
            }

            return grid;
        }


        public static SudokuGrid apply_solving_techniques(SudokuGrid grid)
        {
            grid = set_value_from_instructions(grid, find_hidden_singles(grid._grid_cells), SudokuGrid.tech_hidden_single);

            grid = narrowing_possible_values_from_instructions(grid, find_pointing_pairs(grid._grid_cells), SudokuGrid.tech_pointing_pair);

            grid = narrowing_possible_values_from_instructions(grid, find_block_block_interactions(grid._grid_cells), SudokuGrid.tech_block_block);

            grid = narrowing_possible_values_from_instructions(grid, find_x_y_wing(grid._grid_cells), SudokuGrid.tech_xy_wing);

            return grid;
        }

        private static SudokuGrid set_value_from_instructions(SudokuGrid grid, HashSet<(int, int, int)> instructions, int setting_method)
        {
            foreach ((int x, int y, int val) in instructions)
            {
                //Console.WriteLine($"setting {val} from ({x},{y})");

                grid.set_cell_value_and_update_possible_values(x, y, val);
            }

            if (instructions.Count > 0)
            {
                grid._techniques_used[setting_method] = true;
            }

            return grid;
        }

        private static SudokuGrid narrowing_possible_values_from_instructions(SudokuGrid grid, HashSet<(int, int, int)> instructions, int narrowing_method)
        {
            foreach ((int x, int y, int val) in instructions)
            {
                //Console.WriteLine($"removing {val} from ({x},{y})");

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

            //Console.WriteLine($"end find_hidden_singles. Results={format_coord_and_value_hashset(results)}");

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
                        // we filter the coordinate list to just the ones that should have the value
                        foreach ((int x, int y) in SudokuGrid.get_coordinates_where_value_is_possible(cells, value, g_rows[unique_to_row]))
                        {
                            possible_values_to_remove.Add((x,y,value));
                        }
                        
                    }

                    if (unique_to_col > -1)
                    {
                        // we filter the coordinate list to just the ones that should have the value
                        foreach ((int x, int y) in SudokuGrid.get_coordinates_where_value_is_possible(cells, value, g_cols[unique_to_col]))
                        {
                            possible_values_to_remove.Add((x, y, value));
                        }

                    }
                }
            }

            //Console.WriteLine($"end find_pointing_pairs. Results={format_coord_and_value_hashset(possible_values_to_remove)}");

            return possible_values_to_remove;
        }

        public static HashSet<(int, int, int)> find_block_block_interactions_generic(Cell[,] cells, List<CoordinateList> blocks, List<CoordinateList> lines, int value_to_check)
        {
            //Console.WriteLine($"Start of generic find_block_block_interactions_generic for {value_to_check}. # blocks={blocks.Count()} #lines={lines.Count()}");

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

            //Console.WriteLine($"End of generic find_block_block_interactions_generic, returning {possible_values_to_remove.Count} to remove");
            return possible_values_to_remove;
        }

        public static HashSet<(int, int, int)> find_block_block_interactions(Cell[,] cells)
        {
            //Console.WriteLine("Start find_block_block_interactions_generic");
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

                    HashSet<(int, int, int)> to_remove = find_block_block_interactions_generic(cells, row_blocks, rows, i);
                    //Console.WriteLine($"rows # to remove={to_remove.Count}");
                    possible_values_to_remove.UnionWith(to_remove);
                    //Console.WriteLine($"total # to remove={possible_values_to_remove.Count}");
                    to_remove = find_block_block_interactions_generic(cells, col_blocks, cols, i);
                    //Console.WriteLine($"cols # to remove={to_remove.Count}");
                    possible_values_to_remove.UnionWith(to_remove);
                    //Console.WriteLine($"total # to remove={possible_values_to_remove.Count}");
                }
            }

            //Console.WriteLine($"End find_block_block_interactions_generic, total # to remove={possible_values_to_remove.Count}");
            return possible_values_to_remove;
        }

        // see https://www.sadmansoftware.com/sudoku/xywing.php
        public static HashSet<(int, int, int)> find_x_y_wing(Cell[,] cells)
        {
            HashSet<(int, int, int)> possible_values_to_remove = new HashSet<(int, int, int)>();

            CoordinateList poss_pivots = SudokuGrid.find_cells_with_a_quantity_of_possible_values(cells, 2, 2);

            foreach((int x, int y) pivot_coord in poss_pivots)
            {
                Cell pivot_cell = cells[pivot_coord.x, pivot_coord.y];
                CoordinateList poss_branches = SudokuGrid.find_cells_with_a_quantity_of_possible_values(cells, 2, 2, SudokuGrid.get_interacting_cells(pivot_coord.x,pivot_coord.y));

                //Console.Write($"pivot ({pivot_coord.x},{pivot_coord.y}) v=");
                //foreach(int p_value in pivot_cell._possible_values)
                //{
                //    Console.Write($"{p_value},");
                //}
                //Console.WriteLine(";");

                // we need at least two branches
                if (poss_branches.Count() <= 2)
                {
                    continue;
                }

                // the possible branches can't contain the exact same values as the pivot
                poss_branches = new CoordinateList( poss_branches.Except( SudokuGrid.get_coordinates_where_values_are_possible(cells, pivot_cell._possible_values, poss_branches ) ).ToList() );

                // each branch should have 1 possible value in common with the pivot
                CoordinateList poss_branch_A = SudokuGrid.get_coordinates_where_value_is_possible(cells, pivot_cell._possible_values[0], poss_branches);
                CoordinateList poss_branch_B = SudokuGrid.get_coordinates_where_value_is_possible(cells, pivot_cell._possible_values[1], poss_branches);

                foreach((int x, int y) coord_A in poss_branch_A)
                {
                    Cell branch_A = cells[coord_A.x, coord_A.y];

                    //Console.Write($"branch_A ({coord_A.x},{coord_A.y}) v=");
                    //foreach (int a_value in branch_A._possible_values)
                    //{
                    //    Console.Write($"{a_value},");
                    //}
                    //Console.WriteLine(";");

                    // there needs to be a common value in branch A and B
                    CoordinateList Bs_for_this_A = SudokuGrid.get_coordinates_where_possible_values_match(cells, branch_A._possible_values, poss_branch_B, SudokuGrid.match_type_intersects);

                    if (Bs_for_this_A.Count() != 1 )
                    {
                        if (Bs_for_this_A.Count() > 1)
                        {
                            throw new Exception("I don't know what to do, there is more than 1 match!");
                        } else
                        {
                            //Console.WriteLine("no common B");
                            continue;
                        }
                    }

                    (int x, int y) coord_B = Bs_for_this_A.First();
                    Cell branch_B = cells[coord_B.x, coord_B.y];

                    //Console.Write($"branch_B ({coord_B.x},{coord_B.y}) v=");
                    //foreach (int b_value in branch_B._possible_values)
                    //{
                    //    Console.Write($"{b_value},");
                    //}
                    //Console.WriteLine(";");

                    int value_common_to_A_and_B = branch_A._possible_values.Intersect(branch_B._possible_values).First();

                    // cells that interact with both A and B may not have the common value
                    CoordinateList cells_interacting_with_A_and_B = 
                        new CoordinateList( 
                            SudokuGrid.get_interacting_cells(coord_A.x, coord_A.y)
                            .Intersect(SudokuGrid.get_interacting_cells(coord_B.x, coord_B.y) ).ToList() );

                    // we also filter the list of coordinates to only include those that have the common value
                    foreach ((int x, int y) coord_to_remove in SudokuGrid.get_coordinates_where_value_is_possible(cells, value_common_to_A_and_B, cells_interacting_with_A_and_B) )
                    {
                        possible_values_to_remove.Add((coord_to_remove.x, coord_to_remove.y, value_common_to_A_and_B));
                    }
                }
            }

            return possible_values_to_remove;
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

        // we return all the possible extensions
        // the list will be empty if no new extensions are found
        public static List<CoordinateList> add_link_to_chain(Cell[,] cells, CoordinateList chain)
        {
            List<CoordinateList> built_chains = new List<CoordinateList>();

            (int x, int y) last_link_coord = chain.Last();
            Cell last_link = cells[last_link_coord.x, last_link_coord.y];
            
            CoordinateList link_search_space = new CoordinateList(
                // the next link will be in a cell that interacts with the current end
                SudokuGrid.get_interacting_cells(last_link_coord.x, last_link_coord.y)
                // but it must not already be part of the chain
                .Except(chain)
                .ToList());

            // for now we will only look for chains where every link has the same number of possible values
            link_search_space = SudokuGrid.find_cells_with_a_quantity_of_possible_values(cells, last_link._possible_values.Count(), last_link._possible_values.Count(), link_search_space);

            CoordinateList potential_links = SudokuGrid.get_coordinates_where_possible_values_match(cells, last_link._possible_values, link_search_space, SudokuGrid.match_type_intersects);
            foreach((int x, int y) in potential_links)
            {
                CoordinateList new_chain = new CoordinateList(chain.ToList());
                new_chain.Add(x, y);
                built_chains.Add(new_chain);
            }

            return built_chains;
        }
    }
}
