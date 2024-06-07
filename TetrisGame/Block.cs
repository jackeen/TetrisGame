using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    internal class Block
    {
        public List<(int, int)> Data;
        public int Y, X;
        public int Y0, X0;

        public int limitY, limitX;

        public Block()
        {

        }

        public (int, int) GetSize()
        {
            int h = 0;
            int w = 0;

            Data.ForEach((p) =>
            {
                h = Math.Max(h, p.Item1);
                w = Math.Max(w, p.Item2);
            });

            return (h + 1, w + 1);
        }
    }
}
