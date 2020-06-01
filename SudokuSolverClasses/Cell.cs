using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public class Cell : ICloneable
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
            _possible_values = new List<int>();
          //  _value_is_certain = true;
        }

        public Object Clone()
        {
            Cell cloned_cell = new Cell( this._value );
            cloned_cell._possible_values = new List<int>(this._possible_values);
            return cloned_cell;
        }
    }
}
