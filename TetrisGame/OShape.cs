using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    internal class OShape: Block
    {
        public OShape()
        {
            Shapes[0] = new List<(int, int)>()
            {
                (0, 0),
                (0, 1),
                (1, 0),
                (1, 1),
            };

            Shapes[1] = new List<(int, int)>()
            {
                (0, 0),
                (0, 1),
                (1, 0),
                (1, 1),
            };

            Shapes[2] = new List<(int, int)>()
            {
                (0, 0),
                (0, 1),
                (1, 0),
                (1, 1),
            };

            Shapes[3] = new List<(int, int)>()
            {
                (0, 0),
                (0, 1),
                (1, 0),
                (1, 1),
            };

            Data = Shapes[ShapeNum];
        }

    }
}
