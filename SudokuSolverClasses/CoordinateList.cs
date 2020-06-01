using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public class CoordinateList : IEnumerable<(int, int)>
    {
        List<(int,int)> coords;

        public CoordinateList() : this(new int[] { })
        {
            
        }
        public CoordinateList(int[] coordinates)
        {
            coords = new List<(int x, int y)>();
            int x = 0;
            int y = 0;

            for (int i=0; i<coordinates.Length; i++)
            {
                if ( i % 2 == 1 )
                {
                    y = coordinates[i];

                    Add(x, y);
                } else
                {
                    x = coordinates[i];
                }
            }
        }


        public void Add( int x, int y )
        {
            coords.Add((x, y));
        }

        public override bool Equals(object obj)
        {

            if (this.coords.Count != ((CoordinateList)obj).coords.Count)
            {
                return false;
            }

            bool found_match = false;
            foreach ((int x, int y) c1 in this.coords)
            {
                found_match = false;
                foreach ((int x, int y) c2 in ((CoordinateList)obj).coords)
                {
                    if ( c1 == c2 )
                    {
                        found_match = true;
                        break;
                    }
                }

                if (!found_match)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            string ret_val = "";

            foreach((int x, int y) c in coords )
            {
                ret_val += "(" + c.x + "," + c.y + ") ";
            }
            return ret_val;
        }

        public IEnumerator<(int, int)> GetEnumerator()
        {
            return coords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count()
        {
            return coords.Count;
        }

        public (int, int) get_coordinate(int index)
        {
            return (coords[index].Item1, coords[index].Item2);
        }
    }
}
