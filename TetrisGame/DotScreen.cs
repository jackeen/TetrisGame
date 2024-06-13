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

    /// <summary>
    /// Provide the basic dot screen to display blocks and its moving
    /// </summary>
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


        /// <summary>
        /// Simulation a pixel screen by picturebox
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="stage"></param>
        public DotScreen(int y, int x, Form stage)
        {
            metrixY = y;
            metrixX = x;
            stageForm = stage;

            metrix = new int[y, x];
            pixelMetrix = new PictureBox[y, x];
        }

        /// <summary>
        /// Return the whole width based on a pixel width and the gap size
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return (pixelSize + gap) * metrixX;
        }

        /// <summary>
        /// Return the while height based on a pixel height and the gap size
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return (pixelSize + gap) * metrixY;
        }

        /// <summary>
        /// Init all pixels and put them on the stage at right position
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
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

        public void Draw((int y, int x) pixel, Color color)
        {
            pixelMetrix[pixel.y, pixel.x].BackColor = color;
        }

        /// <summary>
        /// Draw all pixels as default color 
        /// </summary>
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
