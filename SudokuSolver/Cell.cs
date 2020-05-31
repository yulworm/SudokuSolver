using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public class Cell
    {
        public int _value { get; set; }
        //bool _value_is_certain { get; set; }
        public List<int> _possible_values { get; set; }

        public Cell() : this(0)
        {
        }
        
        public Cell( int p_value ) 
        {
            _value = p_value;
          //  _value_is_certain = true;
        }
    }
}
