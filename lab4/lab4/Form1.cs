using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4
{
    public partial class Form1 : Form
    {
        private Polygon currentPolygon; // Храним текущий полигон
        private Polygon currentPolygonRound;
        private bool isDrawing; // Флаг для режима рисования полигона
        private List<Point2D> userInputEdge = new List<Point2D>();
        private bool isClearButtonPressed = false;
        public Form1()
        {
            InitializeComponent();
            currentPolygon = new Polygon();
            currentPolygonRound = new Polygon();
            isDrawing = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox.BackColor = Color.White;
            pictureBox.Paint += pictureBox_Paint;
            pictureBox.MouseClick += pictureBox_MouseClick;
        }
        public static bool IsPointInPolygon(Point2D point, Polygon polygon)
        {
            bool inside = false;
            int count = polygon.Vertices.Count;

            foreach(var pnt in polygon.Vertices) {
                if(pnt == point)
                {
                    return true;
                }
            }

            for (int i = 0, j = count - 1; i < count; j = i++) // пробегаемся по всем соседним вершинам
            {
                Point2D vi = polygon.Vertices[i];
                Point2D vj = polygon.Vertices[j];

                // Проверяем, находится ли точка на ребре (vi, vj)
                if (IsPointOnLineSegment(point, vi, vj))
                {
                    return true; // Точка на границе
                }

                if ((vi.Y > point.Y) != (vj.Y > point.Y)) // vi и vj по разные стороны горизонтальной линии через point
                {
                    double intersectionX = (vj.X - vi.X) * (point.Y - vi.Y) / (vj.Y - vi.Y) + vi.X;
                    if (point.X < intersectionX)
                    {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }

        // Метод для проверки, лежит ли точка на отрезке (vi, vj)
        private static bool IsPointOnLineSegment(Point2D point, Point2D vi, Point2D vj)
        {
            // Проверяем, что point лежит на линии (vi, vj) и находится между vi и vj
            double crossProduct = (point.Y - vi.Y) * (vj.X - vi.X) - (point.X - vi.X) * (vj.Y - vi.Y);

            // Если crossProduct не 0, то точка не лежит на линии
            if (Math.Abs(crossProduct) > double.Epsilon)
            {
                return false;
            }

            // Проверяем, что точка находится в пределах отрезка
            double dotProduct = (point.X - vi.X) * (point.X - vj.X) + (point.Y - vi.Y) * (point.Y - vj.Y);
            if (dotProduct > 0)
            {
                return false;
            }

            return true;
        }
        public static string ClassifyPoint(Point2D p, Point2D a, Point2D b)
        {
            double result = (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);

            if (result > 0)
                return "Слева";
            else if (result < 0)
                return "Справа";
            else
                return "На линии";
        }
        private void btnTranslate_Click(object sender, EventArgs e)
        {
            double dx = double.Parse(txtDx.Text);
            double dy = double.Parse(txtDy.Text);
            currentPolygon.ApplyTransformation(AffineTransform.Translation(dx, dy));
            updateVertices();
            pictureBox.Invalidate(); // Перерисовка
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            double angle = double.Parse(txtAngle.Text);
            var userXValue = userXTextBox.Text;
            var userYValue = userYTextBox.Text;
            Point2D center = (userXValue != "" && userYValue != "") ? new Point2D(double.Parse(userXValue), double.Parse(userYValue)) : currentPolygon.GetCentroid(); // Центр полигона
            currentPolygon.ApplyTransformation(AffineTransform.Rotation(angle, center));
            updateVertices();
            pictureBox.Invalidate(); // Обновление PictureBox
        }
        private void updateVertices()
        {
            vertexList.Items.Clear();
            for (int i = 0; i < currentPolygonRound.Vertices.Count; i++)
            {
                currentPolygonRound.Vertices[i] = new Point2D(Math.Round(currentPolygon.Vertices[i].X), Math.Round(currentPolygon.Vertices[i].Y));
            }
            foreach (var vert in currentPolygonRound.Vertices)
            {
                vertexList.Items.Add(vert.X.ToString() + " " + vert.Y.ToString() + "\n");
                vertexList.SelectedItem = (vert.X.ToString() + " " + vert.Y.ToString() + "\n");
            }
        }
        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if(userInputEdgeCheckBox.Checked && (e.Button == MouseButtons.Left))
            {
                if (userInputEdge.Count < 2)
                {
                userInputEdge.Add(new Point2D(e.X, e.Y));
                }
                else
                {
                    userInputEdge[0] = userInputEdge[1];
                    userInputEdge[1] = new Point2D(e.X, e.Y);
                }
                pictureBox.Invalidate();
            } else
            {
                if (e.Button == MouseButtons.Right)
                {

                    userXTextBox.Text = e.X.ToString();
                    userYTextBox.Text = e.Y.ToString();
                    pictureBox.Invalidate();
                }
                else if (isDrawing)
                {
                    Point2D point = new Point2D(e.X, e.Y);
                    currentPolygon.AddVertex(point);
                    Point2D pointRound = new Point2D(Math.Round((double)e.X), Math.Round((double)e.Y));
                    currentPolygonRound.AddVertex(pointRound);
                    vertexList.Items.Add(e.X.ToString() + " " + e.Y.ToString() + "\n");
                    vertexList.SelectedItem = e.X.ToString() + " " + e.Y.ToString() + "\n";
                    pictureBox.Invalidate(); // Обновляем PictureBox для перерисовки
                }
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if(userXTextBox.Text != "" && userYTextBox.Text != "") {
                e.Graphics.DrawRectangle(
                    new Pen(Color.DimGray, 3),
                    float.Parse(userXTextBox.Text),
                    float.Parse(userYTextBox.Text),
                    2,
                    2
                );
            }
            if(userInputEdge.Count == 2)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.Green, 2),
                    (float)userInputEdge[0].X,
                    (float)userInputEdge[0].Y,
                    (float)userInputEdge[1].X,
                    (float)userInputEdge[1].Y
                );
                if (EdgePoint1Value.Text.Split(' ').Count() == 2 && EdgePoint2Value.Text.Split(' ').Count() == 2)
                {
                    var a = new Point2D(double.Parse(EdgePoint1Value.Text.Split(' ').First()), double.Parse(EdgePoint1Value.Text.Split(' ')[1]));
                    var b = new Point2D(double.Parse(EdgePoint2Value.Text.Split(' ').First()), double.Parse(EdgePoint2Value.Text.Split(' ')[1]));
                    var inda = currentPolygonRound.Vertices.FindIndex(x => x.Equals(a));
                    var indb = currentPolygonRound.Vertices.FindIndex(x => x.Equals(b));
                    var cnt = Math.Abs(inda - indb) == 1;
                    if (cnt)
                    {
                        var intersection = Geometry.FindIntersection(userInputEdge[0], userInputEdge[1], a, b);
                        if(intersection != null)
                        {
                            e.Graphics.DrawRectangle(new Pen(Color.Purple, 3), (float)intersection.X, (float)intersection.Y, 2, 2);
                        }
                        intersectionLabel.Text = (intersection == null) ? "" : (int)intersection.X + " " + (int)intersection.Y;
                    } else
                    {
                        intersectionLabel.Text = "Пара точек не является ребром";
                    }
                }else
                {
                    intersectionLabel.Text = "Некорректные координаты";
                }
            }
            if (userXTextBox.Text != "" && userYTextBox.Text != "")
            {
                isPointInPolygon.Text = IsPointInPolygon(
                new Point2D(double.Parse(userXTextBox.Text),
                double.Parse(userYTextBox.Text)), currentPolygon
                ) ? "Да" : "Нет";
                e.Graphics.DrawRectangle(
                    new Pen(Color.DimGray, 1),
                    float.Parse(userXTextBox.Text),
                    float.Parse(userYTextBox.Text),
                    1,1
                );
                if (EdgePoint1Value.Text.Split(' ').Count() == 2 && EdgePoint2Value.Text.Split(' ').Count() == 2 /*&& Math.Abs(currentPolygon.Vertices.IndexOf(new Point2D(double.Parse(EdgePoint1Value.Text.Split(' ').First()), double.Parse(EdgePoint1Value.Text.Split(' ')[1]))) - currentPolygon.Vertices.IndexOf(new Point2D(double.Parse(EdgePoint2Value.Text.Split(' ').First()), double.Parse(EdgePoint2Value.Text.Split(' ')[1])))) == 1*/)
                {
                    var p = new Point2D(double.Parse(userXTextBox.Text),
                double.Parse(userYTextBox.Text));
                    var a = new Point2D(double.Parse(EdgePoint1Value.Text.Split(' ').First()), double.Parse(EdgePoint1Value.Text.Split(' ')[1]));
                    var b = new Point2D(double.Parse(EdgePoint2Value.Text.Split(' ').First()), double.Parse(EdgePoint2Value.Text.Split(' ')[1]));
                    var inda = currentPolygonRound.Vertices.FindIndex(x => x.Equals(a));
                    var indb = currentPolygonRound.Vertices.FindIndex(x => x.Equals(b));
                    var cnt = Math.Abs(inda - indb) == 1;
                    if (cnt)
                        LeftRightPosition.Text = ClassifyPoint(p, b, a);
                    else
                        LeftRightPosition.Text = "Пара точек не является ребром";
                }else
                {
                    LeftRightPosition.Text = "Некорректные координаты";
                }
            } 
            if(isClearButtonPressed)
            {
                e.Graphics.Clear(Color.White);
                isClearButtonPressed = false;
            }
            if (currentPolygon.Vertices.Count > 1)
            {
                for (int i = 0; i < currentPolygon.Vertices.Count - 1; i++)
                {
                    Point2D v1 = currentPolygon.Vertices[i];
                    Point2D v2 = currentPolygon.Vertices[i + 1];
                    e.Graphics.DrawLine(Pens.Black, (float)v1.X, (float)v1.Y, (float)v2.X, (float)v2.Y);
                }
                // Замыкаем полигон
                Point2D last = currentPolygon.Vertices.Last();
                Point2D first = currentPolygon.Vertices.First();
                e.Graphics.DrawLine(Pens.Black, (float)last.X, (float)last.Y, (float)first.X, (float)first.Y);
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            isClearButtonPressed = true;

            currentPolygon = new Polygon();
            currentPolygonRound = new Polygon();
            vertexList.SelectedItem = null;

            userXTextBox.Text = "";
            userYTextBox.Text = "";

            userInputEdge.Clear();

            vertexList.Items.Clear();
            pictureBox.Invalidate(); // Обновление PictureBox
        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            double scaleX = double.Parse(txtScaleX.Text);
            double scaleY = double.Parse(txtScaleY.Text);
            var userXValue = userXTextBox.Text;
            var userYValue = userYTextBox.Text;
            Point2D center = (userXValue != "" && userYValue != "") ? new Point2D(double.Parse(userXValue), double.Parse(userYValue)) : currentPolygon.GetCentroid(); // Центр полигона
            currentPolygon.ApplyTransformation(AffineTransform.Scaling(scaleX, scaleY, center));
            updateVertices();
            pictureBox.Invalidate(); // Обновление PictureBox
        }
        private void userXTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
         
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void userXTextBox_TextChanged(object sender, EventArgs e)
        {
            pictureBox.Invalidate();
        }
    }
    public class Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2D Transform(double[,] matrix)
        {
            double newX = X * matrix[0, 0] + Y * matrix[0, 1] + matrix[0, 2];
            double newY = X * matrix[1, 0] + Y * matrix[1, 1] + matrix[1, 2];
            return new Point2D(newX, newY);
        }
        public bool Equals(Point2D other)
        {
            return X == other.X && Y == other.Y;
        }
    }
    public class Polygon
    {
        public List<Point2D> Vertices { get; set; }

        public Polygon()
        {
            Vertices = new List<Point2D>();
        }

        // Добавление вершины
        public void AddVertex(Point2D point)
        {
            Vertices.Add(point);
        }

        // Применение трансформации ко всем вершинам полигона
        public void ApplyTransformation(double[,] matrix)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = Vertices[i].Transform(matrix);
            }
        }

        // Найти центр полигона (среднее арифметическое всех вершин)
        public Point2D GetCentroid()
        {
            double sumX = 0, sumY = 0;
            foreach (var v in Vertices)
            {
                sumX += v.X;
                sumY += v.Y;
            }
            return new Point2D(sumX / Vertices.Count, sumY / Vertices.Count);
        }
    }
    public static class AffineTransform
    {
        public static double[,] Translation(double dx, double dy)
        {
            return new double[,]
            {
            { 1, 0, dx },
            { 0, 1, dy },
            { 0, 0, 1 }
            };
        }

        public static double[,] Rotation(double angle, Point2D center)
        {
            double radians = angle * Math.PI / 180.0;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            return new double[,]
            {
            { cos, -sin, center.X - center.X * cos + center.Y * sin },
            { sin, cos, center.Y - center.X * sin - center.Y * cos },
            { 0, 0, 1 }
            };
        }

        public static double[,] Scaling(double sx, double sy, Point2D center)
        {
            return new double[,]
            {
            { sx, 0, center.X - sx * center.X },
            { 0, sy, center.Y - sy * center.Y },
            { 0, 0, 1 }
            };
        }
    }
    public class Geometry
    {
        public static Point2D FindIntersection(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
        {
            double d = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);

            if (d == 0) return null; // Линии параллельны

            double xi = ((p3.X - p4.X) * (p1.X * p2.Y - p1.Y * p2.X) - (p1.X - p2.X) * (p3.X * p4.Y - p3.Y * p4.X)) / d;
            double yi = ((p3.Y - p4.Y) * (p1.X * p2.Y - p1.Y * p2.X) - (p1.Y - p2.Y) * (p3.X * p4.Y - p3.Y * p4.X)) / d;

            if (xi < Math.Min(p1.X, p2.X) || xi > Math.Max(p1.X, p2.X) ||
                xi < Math.Min(p3.X, p4.X) || xi > Math.Max(p3.X, p4.X))
                return null;

            if (yi < Math.Min(p1.Y, p2.Y) || yi > Math.Max(p1.Y, p2.Y) ||
                yi < Math.Min(p3.Y, p4.Y) || yi > Math.Max(p3.Y, p4.Y))
                return null;

            return new Point2D(xi, yi);
        }
    }
    
}
