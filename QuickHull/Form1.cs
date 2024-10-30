using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuickHull
{
    public partial class Form1 : Form
    {
        private List<Point> points = new List<Point>();
        private List<Point> hull = new List<Point>();
        private Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateRandomPoints();
            hull = QuickHull(points);
            pictureBox1.Invalidate();
        }

        private void GenerateRandomPoints()
        {
            points.Clear();
            var pointsCount = (textBox1.Text == "") ? 10 : int.Parse(textBox1.Text); 
            for (int i = 0; i < pointsCount; i++)
            {
                points.Add(new Point(rand.Next(pictureBox1.Width - 50), rand.Next(pictureBox1.Height - 50)));
            }
        }

        private List<Point> QuickHull(List<Point> points)
        {
            if (points.Count < 3)
                return new List<Point>(points);

            Point minPoint = points.Aggregate((p1, p2) => p1.X < p2.X ? p1 : p2);
            Point maxPoint = points.Aggregate((p1, p2) => p1.X > p2.X ? p1 : p2);

            List<Point> upperHull = new List<Point>();
            List<Point> lowerHull = new List<Point>();

            // Построение верхней и нижней оболочек
            FindHull(points, minPoint, maxPoint, upperHull); // Верхняя часть
            FindHull(points, maxPoint, minPoint, lowerHull); // Нижняя часть

            // Объединяем результаты, чтобы сформировать полный упорядоченный контур оболочки
            List<Point> result = new List<Point> { minPoint };
            result.AddRange(upperHull);
            result.Add(maxPoint);
            result.AddRange(lowerHull);

            return result;
        }

        private void FindHull(List<Point> points, Point p, Point q, List<Point> hull)
        {
            Point? farthest = null;
            double maxDistance = 0;

            // Находим точку, максимально удаленную от линии pq
            foreach (var point in points)
            {
                double distance = DistanceFromLine(point, p, q);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthest = point;
                }
            }

            // Если нет удаленной точки, заканчиваем построение данной части оболочки
            if (farthest == null)
                return;

            // Рекурсивное построение: сначала левая часть, затем правая
            List<Point> leftOfPQ = points.Where(point => IsLeftOfLine(point, p, farthest.Value)).ToList();
            List<Point> leftOfFQ = points.Where(point => IsLeftOfLine(point, farthest.Value, q)).ToList();

            // Добавляем точку только после того, как закончены все рекурсивные вызовы для корректного порядка
            FindHull(leftOfPQ, p, farthest.Value, hull);
            hull.Add(farthest.Value);  // Добавляем после завершения левой части
            FindHull(leftOfFQ, farthest.Value, q, hull);
        }

        private double DistanceFromLine(Point p, Point lineStart, Point lineEnd)
        {
            return Math.Abs((lineEnd.Y - lineStart.Y) * p.X -
                            (lineEnd.X - lineStart.X) * p.Y +
                            lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X) /
                   Math.Sqrt(Math.Pow(lineEnd.Y - lineStart.Y, 2) + Math.Pow(lineEnd.X - lineStart.X, 2));
        }

        private bool IsLeftOfLine(Point p, Point lineStart, Point lineEnd)
        {
            return ((lineEnd.X - lineStart.X) * (p.Y - lineStart.Y) -
                    (lineEnd.Y - lineStart.Y) * (p.X - lineStart.X)) > 0;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            // Отрисовка точек
            foreach (var point in points)
            {
                g.FillEllipse(Brushes.Blue, point.X - 2, point.Y - 2, 4, 4);
            }

            // Отрисовка оболочки
            if (hull.Count > 1)
            {
                for (int i = 0; i < hull.Count - 1; i++)
                {
                    g.DrawLine(Pens.Red, hull[i], hull[i + 1]);
                }
                g.DrawLine(Pens.Red, hull[hull.Count - 1], hull[0]); // Замыкаем оболочку
            }
        }
    }
}
