﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{

    /// <summary>
    /// This module defines some behaviours of the blocks
    /// All specific block inherit form this class
    /// </summary>
    internal class Block
    {
        // current shape data
        public List<(int, int)> Data;
        
        // as a block, they have 4 shapes normally
        public List<(int, int)>[] Shapes;

        // the current shape number to index a shape in "shapes"
        public int ShapeNum;

        // to record the movement history for a block
        // for updating movement
        public Queue<(int, int, int)> historyPoint;

        // the X and Y border for limiting the block moving 
        public int limitY, limitX;

        // current position of the block 
        public int Y, X;

        public bool isMoving = true;

        public Block()
        {
            historyPoint = new Queue<(int, int, int)>();
            Shapes = new List<(int, int)>[4];

            RandomRotate();
        }

        /// <summary>
        /// Random the block in its shape by rotating
        /// </summary>
        private void RandomRotate()
        {
            Random r = new Random();
            ShapeNum = r.Next(0, 3);
        }

        /// <summary>
        /// Return the size combined by width and hight gouped by a tuple
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Select a shape in 4 after rotating
        /// </summary>
        public void Rotate()
        {
            ShapeNum++;
            if (ShapeNum > 3)
            {
                ShapeNum = 0;
            }

            Data = Shapes[ShapeNum];
        }
    }
}
