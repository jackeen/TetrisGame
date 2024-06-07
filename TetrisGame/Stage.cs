using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisGame
{
    internal class Stage
    {
        private int width, height;
        private DotScreen screen;
        private Block activeBlock;


        public Stage(int w, int h, DotScreen ds)
        {
            width = w;
            height = h;
            screen = ds;
        }

        private void SetLimitForActiveBlock()
        {
            (int h, int w) size = activeBlock.GetSize();
            activeBlock.limitX = width - size.w;
            activeBlock.limitY = height - size.h;

            MessageBox.Show($"h {size.h}, w {size.w}, width {width}, height {height}");
        }

        public void AddBlock(int y, int x, Block block)
        {
            block.X = block.X0 = x;
            block.Y = block.Y0 = y;
            block.Data.ForEach((p) =>
            {
                (int, int) absPixel = (p.Item1 + y, p.Item2 + x);
                screen.Draw(absPixel);
            });

            activeBlock = block;

            SetLimitForActiveBlock();
        }

        public void MoveActiveToRight()
        {
            int x = activeBlock.X + 1;
            if (x > activeBlock.limitX)
            {
                x = activeBlock.limitX;
            }
            activeBlock.X0 = activeBlock.X;
            activeBlock.X = x;
        }

        public void MoveActiveToLeft()
        {
            int x = activeBlock.X - 1;
            if (x < 0)
            {
                x = 0;
            }
            activeBlock.X0 = activeBlock.X;
            activeBlock.X = x;
        }

        public void Update()
        {
            activeBlock.Data.ForEach((p) =>
            {
                (int, int) absPixel = (p.Item1 + activeBlock.Y0, p.Item2 + activeBlock.X0);
                screen.Clean(absPixel);
            });

            activeBlock.Data.ForEach((p) =>
            {
                (int, int) absPixel = (p.Item1 + activeBlock.Y, p.Item2 + activeBlock.X);
                screen.Draw(absPixel);
            });
        }
    }
}
