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

            int pixelsX = 30;
            int pixelsY = 50;

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

            stage = new Stage(30, 50, screen);
        }

        private void game_Load(object sender, EventArgs e)
        {
            SetpuPlayGround();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            LShape l = new LShape();

            stage.AddBlock(5, 1 , l);

            timerUpdate.Enabled = true;

        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Left":
                    break;
                case "Right":
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

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            stage.Update();
        }
    }
}

            