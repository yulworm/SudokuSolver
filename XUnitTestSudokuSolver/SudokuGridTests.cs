using System.Collections.Generic;
using SudokuSolver;
using Xunit;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace XUnitTestSudokuSolver
{
    public class SudokuGridTests
    {
        [Fact]
        public void init_and_toString_work_std_input()
        {
            string puzzle = "004300209005009001070060043006002087190007400050083000600000105003508690042910300";
            SudokuGrid grid = new SudokuGrid(puzzle);

            var result = grid.ToString().Equals(puzzle);

            Assert.True(result, "to string should return the constructor string");
        }
        [Fact]
        public void init_and_toString_work_square_input()
        {
            string puzzle = "0 0 4 | 300209005009001070060043006002087190007400050083000600000105003508690042910300";
            SudokuGrid grid = new SudokuGrid(puzzle);

            var result = grid.ToString().Equals("004300209005009001070060043006002087190007400050083000600000105003508690042910300");

            Assert.True(result, "to string should return the constructor string");
        }

        [Fact]
        public void equals_success_basic()
        {
            string puzzle = "004300209005009001070060043006002087190007400050083000600000105003508690042910300";
            SudokuGrid grid1 = new SudokuGrid(puzzle);
            SudokuGrid grid2 = new SudokuGrid(puzzle);

            Assert.True(grid1.Equals(grid2), "Grids created with the same puzzles should be equal");
        }

        [Fact]
        public void equals_failure_basic()
        {
            SudokuGrid grid1 = new SudokuGrid("004300209005009001070060043006002087190007400050083000600000105003508690042910300");
            SudokuGrid grid2 = new SudokuGrid("004300209005009001070060043006002087190007400050083000600000105003508690042910301");

            Assert.False(grid1.Equals(grid2), "Grids created with the different puzzles should not be equal");
        }

        [Fact]
        public void equals_success_possible()
        {
            string puzzle = "004300209005009001070060043006002087190007400050083000600000105003508690042910300";
            SudokuGrid grid1 = new SudokuGrid(puzzle);
            SudokuGrid grid2 = new SudokuGrid(puzzle);

            grid1._grid_cells[1, 1]._possible_values = new List<int> { 1, 2, 3 };
            grid1._grid_cells[2, 2]._possible_values = new List<int> { 2 };
            grid1._grid_cells[3, 3]._possible_values = new List<int> { 1, 3 };
            grid1._grid_cells[4, 4]._possible_values = new List<int> { 1, 2, 3 };

            grid2._grid_cells[1, 1]._possible_values = new List<int> { 1, 2, 3 };
            grid2._grid_cells[2, 2]._possible_values = new List<int> { 2 };
            grid2._grid_cells[3, 3]._possible_values = new List<int> { 1, 3 };
            grid2._grid_cells[4, 4]._possible_values = new List<int> { 1, 2, 3 };

            Assert.True(grid1.Equals(grid2), "Grids created with the same puzzles should be equal");
        }

        [Fact]
        public void equals_failure_possible()
        {
            string puzzle = "004300209005009001070060043006002087190007400050083000600000105003508690042910300";
            SudokuGrid grid1 = new SudokuGrid(puzzle);
            SudokuGrid grid2 = new SudokuGrid(puzzle);

            grid1._grid_cells[1, 1]._possible_values = new List<int> { 1, 2, 3 };
            grid1._grid_cells[2, 2]._possible_values = new List<int> { 2 };
            grid1._grid_cells[3, 3]._possible_values = new List<int> { 1, 3 };
            grid1._grid_cells[4, 4]._possible_values = new List<int> { 1, 2, 3 };

            grid1._grid_cells[1, 1]._possible_values = new List<int> { 1, 2, 3, 4 };
            grid1._grid_cells[2, 2]._possible_values = new List<int> { 2 };
            grid1._grid_cells[3, 3]._possible_values = new List<int> { 1, 3 };
            grid1._grid_cells[4, 4]._possible_values = new List<int> { 1, 2, 3 };

            Assert.False(grid1.Equals(grid2), "Grids same puzzles, but different possible values should not be equal");
        }

        [Theory]
        [InlineData("004300209005009001070060043006002087190007400050083000600000105003508690042910300")]
        [InlineData("864371259325849761971265843436192587198657432257483916689734125713528694542916378")]
        [InlineData("000000000|000000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000")]
        [InlineData("300000000|000000000|000000002||000000003|000000000|000000000||000000000|000000000|000000000")]
        public void grid_is_valid_true(string puzzle)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);

            Assert.True(grid.is_grid_valid());
        }

        [Theory]
        [InlineData("004300209004009001070060043006002087190007400050083000600000105003508690042910300")]
        [InlineData("964371259325849761971265843436192587198657432257483916689734125713528694542916378")]
        [InlineData("884371259325549761971265843436192587198657432257483916689734125713528694542916378")]
        [InlineData("220000000|000000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000")]
        [InlineData("300000000|000000000|003000000||000000000|000000000|000000000||000000000|000000000|000000000")]
        [InlineData("100000000|100000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000")]
        public void grid_is_valid_false(string puzzle)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);

            Assert.False(grid.is_grid_valid());
        }

        public static IEnumerable<object[]> get_other_coordinates_for_column_true_input()
        {
            // column1 = { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 };
            // column2 = { 1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6, 1, 7, 1, 8 };
            // column3 = { 2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 6, 2, 7, 2, 8 };

            yield return new object[] {
                2, 3,
                new CoordinateList( new int[] { 2, 0, 2, 1, 2, 2, 2, 4, 2, 5, 2, 6, 2, 7, 2, 8 } ) };
        }

        [Theory]
        [MemberData(nameof(get_other_coordinates_for_column_true_input))]
        public void get_other_coordinates_for_column_true(int x, int y, CoordinateList expected_coords)
        {
            CoordinateList coords = SudokuGrid.get_other_coordinates_for_column(x, y);
            Assert.True(coords.Equals(expected_coords));
        }

        public static IEnumerable<object[]> get_all_coordinates_for_column_true_input()
        {
            // column1 = { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 };
            // column2 = { 1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6, 1, 7, 1, 8 };
            // column3 = { 2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 6, 2, 7, 2, 8 };

            yield return new object[] {
                2, 3,
                new CoordinateList( new int[] { 2, 0, 2, 1, 2, 2, 2,3,2, 4, 2, 5, 2, 6, 2, 7, 2, 8 } ) };
        }

        [Theory]
        [MemberData(nameof(get_all_coordinates_for_column_true_input))]
        public void get_all_coordinates_for_column_true(int x, int y, CoordinateList expected_coords)
        {
            CoordinateList coords = SudokuGrid.get_all_coordinates_for_column(x, y);
            Assert.True(coords.Equals(expected_coords));
        }

        public static IEnumerable<object[]> get_all_coordinates_for_row_true_input()
        {
            // row1 = {0,0, 1,0, 2,0, 3,0, 4,0, 5,0, 6,0, 7,0, 8,0}
            // row2 = {0,1, 1,1, 2,1, 3,1, 4,1, 5,1, 6,1, 7,1, 8,1}
            // row3 = {0,2, 1,2, 2,2, 3,2, 4,2, 5,2, 6,2, 7,2, 8,2}
            // row4 = {0,3, 1,3, 2,3, 3,3, 4,3, 5,3, 6,3, 7,3, 8,3}

            yield return new object[] {
                2, 3,
                new CoordinateList( new int[] {0,3, 1,3, 2,3, 3,3, 4,3, 5,3, 6,3, 7,3, 8,3} ) };
        }

        [Theory]
        [MemberData(nameof(get_all_coordinates_for_row_true_input))]
        public void get_all_coordinates_for_row_true(int x, int y, CoordinateList expected_coords)
        {
            CoordinateList coords = SudokuGrid.get_all_coordinates_for_row(x, y);
            Assert.True(coords.Equals(expected_coords));
        }

        public static IEnumerable<object[]> get_all_coordinates_for_block_true_input()
        {
            // block1 = {0,0, 1,0, 2,0, 0,1, 1,1, 1,2, 0,2, 1,2, 2,2}
            // block4 = {0,3, 1,3, 2,3, 0,4, 1,4, 1,4, 0,5, 1,5, 2,5}

            yield return new object[] {
                2, 3,
                new CoordinateList( new int[] {0,3, 1,3, 2,3, 0,4, 1,4, 2,4, 0,5, 1,5, 2,5} ) };
        }

        [Theory]
        [MemberData(nameof(get_all_coordinates_for_block_true_input))]
        public void get_all_coordinates_for_block_true(int x, int y, CoordinateList expected_coords)
        {
            CoordinateList coords = SudokuGrid.get_all_coordinates_for_block(x, y);
            Assert.True(coords.Equals(expected_coords),coords.ToString());
        }

        public static IEnumerable<object[]> get_intersecting_blocks_true_input()
        {
            // block1 = {0,0, 1,0, 2,0, 0,1, 1,1, 1,2, 0,2, 1,2, 2,2}
            // block4 = {0,3, 1,3, 2,3, 0,4, 1,4, 1,4, 0,5, 1,5, 2,5}

            yield return new object[] {
                new CoordinateList( new int[] {2,3} ),
                new List<CoordinateList> { new CoordinateList( new int[] {0,3, 1,3, 2,3, 0,4, 1,4, 2,4, 0,5, 1,5, 2,5} ) },
                "single coordinate"
            };
            yield return new object[] {
                new CoordinateList( new int[] {2,3, 2,8} ),
                new List<CoordinateList> { new CoordinateList( new int[] {0,3, 1,3, 2,3, 0,4, 1,4, 2,4, 0,5, 1,5, 2,5 } ),
                                           new CoordinateList( new int[] {0,6, 1,6, 2,6, 0,7, 1,7, 2,7, 0,8, 1,8, 2,8 } ) 
                                         },
                "two coordinates in different blocks"
            };
            yield return new object[] {
                new CoordinateList( new int[] {2,3, 2,7, 2,8} ),
                new List<CoordinateList> { new CoordinateList( new int[] {0,3, 1,3, 2,3, 0,4, 1,4, 2,4, 0,5, 1,5, 2,5 } ),
                                           new CoordinateList( new int[] {0,6, 1,6, 2,6, 0,7, 1,7, 2,7, 0,8, 1,8, 2,8 } )
                                         },
                "three coordinates in two different blocks"
            };
            yield return new object[] {
                new CoordinateList( new int[] {2,6, 2,7, 2,8} ),
                new List<CoordinateList> { new CoordinateList( new int[] {0,6, 1,6, 2,6, 0,7, 1,7, 2,7, 0,8, 1,8, 2,8 } )
                                         },
                "three coordinates in single block"
            };
        }

        [Theory]
        [MemberData(nameof(get_intersecting_blocks_true_input))]
        public void get_intersecting_blocks_true(CoordinateList reference_cells, List<CoordinateList> expected_coords, string message)
        {
            List<CoordinateList> block_coords = SudokuGrid.get_intersecting_blocks(reference_cells);

            bool match_found = false;
            bool match_found_for_all = true;

            foreach (CoordinateList expected_block in expected_coords)
            {
                match_found = false;
                foreach(CoordinateList found_block in block_coords)
                {
                    if( expected_block.Equals(found_block))
                    {
                        match_found = true;
                    }
                 }

                if (!match_found)
                {
                    match_found_for_all = false;
                }
            }

            Assert.True(match_found_for_all && (expected_coords.Count() == block_coords.Count()), $"{message}; matches found={match_found_for_all}; expected count={expected_coords.Count()}; found count={block_coords.Count()}");
        }

        public static IEnumerable<object[]> get_values_for_coordinates_test_input()
        {
            yield return new object[] {
                "289765431|317924856|645138729||763891542|521473968|894652173||432519687|956387214|178246395",
                new CoordinateList(new int[] {2,3 } ),
                new List<int> { 3 },
                "1"};
            yield return new object[] {
                "289765431|317924856|645138729||763891542|521473968|894652173||432519687|956387214|178246395",
                new CoordinateList(new int[] {0,2, 1,2, 2,2, 3,2, 4,2, 5,2, 6,2, 7,2, 8,2} ),
                new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                "2" };
        }

        [Theory]
        [MemberData(nameof(get_values_for_coordinates_test_input))]
        public void get_values_for_coordinates_test(string puzzle, CoordinateList coords, List<int> expected_values, string message)
        {

            SudokuGrid grid = new SudokuGrid(puzzle);
            //List<int> values_found = SudokuGrid.get_values_for_coordinates(coords, grid._grid_cells);

            //var firstNotSecond = values_found.Except(expected_values).ToList();
            //var secondNotFirst = expected_values.Except(values_found).ToList();

            Assert.True(SudokuHelper.are_int_lists_equal(SudokuGrid.get_values_for_coordinates(coords, grid._grid_cells), expected_values), message);
        }

        public static IEnumerable<object[]> get_possible_values_for_cell_test_input()
        {
            yield return new object[] {
                "000000000|000000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000",
                2, 3,
                new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                "1"};
            yield return new object[] {
                "002000000|000000000|000000000||000000000|001000000|300000000||000000000|000000000|000000000",
                2, 3,
                new List<int> { 4, 5, 6, 7, 8, 9 },
                "2"};
        }

        [Theory]
        [MemberData(nameof(get_possible_values_for_cell_test_input))]
        public void get_possible_values_for_cell_test(string puzzle, int x, int y, List<int> expected_values, string message)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);
            //List<int> values_found = SudokuGrid.calculate_possible_values_for_cell(x, y, grid._grid_cells);

            //var firstNotSecond = values_found.Except(expected_values).ToList();
            //var secondNotFirst = expected_values.Except(values_found).ToList();

            //Assert.True(values_found.Count == expected_values.Count && !firstNotSecond.Any() && !secondNotFirst.Any(), message);
            Assert.True(SudokuHelper.are_int_lists_equal(SudokuGrid.calculate_possible_values_for_cell(x, y, grid._grid_cells), expected_values), message);
        }

        public static IEnumerable<object[]> set_possible_values_of_all_cells_test_input()
        {
            yield return new object[] {
                "019600005|607840910|840219307|328196070|076300098|001500623||053002080|060458731|704031050",
                2, 2,
                new List<int> { 5 },
                "1"};
            yield return new object[] {
                "019600005|607840910|840219307|328196070|076300098|001500623||053002080|060458731|704031050",
                0, 5,
                new List<int> { 4, 9 },
                "2"};
        }

        [Theory]
        [MemberData(nameof(set_possible_values_of_all_cells_test_input))]
        public void set_possible_values_of_all_cells_test(string puzzle, int x, int y, List<int> expected_values, string message)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);
            grid.set_possible_values_of_all_cells();

            //List<int> target_possibles = grid._grid_cells[x, y]._possible_values;

            //var firstNotSecond = target_possibles.Except(expected_values).ToList();
            //var secondNotFirst = expected_values.Except(target_possibles).ToList();

            //Assert.True(target_possibles.Count == expected_values.Count && !firstNotSecond.Any() && !secondNotFirst.Any(), message);
            Assert.True(SudokuHelper.are_int_lists_equal(grid._grid_cells[x, y]._possible_values, expected_values), message);
        }

        public static IEnumerable<object[]> remove_value_from_permitted_values_in_cells_test_true_input()
        {
            string puzzle = "000000000|000000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000";
            SudokuGrid grid_in = new SudokuGrid(puzzle);

            grid_in._grid_cells[1, 1]._possible_values = new List<int> { 1, 2, 3 };
            grid_in._grid_cells[2, 2]._possible_values = new List<int> { 2 };
            grid_in._grid_cells[3, 3]._possible_values = new List<int> { 1, 3 };
            grid_in._grid_cells[4, 4]._possible_values = new List<int> { 1, 2, 3 };

            SudokuGrid grid_out = (SudokuGrid)grid_in.Clone();
            grid_out._grid_cells[1, 1]._possible_values = new List<int> { 1, 3 };
            grid_out._grid_cells[2, 2]._possible_values = new List<int> ();
            grid_out._grid_cells[3, 3]._possible_values = new List<int> { 1, 3 };

            yield return new object[] {
                grid_in,
                2,
                new CoordinateList(new int[] {1,1, 2,2, 3,3 }),
                grid_out,
                "1"};
        }

        [Theory]
        [MemberData(nameof(remove_value_from_permitted_values_in_cells_test_true_input))]
        public void remove_value_from_permitted_values_in_cells_test_true(SudokuGrid grid_in, int value_to_remove, CoordinateList affected_cells, SudokuGrid grid_out, string message)
        {
            grid_in._grid_cells = SudokuGrid.remove_value_from_permitted_values_in_cells(grid_in._grid_cells,value_to_remove,affected_cells);
            Assert.True(grid_in.Equals(grid_out),message);
        }

        [Fact]
        public void get_coordinates_for_all_shapes_test()
        {
            (CoordinateList[] rows, CoordinateList[] cols, CoordinateList[] blocks) lists = SudokuGrid.get_coordinates_for_all_shapes();
            // row1 = {0,0, 1,0, 2,0, 3,0, 4,0, 5,0, 6,0, 7,0, 8,0}
            // row2 = {0,1, 1,1, 2,1, 3,1, 4,1, 5,1, 6,1, 7,1, 8,1}
            // row3 = {0,2, 1,2, 2,2, 3,2, 4,2, 5,2, 6,2, 7,2, 8,2}
            // row4 = {0,3, 1,3, 2,3, 3,3, 4,3, 5,3, 6,3, 7,3, 8,3}
            Assert.True(lists.rows[0].Equals(new CoordinateList(new int[] { 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8, 0 })));

            Assert.True(lists.rows[3].Equals(new CoordinateList(new int[] { 0, 3, 1, 3, 2, 3, 3, 3, 4, 3, 5, 3, 6, 3, 7, 3, 8, 3 })));
        }

        [Fact]
        public void get_coordinates_where_values_are_possible_test()
        {
            SudokuGrid grid = new SudokuGrid("000000000|000000000|000000000||000000000|000000000|000000000||000000000|000000000|000000000");
            grid._grid_cells[0, 0]._possible_values = new List<int> {1, 2, 3, 4, 5, 6};
            grid._grid_cells[0, 1]._possible_values = new List<int> { 2 };
            grid._grid_cells[0, 2]._possible_values = new List<int> { 2, 5 };
            grid._grid_cells[1, 1]._possible_values = new List<int> { 3, 9 };
            grid._grid_cells[2, 2]._possible_values = new List<int> { 2, 3, 9 };

            HashSet<int> search_vals = new HashSet<int> {2};
            CoordinateList search_coords = new CoordinateList(new int[] {0,0, 0,1, 1,1, 0,2, 7,7 });

            CoordinateList coords_found = SudokuGrid.get_coordinates_where_values_are_possible(grid._grid_cells, search_vals, search_coords);

            Assert.True(coords_found.Equals(new CoordinateList(new int[] {0,0, 0,1, 0,2 })));
            Assert.False(coords_found.Equals(new CoordinateList(new int[] { 0,0 })));

            search_vals = new HashSet<int> { 2, 5 };
            
            coords_found = SudokuGrid.get_coordinates_where_values_are_possible(grid._grid_cells, search_vals, search_coords);

            Assert.True(coords_found.Equals(new CoordinateList(new int[] { 0,0, 0,2 })), "Find pair of values");
        }
        //        public static CoordinateList get_coordinates_where_values_are_possible(Cell[,] cells, HashSet<int> values_to_look_for, CoordinateList coords_to_check)

        [Fact]
        public void clone_test()
        {
            string puzzle = "140060800085010040907400250030070400209000307008900060000740010601305090700002600";
            SudokuGrid grid = new SudokuGrid(puzzle);
            grid.set_possible_values_of_all_cells();

            SudokuGrid grid2 = (SudokuGrid)grid.Clone();

            // the cloned grid is equal to the first
            Assert.True(grid2.Equals(grid));

            grid2._grid_cells[2, 1]._possible_values.Add(4);
            Assert.False(grid2.Equals(grid), "changing a possible value in one grid should not also reflect on the other grid");

            grid2 = (SudokuGrid)grid.Clone();
            Assert.True(grid2.Equals(grid), "setting the grid back to a fresh clone should make them equal again");
        }
    }

}