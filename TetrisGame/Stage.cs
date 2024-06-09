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

        private HashSet<(int, int)>[] existedPoint;

        //private bool activeBlockDisconnected = false;
        public event EventHandler blockStucked;

        public Stage(int h, int w, DotScreen ds)
        {
            width = w;
            height = h;
            screen = ds;

            // init bottom pixels expressed by (int y, int x)
            // and each line wrapped in a set
            existedPoint = new HashSet<(int, int)>[h];
            for (int y = 0; y < h; y++)
            {
                existedPoint[y] = new HashSet<(int, int)>();
            }
        }

        private void SetLimitForActiveBlock()
        {
            (int h, int w) size = activeBlock.GetSize();
            activeBlock.limitX = width - size.w;
            activeBlock.limitY = height - size.h;
        }

        public void DropBlock(int y, int x, Block block)
        {
            block.X = x;
            block.Y = y;
            activeBlock = block;
            //activeBlockDisconnected = false;

            SetLimitForActiveBlock();
            recordHistoryPosition();
            DrawActiveBlock();
        }

        private void recordHistoryPosition()
        {
            activeBlock.historyPoint.Enqueue((activeBlock.Y, activeBlock.X, activeBlock.ShapeNum));
        }

        public void MoveActiveToRight()
        {
            /*if (activeBlock == null)
            {
                return;
            }*/

            recordHistoryPosition();

            int x = activeBlock.X + 1;

            if (detectCollisionX(activeBlock.Y, x))
            {
                return;
            }

            if (x > activeBlock.limitX)
            {
                x = activeBlock.limitX;
            }
            activeBlock.X = x;
        }

        public void MoveActiveToLeft()
        {
            /*if (activeBlock == null)
            {
                return;
            }*/

            recordHistoryPosition();

            int x = activeBlock.X - 1;

            if (detectCollisionX(activeBlock.Y, x))
            {
                return;
            }

            if (x < 0)
            {
                x = 0;
            }
            activeBlock.X = x;
        }

        public void MoveActiveDown()
        {
            /*if (activeBlock == null)
            {
                return;
            }*/
            
            recordHistoryPosition();

            int y = activeBlock.Y + 1;
            int x = activeBlock.X;

            // hit some existed block
            bool res = detectCollisionY(y, x);
            if (res)
            {
                FillBlockToExistedGroup();
                
                blockStucked?.Invoke(this, EventArgs.Empty);
                return;
            }

            // hit bottom
            if (y > activeBlock.limitY)
            {
                y = activeBlock.limitY;
                FillBlockToExistedGroup();
                
                blockStucked?.Invoke(this, EventArgs.Empty);
            }
            activeBlock.Y = y;
            
        }

        private bool detectCollisionX(int blockY, int blockX)
        {

            for (int i = 0; i < height; i++)
            {
                HashSet<(int, int)> line = existedPoint[i];

                foreach ((int y, int x) pixel in activeBlock.Data)
                {
                    int absY = pixel.y + blockY;
                    int absX = pixel.x + blockX;

                    bool res = line.Contains((absY, absX));

                    if (res)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool detectCollisionY(int blockY, int blockX)
        {
            for (int i = 0; i < height; i++)
            {
                HashSet<(int, int)> line = existedPoint[i];

                foreach((int y, int x) pixel in activeBlock.Data)
                {
                    int absY = pixel.y + blockY;
                    int absX = pixel.x + blockX;

                    bool res = line.Contains((absY, absX));

                    if (res)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void FillBlockToExistedGroup()
        {
            foreach ((int y, int x) pixel in activeBlock.Data)
            {
                int absY = pixel.y + activeBlock.Y;
                int absX = pixel.x + activeBlock.X;
                existedPoint[absY].Add((absY, absX));
            }
        }

        public void RotateActiveBlock()
        {
            recordHistoryPosition();
            activeBlock.Rotate();
            SetLimitForActiveBlock();
        }

        public void CleanActiveBlock()
        {
            // consume the history of movement
            if (activeBlock.historyPoint.Count > 0)
            {
                (int y, int x, int shapeN) blockPoint = activeBlock.historyPoint.Dequeue();
                activeBlock.Shapes[blockPoint.shapeN].ForEach(((int y, int x) p) =>
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
            /*if (activeBlock != null)
            {
                
            }*/

            CleanActiveBlock();
            DrawActiveBlock();
        }
    }
}
