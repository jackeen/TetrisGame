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
            block.X = x;
            block.Y = y;
            activeBlock = block;

            SetLimitForActiveBlock();
            recordHistoryPosition();
            DrawActiveBlock();
        }

        private void recordHistoryPosition()
        {
            activeBlock.historyPoint.Enqueue((activeBlock.Y, activeBlock.X));
        }

        public void MoveActiveToRight()
        {
            recordHistoryPosition();

            int x = activeBlock.X + 1;
            if (x > activeBlock.limitX)
            {
                x = activeBlock.limitX;
            }
            activeBlock.X = x;
        }

        public void MoveActiveToLeft()
        {
            recordHistoryPosition();

            int x = activeBlock.X - 1;
            if (x < 0)
            {
                x = 0;
            }
            activeBlock.X = x;
        }

        public void MoveActiveDown()
        {
            recordHistoryPosition();

            int y = activeBlock.Y + 1;
            if (y > activeBlock.limitY)
            {
                y = activeBlock.limitY;
            }
            activeBlock.Y = y;
        }

        public void CleanActiveBlock()
        {
            // consume the history of movement
            if (activeBlock.historyPoint.Count > 0)
            {
                (int y, int x) blockPoint = activeBlock.historyPoint.Dequeue();
                activeBlock.Data.ForEach(((int y, int x) p) =>
                {
                    (int, int) absPixel = (p.y + blockPoint.y, p.x + blockPoint.x);
                    screen.Clean(absPixel);
                });
            }

        }

        public void DrawActiveBlock()
        {
            activeBlock.Data.ForEach(((int y, int x) p) =>
            {
                (int, int) absPixel = (p.y + activeBlock.Y, p.x + activeBlock.X);
                screen.Draw(absPixel);
            });
        }

        public void Update()
        {
            CleanActiveBlock();
            DrawActiveBlock();
        }
    }
}
