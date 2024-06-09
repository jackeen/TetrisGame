using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    internal class IShape: Block
    {
        public IShape()
        {
            Shapes[0] = new List<(int, int)>()
            {
                (0, 0),
                (1, 0),
                (2, 0),
                (3, 0),
            };

            Shapes[1] = new List<(int, int)>()
            {
                (0, 0),
                (0, 1),
                (0, 2),
                (0, 3),
            };

            Shapes[2] = new List<(int, int)>()
            {
                (0, 0),
                (1, 0),
                (2, 0),
                (3, 0),
            };

            Shapes[3] = new List<(int, int)>()
            {
                (0, 0),
                (0, 1),
                (0, 2),
                (0, 3),
            };

            Data = Shapes[ShapeNum];
        }
    }
}
