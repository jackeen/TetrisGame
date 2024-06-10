using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisGame
{
    internal class DotScreen
    {

        int gap = 1;
        int pixelSize = 15;

        Color closeColor = Color.FromArgb(30, 30, 30);
        Color openColor = Color.FromArgb(90, 90, 90);

        int[,] metrix;
        PictureBox[,] pixelMetrix;
        
        int metrixY, metrixX;

        Form stageForm;

        public DotScreen(int y, int x, Form stage)
        {
            metrixY = y;
            metrixX = x;
            stageForm = stage;

            metrix = new int[y, x];
            pixelMetrix = new PictureBox[y, x];
        }

        public int GetWidth()
        {
            return (pixelSize + gap) * metrixX;
        }

        public int GetHeight()
        {
            return (pixelSize + gap) * metrixY;
        }

        public void SetupPixel(int offsetX, int offsetY)
        {
            for (int y = 0; y < metrixY; y++)
            {
                for (int x = 0; x < metrixX; x++)
                {
                    PictureBox pixel = new PictureBox();
                    
                    pixel.Width = pixelSize;
                    pixel.Height = pixelSize;

                    pixel.BackColor = closeColor;

                    pixel.Left = x * (pixelSize + gap) + offsetX;
                    pixel.Top = y * (pixelSize + gap) + offsetY;

                    stageForm.Controls.Add(pixel);
                    pixelMetrix[y, x] = pixel;
                    //pixel.BringToFront();

                }
            }
        }

        public void Clean((int y, int x) pixel)
        {
            pixelMetrix[pixel.y, pixel.x].BackColor = closeColor;
        }

        public void Draw((int y, int x) pixel)
        {
            pixelMetrix[pixel.y, pixel.x].BackColor = openColor;
        }

        public void Reset()
        {
            for (int y = 0; y < metrixY; y++)
            {
                for (int x = 0; x < metrixX; x++)
                {
                    pixelMetrix[y, x].BackColor = closeColor;
                }
            }
        }
    }
}
