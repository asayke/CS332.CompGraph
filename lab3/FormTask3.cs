using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace CG_Lab2
{
    public partial class FormTask3 : Form
    {
        int minDifference = 200; // Минимальная разница между цветами 
        private Random rand = new Random();
        private List<Point> points = new List<Point>(); // Список для хранения точек
        private Bitmap drawingBitmap; // Холст для рисования
        private Graphics graphics;
        public FormTask3()
        {
            InitializeComponent();


            pictureBox1.BackColor = Color.White; // Цвет фона

            // Создание Bitmap для рисования
            drawingBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);


            pictureBox1.MouseClick += PictureBox_MouseClick;
            pictureBox1.Paint += PictureBox_Paint;
        }


        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (points.Count < 3) // Пока не выбрано три точки
            {
                points.Add(new Point(e.X, e.Y)); // Добавляем координаты точки
                ((PictureBox)sender).Invalidate(); // Перерисовываем PictureBox
            }

            if (points.Count == 3) // Когда три точки заданы
            {
                DrawTriangle();
            }
        }


        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {


            e.Graphics.DrawImage(drawingBitmap, 0, 0);

            // Рисуем точки на PictureBox
            foreach (Point p in points)
            {
                e.Graphics.FillEllipse(Brushes.Red, p.X - 2, p.Y - 2, 4, 4); // Рисуем точки
            }
            
        }

        private void RasterizeTriangle(PointF p1, Color c1, PointF p2, Color c2, PointF p3, Color c3)
        {
            // Сортируем вершины треугольника по Y-координате
            if (p2.Y < p1.Y) { Swap(ref p1, ref p2); Swap(ref c1, ref c2); }
            if (p3.Y < p1.Y) { Swap(ref p1, ref p3); Swap(ref c1, ref c3); }
            if (p3.Y < p2.Y) { Swap(ref p2, ref p3); Swap(ref c2, ref c3); }

            using (Graphics g = Graphics.FromImage(drawingBitmap))
            {
                // Перебор всех строк треугольника (Y-координаты)
                for (float y = p1.Y; y <= p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        // Верхняя часть треугольника
                        DrawScanLine(g, y, p1, c1, p3, c3, p1, c1, p2, c2);
                    }
                    else
                    {
                        // Нижняя часть треугольника
                        DrawScanLine(g, y, p1, c1, p3, c3, p2, c2, p3, c3);
                    }
                }
            }
        }

        // Отрисовка одной строки (сканлайна) треугольника
        private void DrawScanLine(Graphics g, float y, PointF pA, Color cA, PointF pB, Color cB, PointF pC, Color cC, PointF pD, Color cD)
        {
            // Интерполируем X и цвет для левой и правой границы строки
            float x1 = Interpolate(pA.Y, pA.X, pB.Y, pB.X, y);
            float x2 = Interpolate(pC.Y, pC.X, pD.Y, pD.X, y);

            Color c1 = InterpolateColor(pA.Y, cA, pB.Y, cB, y);
            Color c2 = InterpolateColor(pC.Y, cC, pD.Y, cD, y);

            if (x1 > x2) { Swap(ref x1, ref x2); Swap(ref c1, ref c2); } // Упорядочим по X

            // Закрашиваем строку пикселей между x1 и x2
            for (float x = x1; x <= x2; x++)
            {
                float t = (x - x1) / (x2 - x1); // Нормализуем положение между x1 и x2
                Color color = LerpColor(c1, c2, t); // Линейная интерполяция цвета
                drawingBitmap.SetPixel((int)x, (int)y, color); // Устанавливаем цвет пикселя
            }
        }

        // Линейная интерполяция значений
        private float Interpolate(float y1, float x1, float y2, float x2, float y)
        {
            if (y1 == y2) return x1; // Защита от деления на 0
            return x1 + (x2 - x1) * ((y - y1) / (y2 - y1));
        }

        // Линейная интерполяция цвета по оси Y
        private Color InterpolateColor(float y1, Color c1, float y2, Color c2, float y)
        {
            if (y1 == y2) return c1; // Защита от деления на 0
            float t = (y - y1) / (y2 - y1);
            byte r = (byte)(c1.R + (c2.R - c1.R) * t);
            byte g = (byte)(c1.G + (c2.G - c1.G) * t);
            byte b = (byte)(c1.B + (c2.B - c1.B) * t);
            return Color.FromArgb(r, g, b);
        }

        // Линейная интерполяция между двумя цветами
        private Color LerpColor(Color c1, Color c2, float t)
        {
            byte r = (byte)(c1.R + (c2.R - c1.R) * t);
            byte g = (byte)(c1.G + (c2.G - c1.G) * t);
            byte b = (byte)(c1.B + (c2.B - c1.B) * t);
            return Color.FromArgb(r, g, b);
        }

        // Метод для обмена значениями
        private void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        // Метод для рисования треугольника на Bitmap
        private void DrawTriangle()
        {
            using (Graphics g = Graphics.FromImage(drawingBitmap))
            {
                Pen pen = new Pen(Color.DarkCyan, 1);
                g.DrawPolygon(pen, points.ToArray()); // Рисуем треугольник
            }

            Invalidate(); // Перерисовываем форму
        }

        // Сбрасываем точки
        private void ResetPoints()
        {
            points.Clear(); // Очищаем список точек
            drawingBitmap = new Bitmap(drawingBitmap.Width, drawingBitmap.Height);
            pictureBox1.Image = drawingBitmap;
            Invalidate(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetPoints();
            pictureBox1.Refresh();  
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (points.Count == 3) // Если выбраны три точки
            {
                
                List<Color> vertexColors = GenerateDistinctColors(minDifference);
                // Задаем случайные цвета для каждой вершины
                Color c1 = vertexColors[0];
                Color c2 = vertexColors[1];
                Color c3 = vertexColors[2];

                // Растеризация треугольника с градиентной заливкой
                RasterizeTriangle(points[0], c1, points[1], c2, points[2], c3);

                // Отображаем на PictureBox
                pictureBox1.Image = drawingBitmap;

                // Очищаем список точек для новой отрисовки
                points.Clear();

            }
            else
            {
                MessageBox.Show("Выберите три точки на изображении!");
            }
        }

        // Генерация трех случайных цветов с минимальной разницей по RGB
        private List<Color> GenerateDistinctColors(int minDifference)
        {
            List<Color> colors = new List<Color>();

            while (colors.Count < 3)
            {
                Color newColor = GenerateRandomColor();
                // Если список пуст, условие вернёт true, и цвет добавится
                if (colors.All(existingColor => ColorDifference(existingColor, newColor) >= minDifference))
                {
                    colors.Add(newColor);
                }
            }

            return colors;
        }

        // Вычисление различия между двумя цветами (евклидово расстояние в цветовом пространстве RGB)
        private int ColorDifference(Color c1, Color c2)
        {
            int rDiff = c1.R - c2.R;
            int gDiff = c1.G - c2.G;
            int bDiff = c1.B - c2.B;

            return (int)Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        }
        // Генерация случайного цвета
        private Color GenerateRandomColor()
        {
            int r = rand.Next(256); 
            int g = rand.Next(256); 
            int b = rand.Next(256); 

            return Color.FromArgb(r, g, b);
        }

    }
}
