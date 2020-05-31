using System.Collections.Generic;
using SudokuSolver;
using Xunit;
using System.Linq;

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
            List<int> values_found = SudokuGrid.get_values_for_coordinates(coords, grid._grid_cells);

            var firstNotSecond = values_found.Except(expected_values).ToList();
            var secondNotFirst = expected_values.Except(values_found).ToList();

            Assert.True(values_found.Count == expected_values.Count && !firstNotSecond.Any() && !secondNotFirst.Any(), message);
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
            List<int> values_found = SudokuGrid.get_possible_values_for_cell(x, y, grid._grid_cells);

            var firstNotSecond = values_found.Except(expected_values).ToList();
            var secondNotFirst = expected_values.Except(values_found).ToList();

            Assert.True(values_found.Count == expected_values.Count && !firstNotSecond.Any() && !secondNotFirst.Any(), message);
        }

        public static IEnumerable<object[]> set_possible_values_of_all_cells_input()
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
        [MemberData(nameof(set_possible_values_of_all_cells_input))]
        public void set_possible_values_of_all_cells(string puzzle, int x, int y, List<int> expected_values, string message)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);
            grid.set_possible_values_of_all_cells();

            List<int> target_possibles = grid._grid_cells[x, y]._possible_values;

            var firstNotSecond = target_possibles.Except(expected_values).ToList();
            var secondNotFirst = expected_values.Except(target_possibles).ToList();

            Assert.True(target_possibles.Count == expected_values.Count && !firstNotSecond.Any() && !secondNotFirst.Any(), message);
        }

    }

    }