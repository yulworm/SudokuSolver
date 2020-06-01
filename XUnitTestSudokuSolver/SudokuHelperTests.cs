using System;
using System.Collections.Generic;
using SudokuSolver;
using Xunit;
using System.Linq;

namespace XUnitTestSudokuSolver
{
    public class SudokuHelperTests
    {
        [Fact]
        public void find_hidden_singles_test()
        {
            SudokuGrid grid = new SudokuGrid("000000000|000200000|000000000||000060000|000000000|000080000||000000000|000000000|000002000");
            grid.set_possible_values_of_all_cells();
            Dictionary<(int x, int y), List<int>> results = SudokuHelper.find_hidden_singles(grid._grid_cells);
            Dictionary<(int x, int y), List<int>> expected_results = new Dictionary<(int x, int y), List<int>>() { { (4, 4), new List<int> { 2 } } };

            var a_not_b_keys = results.Keys.Except(expected_results.Keys);

            List<int> exp_result_value;
            List<int> actual_result_value;
            bool vals_same = true;
            foreach (var pair in results)
            {
                actual_result_value = pair.Value;
                expected_results.TryGetValue(pair.Key, out exp_result_value);

                if ( actual_result_value.Count != exp_result_value.Count ) {
                    vals_same = false;
                    break;
                }

                foreach(int v in actual_result_value)
                {
                    if ( !exp_result_value.Contains(v) )
                    {
                        vals_same = false;
                        break;
                    }
                }
            }

            Assert.True(results.Count == expected_results.Count && a_not_b_keys.Count() == 0 && vals_same);
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
            Assert.True(SudokuHelper.are_lists_equal(a, b), message );
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
            Assert.False(SudokuHelper.are_lists_equal(a, b), message);
        }

        [Theory]
        [InlineData("019600005|607840910|840219307|328196070|076300098|001500623||053002080|060458731|704031050", "219673845637845912845219367328196574576324198491587623153762489962458731784931256","Naked singles only")]
        [InlineData("000000120240010000901004000400003650000090000036400001000100506000050043072000000", "687539124243718965951264387419873652725691438836425791394182576168957243572346819", "Naked and hidden singles")]
        [InlineData("", "", "Naked and hidden singles")]
        public void solve_puzzle_test_true(string puzzle, string solution, string test_message)
        {
            Assert.True(SudokuHelper.solve_puzzle(puzzle).Equals(new SudokuGrid(solution)), test_message);
        }
    }
}
