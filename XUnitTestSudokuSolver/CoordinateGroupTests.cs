using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SudokuSolver;

namespace XUnitTestSudokuSolver
{
    public class CoordinateGroupTests
    {

        [Fact]
        public void init_and_toString_work_std_input()
        {
            CoordinateList cg = new CoordinateList(new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 });

            Assert.True(cg.ToString().Equals("(0,0) (0,1) (0,2) (0,3) (0,4) (0,5) (0,6) (0,7) (0,8) "));
        }

        [Fact]
        public void init_and_toString_work_empty_input()
        {
            CoordinateList cg = new CoordinateList();

            Assert.True(cg.ToString().Equals(""));
        }

        public static IEnumerable<object[]> GetEqualTrueGroups()
        {
            yield return new object[] { 
                new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 },
                new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 } };
            yield return new object[] {
                new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 9 },
                new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 9 } };
            //yield return new object[] { 7, 1, 5, 3 };
        }

        [Theory]
        [MemberData(nameof(GetEqualTrueGroups))]
        public void equals_true(int[] coords1, int[] coords2)
        {
            CoordinateList cg1 = new CoordinateList(coords1);
            CoordinateList cg2 = new CoordinateList(coords2);

            Assert.True(cg1.Equals(cg2));
        }

        public static IEnumerable<object[]> GetEqualFalseGroups()
        {
            yield return new object[] {
                new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 },
                new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 2 } };
            yield return new object[] {
                new int[] { 0, 0, 0, 1, 0, 2, 1, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 9 },
                new int[] { 0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 9 } };
            //yield return new object[] { 7, 1, 5, 3 };
        }

        [Theory]
        [MemberData(nameof(GetEqualFalseGroups))]
        public void equals_false(int[] coords1, int[] coords2)
        {
            CoordinateList cg1 = new CoordinateList(coords1);
            CoordinateList cg2 = new CoordinateList(coords2);

            Assert.False(cg1.Equals(cg2));
        }
    }
}