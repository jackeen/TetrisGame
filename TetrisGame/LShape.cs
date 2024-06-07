using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    internal class LShape: Block
    {

        public LShape()
        {
            Data = new List<(int, int)>()
            {
                (0, 0),
                (1, 0),
                (2, 0),
                (2, 1),
            };
        }




    }
}
