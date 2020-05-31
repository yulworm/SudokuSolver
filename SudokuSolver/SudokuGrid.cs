using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public class SudokuGrid
    {
        public Cell[,] _grid_cells { get; set; }

        public SudokuGrid( string puzzle ) {
            _grid_cells = new Cell[9, 9];

            Regex digitsOnly = new Regex(@"[^\d]");
            string cell_values = digitsOnly.Replace(puzzle, "");

            for (int i = 0; i < cell_values.Length; i++) {
                int x = i % 9;
                int y = i / 9;
                _grid_cells[x, y] = new Cell( int.Parse(cell_values[i].ToString() ) );

                //Console.WriteLine("Adding cell " + _cell_values[i]);
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
                }
            }
            
            return true;
        }

        public bool is_grid_valid()
        {
            return is_grid_valid(_grid_cells);
        }

        public static bool is_grid_valid(Cell[,] grid_cells)
        {
            bool is_valid = true;

            for (int i = 0; i < 9; i++)
            {
                // check column
                if ( !are_cells_valid( get_all_coordinates_for_column(i, 0), grid_cells ) )
                {
                    return false;
                }

                // check row
                if (!are_cells_valid(get_all_coordinates_for_row(0, i), grid_cells))
                {
                    return false;
                }

                // check block
                if (!are_cells_valid(get_all_coordinates_for_block(3*(i%3), i), grid_cells))
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

        public static List<int> get_possible_values_for_cell(int x, int y, Cell[,] cells)
        {
            List<int> possible_values = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // get all values in the same row 
            var vals = possible_values.Except( get_values_for_coordinates(get_other_coordinates_for_row(x, y), cells) );

            // get all the values in the same column 
            vals = vals.Except( get_values_for_coordinates(get_other_coordinates_for_column(x, y), cells) );

            // get all the values in the same block 
            vals = vals.Except( get_values_for_coordinates(get_other_coordinates_for_block(x, y), cells) );
            
            return vals.ToList();
        }

        public void set_possible_values_of_all_cells()
        {
            for (int x=0; x<9; x++)
            {
                for (int y=0; y<9; y++)
                {
                    _grid_cells[x, y]._possible_values = get_possible_values_for_cell(x,y,_grid_cells);
                }
            }
        }
    }
}
