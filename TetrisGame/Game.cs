﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisGame
{

    /// <summary>
    /// Game logic and the other user interface for controlling and interacting with the player
    /// </summary>
    public partial class Game : Form
    {

        private DotScreen screen;
        private Stage stage;

        private int score = 0;
        private bool isRunning = false;
        private bool isGameOver = false;

        private int timerUpdateTime = 50;
        private int timerMoveDownTime = 500;

        public Game()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initilize the dotscreen for the game, 
        /// </summary>
        private void SetpuPlayGround()
        {
            this.Width = 800;
            this.Height = 600;
            this.AutoSize = false;

            int pixelsX = 16;
            int pixelsY = 32;

            int offsetLeft = 50;
            int offsetTop = 20;

            DotScreen ds = new DotScreen(pixelsY, pixelsX, this);
            screen = ds;

            ds.SetupPixel(offsetLeft, offsetTop);
            screenBg.Left = offsetLeft;
            screenBg.Top = offsetTop;
            screenBg.Width = ds.GetWidth();
            screenBg.Height = ds.GetHeight();
            screenBg.SendToBack();

            stage = new Stage(pixelsY, pixelsX, screen);
            stage.activeblockDie += Stage_blockStucked;
            stage.linesCleaned += Stage_linesCleaned;
            stage.linesNotEmpty += Stage_linesNotEmpty;

            btnDown.MouseDown += BtnDown_MouseDown;
            btnDown.MouseUp += BtnDown_MouseUp;

            timerUpdate.Interval = timerUpdateTime;
            timerMoveDown.Interval = timerMoveDownTime;

            btnReset.Enabled = false;
        }

        /// <summary>
        /// GameOver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stage_linesNotEmpty(object sender, EventArgs e)
        {
            isGameOver = true;
            pauseGame(true);
            MessageBox.Show("Game over");
            btnReset.Enabled = true;
        }

        /// <summary>
        /// Speed up the block by speed up the timer
        /// </summary>
        private void DownSpeedUp()
        {
            timerMoveDown.Interval = timerMoveDownTime / 10;
        }

        /// <summary>
        /// Change the down speed to normal speed
        /// </summary>
        private void DownSpeedBackNormal()
        {
            timerMoveDown.Interval = timerMoveDownTime;
        }

        private void BtnDown_MouseUp(object sender, MouseEventArgs e)
        {
            DownSpeedBackNormal();
        }

        private void BtnDown_MouseDown(object sender, MouseEventArgs e)
        {
            DownSpeedUp();
        }

        private void Stage_linesCleaned(object sender, LineCleanEventArgs e)
        {
            //MessageBox.Show($"cut {e.lineNumber} line(s).");
            score += e.lineNumber;
            labelScore.Text = $"SCORE: {score}";
        }

        /// <summary>
        /// Return the random block with existed shape
        /// </summary>
        /// <returns></returns>
        private Block GetRandomBlock()
        {
            Random r = new Random();
            int n = r.Next(0, 6);

            switch (n)
            {
                case 0:
                    return new LShape();
                    
                case 1:
                    return new ZShape();
                    
                case 2:
                    return new IShape();
                    
                case 3:
                    return new OShape();

                case 4:
                    return new HShape();

                case 5:
                    return new RZShape();

                //case 6:
                //    return new DShape();

            }
            return new OShape();
        }

        private void DropRandomBlock()
        {
            stage.DropBlock(0, 8, GetRandomBlock());
            //Console.WriteLine("----------------------------");
        }

        /// <summary>
        /// The event handler for the stage detect a block hit the bottom or other existed point in the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stage_blockStucked(object sender, EventArgs e)
        {
            DropRandomBlock();
        }

        private void game_Load(object sender, EventArgs e)
        {
            SetpuPlayGround();
        }

        /// <summary>
        /// Start to play the game.
        /// This button used for pause and play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!stage.HasActiveBlock())
            {
                DropRandomBlock();
            }

            Button btn = sender as Button;

            if (isRunning)
            {
                pauseGame(true);
                btn.Text = "Play";
            }
            else
            {
                pauseGame(false);
                btn.Text = "Pause";
            }

            
        }

        /// <summary>
        /// Pause the game by disabled the timers
        /// </summary>
        /// <param name="s"></param>
        private void pauseGame(bool s)
        {
            isRunning = !s;
            timerUpdate.Enabled = !s;
            timerMoveDown.Enabled = !s;
        }

        /// <summary>
        /// Start speeding up temporary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S)
            {
                DownSpeedUp();
            }
        }

        /// <summary>
        /// use A and D to control direction of moving block
        /// W for rotate the block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Game_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar.ToString())
            {
                case "a":
                    stage.MoveActiveToLeft();
                    break;
                case "d":
                    stage.MoveActiveToRight();
                    break;
                case "w":
                    stage.RotateActiveBlock();
                    break;
            }
        }

        /// <summary>
        /// Stop the active block speeding up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S)
            {
                DownSpeedBackNormal();
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            stage.MoveActiveToLeft();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            stage.MoveActiveToRight();
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            stage.RotateActiveBlock();
        }


        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            stage.Update();
        }

        private void timerMoveDown_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine($"move down tick {DateTime.Now.Second}");
            stage.MoveActiveDown();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string msg = "Do you want to reset the game?";
            DialogResult res = MessageBox.Show(msg, "Alert", MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                isGameOver = false;
                
                timerMoveDown.Enabled = false;
                timerUpdate.Enabled = false;
                stage.Reset();
                DropRandomBlock();

                timerMoveDown.Interval = timerMoveDownTime;
                timerMoveDown.Enabled = true;
                timerUpdate.Enabled = true;

                btnReset.Enabled = false;
            }
        }

    }
}

            