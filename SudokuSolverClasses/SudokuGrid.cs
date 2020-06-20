using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace SudokuSolver
{
    public class SudokuGrid : ICloneable
    {
        public Cell[,] _grid_cells { get; set; }

        public static int min_x = 0;
        public static int max_x = 8;
        public static int min_y = 0;
        public static int max_y = 8;

        public SudokuGrid( string puzzle ) {
            _grid_cells = new Cell[9, 9];

            Regex digitsOnly = new Regex(@"[^\d]");
            string cell_values = digitsOnly.Replace(puzzle, "");

            for (int i = 0; i < cell_values.Length; i++) {
                int x = i % 9;
                int y = i / 9;

                if (cell_values.Length>=i)
                {
                    _grid_cells[x, y] = new Cell(int.Parse(cell_values[i].ToString()));
                } else
                {
                    _grid_cells[x, y] = new Cell();
                }
                
                //Console.WriteLine("Adding cell " + _cell_values[i]);
            }
        }

        public SudokuGrid(Cell[,] cells)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    this._grid_cells[x, y] = (Cell)cells[x, y].Clone();
                }
            }

        }
        public override string ToString()
        {
            string values = "";
            for (int y = 0; y<9; y++) {
                for (int x = 0; x<9; x++) {
                    Cell c = _grid_cells[x, y];
                    values += c._value;
                    //Console.WriteLine("to string " + c._value);
                }
            }

            return values;
        }

        public string ToStringFormatted() {

            string values = "";
            for (int y = 0; y < 9; y++)
            {
                if (y > 0 && y % 3 == 0) {
                    values += " - - - - - - - - - - -" + System.Environment.NewLine;
                }

                for (int x = 0; x < 9; x++)
                {
                    if (x > 0 && x % 3 == 0 )
                    {
                        values += " |";
                    }

                    values += " " + _grid_cells[x, y]._value;
                }
                values += System.Environment.NewLine;
            }

            return values.Replace('0', ' ');
        }

        public override bool Equals(object obj)
        {
            SudokuGrid other_grid = obj as SudokuGrid;
            if (other_grid == null )
            {
                return false;
            }

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (this._grid_cells[x,y]._value != other_grid._grid_cells[x,y]._value ) 
                    {
                        return false;
                    }

                    var firstNotSecond = this._grid_cells[x, y]._possible_values.Except(other_grid._grid_cells[x, y]._possible_values).ToList();
                    var secondNotFirst = other_grid._grid_cells[x, y]._possible_values.Except(this._grid_cells[x, y]._possible_values).ToList();

                    if (firstNotSecond.Any() || secondNotFirst.Any() || this._grid_cells[x, y]._possible_values.Count != other_grid._grid_cells[x, y]._possible_values.Count)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        public object Clone()
        {
            SudokuGrid cloned_grid = new SudokuGrid( "" );

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    cloned_grid._grid_cells[x, y] = (Cell)this._grid_cells[x, y].Clone();
                }
            }
            
            return cloned_grid;
        }

        public bool is_grid_valid()
        {
            return is_grid_valid(_grid_cells);
        }

        public static bool is_grid_valid(Cell[,] grid_cells)
        {
            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) coords = get_coordinates_for_all_shapes();

            for (int i = 0; i < 9; i++)
            {
                // check column
                if ( !are_cells_valid( coords.cols[i], grid_cells ) )
                {
                    return false;
                }

                // check row
                if (!are_cells_valid( coords.rows[i], grid_cells))
                {
                    return false;
                }

                // check block
                if (!are_cells_valid( coords.blocks[i], grid_cells))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool are_cells_valid(CoordinateList cell_coords, Cell[,] cells)
        {
            HashSet<int> found_values = new HashSet<int>();

            foreach((int x, int y) in cell_coords)
            {
                int cell_value = cells[x, y]._value;

                if ( cell_value != 0 )
                {
                    if ( !found_values.Add( cell_value ) ) {
                        return false;
                    }
                }
            }
            return true;
        }

        public static CoordinateList get_all_coordinates_for_column(int x, int y)
        {
            CoordinateList coords = get_other_coordinates_for_column(x, y);
            coords.Add( x, y);
            return coords;
        }
        public static CoordinateList get_other_coordinates_for_column( int x, int y )
        {
            CoordinateList coords = new CoordinateList();
            for (int i=0; i<9; i++)
            {
                if (i == y)
                {
                    //do nothing, we don't want to duplicate the input coordinate
                } else
                {
                    coords.Add( x, i );
                }
            }

            return coords;
        }

        public static CoordinateList get_all_coordinates_for_row(int x, int y)
        {
            CoordinateList coords = get_other_coordinates_for_row(x, y);
            coords.Add( x, y );
            return coords;
        }
        public static CoordinateList get_other_coordinates_for_row(int x, int y)
        {
            CoordinateList coords = new CoordinateList();
            for (int i = 0; i < 9; i++)
            {
                if (i == x)
                {
                    //do nothing, we don't want to duplicate the input coordinate
                }
                else
                {
                    coords.Add( i, y );
                }
            }

            return coords;
        }

        public static CoordinateList get_all_coordinates_for_block(int x, int y)
        {
            //Console.WriteLine($"{x}, {y}");
            CoordinateList coords = get_other_coordinates_for_block(x, y);
            coords.Add( x, y );
            return coords;
        }
        public static CoordinateList get_other_coordinates_for_block(int x, int y)
        {
            CoordinateList coords = new CoordinateList();
            int bx;
            int by;
            for (int i = 0; i < 9; i++)
            {
                bx = (i % 3) + (3 * (x/3));
                by = (i / 3) + (3 * (y/3));

                if (bx == x && by == y)
                {
                    //do nothing, we don't want to duplicate the input coordinate
                }
                else
                {
                    coords.Add(bx, by);
                }
            }

            return coords;
        }

        public static List<int> get_values_for_coordinates(CoordinateList coords, Cell[,] cells)
        {
            List<int> values = new List<int>();

            foreach ((int cx, int cy) in coords)
            {
                if (cells[cx, cy]._value != 0)
                {
                    values.Add(cells[cx, cy]._value);
                }
            }

            return values;
        }

        public static List<int> calculate_possible_values_for_cell(int x, int y, Cell[,] cells)
        {
            List<int> possible_values = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // get all values in the same row 
            possible_values = possible_values.Except( get_values_for_coordinates(get_other_coordinates_for_row(x, y), cells) ).ToList();

            // get all the values in the same column 
            possible_values = possible_values.Except( get_values_for_coordinates(get_other_coordinates_for_column(x, y), cells) ).ToList();

            // get all the values in the same block 
            possible_values = possible_values.Except( get_values_for_coordinates(get_other_coordinates_for_block(x, y), cells) ).ToList();

            return possible_values;
        }

        public void set_possible_values_of_all_cells()
        {
            foreach((int x, int y) in get_all_coordinates_for_grid())
            {
                Cell c = _grid_cells[x, y];
                if(c._value == 0)
                {
                    //Console.WriteLine($"Setting possible values for ({x},{y})");
                    c._possible_values = calculate_possible_values_for_cell(x, y, _grid_cells);
                }
            }
        }

        public void set_cell_value_and_update_possible_values(int x, int y, int new_value)
        {
            this._grid_cells = set_cell_value_and_update_possible_values(this._grid_cells, x, y, new_value);

        }
        public static Cell[,] set_cell_value_and_update_possible_values(Cell[,] grid, int x, int y, int new_value)
        {
            //Console.WriteLine($"set_cell_value_and_update_possible_values ({x},{y}) {new_value}");
            grid[x, y]._value = new_value;

            // we use get_all so that we also update the possible values of the target cell
            grid = remove_value_from_permitted_values_in_cells(grid, new_value, get_all_coordinates_for_row(x,y));

            grid = remove_value_from_permitted_values_in_cells(grid, new_value, get_other_coordinates_for_column(x, y));

            grid = remove_value_from_permitted_values_in_cells(grid, new_value, get_other_coordinates_for_block(x, y));

            return grid;
        }

        public static Cell[,] remove_value_from_permitted_values_in_cells(Cell[,] grid, int value, CoordinateList affected_cells)
        {
            foreach ((int x, int y) in affected_cells )
            {
                grid[x, y]._possible_values.Remove( value );
            }
            return grid;
        }

        public bool is_grid_solved()
        {
            // we don't care if the grid is valid, we just want to know if there are any cells without a value
            return get_unsolved_cell_coordinates( get_all_coordinates_for_grid(), this._grid_cells ).Count() == 0;
        }

        public static CoordinateList get_all_coordinates_for_grid()
        {
            CoordinateList coords = new CoordinateList();

            for (int x = SudokuGrid.min_x; x <= SudokuGrid.max_x; x++)
            {
                for (int y = SudokuGrid.min_y; y <= SudokuGrid.max_y; y++)
                {
                    coords.Add(x, y);
                }
            }

            return coords;
        }

        public static CoordinateList get_unsolved_cell_coordinates(CoordinateList coords_to_check, Cell[,] grid)
        {
            CoordinateList unsolved_coords = new CoordinateList();

            foreach( (int x, int y) in coords_to_check )
            {
                if ( grid[x,y]._value == 0 )
                {
                    unsolved_coords.Add(x, y);
                }
            }

            return unsolved_coords;
        }

        public void display_all_possible_values()
        {
            foreach((int x, int y) in get_all_coordinates_for_grid())
            {
                Cell c = this._grid_cells[x, y];
                Console.Write($"({x},{y})=");
                foreach(int pv in c._possible_values)
                {
                    Console.Write($"{pv},");
                }
                Console.WriteLine("");
            }
        }

        public static (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) get_coordinates_for_all_shapes()
        {
            CoordinateList[] rows = new CoordinateList[9];
            CoordinateList[] cols = new CoordinateList[9];
            CoordinateList[] blocks = new CoordinateList[9];

            for (int i = 0; i < 9; i++)
            {
                rows[i] = get_all_coordinates_for_row(0, i);

                cols[i] = get_all_coordinates_for_column(i, 0);

                blocks[i] = get_all_coordinates_for_block(3 * (i % 3), i);
            }
            return (rows: rows, cols: cols, blocks: blocks);
        }

        //public static Dictionary<int[],List<int>> get_possible_values_for_coordinates(Cell[,] cells, CoordinateList coords)
        //{
        //    Dictionary<int[], List<int>> ret_vals = new Dictionary<int[], List<int>>();

        //    foreach((int x, int y) in coords)
        //    {
        //        Cell c = cells[x, y];
        //        if ( c._possible_values.Count > 0 )
        //        {
        //            ret_vals.Add(new int[2] {x, y}, c._possible_values);
        //        }
        //    }
        //    return ret_vals;
        //}

        public static CoordinateList get_coordinates_where_value_is_possible(Cell[,] cells, int value_to_look_for, CoordinateList coords_to_check)
        {
            return get_coordinates_where_values_are_possible(cells, new HashSet<int> { value_to_look_for }, coords_to_check);
        }
        public static CoordinateList get_coordinates_where_values_are_possible(Cell[,] cells, HashSet<int> values_to_look_for, CoordinateList coords_to_check)
        {
            //Console.Write($"Starting get_coordinates_where_values_are_possible, looking for({values_to_look_for.Count})=");
            //foreach(int v in values_to_look_for)
            //{
            //    Console.Write($"{v},");
            //}
            //Console.WriteLine("");

            CoordinateList where_found = new CoordinateList();

            foreach((int x, int y) in coords_to_check)
            {
                Cell c = cells[x,y];

                //Console.WriteLine($"({x},{y}) {c} # possible={c._possible_values.Count}");
                if ( c._possible_values.Count < values_to_look_for.Count )
                {
                    continue;
                }

                IEnumerable<int> common_vals = values_to_look_for.Intersect(c._possible_values);
                //Console.WriteLine($"nbr common_vals={common_vals.Count()}");
                if (common_vals.Count() == values_to_look_for.Count)
                {
                    where_found.Add(x, y);
                }
            }

            //Console.WriteLine("Ending get_coordinates_where_values_are_possible");

            return where_found;
        }

        public static List<CoordinateList> get_intersecting_blocks(CoordinateList reference_cells)
        {
            List<CoordinateList> return_blocks = new List<CoordinateList>();

            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) shapes = get_coordinates_for_all_shapes();
            
            foreach(CoordinateList block in shapes.blocks)
            {
                if(reference_cells.Intersect(block).Count() > 0)
                {
                    return_blocks.Add(block);
                }
            }

            return return_blocks;
        }
    }
}
