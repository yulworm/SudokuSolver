using System;
using System.Collections.Generic;
using SudokuSolver;
using Xunit;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace XUnitTestSudokuSolver
{
    public class SudokuHelperTests
    {
        [Fact]
        public void find_hidden_singles_test()
        {
            SudokuGrid grid = new SudokuGrid("000000000|000200000|000000000||000060000|000000000|000080000||000000000|000000000|000002000");
            grid.set_possible_values_of_all_cells();

            HashSet<(int, int, int)> results = SudokuHelper.find_hidden_singles(grid._grid_cells);
            HashSet<(int, int, int)> expected_results = new HashSet<(int, int, int)> { (4, 4, 2) };

            int a_not_b = results.Except(expected_results).Count();
            int b_not_a = expected_results.Except(results).Count();

            Assert.True(results.Count == expected_results.Count && a_not_b == 0 && b_not_a == 0);
        }

        [Fact]
        public void find_pointing_pair_test()
        {
            SudokuGrid grid = new SudokuGrid("306000000|000070000|405000000||000000000|000000000|000000000||000000000|000000000|000000000");
            grid.set_possible_values_of_all_cells();
            HashSet<(int, int, int)> results = SudokuHelper.find_pointing_pairs(grid._grid_cells);
            HashSet<(int, int, int)> expected_results = new HashSet<(int, int, int)>{ (1, 3, 7), (1, 4, 7), (1, 5, 7), (1, 6, 7), (1, 7, 7), (1, 8, 7) };

            int a_not_b = results.Except(expected_results).Count();
            int b_not_a = expected_results.Except(results).Count();

            Assert.True(results.Count == expected_results.Count && a_not_b == 0 && b_not_a == 0);
        }

        public static IEnumerable<object[]> are_lists_equal_true_input()
        {
            yield return new object[] {
                new List<int> { 5 },
                new List<int> { 5 },
                "1"};
        }

        [Theory]
        [MemberData(nameof(are_lists_equal_true_input))]
        public void are_lists_equal_true(List<int> a, List<int> b, string message)
        {
            Assert.True(SudokuHelper.are_int_lists_equal(a, b), message );
        }

        public static IEnumerable<object[]> are_lists_equal_false_input()
        {
            yield return new object[] {
                new List<int> { 5 },
                new List<int> { 6 },
                "1"};
        }

        [Theory]
        [MemberData(nameof(are_lists_equal_false_input))]
        public void are_lists_equal_false(List<int> a, List<int> b, string message)
        {
            Assert.False(SudokuHelper.are_int_lists_equal(a, b), message);
        }

        [Theory]
        [InlineData("019600005|607840910|840219307|328196070|076300098|001500623||053002080|060458731|704031050","Naked singles only")]
        [InlineData("000000120240010000901004000400003650000090000036400001000100506000050043072000000", "Naked and hidden singles")]
        [InlineData("003700050070050800100006004502000000800904006000000902300500007004090060020007400", "Block and row or column")]
        [InlineData("140060800085010040907400250030070400209000307008900060000740010601305090700002600", "unknown")]
        [InlineData("000003948309008500004000002500900000007010600000007001600000100008700209175300000", "Block Block 1")]
        [InlineData("879126354|136954782|542873060||680090040|720468010|490030806||968345000|257619438|314287695", "xy wing 1")]
        [InlineData("090000000000000678000063050000300000008050201005290300009005000800034000302008004", "xy wing 2")]
        [InlineData("684070000300000070000510000800400100051080960007006002000045000090000005000020843", "xy wing 3")]
        public void solve_puzzle_test_true(string puzzle, string test_message)
        {
            SudokuGrid result = SudokuHelper.solve_puzzle(puzzle);

            Assert.True(result.is_grid_solved(), $"{test_message} grid not solved. {System.Environment.NewLine}{result.ToStringFormatted()}");
            Assert.True(result.is_grid_valid(), $"{test_message} grid is invalid. {System.Environment.NewLine}{result.ToStringFormatted()}");
        }

        public static IEnumerable<object[]> find_block_block_interactions_test_input()
        {
            yield return new object[] {
                "003000000|000425000|000001000||000904000|030000000|000076000||000000000|000000000|000000000",
                new HashSet<(int, int, int)> { (3, 6, 3), (4, 6, 3), (3, 7, 3), (4, 7, 3), (3, 8, 3), (4, 8, 3), (7, 2, 3), (8, 2, 3), (6, 2, 3) },
                "1"};
            yield return new object[] {
                "000000000|000000000|000000002||000000000|560000430|000000000||000000000|002000000|000000000",
                new HashSet<(int, int, int)> { (3, 3, 2), (4, 3, 2), (5, 3, 2), (3, 5, 2), (4, 5, 2), (5, 5, 2) },
                "2"};
        }

        [Theory]
        [MemberData(nameof(find_block_block_interactions_test_input))]
        public void find_block_block_interactions_test(string puzzle, HashSet<(int, int, int)> expected_results, string message)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);
            grid.set_possible_values_of_all_cells();
            HashSet<(int, int, int)> results = SudokuHelper.find_block_block_interactions(grid._grid_cells);

            int a_not_b = results.Except(expected_results).Count();
            int b_not_a = expected_results.Except(results).Count();

            Assert.True(results.Count == expected_results.Count && a_not_b == 0 && b_not_a == 0, $"{message} results={SudokuHelper.format_coord_and_value_hashset(results)};");
        }

        public static IEnumerable<object[]> find_x_y_wing_test_input()
        {
            yield return new object[] {
                "879126354|136954782|542873060||680090040|720468010|490030806||968345000|257619438|314287695",
                new HashSet<(int, int, int)> { (8, 3, 7), (3, 5, 7), (3, 3, 5) },
                "1"};
        }

        [Theory]
        [MemberData(nameof(find_x_y_wing_test_input))]
        public void find_x_y_wing_test(string puzzle, HashSet<(int, int, int)> expected_results, string message)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);
            grid.set_possible_values_of_all_cells();
            HashSet<(int, int, int)> results = SudokuHelper.find_x_y_wing(grid._grid_cells);

            int a_not_b = results.Except(expected_results).Count();
            int b_not_a = expected_results.Except(results).Count();

            Assert.True(results.Count == expected_results.Count && a_not_b == 0 && b_not_a == 0, $"{message} results={SudokuHelper.format_coord_and_value_hashset(results)};");
        }

        public static IEnumerable<object[]> add_link_to_chain_test_input()
        {
            yield return new object[] {
                "879126354|136954782|542873060||680090040|720468010|490030806||968345000|257619438|314287695",
                new CoordinateList(new int[] { 3,3}),
                new CoordinateList(new int[] { 3,3, 6,3}),
                "1"};
            yield return new object[] {
                "879126354|136954782|542873060||680090040|720468010|490030806||968345000|257619438|314287695",
                new CoordinateList(new int[] { 3,3, 6,3}),
                new CoordinateList(new int[] { 3,3, 6,3, 7,5}),
                "2"};
            yield return new object[] {
                "879126354|136954782|542873060||680090040|720468010|490030806||968345000|257619438|314287695",
                new CoordinateList(new int[] { 3,3, 6,3, 7,5}),
                new CoordinateList(new int[] { 3,3, 6,3, 7,5, 3,5}),
                "3"};
        }

        [Theory]
        [MemberData(nameof(add_link_to_chain_test_input))]
        public void add_link_to_chain_test(string puzzle, CoordinateList start_chain, CoordinateList expected_chain, string message)
        {
            SudokuGrid grid = new SudokuGrid(puzzle);
            grid.set_possible_values_of_all_cells();
            List<CoordinateList> found_chains = SudokuHelper.add_link_to_chain(grid._grid_cells, start_chain);

            bool expected_chain_found = false;
            string found_chains_string = "";

            foreach (CoordinateList chain in found_chains)
            {
                if (chain.Equals(expected_chain))
                {
                    expected_chain_found = true;
                }

                found_chains_string += chain.ToString() + System.Environment.NewLine;
            }

            Assert.True(expected_chain_found, $"{message} expected: {System.Environment.NewLine}{expected_chain.ToString()}{System.Environment.NewLine}-----------{System.Environment.NewLine}found:{System.Environment.NewLine}{found_chains_string}");
        }

    }
}
