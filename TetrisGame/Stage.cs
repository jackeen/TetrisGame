using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisGame
{

    /// <summary>
    /// The stage for checking collision between blocks and borders.
    /// Also for checking game ending and clean the full pixels line for score up
    /// </summary>
    internal class Stage
    {
        private int width, height;
        private DotScreen screen;

        // current moving block in the screen
        private Block activeBlock;

        // all fixed pixels stored here
        private HashSet<(int, int)>[] existedPointsFromBlock;

        // events for trigger game's behavior
        public event EventHandler linesNotEmpty;
        public event EventHandler activeblockDie;
        public event EventHandler<LineCleanEventArgs> linesCleaned;

        public Stage(int h, int w, DotScreen ds)
        {
            width = w;
            height = h;
            screen = ds;

            InitExistedPoints();
        }


        /// <summary>
        /// Initialize the storage for fixed pixels
        /// </summary>
        private void InitExistedPoints()
        {
            // init bottom pixels expressed by (int y, int x)
            // and each line wrapped in a set
            existedPointsFromBlock = new HashSet<(int, int)>[height];
            for (int y = 0; y < height; y++)
            {
                existedPointsFromBlock[y] = new HashSet<(int, int)>();
            }
        }

        /// <summary>
        /// Set current moving block's border based on its width and hight
        /// </summary>
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
            DrawActiveBlockHighLight();
        }

        /// <summary>
        /// record the current position and its current shape of the screen in the queue
        /// </summary>
        private void recordHistoryPosition()
        {
            activeBlock.historyPoint.Enqueue((activeBlock.Y, activeBlock.X, activeBlock.ShapeNum));
        }

        /// <summary>
        /// Move the current moving block to right step and check the border not going outside
        /// </summary>
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

        /// <summary>
        /// Move the current moving block to left step and limit it not going outside
        /// </summary>
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

        /// <summary>
        /// Move the current moving block down step, 
        /// and check it wether or not hit the existed pixels or the bottom of the screen.
        /// </summary>
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

        /// <summary>
        /// When find out some collision on bottom of screen or existed pixels,
        /// This will invoke for fixing the current block and checking full line to clean.
        /// </summary>
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

        /// <summary>
        /// Detect there is no empty line for gaming 
        /// </summary>
        private void detectAllLinesNotEmpty()
        {
            HashSet<(int, int)> firstLine = existedPointsFromBlock[0];

            if (firstLine.Count != 0)
            {
                linesNotEmpty?.Invoke(this, EventArgs.Empty);
            }
            
        }

        /// <summary>
        /// Detect the all lines in the screen,
        /// If there are full line existed, delete them and redraw the screen
        /// </summary>
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

        /// <summary>
        /// Reset the all pixels in the screen
        /// </summary>
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

        /// <summary>
        /// detext collision in every line
        /// </summary>
        /// <param name="blockY"></param>
        /// <param name="blockX"></param>
        /// <returns></returns>
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


        /// <summary>
        /// put all pixels of the active block to fixed group
        /// </summary>
        private void FillBlockToExistedGroup()
        {
            foreach ((int y, int x) pixel in activeBlock.Data)
            {
                int absY = pixel.y + activeBlock.Y;
                int absX = pixel.x + activeBlock.X;
                existedPointsFromBlock[absY].Add((absY, absX));
            }
            DrawActiveBlock();
        }

        /// <summary>
        /// Rotate the moving block,
        /// Before do it record current positon in the history queue
        /// After rotating, recalculate the limitation of the block
        /// </summary>
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

        /// <summary>
        /// Clean the trace history in the queue, after the block moving 
        /// </summary>
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

        /// <summary>
        /// Draw the moving block at the screen
        /// </summary>
        public void DrawActiveBlock()
        {
            activeBlock.Data.ForEach(((int y, int x) p) =>
            {
                (int, int) absPixel = (p.y + activeBlock.Y, p.x + activeBlock.X);
                screen.Draw(absPixel);
            });
        }

        /// <summary>
        /// Draw the moving block at the screen by hightlight color
        /// </summary>
        public void DrawActiveBlockHighLight()
        {
            activeBlock.Data.ForEach(((int y, int x) p) =>
            {
                (int, int) absPixel = (p.y + activeBlock.Y, p.x + activeBlock.X);
                screen.Draw(absPixel, Color.DarkCyan);
            });
        }

        /// <summary>
        /// Update the active block based on its new position
        /// </summary>
        public void Update()
        {
            if (activeBlock != null && activeBlock.isMoving)
            {
                CleanActiveBlock();
                DrawActiveBlockHighLight();
            }
        }

        /// <summary>
        /// Return wether or not existed the active block
        /// </summary>
        /// <returns></returns>
        public bool HasActiveBlock()
        {
            return activeBlock != null;
        }

        /// <summary>
        /// Reset the stage for new game
        /// </summary>
        public void Reset()
        {
            InitExistedPoints();
            activeBlock = null;
            screen.Reset();
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
