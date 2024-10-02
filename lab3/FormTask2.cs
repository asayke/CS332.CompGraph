using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace CG_Lab2
{
    public partial class FormTask2 : Form
    {
        Point[] points = new Point[2];
        Point defaultPoint = new Point(-1, -1);
        Color bg = Color.White;

        public FormTask2()
        {
            InitializeComponent();
            Bitmap PictureBoxClear = new Bitmap(PictureBox.Width, PictureBox.Height);
            using (Graphics g = Graphics.FromImage(PictureBoxClear))
                g.Clear(bg);
            PictureBox.Image = PictureBoxClear;
            points[0] = defaultPoint;
            points[1] = defaultPoint;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap PictureBoxClear = new Bitmap(PictureBox.Width, PictureBox.Height);
            using (Graphics g = Graphics.FromImage(PictureBoxClear))
                g.Clear(bg);
            PictureBox.Image = PictureBoxClear;
            points[0] = defaultPoint;
            points[1] = defaultPoint;
        }

        private void DrawLineBresenham(int x0, int x1, int y0, int y1)
        {
            Bitmap pb = new Bitmap(PictureBox.Image);
            
            int deltaX = Math.Abs(x0 - x1);
            int deltaY = Math.Abs(y0 - y1);
            float m = (float)deltaY / (float)deltaX;

            int error = 0;
            int deltaErrY = deltaY + 1;
            int deltaErrX = deltaX + 1;

            int dirY = y1 >= y0 ? 1 : -1;
            int dirX = x1 >= x0 ? 1 : -1;

            if (m <= 1)
            {
                if (x0 <= x1)
                {
                    int y = y0;
                    for (int x = x0; x <= x1; x++)
                    {
                        pb.SetPixel(x, y, Color.Black);
                        error += deltaErrY;
                        if (error >= deltaX + 1)
                        {
                            y += dirY;
                            error -= deltaX + 1;
                        }
                    }
                }
                else
                {
                    int y = y0;
                    for (int x = x0; x >= x1; x--)
                    {
                        pb.SetPixel(x, y, Color.Black);
                        error += deltaErrY;
                        if (error >= deltaX + 1)
                        {
                            y += dirY;
                            error -= deltaX + 1;
                        }
                    }
                }
            }
            else
            {
                if (y0 <= y1)
                {
                    int x = x0;
                    for (int y = y0; y <= y1; y++)
                    {
                        pb.SetPixel(x, y, Color.Black);
                        error += deltaErrX;
                        if (error >= deltaY + 1)
                        {
                            x += dirX;
                            error -= deltaY + 1;
                        }
                    }
                }
                else
                {
                    int x = x0;
                    for (int y = y0; y >= y1; y--)
                    {
                        pb.SetPixel(x, y, Color.Black);
                        error += deltaErrX;
                        if (error >= deltaY + 1)
                        {
                            x += dirX;
                            error -= deltaY + 1;
                        }
                    }
                }
            }
            PictureBox.Image = pb;
        }

        private void DrawLineVu(int x0, int x1, int y0, int y1)
        {
            Bitmap pb = new Bitmap(PictureBox.Image);
            
            pb.SetPixel(x1, y1, Color.FromArgb(255, 0, 0, 0));

            float deltaX = x1 - x0;
            float deltaY = y1 - y0;
            float m = Math.Abs(deltaY / deltaX);

            if (m <= 1)
            {
                float gradient = deltaY / deltaX;
                if (x0 <= x1)
                {
                    float y = y0 + gradient;
                    for (int x = x0 + 1; x <= x1 - 1; x++)
                    {
                        int alpha = (int)((1 - (y - (int)y)) * 255);
                        //pb.SetPixel(x, (int)y, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel(x, (int)y, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        alpha = (int)((y - (int)y) * 255);
                        //pb.SetPixel(x, (int)y + 1, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel(x, (int)y + 1, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        y += gradient;
                    }
                }
                else
                {
                    gradient *= -1;
                    float y = y0 + gradient;
                    for (int x = x0 - 1; x >= x1 + 1; x--)
                    {
                        int alpha = (int)((1 - (y - (int)y)) * 255);
                        //pb.SetPixel(x, (int)y, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel(x, (int)y, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        alpha = (int)((y - (int)y) * 255);
                        //pb.SetPixel(x, (int)y + 1, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel(x, (int)y + 1, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        y += gradient;
                    }
                }
            }
            else
            {
                float gradient = deltaX / deltaY;
                if (y0 <= y1)
                {
                    float x = x0 + gradient;
                    for (int y = y0 + 1; y <= y1 - 1; y++)
                    {
                        int alpha = (int)((1 - (x - (int)x)) * 255);
                        //pb.SetPixel((int)x, y, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel((int)x, y, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        alpha = (int)((x - (int)x) * 255);
                        //pb.SetPixel((int)x + 1, y, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel((int)x + 1, y, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        x += gradient;
                    }
                }
                else
                {
                    gradient *= -1;
                    float x = x0 + gradient;
                    for (int y = y0 - 1; y >= y1 + 1; y--)
                    {
                        int alpha = (int)((1 - (x - (int)x)) * 255);
                        //pb.SetPixel((int)x, y, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel((int)x, y, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        alpha = (int)((x - (int)x) * 255);
                        //pb.SetPixel((int)x + 1, y, Color.FromArgb(alpha, 0, 0, 0));
                        pb.SetPixel((int)x + 1, y, Color.FromArgb(255, 0 + (255 - alpha) * bg.R / 255,
                            0 + (255 - alpha) * bg.G / 255,
                            0 + (255 - alpha) * bg.B / 255));
                        x += gradient;
                    }
                }
            }
            PictureBox.Image = pb;
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (points[0].X == -1)
            {
                points[0].X = e.Location.X;
                points[0].Y = e.Location.Y;
                Bitmap pb = new Bitmap(PictureBox.Image);
                pb.SetPixel(points[0].X, points[0].Y, Color.Black);
                PictureBox.Image = pb;
                return;
            }
            else
            {

                if (points[1].X == -1)
                {
                    points[1].X = e.Location.X;
                    points[1].Y = e.Location.Y;
                }
                else
                {
                    points[0].X = points[1].X;
                    points[0].Y = points[1].Y;
                    points[1].X = e.Location.X;
                    points[1].Y = e.Location.Y;
                }
                if (!checkBox1.Checked)
                    DrawLineBresenham(points[0].X, points[1].X, points[0].Y, points[1].Y);
                else
                    DrawLineVu(points[0].X, points[1].X, points[0].Y, points[1].Y);
                Bitmap pb = new Bitmap(PictureBox.Image);
                PictureBox.Image = pb;
            }
        }
    }
}
