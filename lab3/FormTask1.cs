using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CG_Lab2
{
    public partial class FormTask1 : Form
    {

        private bool isDrawing = false;
        private Point previousPoint;
        private Bitmap bitmap;
        private Graphics graphics;

        private Pen penForDrawing = new Pen(Color.Black, 5);

        private Color targetColor;

        private enum Mode { Drawing, Fill1, Fill2 }
        private Mode currentMode = Mode.Drawing;

        private Point clickedPoint;

        private Bitmap pattern;


        private List<Point> borderPoints = new List<Point>();

        public FormTask1()
        {
            InitializeComponent();

            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            pictureBox1.Image = bitmap;
            targetColor = colorPreviewPanel.BackColor;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentMode != Mode.Drawing)
                return;

            isDrawing = true;
            previousPoint = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                graphics.DrawLine(penForDrawing, previousPoint, e.Location);
                previousPoint = e.Location;
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void FillArea(int x, int y)
        {
            if (x < 0 || x >= bitmap.Width || y < 0 || y >= bitmap.Height)
                return;

            Color currentColor = bitmap.GetPixel(x, y);
            if (currentColor.ToArgb() == penForDrawing.Color.ToArgb() || currentColor.ToArgb() == targetColor.ToArgb())
                return;

            int leftX = x;
            while (leftX >= 0 && bitmap.GetPixel(leftX, y).ToArgb() != penForDrawing.Color.ToArgb())
            {
                bitmap.SetPixel(leftX, y, targetColor);
                leftX--;
            }

            int rightX = x;
            while (rightX < bitmap.Width && bitmap.GetPixel(rightX, y).ToArgb() != penForDrawing.Color.ToArgb())
            {
                bitmap.SetPixel(rightX, y, targetColor);
                rightX++;
            }

            pictureBox1.Refresh();

            for (int i = leftX + 1; i < rightX; i++)
            {
                FillArea(i, y - 1);
                FillArea(i, y + 1);
            }
        }

        private void FillAreaWithPictures(int x, int y, int depth = 0)
        {
            if (x < 0 || x >= bitmap.Width || y < 0 || y >= bitmap.Height)
                return;

            Color currentColor = bitmap.GetPixel(x, y);

            int patternX = ((x - clickedPoint.X) % pattern.Width + pattern.Width) % pattern.Width;
            int patternY = ((y - clickedPoint.Y) % pattern.Height + pattern.Height) % pattern.Height;

            Color patternColor = pattern.GetPixel(patternX, patternY);

            if (currentColor.ToArgb() == penForDrawing.Color.ToArgb() || (currentColor.ToArgb() == patternColor.ToArgb() && depth > 0))
                return;

            int leftX = x;
            while (leftX >= 0 && bitmap.GetPixel(leftX, y).ToArgb() != penForDrawing.Color.ToArgb())
            {
                patternX = ((leftX - clickedPoint.X) % pattern.Width + pattern.Width) % pattern.Width;

                patternColor = pattern.GetPixel(patternX, patternY);

                bitmap.SetPixel(leftX, y, patternColor);
                leftX--;
            }

            int rightX = x;
            while (rightX < bitmap.Width && bitmap.GetPixel(rightX, y).ToArgb() != penForDrawing.Color.ToArgb())
            {
                patternX = ((rightX - clickedPoint.X) % pattern.Width + pattern.Width) % pattern.Width;

                patternColor = pattern.GetPixel(patternX, patternY);

                bitmap.SetPixel(rightX, y, patternColor);
                rightX++;
            }

            pictureBox1.Refresh();

            for (int i = leftX + 1; i < rightX; i++)
            {
                FillAreaWithPictures(i, y - 1, depth + 1);
                FillAreaWithPictures(i, y + 1, depth + 1);
            }
        }

        private void FindBorder()
        {
            Point point = clickedPoint;

            Color c = bitmap.GetPixel(point.X, point.Y);

            int x = point.X;

            while (c.ToArgb() != penForDrawing.Color.ToArgb())
            {
                x++;
                c = bitmap.GetPixel(x, point.Y);
            }

            point = new Point(x, point.Y);

            this.borderPoints.Clear();

            borderPoints.Add(point);

            Point prevPoint = point;

            int prevDirection = 6;

            while (true)
            {
                int newDirection = (prevDirection - 2 + 8) % 8;

                Point currentPoint = GetNextBorderPoint(prevPoint, newDirection);

                Color color = bitmap.GetPixel(currentPoint.X, currentPoint.Y);
                
                while (color.ToArgb() != penForDrawing.Color.ToArgb())
                {
                    newDirection = (newDirection + 1) % 8;

                    currentPoint = GetNextBorderPoint(prevPoint, newDirection);

                    color = bitmap.GetPixel(currentPoint.X, currentPoint.Y);
                }

                borderPoints.Add(currentPoint);

                prevDirection = newDirection;

                prevPoint = currentPoint;

                if (currentPoint == point)
                    break;
            }

            graphics.DrawLines(new Pen(Color.Red, 3), this.borderPoints.ToArray());

            pictureBox1.Refresh();
        }

        private Point GetNextBorderPoint(Point currentPoint, int direction)
        {
            switch (direction)
            {
                case 0: 
                    return new Point(currentPoint.X + 1, currentPoint.Y);
                case 1: 
                    return new Point(currentPoint.X + 1, currentPoint.Y - 1);
                case 2: 
                    return new Point(currentPoint.X, currentPoint.Y - 1);
                case 3: 
                    return new Point(currentPoint.X - 1, currentPoint.Y - 1);
                case 4:
                    return new Point(currentPoint.X - 1, currentPoint.Y);
                case 5:
                    return new Point(currentPoint.X - 1, currentPoint.Y + 1);
                case 6:
                    return new Point(currentPoint.X, currentPoint.Y + 1);
                case 7:
                    return new Point(currentPoint.X + 1, currentPoint.Y + 1);
                default:
                    throw new ArgumentException("Invalid direction");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            clickedPoint = pictureBox1.PointToClient(Cursor.Position);

            if (pictureBox1.Image == null || (currentMode != Mode.Fill1 && currentMode != Mode.Fill2))
                return;

            if (currentMode == Mode.Fill1)
                FillArea(clickedPoint.X, clickedPoint.Y);
            else if (pictureBox2.Image != null)
                FillAreaWithPictures(clickedPoint.X, clickedPoint.Y);

            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pattern = new Bitmap(openFileDialog.FileName);
                    pictureBox2.Image = pattern;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            pictureBox1.Refresh();
        }

        ~FormTask1()
        {
            graphics.Dispose();
            graphics = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                targetColor = colorDialog.Color;
                colorPreviewPanel.BackColor = targetColor;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                currentMode = Mode.Drawing;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                currentMode = Mode.Fill1;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                currentMode = Mode.Fill2;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FindBorder();
        }
    }
}
