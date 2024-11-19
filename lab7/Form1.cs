using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace lab6
{
    public partial class Form1 : Form
    {
        private Polyhedron polyhedron;
        private ProjectionType projection = ProjectionType.Perspective;

        public Form1()
        {
            InitializeComponent();
            polyhedron = new Polyhedron();
            polyhedron.CreateTetrahedron(); // По умолчанию отображаем тетраэдр
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            polyhedron.Draw(e.Graphics, ClientRectangle, projection);
            if (float.TryParse(x1TextBox.Text, out float x1) && float.TryParse(y1TextBox.Text, out float y1) && float.TryParse(z1TextBox.Text, out float z1) &&
                float.TryParse(x2TextBox.Text, out float x2) && float.TryParse(y2TextBox.Text, out float y2) && float.TryParse(z2TextBox.Text, out float z2) &&
                float.TryParse(RotateTextBox.Text, out float angle))
            {
                Point3D point1 = new Point3D(x1, y1, z1);
                Point3D point2 = new Point3D(x2, y2, z2);
                polyhedron.DrawLine(e.Graphics, point1, point2, ClientRectangle, projection);
            }
        }

        private void ScaleButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(ScaleTextBox.Text, out float factor))
            {
                polyhedron.Scale(factor);
                Invalidate();
            }
        }

        private void OffsetButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(OffsetXTextBox.Text, out float dx) &&
                float.TryParse(OffsetYTextBox.Text, out float dy) &&
                float.TryParse(OffsetZTextBox.Text, out float dz))
            {
                polyhedron.Offset(dx, dy, dz);
                Invalidate();
            }
        }

        private void RotateXButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(RotateTextBox.Text, out float angle))
            {
                polyhedron.RotateAroundAxis(Axis.X, angle);
                Invalidate();
            }
        }

        private void RotateYButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(RotateTextBox.Text, out float angle))
            {
                polyhedron.RotateAroundAxis(Axis.Y, angle);
                Invalidate();
            }
        }
        private void LoadModelButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "OBJ Files (*.obj)|*.obj|All Files (*.*)|*.*";
                openFileDialog.Title = "Load 3D Model";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        polyhedron = new Polyhedron();
                        polyhedron.LoadFromOBJ(openFileDialog.FileName);
                        Invalidate(); // Перерисовываем форму
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "OBJ files (*.obj)|*.obj",
                Title = "Сохранить объект"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                polyhedron.SaveToFile(saveFileDialog.FileName);
                MessageBox.Show("Файл успешно сохранён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BuildRevolutionFigureButton_Click(object sender, EventArgs e)
        {
            List<Point3D> generatingPoints = new List<Point3D>();
            foreach (DataGridViewRow row in GeneratingPointsGrid.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null && row.Cells[2].Value != null)
                {
                    
                    float x =  (float)Convert.ToDouble(row.Cells[0].Value);
                    float y = (float)Convert.ToDouble(row.Cells[1].Value);
                    float z = (float)Convert.ToDouble(row.Cells[2].Value);
                    generatingPoints.Add(new Point3D(x, y, z));
                }
            }

            string axis = AxisComboBox1.SelectedItem.ToString();
            int segments = int.Parse(SegmentsNumericUpDown.Text);

            polyhedron = CreateRevolutionFigure(generatingPoints, axis, segments);
            Invalidate(); // Перерисовка сцены
        }

        private void btnGenerateSurface_Click(object sender, EventArgs e)
        {
            try
            {
                // Чтение данных из формы
                float x0 = float.Parse(txtX0.Text);
                float x1 = float.Parse(txtX1.Text);
                float y0 = float.Parse(txtY0.Text);
                float y1 = float.Parse(txtY1.Text);
                int divisions = int.Parse(txtDivisions.Text);

                // Выбор функции
                Func<float, float, float> func = GetSelectedFunction();

                // Генерация поверхности
                Polyhedron surface = SurfaceGenerator.GenerateSurface(func, x0, x1, y0, y1, divisions);

                // Сохранение модели в файл
                string filePath = "surface.obj"; // Можете добавить диалог сохранения файла
                surface.SaveToFile(filePath);

                MessageBox.Show($"График сохранен в файл {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Функция для выбора нужной функции f(x, y)
        private Func<float, float, float> GetSelectedFunction()
        {
            if (cmbFunction.SelectedIndex == 0) // Синус и косинус
            {
                return (x, y) => (float)Math.Sin(x) * (float)Math.Cos(y);
            }
            else if (cmbFunction.SelectedIndex == 1) // x^2 + y^2
            {
                return (x, y) => x * x + y * y;
            }
            else
            {
                throw new Exception("Функция не выбрана!");
            }
        }
        public Polyhedron CreateRevolutionFigure(List<Point3D> generatingPoints, string axis, int segments)
        {
            Polyhedron polyhedron = new Polyhedron();
            List<Polygon> faces = new List<Polygon>();
            List<Point3D> allPoints = new List<Point3D>();

            // Угол шага (в радианах)
            float angleStep = (float)(2 * Math.PI / segments);

            // Генерация всех вершин
            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleStep;
                float[,] rotationMatrix = GetRotationMatrix(angle, axis);

                foreach (var point in generatingPoints)
                {
                    Point3D rotatedPoint = point.Transform(rotationMatrix);
                    allPoints.Add(rotatedPoint);
                }
            }

            // Создание граней
            int pointsPerLevel = generatingPoints.Count;
            for (int i = 0; i < segments; i++)
            {
                int nextSegment = (i + 1) % segments;

                for (int j = 0; j < pointsPerLevel - 1; j++)
                {
                    // Индексы текущего сегмента
                    int current = i * pointsPerLevel + j;
                    int next = current + 1;

                    // Индексы следующего сегмента
                    int currentNextSegment = nextSegment * pointsPerLevel + j;
                    int nextNextSegment = currentNextSegment + 1;

                    // Создание грани
                    Polygon face = new Polygon(new List<Point3D>
            {
                allPoints[current],
                allPoints[next],
                allPoints[nextNextSegment],
                allPoints[currentNextSegment]
            });

                    faces.Add(face);
                }
            }

            // Добавляем грани в полиэдр
            foreach (var face in faces)
            {
                polyhedron.faces.Add(face);
            }

            return polyhedron;
        }

        private float[,] GetRotationMatrix(float angle, string axis)
        {
            float cosA = (float)Math.Cos(angle);
            float sinA = (float)Math.Sin(angle);

            switch (axis.ToUpper())
            {
                case "X":
                    return new float[,]
                    {
                { 1, 0, 0, 0 },
                { 0, cosA, -sinA, 0 },
                { 0, sinA, cosA, 0 },
                { 0, 0, 0, 1 }
                    };
                case "Y":
                    return new float[,]
                    {
                { cosA, 0, sinA, 0 },
                { 0, 1, 0, 0 },
                { -sinA, 0, cosA, 0 },
                { 0, 0, 0, 1 }
                    };
                case "Z":
                    return new float[,]
                    {
                { cosA, -sinA, 0, 0 },
                { sinA, cosA, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
                    };
                default:
                    throw new ArgumentException("Invalid axis specified. Use 'X', 'Y', or 'Z'.");
            }
        }




        private void RotateZButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(RotateTextBox.Text, out float angle))
            {
                polyhedron.RotateAroundAxis(Axis.Z, angle);
                Invalidate();
            }
        }

        private void ReflectXYButton_Click(object sender, EventArgs e)
        {
            polyhedron.Reflect(Axis.Z);
            Invalidate();
        }

        private void ReflectXZButton_Click(object sender, EventArgs e)
        {
            polyhedron.Reflect(Axis.Y);
            Invalidate();
        }

        private void ReflectYZButton_Click(object sender, EventArgs e)
        {
            polyhedron.Reflect(Axis.X);
            Invalidate();
        }

        private void ProjectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProjectionComboBox.SelectedItem.ToString() == "Perspective")
                projection = ProjectionType.Perspective;
            else
                projection = ProjectionType.Axonometric;

            Invalidate();
        }

        private void TetrahedronButton_Click(object sender, EventArgs e)
        {
            polyhedron.CreateTetrahedron();
            Invalidate();
        }

        private void HexahedronButton_Click(object sender, EventArgs e)
        {
            polyhedron.CreateHexahedron();
            Invalidate();
        }

        private void OctahedronButton_Click(object sender, EventArgs e)
        {
            polyhedron.CreateOctahedron();
            Invalidate();
        }

        private void IcosahedronButton_Click(object sender, EventArgs e)
        {
            polyhedron.CreateIcosahedron();
            Invalidate();
        }

        private void DodecahedronButton_Click(object sender, EventArgs e)
        {
            polyhedron.CreateDodecahedron();
            Invalidate();
        }

        private void RotateAroundAxisCenterButton_Click(object sender, EventArgs e)
        {
            if (!validateRotateAroundAxisCenter())
            { 
                return;
            }
            if (Enum.TryParse(axisComboBox.SelectedItem.ToString(), out Axis axis) && float.TryParse(RotateTextBox.Text, out float angle))
            {
                polyhedron.RotateAroundAxisCenter(axis, angle);
                Invalidate();
            }
        }

        private void RotateAroundLineButton_Click(object sender, EventArgs e)
        {
            if(!validateRotateAroundLine())
            {
                return;
            }
            if (float.TryParse(x1TextBox.Text, out float x1) && float.TryParse(y1TextBox.Text, out float y1) && float.TryParse(z1TextBox.Text, out float z1) &&
                float.TryParse(x2TextBox.Text, out float x2) && float.TryParse(y2TextBox.Text, out float y2) && float.TryParse(z2TextBox.Text, out float z2) &&
                float.TryParse(RotateTextBox.Text, out float angle)
                )
            {
                Point3D point1 = new Point3D(x1, y1, z1);
                Point3D point2 = new Point3D(x2, y2, z2);
                polyhedron.RotateAroundLine(point1, point2, angle);
                Invalidate();
            }
        }

        bool isPoint(float x1, float x2, float y1, float y2, float z1, float z2)
        {
            return (x1 == x2) && (y1 == y2) && (z1 == z2);
        }

        bool validateRotateAroundLine()
        {
            if (
               !(float.TryParse(x1TextBox.Text, out float x1) && float.TryParse(y1TextBox.Text, out float y1) && float.TryParse(z1TextBox.Text, out float z1) &&
               float.TryParse(x2TextBox.Text, out float x2) && float.TryParse(y2TextBox.Text, out float y2) && float.TryParse(z2TextBox.Text, out float z2)
               )
            )
            {
                MessageBox.Show("Неправильно введённые координаты!");
                return false;
            }
            if (!(float.TryParse(RotateTextBox.Text, out float angle)))
            {
                MessageBox.Show("Неправильно указан параметр угла!");
                return false;
            }
            if (isPoint(float.Parse(x1TextBox.Text), float.Parse(x2TextBox.Text), float.Parse(y1TextBox.Text), float.Parse(y2TextBox.Text), float.Parse(z1TextBox.Text), float.Parse(z2TextBox.Text)))
            {
                MessageBox.Show("Неправильно введённые координаты (образуется точка)!");
                return false;
            };
            return true;
        }

        bool validateRotateAroundAxisCenter()
        {
            if(!(float.TryParse(RotateTextBox.Text, out float angle)))
            {
                MessageBox.Show("Неправильно указан параметр угла!");
                return false;
            }
            if(axisComboBox.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана ось!");
                return false;
            }
            return true;
        }
    }

    public class Point3D
    {
        public float X, Y, Z;

        public Point3D(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public Point3D Transform(float[,] matrix)
        {
            float newX = X * matrix[0, 0] + Y * matrix[0, 1] + Z * matrix[0, 2] + matrix[0, 3];
            float newY = X * matrix[1, 0] + Y * matrix[1, 1] + Z * matrix[1, 2] + matrix[1, 3];
            float newZ = X * matrix[2, 0] + Y * matrix[2, 1] + Z * matrix[2, 2] + matrix[2, 3];
            return new Point3D(newX, newY, newZ);
        }
    }

    public class Polygon
    {
        public List<Point3D> Points;

        public Polygon(List<Point3D> points)
        {
            Points = points;
        }
    }

    public class Polyhedron
    {
        public List<Polygon> faces = new List<Polygon>();
        public void AddFaces(List<Polygon> newFaces)
        {
            this.faces.AddRange(newFaces);
        }

        public void LoadFromOBJ(string filePath)
        {
            List<Point3D> vertices = new List<Point3D>();
            List<Polygon> polygons = new List<Polygon>();

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("v ")) // Вершины
                {
                    var parts = line.Replace('.', ',').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    vertices.Add(new Point3D(x, y, z));
                }
                else if (line.StartsWith("f ")) // Грани
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var facePoints = new List<Point3D>();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        int vertexIndex = int.Parse(parts[i].Split('/')[0]) - 1; // Индексы в OBJ начинаются с 1
                        facePoints.Add(vertices[vertexIndex]);
                    }
                    polygons.Add(new Polygon(facePoints));
                }
            }

            this.faces = polygons;
        }

        public void SaveToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Сохраняем вершины
                int vertexIndex = 1;
                Dictionary<Point3D, int> vertexIndices = new Dictionary<Point3D, int>();
                foreach (var face in faces)
                {
                    foreach (var point in face.Points)
                    {
                        if (!vertexIndices.ContainsKey(point))
                        {
                            writer.WriteLine($"v {(double)point.X} {(double)point.Y} {(double)point.Z}");
                            vertexIndices[point] = vertexIndex++;
                        }
                    }
                }

                // Сохраняем грани
                foreach (var face in faces)
                {
                    List<int> indices = new List<int>();
                    foreach (var point in face.Points)
                    {
                        indices.Add(vertexIndices[point]);
                    }
                    writer.WriteLine($"f {string.Join(" ", indices)}");
                }
            }
        }



        public void CreateTetrahedron()
        {
            Point3D p1 = new Point3D(1, 1, 1);
            Point3D p2 = new Point3D(-1, -1, 1);
            Point3D p3 = new Point3D(-1, 1, -1);
            Point3D p4 = new Point3D(1, -1, -1);

            faces.Clear();
            faces.Add(new Polygon(new List<Point3D> { p1, p2, p3 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p2, p4 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p3, p4 }));
            faces.Add(new Polygon(new List<Point3D> { p2, p3, p4 }));
        }

        public void CreateHexahedron()
        {
            Point3D p1 = new Point3D(-1, -1, -1);
            Point3D p2 = new Point3D(1, -1, -1);
            Point3D p3 = new Point3D(1, 1, -1);
            Point3D p4 = new Point3D(-1, 1, -1);
            Point3D p5 = new Point3D(-1, -1, 1);
            Point3D p6 = new Point3D(1, -1, 1);
            Point3D p7 = new Point3D(1, 1, 1);
            Point3D p8 = new Point3D(-1, 1, 1);

            faces.Clear();
            faces.Add(new Polygon(new List<Point3D> { p1, p2, p3, p4 }));
            faces.Add(new Polygon(new List<Point3D> { p5, p6, p7, p8 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p2, p6, p5 }));
            faces.Add(new Polygon(new List<Point3D> { p2, p3, p7, p6 }));
            faces.Add(new Polygon(new List<Point3D> { p3, p4, p8, p7 }));
            faces.Add(new Polygon(new List<Point3D> { p4, p1, p5, p8 }));
        }

        public void CreateOctahedron()
        {
            Point3D p1 = new Point3D(0, 0, 1);
            Point3D p2 = new Point3D(1, 0, 0);
            Point3D p3 = new Point3D(0, 1, 0);
            Point3D p4 = new Point3D(-1, 0, 0);
            Point3D p5 = new Point3D(0, -1, 0);
            Point3D p6 = new Point3D(0, 0, -1);

            faces.Clear();
            faces.Add(new Polygon(new List<Point3D> { p1, p2, p3 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p3, p4 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p4, p5 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p5, p2 }));
            faces.Add(new Polygon(new List<Point3D> { p6, p3, p2 }));
            faces.Add(new Polygon(new List<Point3D> { p6, p4, p3 }));
            faces.Add(new Polygon(new List<Point3D> { p6, p5, p4 }));
            faces.Add(new Polygon(new List<Point3D> { p6, p2, p5 }));
        }

        public void CreateIcosahedron()
        {
            float phi = (float)((1 + Math.Sqrt(5)) / 2); // Золотое сечение

            // Координаты вершин икосаэдра
            Point3D p1 = new Point3D(-1f, phi, 0f);
            Point3D p2 = new Point3D(1f, phi, 0f);
            Point3D p3 = new Point3D(-1f, -phi, 0f);
            Point3D p4 = new Point3D(1f, -phi, 0f);

            Point3D p5 = new Point3D(0f, -1f, phi);
            Point3D p6 = new Point3D(0f, 1f, phi);
            Point3D p7 = new Point3D(0f, -1f, -phi);
            Point3D p8 = new Point3D(0f, 1f, -phi);

            Point3D p9 = new Point3D(phi, 0f, -1f);
            Point3D p10 = new Point3D(phi, 0f, 1f);
            Point3D p11 = new Point3D(-phi, 0f, -1f);
            Point3D p12 = new Point3D(-phi, 0f, 1f);

            // Очистка и добавление граней икосаэдра
            faces.Clear();
            faces.Add(new Polygon(new List<Point3D> { p1, p2, p6 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p6, p12 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p12, p11 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p11, p8 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p8, p2 }));

            faces.Add(new Polygon(new List<Point3D> { p2, p8, p9 }));
            faces.Add(new Polygon(new List<Point3D> { p2, p9, p10 }));
            faces.Add(new Polygon(new List<Point3D> { p2, p10, p6 }));

            faces.Add(new Polygon(new List<Point3D> { p6, p10, p5 }));
            faces.Add(new Polygon(new List<Point3D> { p6, p5, p12 }));

            faces.Add(new Polygon(new List<Point3D> { p12, p5, p3 }));
            faces.Add(new Polygon(new List<Point3D> { p12, p3, p11 }));

            faces.Add(new Polygon(new List<Point3D> { p11, p3, p7 }));
            faces.Add(new Polygon(new List<Point3D> { p11, p7, p8 }));

            faces.Add(new Polygon(new List<Point3D> { p8, p7, p9 }));
            faces.Add(new Polygon(new List<Point3D> { p10, p9, p4 }));

            faces.Add(new Polygon(new List<Point3D> { p10, p4, p5 }));
            faces.Add(new Polygon(new List<Point3D> { p5, p4, p3 }));
            faces.Add(new Polygon(new List<Point3D> { p3, p4, p7 }));
            faces.Add(new Polygon(new List<Point3D> { p9, p7, p4 }));
        }

        public void CreateDodecahedron()
        {
            float phi = (float)((1 + Math.Sqrt(5)) / 2); // Золотое сечение

            // Координаты 20 вершин додекаэдра
            Point3D p1 = new Point3D(1, 1, 1);
            Point3D p2 = new Point3D(1, 1, -1);
            Point3D p3 = new Point3D(1, -1, 1);
            Point3D p4 = new Point3D(1, -1, -1);
            Point3D p5 = new Point3D(-1, 1, 1);
            Point3D p6 = new Point3D(-1, 1, -1);
            Point3D p7 = new Point3D(-1, -1, 1);
            Point3D p8 = new Point3D(-1, -1, -1);

            Point3D p9 = new Point3D(0, phi, 1 / phi);
            Point3D p10 = new Point3D(0, phi, -1 / phi);
            Point3D p11 = new Point3D(0, -phi, 1 / phi);
            Point3D p12 = new Point3D(0, -phi, -1 / phi);

            Point3D p13 = new Point3D(1 / phi, 0, phi);
            Point3D p14 = new Point3D(-1 / phi, 0, phi);
            Point3D p15 = new Point3D(1 / phi, 0, -phi);
            Point3D p16 = new Point3D(-1 / phi, 0, -phi);

            Point3D p17 = new Point3D(phi, 1 / phi, 0);
            Point3D p18 = new Point3D(phi, -1 / phi, 0);
            Point3D p19 = new Point3D(-phi, 1 / phi, 0);
            Point3D p20 = new Point3D(-phi, -1 / phi, 0);

            // Очистка и добавление граней додекаэдра
            faces.Clear();

            // Правильный порядок для 12 пятиугольных граней додекаэдра
            faces.Add(new Polygon(new List<Point3D> { p1, p9, p5, p14, p13 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p13, p3, p18, p17 }));
            faces.Add(new Polygon(new List<Point3D> { p1, p17, p2, p10, p9 }));
            faces.Add(new Polygon(new List<Point3D> { p2, p15, p4, p18, p17 }));
            faces.Add(new Polygon(new List<Point3D> { p2, p10, p6, p16, p15 }));
            faces.Add(new Polygon(new List<Point3D> { p3, p11, p7, p14, p13 }));
            faces.Add(new Polygon(new List<Point3D> { p3, p18, p4, p12, p11 }));
            faces.Add(new Polygon(new List<Point3D> { p4, p15, p16, p8, p12 }));
            faces.Add(new Polygon(new List<Point3D> { p5, p19, p6, p10, p9 }));
            faces.Add(new Polygon(new List<Point3D> { p5, p14, p7, p20, p19 }));
            faces.Add(new Polygon(new List<Point3D> { p6, p19, p20, p8, p16 }));
            faces.Add(new Polygon(new List<Point3D> { p7, p11, p12, p8, p20 }));


        }

        public void DrawAxes(Graphics g, Rectangle clientRect, ProjectionType projection)
        {
            // Ось X — красная, Y — зелёная, Z — синяя
            Point3D origin = new Point3D(0, 0, 0);
            Point3D xEnd = new Point3D(2, 0, 0);
            Point3D yEnd = new Point3D(0, 2, 0);
            Point3D zEnd = new Point3D(0, 0, 2);

            // Преобразуем точки для отображения в выбранной проекции
            Point origin2D = ProjectTo2D(origin, clientRect, projection);
            Point xEnd2D = ProjectTo2D(xEnd, clientRect, projection);
            Point yEnd2D = ProjectTo2D(yEnd, clientRect, projection);
            Point zEnd2D = ProjectTo2D(zEnd, clientRect, projection);

            // Рисуем оси
            g.DrawLine(Pens.Red, origin2D, xEnd2D);    // Ось X
            g.DrawLine(Pens.Green, origin2D, yEnd2D);  // Ось Y
            g.DrawLine(Pens.Blue, origin2D, zEnd2D);   // Ось Z
        }

        private Point ProjectTo2D(Point3D point, Rectangle clientRect, ProjectionType projection)
        {
            // Получаем матрицу проекции на основе типа проекции
            float[,] projectionMatrix = GetProjectionMatrix(projection);

            // Применяем проекцию к точке
            PointF projectedPoint = ApplyProjection(point, projectionMatrix, clientRect);

            // Преобразуем в целочисленные координаты для отрисовки
            return new Point((int)projectedPoint.X, (int)projectedPoint.Y);
        }

        public void DrawLine(Graphics g, Point3D p1, Point3D p2, Rectangle clientRect, ProjectionType projection)
        {
            // Преобразуем 3D точки в 2D с учетом проекции
            Point p1_2D = ProjectTo2D(p1, clientRect, projection);
            Point p2_2D = ProjectTo2D(p2, clientRect, projection);

            // Рисуем линию между этими точками
            g.DrawLine(Pens.Black, p1_2D, p2_2D);
        }

        public void Draw(Graphics g, Rectangle clientRect, ProjectionType projection)
        {
            DrawAxes(g, clientRect, projection);

            float[,] projectionMatrix = GetProjectionMatrix(projection);


            foreach (var face in faces)
            {
                Point[] points = new Point[face.Points.Count];
                for (int i = 0; i < face.Points.Count; i++)
                {
                    Point3D point = face.Points[i];
                    PointF projectedPoint = ApplyProjection(point, projectionMatrix, clientRect);
                    points[i] = new Point((int)projectedPoint.X, (int)projectedPoint.Y);
                }
                g.DrawPolygon(Pens.Black, points);
            }
        }

        // Метод для получения матрицы проекции
        private float[,] GetProjectionMatrix(ProjectionType projection)
        {
            if (projection == ProjectionType.Perspective)
            {
                float c = 5; // Коэффициент расстояния до камеры
                return new float[,]
                {
            { 1, 0, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 1, -1 / c },
            { 0, 0, 0, 1 }
                };
            }
            else if (projection == ProjectionType.Axonometric)
            {
                float phi = (float)(Math.PI / 4); // Угол φ
                float psi = (float)(Math.PI / 6); // Угол ψ
                return new float[,]
                {
            { (float)Math.Cos(psi), (float)(Math.Sin(phi) * Math.Sin(psi)), 0, 0 },
            { 0, (float)Math.Cos(phi), 0, 0 },
            { (float)Math.Sin(psi), (float)(-Math.Sin(phi) * Math.Cos(psi)), 0, 0 },
            { 0, 0, 0, 1 }
                };
            }
            else
            {
                throw new ArgumentException("Неизвестный тип проекции");
            }
        }

        // Метод для применения матрицы проекции к точке
        private PointF ApplyProjection(Point3D point, float[,] matrix, Rectangle clientRect)
        {
            // Умножаем вектор точки на матрицу проекции
            float x = point.X * matrix[0, 0] + point.Y * matrix[1, 0] + point.Z * matrix[2, 0] + matrix[3, 0];
            float y = point.X * matrix[0, 1] + point.Y * matrix[1, 1] + point.Z * matrix[2, 1] + matrix[3, 1];
            float z = point.X * matrix[0, 2] + point.Y * matrix[1, 2] + point.Z * matrix[2, 2] + matrix[3, 2];
            float w = point.X * matrix[0, 3] + point.Y * matrix[1, 3] + point.Z * matrix[2, 3] + matrix[3, 3];

            // Для перспективной проекции делим на коэффициент w
            if (w != 0)
            {
                x /= w;
                y /= w;
                z /= w;
            }

            // Приводим координаты к области отрисовки
            float screenX = clientRect.Width / 2 + x * 40;
            float screenY = clientRect.Height / 2 - y * 40;
            return new PointF(screenX, screenY);
        }

        public void Scale(float factor)
        {
            // Получаем центр многогранника через существующий метод GetCenter()
            Point3D center = GetCenter();
            float centerX = center.X;
            float centerY = center.Y;
            float centerZ = center.Z;

            // Матрица переноса к началу координат
            float[,] translateToOrigin = {
        { 1, 0, 0, -centerX },
        { 0, 1, 0, -centerY },
        { 0, 0, 1, -centerZ },
        { 0, 0, 0, 1 }
    };

            // Матрица масштабирования
            float[,] scaleMatrix = {
        { factor, 0, 0, 0 },
        { 0, factor, 0, 0 },
        { 0, 0, factor, 0 },
        { 0, 0, 0, 1 }
    };

            // Матрица обратного переноса к исходному положению
            float[,] translateBack = {
        { 1, 0, 0, centerX },
        { 0, 1, 0, centerY },
        { 0, 0, 1, centerZ },
        { 0, 0, 0, 1 }
    };

            // Применяем последовательное преобразование:
            // 1. Перемещаем центр к началу координат.
            // 2. Масштабируем.
            // 3. Возвращаем многогранник на исходное место.
            Transform(translateToOrigin);
            Transform(scaleMatrix);
            Transform(translateBack);
        }

        public void Offset(float dx, float dy, float dz)
        {
            float[,] offsetMatrix = {
                { 1, 0, 0, dx },
                { 0, 1, 0, dy },
                { 0, 0, 1, dz },
                { 0, 0, 0, 1 }
            };
            Transform(offsetMatrix);
        }

        public void RotateAroundAxis(Axis axis, float angle)
        {
            float radians = angle * (float)Math.PI / 180;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            float[,] rotationMatrix;
            if (axis == Axis.X)
            {
                rotationMatrix = new float[,] {
                    { 1, 0, 0, 0 },
                    { 0, cos, -sin, 0 },
                    { 0, sin, cos, 0 },
                    { 0, 0, 0, 1 }
                };
            }
            else if (axis == Axis.Y)
            {
                rotationMatrix = new float[,] {
                    { cos, 0, sin, 0 },
                    { 0, 1, 0, 0 },
                    { -sin, 0, cos, 0 },
                    { 0, 0, 0, 1 }
                };
            }
            else
            {
                rotationMatrix = new float[,] {
                    { cos, -sin, 0, 0 },
                    { sin, cos, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 }
                };
            }
            Transform(rotationMatrix);
        }

        public void RotateAroundAxisCenter(Axis axis, float angle)
        {
            // Сначала находим центр многогранника
            Point3D center = GetCenter();

            // Перемещаем многогранник так, чтобы его центр оказался в начале координат
            Offset(-center.X, -center.Y, -center.Z);

            // Выполняем вращение вокруг оси
            RotateAroundAxis(axis, angle);

            // Возвращаем многогранник на место
            Offset(center.X, center.Y, center.Z);
        }

        private Point3D GetCenter()
        {
            float x = 0, y = 0, z = 0;
            int pointCount = 0;
            foreach (var face in faces)
            {
                foreach (var point in face.Points)
                {
                    x += point.X;
                    y += point.Y;
                    z += point.Z;
                    pointCount++;
                }
            }
            return new Point3D(x / pointCount, y / pointCount, z / pointCount);
        }

        public void RotateAroundLine(Point3D point1, Point3D point2, float angle)
        {
            // 1. Находим вектор направления прямой
            float dx = point2.X - point1.X;
            float dy = point2.Y - point1.Y;
            float dz = point2.Z - point1.Z;

            // 2. Нормализуем вектор направления прямой
            float length = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            dx /= length;
            dy /= length;
            dz /= length;

            // 3. Перемещаем многогранник так, чтобы точка point1 стала в начале координат
            Offset(-point1.X, -point1.Y, -point1.Z);

            // 4. Выполняем вращение вокруг оси
            float radians = angle * (float)Math.PI / 180;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            // Создаем матрицу для вращения вокруг произвольной оси
            float[,] rotationMatrix = new float[,]
            {
        { cos + dx * dx * (1 - cos), dx * dy * (1 - cos) - dz * sin, dx * dz * (1 - cos) + dy * sin, 0 },
        { dy * dx * (1 - cos) + dz * sin, cos + dy * dy * (1 - cos), dy * dz * (1 - cos) - dx * sin, 0 },
        { dz * dx * (1 - cos) - dy * sin, dz * dy * (1 - cos) + dx * sin, cos + dz * dz * (1 - cos), 0 },
        { 0, 0, 0, 1 }
            };

            Transform(rotationMatrix);

            // 5. Возвращаем многогранник обратно
            Offset(point1.X, point1.Y, point1.Z);
        }

        public void Reflect(Axis axis)
        {
            float[,] reflectMatrix;
            if (axis == Axis.X)
            {
                reflectMatrix = new float[,] {
                    { -1, 0, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 }
                };
            }
            else if (axis == Axis.Y)
            {
                reflectMatrix = new float[,] {
                    { 1, 0, 0, 0 },
                    { 0, -1, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 }
                };
            }
            else
            {
                reflectMatrix = new float[,] {
                    { 1, 0, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, -1, 0 },
                    { 0, 0, 0, 1 }
                };
            }
            Transform(reflectMatrix);
        }

        private void Transform(float[,] matrix)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                for (int j = 0; j < faces[i].Points.Count; j++)
                {
                    faces[i].Points[j] = faces[i].Points[j].Transform(matrix);
                }
            }
        }
    }
    public class SurfaceGenerator
    {
        public static Polyhedron GenerateSurface(Func<float, float, float> func, float x0, float x1, float y0, float y1, int divisions)
        {
            List<Point3D> vertices = new List<Point3D>();
            List<Polygon> faces = new List<Polygon>();

            float stepX = (x1 - x0) / divisions;
            float stepY = (y1 - y0) / divisions;

            // Генерация вершин
            for (int i = 0; i <= divisions; i++)
            {
                float x = x0 + i * stepX;
                for (int j = 0; j <= divisions; j++)
                {
                    float y = y0 + j * stepY;
                    float z = func(x, y);
                    vertices.Add(new Point3D(x, y, z));
                }
            }

            // Генерация граней
            for (int i = 0; i < divisions; i++)
            {
                for (int j = 0; j < divisions; j++)
                {
                    // Индексы вершин текущего сегмента
                    int topLeft = i * (divisions + 1) + j;
                    int topRight = topLeft + 1;
                    int bottomLeft = (i + 1) * (divisions + 1) + j;
                    int bottomRight = bottomLeft + 1;

                    // Первая треугольная грань
                    faces.Add(new Polygon(new List<Point3D>
                {
                    vertices[topLeft],
                    vertices[bottomLeft],
                    vertices[bottomRight]
                }));

                    // Вторая треугольная грань
                    faces.Add(new Polygon(new List<Point3D>
                {
                    vertices[topLeft],
                    vertices[bottomRight],
                    vertices[topRight]
                }));
                }
            }

            Polyhedron polyhedron = new Polyhedron();
            polyhedron.AddFaces(faces);
            return polyhedron;
        }
    }


    public enum Axis { X, Y, Z }
    public enum ProjectionType { Perspective, Axonometric }
}
