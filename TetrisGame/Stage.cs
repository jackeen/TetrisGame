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

        // 
        private HashSet<(int, int)>[] existedPointsFromBlock;

        public event EventHandler linesNotEmpty;
        public event EventHandler activeblockDie;
        public event EventHandler<LineCleanEventArgs> linesCleaned;

        public Stage(int h, int w, DotScreen ds)
        {
            width = w;
            height = h;
            screen = ds;

            // init bottom pixels expressed by (int y, int x)
            // and each line wrapped in a set
            existedPointsFromBlock = new HashSet<(int, int)>[h];
            for (int y = 0; y < h; y++)
            {
                existedPointsFromBlock[y] = new HashSet<(int, int)>();
            }
        }

        private void SetLimitForActiveBlock()
        {
            (int h, int w) size = activeBlock.GetSize();
            activeBlock.limitX = width - size.w;
            activeBlock.limitY = height - size.h;
        }

        /// <summary>
        /// Drop a block to the stage for next controling by player
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="block"></param>
        public void DropBlock(int y, int x, Block block)
        {
            block.X = x;
            block.Y = y;
            activeBlock = block;

            SetLimitForActiveBlock();
            recordHistoryPosition();
            DrawActiveBlock();
        }

        /// <summary>
        /// record the current position and its current shape of the screen in the queue
        /// </summary>
        private void recordHistoryPosition()
        {
            activeBlock.historyPoint.Enqueue((activeBlock.Y, activeBlock.X, activeBlock.ShapeNum));
        }

        public void MoveActiveToRight()
        {
            if (activeBlock == null || !activeBlock.isMoving)
            {
                return;
            }

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
            if (activeBlock == null || !activeBlock.isMoving)
            {
                return;
            }

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
            if (activeBlock == null || !activeBlock.isMoving)
            {
                return;
            }
            
            recordHistoryPosition();

            int y = activeBlock.Y + 1;
            int x = activeBlock.X;

            Console.WriteLine($"move to {y} in Y");

            // hit some existed block
            bool res = detectCollisionY(y, x);
            if (res)
            {
                blockFixed();
                return;
            }

            // hit bottom
            if (y > activeBlock.limitY)
            {
                activeBlock.Y = activeBlock.limitY;
                blockFixed();
                return;
            }

            activeBlock.Y = y;
            
        }

        private void blockFixed()
        {
            if (!activeBlock.isMoving)
            {
                return;
            }
            activeBlock.isMoving = false;

            FillBlockToExistedGroup();
            detectAndCleanFullLines();
            detectAllLinesNotEmpty();

            activeblockDie?.Invoke(this, EventArgs.Empty);
        }

        private void detectAllLinesNotEmpty()
        {
            HashSet<(int, int)> firstLine = existedPointsFromBlock[0];

            if (firstLine.Count != 0)
            {
                linesNotEmpty?.Invoke(this, EventArgs.Empty);
            }
            
        }

        private void detectAndCleanFullLines()
        {
            int fullLineNum = 0;

            // store the line includes the number of point over 0 but not full
            Stack<HashSet<(int, int)>> temp = new Stack<HashSet<(int, int)>>();

            for (int i = 0; i < height; i++)
            {
                HashSet<(int, int)> line = existedPointsFromBlock[i];

                int lineElementNum = line.Count;
                if (lineElementNum == width)
                {
                    fullLineNum++;
                    line.Clear();
                    continue;
                }
                if (lineElementNum > 0)
                {
                    temp.Push(line);
                    existedPointsFromBlock[i] = new HashSet<(int, int)>();
                }
            }

            // copy not empty lines back and update its position data
            int index = height - 1;
            while (temp.Count > 0)
            {
                HashSet<(int, int)> oldLine = temp.Pop();
                HashSet<(int, int)> newLine = new HashSet<(int, int)>();

                foreach ((int y, int x) pixel in oldLine)
                {
                    newLine.Add((index, pixel.x));
                }

                existedPointsFromBlock[index] = newLine;
                index--;
            }

            if (fullLineNum > 0)
            {
                RedrawScreen();
                LineCleanEventArgs eventArgs = new LineCleanEventArgs();
                eventArgs.lineNumber = fullLineNum;
                linesCleaned?.Invoke(this, eventArgs);
            }
            
        }

        private void RedrawScreen()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    screen.Clean((i, j));
                }

                HashSet<(int, int)> line = existedPointsFromBlock[i];
                foreach ((int y, int x) pixel in line)
                {
                    screen.Draw(pixel);
                }
            }
        }

        private bool detectCollisionX(int blockY, int blockX)
        {

            for (int i = 0; i < height; i++)
            {
                HashSet<(int, int)> line = existedPointsFromBlock[i];

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
                HashSet<(int, int)> line = existedPointsFromBlock[i];

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
                existedPointsFromBlock[absY].Add((absY, absX));
            }
        }

        public void RotateActiveBlock()
        {

            if (activeBlock == null || !activeBlock.isMoving)
            {
                return;
            }

            recordHistoryPosition();
            activeBlock.Rotate();
            SetLimitForActiveBlock();

            // sometimes, the "Rotate" will cause the part of the block outside of the screen
            // this will fix this
            activeBlock.X = Math.Min(activeBlock.limitX, activeBlock.X);
        }

        public void CleanActiveBlock()
        {
            // clean the history of movement sign
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
            if (activeBlock != null && activeBlock.isMoving)
            {
                CleanActiveBlock();
                DrawActiveBlock();
            }
        }

        public void Reset()
        {

        }
    }

    public class LineCleanEventArgs: EventArgs
    {

        public int lineNumber;

        public LineCleanEventArgs()
        {

        }
    }
}
