using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    internal class HShape: Block
    {
        public HShape()
        {
            Shapes[0] = new List<(int, int)>()
            {
                (0, 1),
                (1, 0),
                (1, 1),
                (1, 2),
            };

            Shapes[1] = new List<(int, int)>()
            {
                (0, 0),
                (1, 0),
                (1, 1),
                (2, 0),
            };

            Shapes[2] = new List<(int, int)>()
            {
                (0, 0),
                (0, 1),
                (0, 2),
                (1, 1),
            };

            Shapes[3] = new List<(int, int)>()
            {
                (0, 1),
                (1, 0),
                (1, 1),
                (2, 1),
            };

            Data = Shapes[ShapeNum];
        }
    }
}
