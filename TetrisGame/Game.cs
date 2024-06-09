using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisGame
{
    public partial class Game : Form
    {

        DotScreen screen;
        Stage stage;

        public Game()
        {
            InitializeComponent();
        }

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
            stage.blockStucked += Stage_blockStucked;

            
        }

        private Block GetRandomBlock()
        {
            Random r = new Random();
            int n = r.Next(0, 3);

            Block b;

            switch (n)
            {
                case 0:
                    b = new LShape();
                    break;
                case 1:
                    b = new ZShape();
                    break;
                case 2:
                    b = new IShape();
                    break;
                default:
                    b = new OShape();
                    break;

            }
            return b;
        }

        private void Stage_blockStucked(object sender, EventArgs e)
        {
            //MessageBox.Show("block stucked");
            //stage.DropBlock(0, 8, GetRandomBlock());
            timerUpdate.Enabled = false;
            timerMoveDown.Enabled = false;
        }

        private void game_Load(object sender, EventArgs e)
        {
            SetpuPlayGround();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            stage.DropBlock(0, 8, GetRandomBlock());
            timerUpdate.Enabled = true;
            timerMoveDown.Enabled = true;
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
            /*
            Action movement;
            if (movementQueue.Count > 0)
            {
                movement = movementQueue.Dequeue();
                movement();
            }
            */

            stage.Update();
        }

        private void timerMoveDown_Tick(object sender, EventArgs e)
        {
            
            //movementQueue.Enqueue(stage.Update);
            stage.MoveActiveDown();
        }

        
    }
}

            