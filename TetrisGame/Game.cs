using System;
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
    public partial class Game : Form
    {

        private DotScreen screen;
        private Stage stage;

        private int score = 0;
        private bool isRunning = false;

        private int timerUpdateTime = 50;
        private int timerMoveDownTime = 1000;

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

            btnDown.MouseDown += BtnDown_MouseDown;
            btnDown.MouseUp += BtnDown_MouseUp;

            timerUpdate.Interval = timerUpdateTime;
            timerMoveDown.Interval = timerMoveDownTime;
        }

        private void BtnDown_MouseUp(object sender, MouseEventArgs e)
        {
            timerMoveDown.Interval = timerMoveDownTime;
        }

        private void BtnDown_MouseDown(object sender, MouseEventArgs e)
        {
            timerMoveDown.Interval = 80;
        }

        private void Stage_linesCleaned(object sender, LineCleanEventArgs e)
        {
            //MessageBox.Show($"cut {e.lineNumber} line(s).");
            labelScore.Text = $"SCORE: {++score}";
        }

        /// <summary>
        /// Return the random block with existed shape
        /// </summary>
        /// <returns></returns>
        private Block GetRandomBlock()
        {
            Random r = new Random();
            int n = r.Next(0, 7);

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

                case 6:
                    return new DShape();

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

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                DropRandomBlock();
            }
            isRunning = true;
            timerUpdate.Enabled = true;
            timerMoveDown.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            timerUpdate.Enabled = false;
            timerMoveDown.Enabled = false;
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.KeyCode.ToString());
            switch (e.KeyCode.ToString())
            {
                case "Left":
                    stage.MoveActiveToLeft();
                    break;
                case "Right":
                    stage.MoveActiveToRight();
                    break;
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

        
    }
}

            