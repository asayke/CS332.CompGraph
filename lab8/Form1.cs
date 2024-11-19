using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CG_Lab
{
    public partial class Form1 : Form
    {
        private enum RenderingOp
        {
            DrawCube = 0, DrawTetrahedron, DrawOctahedron, DrawIcosahedron, DrawDodecahedron,
            Func1, Func2, Func3, Func4
        }

        private enum AffineOp { Move = 0, Scaling, Rotation, LineRotation, AxisXRotation, AxisYRotation, AxisZRotation }

        private enum Operation { Reflect_XY = 0, Reflect_YZ = 1, Reflect_XZ = 2 }

        Graphics g;
        Bitmap clearPB;

        string currPlane = "XY";
        Pen defaultPen = new Pen(Color.Black, 3);

        PolyHedron currentPolyhedron;
        private List<Vertex> profilePoints = new List<Vertex>();

        Camera camera;

        private Timer actionTimer;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            g = Graphics.FromImage(pictureBox1.Image);
            clearPB = new Bitmap(pictureBox1.Image);

            camera = new Camera(
                                position:    new Vertex(pictureBox1.Width / 2, pictureBox1.Height / 2, -10f),
                                target:      new Vertex(pictureBox1.Width / 2, pictureBox1.Height / 2, 0),
                                fov:         (float)Math.PI / 3,
                                aspectRatio: 1f, // (float)pictureBox1.Width / pictureBox1.Height,
                                near:        0.1f,
                                far:         100f
                                );

            currentPolyhedron = PolyHedron.GetCube().Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);

            reflectionComboBox.Items.Add("Отображение на XY");
            reflectionComboBox.Items.Add("Отображение на YZ");
            reflectionComboBox.Items.Add("Отображение на XZ");

            comboBox1.SelectedIndex = 0;

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.KeyUp += MyForm_KeyUp;

            actionTimer = new Timer();
            actionTimer.Interval = (int)(1000f / 60f); // 60 frames per second
            actionTimer.Tick += ActionTimer_Tick;

            RenderScene();
        }

        private void ActionTimer_Tick(object sender, EventArgs e)
        {
            RenderScene();
        }

        private void RenderScene()
        {
            if (currentPolyhedron == null) return;

            // Получаем View и Projection матрицы из камеры
            var viewMatrix = camera.ViewMatrix;
            var projectionMatrix = camera.ProjectionMatrix;

            // Очистка экрана
            pictureBox1.Image = clearPB;
            
            foreach (var face in currentPolyhedron.Faces)
            {
                List<PointF> projectedPoints = new List<PointF>();

                // Перебор вершин грани
                foreach (var vertex in face.Vertices)
                {
                    // Преобразование в пространстве камеры
                    Vertex transformedVertex = currentPolyhedron.Vertices[vertex] * viewMatrix;

                    // Преобразование в экранные координаты
                    transformedVertex *= projectionMatrix;

                    // Масштабирование для соответствия размеру экрана
                    float x = (transformedVertex.X + 1) * pictureBox1.Width / 2;
                    float y = (1 - transformedVertex.Y) * pictureBox1.Height / 2;

                    projectedPoints.Add(new PointF(x, y));
                }

                // Отрисовка грани, если она видима
                if (projectedPoints.Count >= 3)
                {
                    for (int i = 1; i < projectedPoints.Count; i++)
                        DrawLineVu(pictureBox1, Color.Black, projectedPoints[i - 1], projectedPoints[i]);
                    DrawLineVu(pictureBox1, Color.Black, projectedPoints[projectedPoints.Count - 1], projectedPoints[0]);
                }
            }

            pictureBox1.Invalidate();
        }


        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            HandleCameraInput(e.KeyCode);
        }

        private float moveSpeed = 0.2f;
        private float rotateSpeed = 0.03f;
        private float horizontalAngle = (float)Math.PI / 2;
        private float verticalAngle = 0f;

        private void HandleCameraInput(Keys key)
        {
            Keys[] contolKeys = { Keys.W, Keys.A, Keys.S, Keys.D, Keys.Up, Keys.Down, Keys.Right, Keys.Left };

            if (contolKeys.Contains(key))
            {
                switch (key)
                {
                    case Keys.W: // Вперед
                        camera.Position += moveSpeed * (camera.Target - camera.Position).Normalize();
                        break;
                    case Keys.S: // Назад
                        camera.Position -= moveSpeed * (camera.Target - camera.Position).Normalize();
                        break;
                    case Keys.A: // Влево
                        camera.Position += moveSpeed * Vertex.Cross(camera.Up, camera.Target - camera.Position);
                        break;
                    case Keys.D: // Вправо
                        camera.Position -= moveSpeed * Vertex.Cross(camera.Up, camera.Target - camera.Position);
                        break;
                    case Keys.Up: // Поворот вверх
                        verticalAngle += rotateSpeed;
                        break;
                    case Keys.Down: // Поворот вниз
                        verticalAngle -= rotateSpeed;
                        break;
                    case Keys.Left: // Поворот влево
                        horizontalAngle += rotateSpeed;
                        break;
                    case Keys.Right: // Поворот вправо
                        horizontalAngle -= rotateSpeed;
                        break;
                }

                // Обновить направление камеры
                Vertex direction = new Vertex(
                    (float)(Math.Cos(horizontalAngle) * Math.Cos(verticalAngle)),
                    (float)Math.Sin(verticalAngle),
                    (float)(Math.Sin(horizontalAngle) * Math.Cos(verticalAngle))
                );

                camera.SetDirection(direction);

                actionTimer.Start();
            }
        }

        private void MyForm_KeyUp(object sender, KeyEventArgs e)
        {
            actionTimer.Stop();
        }

        private void DrawPolyhedron(PolyHedron polyhedron, string plane)
        {
            float scaleFactor = (float)numericScale.Value;
            //g.Clear(pictureBox1.BackColor);

            pictureBox1.Image = clearPB;

            PolyHedron ph = polyhedron.ScaledAroundCenter(scaleFactor, scaleFactor, scaleFactor);

            float centerY = pictureBox1.Height / 2;

            foreach (var face in ph.Faces)
            {
                var points = new List<PointF>();

                foreach (var vertexIndex in face.Vertices)
                {
                    Vertex v = ph.Vertices[vertexIndex];
                    Vertex projectedVertex;

                    projectedVertex = new Vertex(v.X, v.Y, v.Z);
                    PointF projectedPoint = projectedVertex.GetProjection(projectionListBox.SelectedIndex, pictureBox1.Width / 2, pictureBox1.Height / 2,
                        (float)axisXNumeric.Value, (float)axisYNumeric.Value);

                    points.Add(projectedPoint);
                }
                for (int i = 1; i < points.Count; i++)
                    DrawLineVu(pictureBox1, Color.Black, points[i - 1], points[i]);
                DrawLineVu(pictureBox1, Color.Black, points[points.Count - 1], points[0]);
                //g.DrawPolygon(defaultPen, points.ToArray());
            }

            double cX = 0, cY = 0, cZ = 0;
            polyhedron.FindCenter(polyhedron.Vertices, ref cX, ref cY, ref cZ);

            textBox1.Text = cX.ToString();
            textBox2.Text = cY.ToString();
            textBox3.Text = cZ.ToString();

            //pictureBox1.Invalidate();
        }

        public void DrawLineVu(PictureBox pictureBox, Color color, PointF p0, PointF p1)
        {
            int x0 = (int)p0.X;
            int x1 = (int)p1.X;
            int y0 = (int)p0.Y;
            int y1 = (int)p1.Y;

            Bitmap pb = new Bitmap(pictureBox.Image);

            if (x1 >= 0 && x1 < pictureBox.Width && y1 >= 0 && y1 < pictureBox.Height)
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
                    for (int x = x0 + 1; x <= x1; x++)
                    {
                        if (x >= 0 && x < pictureBox.Width && (int)y >= 0 && (int)y < pictureBox.Height)
                        {
                            int alpha = (int)((1 - (y - (int)y)) * 255);
                            pb.SetPixel(x, (int)y, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
                        if (x >= 0 && x < pictureBox.Width && (int)y + 1 >= 0 && (int)y + 1 < pictureBox.Height)
                        {
                            int alpha = (int)((y - (int)y) * 255);
                            pb.SetPixel(x, (int)y + 1, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
                        y += gradient;
                    }
                }
                else
                {
                    gradient *= -1;
                    float y = y0 + gradient;
                    for (int x = x0 - 1; x >= x1; x--)
                    {
                        if (x >= 0 && x < pictureBox.Width && (int)y >= 0 && (int)y < pictureBox.Height)
                        {
                            int alpha = (int)((1 - (y - (int)y)) * 255);
                            pb.SetPixel(x, (int)y, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
                        if (x >= 0 && x < pictureBox.Width && (int)y + 1 >= 0 && (int)y + 1 < pictureBox.Height)
                        {
                            int alpha = (int)((y - (int)y) * 255);
                            pb.SetPixel(x, (int)y + 1, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
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
                    for (int y = y0 + 1; y <= y1; y++)
                    {
                        if ((int)x >= 0 && (int)x < pictureBox.Width && y >= 0 && y < pictureBox.Height)
                        {
                            int alpha = (int)((1 - (x - (int)x)) * 255);
                            pb.SetPixel((int)x, y, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
                        if ((int)x + 1 >= 0 && (int)x + 1 < pictureBox.Width && y >= 0 && y < pictureBox.Height)
                        {
                            int alpha = (int)((x - (int)x) * 255);
                            pb.SetPixel((int)x + 1, y, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
                        x += gradient;
                    }
                }
                else
                {
                    gradient *= -1;
                    float x = x0 + gradient;
                    for (int y = y0 - 1; y >= y1; y--)
                    {
                        if ((int)x >= 0 && (int)x < pictureBox.Width && y >= 0 && y < pictureBox.Height)
                        {
                            int alpha = (int)((1 - (x - (int)x)) * 255);
                            pb.SetPixel((int)x, y, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
                        if ((int)x + 1 >= 0 && (int)x + 1 < pictureBox.Width && y >= 0 && y < pictureBox.Height)
                        {
                            int alpha = (int)((x - (int)x) * 255);
                            pb.SetPixel((int)x + 1, y, Color.FromArgb(255, Math.Max(0, Math.Min(255, color.R * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.G * alpha / 255 + (255 - alpha) * 255 / 255)),
                                Math.Max(0, Math.Min(255, color.B * alpha / 255 + (255 - alpha) * 255 / 255))));
                        }
                        x += gradient;
                    }
                }
            }
            pictureBox.Image = pb;
        }

        ~Form1()
        {
            g.Dispose();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // g.Clear(pictureBox1.BackColor);
            

            switch (comboBox1.SelectedIndex)
            {
                case (int)RenderingOp.DrawCube:
                    currentPolyhedron = PolyHedron.GetCube().Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    /*DrawPolyhedron(currentPolyhedron = PolyHedron.GetCube()
                                             // .Rotated(20, 20, 0)
                                             .Scaled(100, 100, 100)
                                             .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0),
                                             currPlane);*/
                    break;
                case (int)RenderingOp.DrawTetrahedron:
                    currentPolyhedron = PolyHedron.GetTetrahedron().Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    /*DrawPolyhedron(currentPolyhedron = PolyHedron.GetTetrahedron()
                                             .Rotated(10, 10, 0)
                                             .Scaled(100, 100, 100)
                                             .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0),
                                             currPlane);*/
                    break;
                case (int)RenderingOp.DrawOctahedron:
                    currentPolyhedron = PolyHedron.GetOctahedron().Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    /*DrawPolyhedron(currentPolyhedron = PolyHedron.GetOctahedron()
                                             .Rotated(20, 20, 0)
                                             .Scaled(100, 100, 100)
                                             .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0),
                                             currPlane);*/
                    break;
                case (int)RenderingOp.DrawIcosahedron:
                    currentPolyhedron = PolyHedron.GetIcosahedron().Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    /*DrawPolyhedron(currentPolyhedron = PolyHedron.GetIcosahedron()
                                             .Rotated(10, 10, 0)
                                             .Scaled(150, 150, 150)
                                             .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0),
                                             currPlane);*/
                    break;
                case (int)RenderingOp.DrawDodecahedron:
                    currentPolyhedron = PolyHedron.GetDodecahedron().Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    /*DrawPolyhedron(currentPolyhedron = PolyHedron.GetDodecahedron()
                                             .Rotated(10, 10, 0)
                                             .Scaled(200, 200, 200)
                                             .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0),
                                             currPlane);*/
                    break;

            }

            RenderScene();
        }

        private void reflectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericScale.Value = 1;
            g.Clear(pictureBox1.BackColor);

            switch (reflectionComboBox.SelectedIndex)
            {
                case (int)Operation.Reflect_XY:
                    currPlane = "XY";
                    currentPolyhedron = currentPolyhedron.Moved(-pictureBox1.Width / 2, -pictureBox1.Height / 2, 0)
                        .Reflected("XY")
                        .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    DrawPolyhedron(currentPolyhedron, "XY");
                    break;
                case (int)Operation.Reflect_YZ:
                    currPlane = "YZ";
                    currentPolyhedron = currentPolyhedron.Moved(-pictureBox1.Width / 2, -pictureBox1.Height / 2, 0)
                        .Reflected("YZ")
                        .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    DrawPolyhedron(currentPolyhedron, "YZ");
                    break;
                case (int)Operation.Reflect_XZ:
                    currPlane = "XZ";
                    currentPolyhedron = currentPolyhedron.Moved(-pictureBox1.Width / 2, -pictureBox1.Height / 2, 0)
                        .Reflected("XZ")
                        .Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
                    DrawPolyhedron(currentPolyhedron, "XZ");
                    break;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            numericUpDown12.Value = numericUpDown4.Value;
            numericUpDown11.Value = numericUpDown5.Value;
            numericUpDown4.Value = e.X;
            numericUpDown5.Value = e.Y;

            profilePoints.Add(new Vertex(e.X - pictureBox1.Width / 2, e.Y - pictureBox1.Height / 2, 0));
            DrawProfile();
        }
        private void DrawProfile()
        {
            // Отображаем образующую на PictureBox
            var g = pictureBox1.CreateGraphics();
            g.Clear(pictureBox1.BackColor);

            foreach (var point in profilePoints)
            {
                g.FillEllipse(Brushes.Red, point.X - 2 + pictureBox1.Width / 2, point.Y - 2 + pictureBox1.Height / 2, 4, 4);
            }
        }



        private void affineOpButton_Click(object sender, EventArgs e)
        {
            g.Clear(pictureBox1.BackColor);

            Vertex anchor;
            double centerX, centerY, centerZ;

            switch (comboBox2.SelectedIndex)
            {
                case (int)AffineOp.Move:
                    DrawPolyhedron(currentPolyhedron = currentPolyhedron
                                                       .Moved((float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value),
                                                       currPlane);
                    break;
                case (int)AffineOp.Scaling:
                    anchor = new Vertex((float)numericUpDown4.Value, (float)numericUpDown5.Value, (float)numericUpDown6.Value);

                    DrawPolyhedron(currentPolyhedron = currentPolyhedron
                                                       .Moved(-anchor.X, -anchor.Y, -anchor.Z)
                                                       .Scaled((float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value)
                                                       .Moved(anchor.X, anchor.Y, anchor.Z),
                                                       currPlane);
                    break;

                case (int)AffineOp.Rotation:
                    anchor = new Vertex((float)numericUpDown4.Value, (float)numericUpDown5.Value, (float)numericUpDown6.Value);

                    DrawPolyhedron(currentPolyhedron = currentPolyhedron
                                                       .Moved(-anchor.X, -anchor.Y, -anchor.Z)
                                                       .Rotated((float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value)
                                                       .Moved(anchor.X, anchor.Y, anchor.Z),
                                                       currPlane);
                    break;
                case (int)AffineOp.LineRotation:
                    anchor = new Vertex((float)numericUpDown4.Value, (float)numericUpDown5.Value, (float)numericUpDown6.Value);
                    Vertex v = new Vertex(anchor.X - (float)numericUpDown12.Value, anchor.Y - (float)numericUpDown11.Value, anchor.Z - (float)numericUpDown10.Value);
                    float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
                    float l = v.X / length;
                    float m = v.Y / length;
                    float n = v.Z / length;
                    DrawPolyhedron(currentPolyhedron = currentPolyhedron
                        .Moved(-anchor.X, -anchor.Y, -anchor.Z)
                        .LineRotated(l, m, n, (float)numericUpDown1.Value)
                        .Moved(anchor.X, anchor.Y, anchor.Z),
                        currPlane);
                    break;
                case (int)AffineOp.AxisXRotation:
                    centerX = 0;
                    centerY = 0;
                    centerZ = 0;
                    currentPolyhedron.FindCenter(currentPolyhedron.Vertices, ref centerX, ref centerY, ref centerZ);

                    DrawPolyhedron(currentPolyhedron = currentPolyhedron
                                                       .Moved((float)-centerX, (float)-centerY, (float)-centerZ)
                                                       .Rotated((float)numericUpDown1.Value, 0, 0)
                                                       .Moved((float)centerX, (float)centerY, (float)centerZ),
                                                       currPlane);
                    break;
                case (int)AffineOp.AxisYRotation:
                    centerX = 0;
                    centerY = 0;
                    centerZ = 0;
                    currentPolyhedron.FindCenter(currentPolyhedron.Vertices, ref centerX, ref centerY, ref centerZ);

                    DrawPolyhedron(currentPolyhedron = currentPolyhedron
                                                       .Moved((float)-centerX, (float)-centerY, (float)-centerZ)
                                                       .Rotated(0, (float)numericUpDown2.Value, 0)
                                                       .Moved((float)centerX, (float)centerY, (float)centerZ),
                                                       currPlane);
                    break;
                case (int)AffineOp.AxisZRotation:
                    centerX = 0;
                    centerY = 0;
                    centerZ = 0;
                    currentPolyhedron.FindCenter(currentPolyhedron.Vertices, ref centerX, ref centerY, ref centerZ);

                    DrawPolyhedron(currentPolyhedron = currentPolyhedron
                                                       .Moved((float)-centerX, (float)-centerY, (float)-centerZ)
                                                       .Rotated(0, 0, (float)numericUpDown2.Value)
                                                       .Moved((float)centerX, (float)centerY, (float)centerZ),
                                                       currPlane);
                    break;
            }
        }
       
        
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DrawPolyhedron(currentPolyhedron, currPlane);
        }

        private void axisXNumeric_ValueChanged(object sender, EventArgs e)
        {
            axisZNumeric.Value = 360 - axisXNumeric.Value - axisYNumeric.Value;
            DrawPolyhedron(currentPolyhedron, currPlane);
        }

        private void axisYNumeric_ValueChanged(object sender, EventArgs e)
        {
            axisZNumeric.Value = 360 - axisXNumeric.Value - axisYNumeric.Value;
            DrawPolyhedron(currentPolyhedron, currPlane);
        }

        private void projectionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            g.Clear(pictureBox1.BackColor);
            DrawPolyhedron(currentPolyhedron, currPlane);
        }


        private void DrawRevolveFigure_Click(object sender, EventArgs e)
        {
            int divisions = (int)numericUpDownDivisions.Value;
            char axis = comboBoxAxis.SelectedItem.ToString()[0];

            /*
            if (axis == 'X')
                currentPolyhedron = PolyHedron.GetRevolvedFigure(profilePoints, divisions, axis)
                                           .Moved(0, (float)pictureBox1.Height / 2 , (float)0);
            else if (axis == 'Z')
                currentPolyhedron = PolyHedron.GetRevolvedFigure(profilePoints, divisions, axis)
                                           .Moved((float)pictureBox1.Width / 2, (float)pictureBox1.Height / 2, (float)0);
            else
                currentPolyhedron = PolyHedron.GetRevolvedFigure(profilePoints, divisions, axis)
                                           .Moved((float)pictureBox1.Width / 2, 0, (float)0);
            */
            currentPolyhedron = PolyHedron.GetRevolvedFigure(profilePoints, divisions, axis)
                .Moved(pictureBox1.Width / 2 , pictureBox1.Height / 2, 0);
            
            // Рисуем полученную фигуру вращения
            DrawPolyhedron(currentPolyhedron, currPlane);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            profilePoints.Clear();
            //g.Clear(pictureBox1.BackColor);
            //pictureBox1.Invalidate();
            pictureBox1.Image = clearPB;
        }

        private void numericUpDownDivisions_ValueChanged(object sender, EventArgs e)
        {
            //DrawRevolveFigure_Click(sender,e);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SaveModel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadModel();
        }

        private void SaveModel()
        {

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //try
                {
                    currentPolyhedron.SaveToObj(saveFileDialog1.FileName);
                    MessageBox.Show("Модель успешно сохранена.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                /*catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
            }
        }

        private void LoadModel()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DrawPolyhedron(currentPolyhedron = PolyHedron.LoadFromObj(openFileDialog1.FileName), currPlane);
                        //.Scaled(100, 100, 100)
                        //.RotatedXAxis(180)
                        //.Moved(pictureBox1.Width / 2, pictureBox1.Height / 2, 0), currPlane);
                    MessageBox.Show("Модель успешно загружена.", "Загрузка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }
    }

    public class Camera
    {
        public Vertex Position { get; set; } // Позиция камеры
        public Vertex Target { get; set; }   // Точка, куда смотрит камера
        public Vertex Up { get; set; } = new Vertex(0, -1, 0); // Вектор вверх
        public Matrix<float> ViewMatrix => Utilities.CreateViewMatrix(Position, Target, Up);
        public Matrix<float> ProjectionMatrix { get; set; }

        public Camera(Vertex position, Vertex target, float fov, float aspectRatio, float near, float far)
        {
            Position = position;
            SetDirection(new Vertex(0, 0, 1));
            ProjectionMatrix = Utilities.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);
        }

        public void SetDirection(Vertex direction)
        {
            // float distance = (Target - Position).Length();

            Target = Position + direction; // Сместить цель взгляда по направлению
        }
    }

    public static class Utilities
    {
        public static Matrix<float> CreateViewMatrix(Vertex position, Vertex target, Vertex up)
        {
            // Вектор направления камеры (от камеры к цели)
            Vertex forward = (target - position).Normalize();

            // Вектор правой стороны камеры
            Vertex right = Vertex.Cross(up, forward).Normalize();

            // Новый вектор "вверх" с учётом правого и направления
            Vertex adjustedUp = Vertex.Cross(forward, right);

            // Матрица вида
            return new float[4, 4] {
               { right.X, adjustedUp.X, forward.X, 0 },
               { right.Y, adjustedUp.Y, forward.Y, 0 },
               { right.Z, adjustedUp.Z, forward.Z, 0 },
               { -Vertex.Dot(right, position), -Vertex.Dot(adjustedUp, position), -Vertex.Dot(forward, position), 1 }
            };
        }

        public static Matrix<float> CreatePerspectiveFieldOfView(float fov, float aspectRatio, float near, float far)
        {
            if (fov <= 0 || fov >= Math.PI)
                throw new ArgumentOutOfRangeException(nameof(fov), "Field of view must be in range (0, π).");
            if (aspectRatio <= 0)
                throw new ArgumentOutOfRangeException(nameof(aspectRatio), "Aspect ratio must be greater than zero.");
            if (near <= 0)
                throw new ArgumentOutOfRangeException(nameof(near), "Near plane distance must be greater than zero.");
            if (far <= near)
                throw new ArgumentOutOfRangeException(nameof(far), "Far plane distance must be greater than near plane distance.");

            float f = 1.0f / (float)Math.Tan(fov / 2.0f); // Cotangent of half the field of view

            return new float[4, 4] {
                { f / aspectRatio, 0, 0, 0 },
                { 0, f, 0, 0 },
                { 0, 0, (far + near) / (near - far), -1 },
                { 0, 0, (2 * far * near) / (near - far), 0 }
                /*{ f / aspectRatio, 0, 0, 0 },
                { 0, f, 0, 0 },
                { 0, 0, (far + near) / (near - far), (2 * far * near) / (near - far) },
                { 0, 0, -1, 0 }*/
                /*{ f / aspectRatio, 0, 0, 0 },
                { 0, f, 0, 0 },
                { 0, 0, (far) / (near - far), -1 },
                { 0, 0, (far * near) / (near - far), 0 }*/
            };
        }
    }

    public class Matrix<T> where T : struct, IConvertible
    {
        public T[,] Values { get; }

        public Matrix(T[,] values)
        {
            Values = values;
        }

        public static implicit operator Matrix<T>(T[,] values)
        {
            return new Matrix<T>(values);
        }

        /*public static implicit operator Vertex(Matrix<T> m)
        {
            return new Vertex(Convert.ToSingle(m[0, 0]), Convert.ToSingle(m[0, 1]), Convert.ToSingle(m[0, 2]));
        }*/

        public static implicit operator Normal(Matrix<T> m)
        {
            return new Normal(Convert.ToSingle(m[0, 0]), Convert.ToSingle(m[0, 1]), Convert.ToSingle(m[0, 2]));
        }

        public Matrix(Vertex vertex)
        {
            Values = new T[1, 4] {
                  {
                    (T)Convert.ChangeType(vertex.X, typeof(T)),
                    (T)Convert.ChangeType(vertex.Y, typeof(T)),
                    (T)Convert.ChangeType(vertex.Z, typeof(T)),
                    (T)Convert.ChangeType(1, typeof(T))
                  }
            };
        }

        public Matrix(Normal normal)
        {
            Values = new T[1, 4] {
                  {
                    (T)Convert.ChangeType(normal.NX, typeof(T)),
                    (T)Convert.ChangeType(normal.NY, typeof(T)),
                    (T)Convert.ChangeType(normal.NZ, typeof(T)),
                    (T)Convert.ChangeType(1, typeof(T))
                  }
            };
        }

        public static implicit operator Matrix<T>(Vertex vertex)
        {
            return new Matrix<T>(vertex);
        }

        public static implicit operator Matrix<T>(Normal normal)
        {
            return new Matrix<T>(normal);
        }

        public static Matrix<T> operator *(Matrix<T> A, Matrix<T> B)
        {
            int rowsA = A.Values.GetLength(0);
            int colsA = A.Values.GetLength(1);
            int rowsB = B.Values.GetLength(0);
            int colsB = B.Values.GetLength(1);

            T[,] result = new T[rowsA, colsB];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    result[i, j] = default;
                    for (int k = 0; k < colsA; k++)
                    {
                        result[i, j] += (dynamic)A.Values[i, k] * (dynamic)B.Values[k, j];
                    }
                }
            }

            return new Matrix<T>(result);
        }

        public T this[int row, int column]
        {
            get
            {
                return Values[row, column];
            }
            set
            {
                Values[row, column] = value;
            }
        }


    }

    // Нормаль вершины
    public struct Normal
    {
        public float NX { get; private set; }
        public float NY { get; private set; }
        public float NZ { get; private set; }

        public Normal(float nx, float ny, float nz)
        {
            // Нормируем сразу
            float length = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
            if (length > 0)
            {
                NX = nx / length;
                NY = ny / length;
                NZ = nz / length;
            }
            else
            {
                NX = 0;
                NY = 0;
                NZ = 0;
            }
        }
    }

    public struct Vertex
    {
        public float X { get; private set; }

        public float Y { get; private set; }

        public float Z { get; private set; }

        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public Vertex Clone()
        {
            return new Vertex(X, Y, Z);
        }

        public static implicit operator Vertex(Matrix<float> m)
        {
            if (m[0, 3] != 0)
            {
                return new Vertex(m[0, 0] / m[0, 3], m[0, 1] / m[0, 3], m[0, 2] / m[0, 3]);
            }
            else
            {
                return new Vertex(m[0, 0], m[0, 1], m[0, 2]);
            }
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        // Метод для вычитания двух векторов (вершин)
        public static Vertex Subtract(Vertex v1, Vertex v2)
        {
            return new Vertex(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vertex operator-(Vertex v1, Vertex v2)
        {
            return Subtract(v1, v2);
        }

        public static Vertex operator+(Vertex v1, Vertex v2)
        {
            return new Vertex(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vertex operator*(float c, Vertex v)
        {
            return new Vertex(c * v.X, c * v.Y, c * v.Z);
        }

        // Нормализация вектора
        public Vertex Normalize()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            return new Vertex(X / length, Y / length, Z / length);
        }

        // Метод для вычисления скалярного произведения двух векторов
        public static float Dot(Vertex v1, Vertex v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        // Дополнительный метод для вычисления векторного произведения (для нормалей)
        public static Vertex Cross(Vertex v1, Vertex v2)
        {
            return new Vertex(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            ).Normalize();
        }

        public float DistanceTo(in Vertex other)
        {
            float xDelta = X - other.X;
            float yDelta = Y - other.Y;
            float zDelta = Z - other.Z;

            return (float)Math.Sqrt(xDelta * xDelta + yDelta * yDelta + zDelta * zDelta);
        }
        // Метод для применения матрицы трансформации к вершине
        public void ApplyMatrix(Matrix<float> transformationMatrix)
        {
            // Преобразуем вершину в однородные координаты (добавляем четвертую координату w = 1)
            float[] vertexCoords = { X, Y, Z, 1 };

            // Создаем массив для хранения новых координат после трансформации
            float[] transformedCoords = new float[4];

            // Умножаем матрицу на вершину (однородные координаты)
            for (int row = 0; row < 4; row++)
            {
                transformedCoords[row] = 0;
                for (int col = 0; col < 4; col++)
                {
                    transformedCoords[row] += transformationMatrix[row, col] * vertexCoords[col];
                }
            }

            // Обновляем координаты вершины
            X = transformedCoords[0];
            Y = transformedCoords[1];
            Z = transformedCoords[2];
        }
        public PointF GetProjection(int projIndex, float w, float h, float ax, float ay)
        {
            PointF res = new PointF(0, 0);

            switch (projIndex)
            {
                // Перспективная проекция
                case 0:
                    Vertex v = new Vertex(X - w, Y - h, Z);
                    Matrix<float> m = new float[4, 4] {
                        { 1, 0, 0, 0 },
                        { 0, 1, 0, 0 },
                        { 0, 0, 0, 1.0f / 400 },
                        { 0, 0, 0, 1 }
                    };
                    Matrix<float> m1 = v * m;
                    v = new Vertex(m1[0, 0] / m1[0, 3], m1[0, 1] / m1[0, 3], m1[0, 2] / m1[0, 3]);
                    res = new PointF(v.X + w, v.Y + h);
                    break;
                case 1:
                    res = new PointF(X, Y);
                    break;
                // Аксонометрическая проекция
                case 2:
                    double angleX = ax * (Math.PI / 180);
                    double angleY = ay * (Math.PI / 180);

                    float cosX = (float)Math.Cos(angleX);
                    float cosY = (float)Math.Cos(angleY);
                    float sinX = (float)Math.Sin(angleX);
                    float sinY = (float)Math.Sin(angleY);

                    Matrix<float> m2 = new float[4, 4] {
                        { cosY, sinX * sinY, 0, 0 },
                        { 0, cosX, 0, 0 },
                        { sinY, -sinX * cosY, 0, 0 },
                        { 0, 0, 0, 1 }
                    };

                    Vertex v1 = new Vertex(X - w, Y - h, Z);
                    v1 = v1 * m2;
                    res = new PointF(v1.X + w, v1.Y + h);
                    break;
            }

            return res;
        }
    }

    public class Face
    {
        public int[] Vertices { get; private set; }

        public Face(params int[] vertices)
        {
            Vertices = vertices;
        }
    }

    public class PolyHedron
    {
        public List<Face> Faces { get; private set; }

        public List<Vertex> Vertices { get; private set; }

        public List<Normal> Normals { get; private set; }

        public PolyHedron()
        {
            Faces = new List<Face>();
            Vertices = new List<Vertex>();
            Normals = new List<Normal>();
        }

        public PolyHedron(List<Face> faces, List<Vertex> vertices, List<Normal> normals)
        {
            Faces = faces;
            Vertices = vertices;
            Normals = normals;
        }

        public void FindCenter(List<Vertex> vertices, ref double a, ref double b, ref double c)
        {
            a = 0;
            b = 0;
            c = 0;
            foreach (var vertex in vertices)
            {
                a += vertex.X;
                b += vertex.Y;
                c += vertex.Z;
            }

            a /= vertices.Count;
            b /= vertices.Count;
            c /= vertices.Count;
        }

        public PolyHedron LineRotated(float l, float m, float n, float angle)
        {
            var newPoly = this.Clone();

            double angleRadians = (double)angle * (Math.PI / 180);

            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            Matrix<float> RxMatrix = new float[4, 4]
            {
                { l*l+cos*(1-l*l), l*(1-cos)*m+n*sin,  l*(1-cos)*n-m*sin,  0 },
                { l*(1-cos)*m-n*sin, m*m+cos*(1-m*m), m*(1-cos)*n+l*sin,  0 },
                { l*(1-cos)*n+m*sin, m*(1-cos)*n-l*sin,  n*n+cos*(1-n*n),  0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= RxMatrix;
                newPoly.Normals[i] *= RxMatrix;
            }

            return newPoly;
        }

        public PolyHedron ApplyRx(float l, float m, float n, bool reverse = false)
        {
            var newPoly = this.Clone();

            float d = (float)Math.Sqrt(m * m + n * n);
            float mult = reverse ? -1 : 1;

            Matrix<float> RxMatrix = new float[4, 4]
            {
                { 1,  0,  0,  0 },
                { 0,  n/d, m/d * mult,  0 },
                { 0,  -m/d * mult,  n/d,  0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= RxMatrix;
                newPoly.Normals[i] *= RxMatrix;
            }

            return newPoly;
        }

        public PolyHedron ApplyRy(float l, float m, float n, bool reverse = false)
        {
            var newPoly = this.Clone();

            float d = (float)Math.Sqrt(m * m + n * n);
            d = reverse ? -d : d;

            Matrix<float> RyMatrix = new float[4, 4]
            {
                { l,  0,  d,  0 },
                { 0,  1, 0,  0 },
                { -d,  0,  l,  0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= RyMatrix;
                newPoly.Normals[i] *= RyMatrix;
            }

            return newPoly;
        }

        public PolyHedron ApplyRz(float angle)
        {
            var newPoly = this.Clone();

            double angleRadians = (double)angle * (Math.PI / 180);

            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            Matrix<float> RzMatrix = new float[4, 4]
            {
                { cos,  sin,  0,  0 },
                { -sin,  cos, 0,  0 },
                { 0,  0,  1,  0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= RzMatrix;
                newPoly.Normals[i] *= RzMatrix;
            }

            return newPoly;
        }

        public PolyHedron Scaled(float c1, float c2, float c3)
        {
            var newPoly = this.Clone();

            Matrix<float> translationMatrix = new float[4, 4]
            {
                { c1, 0,  0,  0 },
                { 0,  c2, 0,  0 },
                { 0,  0,  c3, 0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= translationMatrix;
            }

            return newPoly;
        }

        public PolyHedron Moved(float a, float b, float c)
        {
            var newPoly = this.Clone();

            Matrix<float> translationMatrix = new float[4, 4]
            {
                { 1,  0,  0,  0 },
                { 0,  1,  0,  0 },
                { 0,  0,  1,  0 },
                { a,  b,  c,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= translationMatrix;
            }

            return newPoly;
        }

        public PolyHedron RotatedXAxis(float alpha)
        {
            var newPoly = this.Clone();

            double angleRadians = (double)alpha * (Math.PI / 180);

            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            Matrix<float> translationMatrix = new float[4, 4]
            {
                { 1,  0,  0,  0 },
                { 0,  cos, sin,  0 },
                { 0,  -sin,  cos,  0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= translationMatrix;
                newPoly.Normals[i] *= translationMatrix;
            }

            return newPoly;
        }

        public PolyHedron RotatedYAxis(float alpha)
        {
            var newPoly = this.Clone();

            double angleRadians = (double)alpha * (Math.PI / 180);

            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            Matrix<float> translationMatrix = new float[4, 4]
            {
                { cos,  0,  -sin,  0 },
                { 0,  1, 0,  0 },
                { sin,  0,  cos,  0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= translationMatrix;
                newPoly.Normals[i] *= translationMatrix;
            }

            return newPoly;
        }

        public PolyHedron RotatedZAxis(float alpha)
        {
            var newPoly = this.Clone();

            double angleRadians = (double)alpha * (Math.PI / 180);

            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            Matrix<float> translationMatrix = new float[4, 4]
            {
                { cos,  sin,  0,  0 },
                { -sin, cos, 0,  0 },
                { 0,  0,  1,  0 },
                { 0,  0,  0,  1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= translationMatrix;
                newPoly.Normals[i] *= translationMatrix;
            }

            return newPoly;
        }

        public PolyHedron Rotated(float xAngle, float yAngle, float zAngle)
        {
            return this.Clone()
                .RotatedXAxis(xAngle)
                .RotatedYAxis(yAngle)
                .RotatedZAxis(zAngle);
        }
        // Метод для создания фигуры вращения
        public static PolyHedron GetRevolvedFigure(List<Vertex> profile, int divisions, char axis)
        {
            PolyHedron polyhedron = new PolyHedron();
            float angleStep = 360f / divisions;

            // Добавляем вершины
            for (int i = 0; i < divisions; i++)
            {
                float angle = i * angleStep;
                Matrix<float> rotationMatrix = GetRotationMatrix(axis, angle);

                foreach (var vertex in profile)
                {
                    var rotatedVertex = vertex.Clone();
                    rotatedVertex.ApplyMatrix(rotationMatrix);
                    polyhedron.Vertices.Add(rotatedVertex);
                }
            }

            // Создаем грани, соединяющие вершины
            int profileCount = profile.Count;
            for (int i = 0; i < divisions; i++)
            {
                int nextDiv = (i + 1) % divisions;
                for (int j = 0; j < profileCount - 1; j++)
                {
                    int v1 = i * profileCount + j;
                    int v2 = nextDiv * profileCount + j;
                    int v3 = nextDiv * profileCount + j + 1;
                    int v4 = i * profileCount + j + 1;
                    polyhedron.Faces.Add(new Face(v1, v2, v3, v4));
                }
            }

            CalculateNormals(polyhedron);

            return polyhedron;
        }

        private static Matrix<float> GetRotationMatrix(char axis, float angleDegrees)
        {
            float angle = angleDegrees * (float)(Math.PI / 180);
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            switch (axis)
            {
                case 'X':
                    return new Matrix<float>(new float[,] { { 1, 0, 0, 0 }, { 0, cos, -sin, 0 }, { 0, sin, cos, 0 }, { 0, 0, 0, 1 } });
                case 'Y':
                    return new Matrix<float>(new float[,] { { cos, 0, sin, 0 }, { 0, 1, 0, 0 }, { -sin, 0, cos, 0 }, { 0, 0, 0, 1 } });
                case 'Z':
                    return new Matrix<float>(new float[,] { { cos, -sin, 0, 0 }, { sin, cos, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } });
                default:
                    throw new ArgumentException("Axis must be X, Y, or Z.");
            };
        }
        public static PolyHedron GetCube()
        {
            var cube = new PolyHedron();

            cube.Vertices.Add(new Vertex(-1, -1, -1));
            cube.Vertices.Add(new Vertex(1, -1, -1));
            cube.Vertices.Add(new Vertex(1, 1, -1));
            cube.Vertices.Add(new Vertex(-1, 1, -1));
            cube.Vertices.Add(new Vertex(-1, -1, 1));
            cube.Vertices.Add(new Vertex(1, -1, 1));
            cube.Vertices.Add(new Vertex(1, 1, 1));
            cube.Vertices.Add(new Vertex(-1, 1, 1));

            cube.Faces.Add(new Face(0, 3, 2, 1)); // Нижняя грань
            cube.Faces.Add(new Face(4, 7, 6, 5)); // Верхняя грань
            cube.Faces.Add(new Face(0, 4, 5, 1)); // Передняя грань
            cube.Faces.Add(new Face(3, 7, 6, 2)); // Задняя грань
            cube.Faces.Add(new Face(1, 5, 6, 2)); // Правая грань
            cube.Faces.Add(new Face(0, 3, 7, 4)); // Левая грань

            CalculateNormals(cube);

            return cube;
        }

        private static void CalculateNormals(PolyHedron poly)
        {
            var vertexNormals = new Dictionary<int, List<Normal>>();

            // Вычисляем нормали для каждой грани
            foreach (var face in poly.Faces)
            {
                // достаточно 3 вершин для нормали
                var normal = CalculateFaceNormal(poly.Vertices[face.Vertices[0]],
                                                 poly.Vertices[face.Vertices[1]],
                                                 poly.Vertices[face.Vertices[2]]);

                
                foreach (var vertexIndex in face.Vertices)
                {
                    if (!vertexNormals.ContainsKey(vertexIndex))
                    {
                        vertexNormals[vertexIndex] = new List<Normal>();
                    }
                    vertexNormals[vertexIndex].Add(normal);
                }
            }

            // Устанавливаем усредненные нормали для каждой вершины
            for (int i = 0; i < poly.Vertices.Count; i++)
            {
                if (vertexNormals.TryGetValue(i, out var normals))
                {
                    // Усредняем нормали
                    float nx = 0, ny = 0, nz = 0;
                    foreach (var normal in normals)
                    {
                        nx += normal.NX;
                        ny += normal.NY;
                        nz += normal.NZ;
                    }

                    poly.Normals.Add(new Normal(nx, ny, nz));
                }
            }
        }

        private static Normal CalculateFaceNormal(Vertex v1, Vertex v2, Vertex v3)
        {
            float ux = v2.X - v1.X;
            float uy = v2.Y - v1.Y;
            float uz = v2.Z - v1.Z;

            float vx = v3.X - v1.X;
            float vy = v3.Y - v1.Y;
            float vz = v3.Z - v1.Z;

            float nx = uy * vz - uz * vy;
            float ny = uz * vx - ux * vz;
            float nz = ux * vy - uy * vx;

            return new Normal(nx, ny, nz);
        }

        public static PolyHedron GetTetrahedron()
        {
            var tetra = new PolyHedron();

            tetra.Vertices.Add(new Vertex(-1, 1, -1));
            tetra.Vertices.Add(new Vertex(1, -1, -1));
            tetra.Vertices.Add(new Vertex(1, 1, 1));
            tetra.Vertices.Add(new Vertex(-1, -1, 1));

            tetra.Faces.Add(new Face(2, 1, 0));
            tetra.Faces.Add(new Face(3, 1, 0));
            tetra.Faces.Add(new Face(3, 2, 0));
            tetra.Faces.Add(new Face(3, 2, 1));

            CalculateNormals(tetra);

            return tetra;
        }

        public static PolyHedron GetOctahedron()
        {
            var cube = GetCube();

            var octa = new PolyHedron();

            foreach (Face face in cube.Faces)
            {
                octa.Vertices.Add(cube.GetFaceCenter(face));
            }

            var octaCenters = cube.Scaled(1 / 3f, 1 / 3f, 1 / 3f).Vertices;

            for (int i = 0; i < 8; i++)
            {
                // Находим три ближайших центра к текущей вершине октаэдра
                var faceVertices = octa.Vertices
                    .Select((v, ind) => (v, ind))
                    .OrderBy(p => octaCenters[i].DistanceTo(p.v))
                    .Select(p => p.ind)
                    .Take(3)
                    .ToArray();

                // Проверка и упорядочивание вершин против часовой стрелки
                Vertex v0 = octa.Vertices[faceVertices[0]];
                Vertex v1 = octa.Vertices[faceVertices[1]];
                Vertex v2 = octa.Vertices[faceVertices[2]];

                var normal = Vertex.Cross(Vertex.Subtract(v1, v0), Vertex.Subtract(v2, v0)).Normalize();

                // Проверка направления нормали (предполагаем направление взгляда наружу)
                var centerToVertex = Vertex.Subtract(v0, octaCenters[i]).Normalize();

                // Проверка направления нормали
                if (Vertex.Dot(normal, centerToVertex) < 0)
                {
                    // Меняем порядок вершин, если нормаль направлена внутрь
                    Array.Reverse(faceVertices);
                }

                // Добавляем упорядоченные вершины как грань
                octa.Faces.Add(new Face(faceVertices));
            }

            CalculateNormals(octa);

            return octa;
        }

        public static PolyHedron GetIcosahedron()
        {
            var icosa = new PolyHedron();

            var verticesBottom = new List<(Vertex v, int number)>(5);

            var verticesTop = new List<(Vertex v, int number)>(5);

            double angle = -90;

            int number = 1;

            for (int i = 0; i < 5; i++)
            {
                var angleRadians = angle * (Math.PI / 180);

                verticesBottom.Add((new Vertex((float)Math.Cos(angleRadians), -0.5f, (float)Math.Sin(angleRadians)), number));

                angle += 72;

                number += 2;
            }

            angle = -54;

            number = 2;

            for (int i = 0; i < 5; i++)
            {
                var angleRadians = angle * (Math.PI / 180);

                verticesTop.Add((new Vertex((float)Math.Cos(angleRadians), 0.5f, (float)Math.Sin(angleRadians)), number));

                angle += 72;

                number += 2;
            }

            icosa.Vertices = verticesBottom.Concat(verticesTop).OrderBy(p => p.number).Select(p => p.v).ToList();

            for (int i = 1; i <= 8; i++)
            {
                icosa.Faces.Add(new Face(i + 1, i, i - 1));
            }

            icosa.Faces.Add(new Face(0, 9, 8));
            icosa.Faces.Add(new Face(1, 0, 9));

            icosa.Vertices.Add(new Vertex(0, -(float)Math.Sqrt(5) / 2, 0));
            icosa.Vertices.Add(new Vertex(0, (float)Math.Sqrt(5) / 2, 0));

            number = 1;

            for (int i = 0; i < 4; i++)
            {
                icosa.Faces.Add(new Face(number + 1, number - 1, 10));

                number += 2;
            }

            icosa.Faces.Add(new Face(0, 8, 10));

            number = 2;

            for (int i = 0; i < 4; i++)
            {
                icosa.Faces.Add(new Face(number + 1, number - 1, 11));

                number += 2;
            }

            icosa.Faces.Add(new Face(1, 9, 11));

            CalculateNormals(icosa);

            return icosa;
        }

        public static PolyHedron GetDodecahedron()
        {
            var icosa = GetIcosahedron();

            var dodeca = new PolyHedron();

            foreach (Face face in icosa.Faces)
            {
                dodeca.Vertices.Add(icosa.GetFaceCenter(face));
            }

            for (int i = 0; i < 12; i++)
            {
                var faceVertices = dodeca.Vertices.Select((v, ind) => (v, ind))
                                                .OrderBy(p => icosa.Vertices[i].DistanceTo(in p.v))
                                                .Select(p => p.ind)
                                                .Take(5).ToArray();

                int first = faceVertices.First();

                var rest = faceVertices.Skip(1).Select(ind => (dodeca.Vertices[ind], ind)).OrderBy(p => dodeca.Vertices[first].DistanceTo(in p.Item1));

                var next = rest.First().ind;

                var lastTwo = rest.Skip(2).OrderBy(p => dodeca.Vertices[next].DistanceTo(in p.Item1));

                faceVertices = faceVertices.Take(1)
                                          .Concat(new int[1] { next })
                                          .Concat(lastTwo.Select(p => p.ind))
                                          .Concat(rest.Select(p => p.ind).Skip(1).Take(1))
                                          .ToArray();

                // Проверка и упорядочивание вершин против часовой стрелки
                Vertex v0 = dodeca.Vertices[faceVertices[0]];
                Vertex v1 = dodeca.Vertices[faceVertices[1]];
                Vertex v2 = dodeca.Vertices[faceVertices[2]];

                var normal = Vertex.Cross(Vertex.Subtract(v1, v0), Vertex.Subtract(v2, v0)).Normalize();

                // Проверка направления нормали (предполагаем направление взгляда наружу)
                var centerToVertex = Vertex.Subtract(v0, icosa.Vertices[i]).Normalize();

                // Проверка направления нормали
                if (Vertex.Dot(normal, centerToVertex) < 0)
                {
                    // Меняем порядок вершин, если нормаль направлена внутрь
                    Array.Reverse(faceVertices);
                }

                dodeca.Faces.Add(new Face(faceVertices));
            }

            CalculateNormals(dodeca);

            return dodeca;
        }

        public static PolyHedron GetFunc1(float x0, float x1, float y0, float y1, float step)
        {
            if (x0 > x1)
            {
                float x = x0;
                x0 = x1;
                x1 = x;
            }
            if (y0 > y1)
            {
                float y = y0;
                y0 = y1;
                y1 = y;
            }

            var surface = new PolyHedron();
            int xLength = (int)((y1 - y0) / step + 1);
            int ind = -1;

            for (float i = x0; i <= x1; i+= step)
            {
                for (float j = y0; j <= y1; j += step)
                {
                    surface.Vertices.Add(new Vertex(i, -(i*i + j*j), j));
                    ind++;
                    if (i != x0 && j != y0)
                        surface.Faces.Add(new Face(ind, ind - xLength, ind - xLength - 1, ind - 1));
                }
            }

            CalculateNormals(surface);

            return surface;
        }

        public static PolyHedron GetFunc2(float x0, float x1, float y0, float y1, float step)
        {
            if (x0 > x1)
            {
                float x = x0;
                x0 = x1;
                x1 = x;
            }
            if (y0 > y1)
            {
                float y = y0;
                y0 = y1;
                y1 = y;
            }

            var surface = new PolyHedron();
            int xLength = (int)((y1 - y0) / step + 1);
            int ind = -1;

            for (float i = x0; i <= x1; i += step)
            {
                for (float j = y0; j <= y1; j += step)
                {
                    surface.Vertices.Add(new Vertex(i, (float)-(Math.Sin(i) + Math.Cos(j)), j));
                    ind++;
                    if (i != x0 && j != y0)
                        surface.Faces.Add(new Face(ind, ind - xLength, ind - xLength - 1, ind - 1));
                }
            }

            CalculateNormals(surface);

            return surface;
        }

        public static PolyHedron GetFunc3(float x0, float x1, float y0, float y1, float step)
        {
            if (x0 > x1)
            {
                float x = x0;
                x0 = x1;
                x1 = x;
            }
            if (y0 > y1)
            {
                float y = y0;
                y0 = y1;
                y1 = y;
            }

            var surface = new PolyHedron();
            int xLength = (int)((y1 - y0) / step + 1);
            int ind = -1;

            for (float i = x0; i <= x1; i += step)
            {
                for (float j = y0; j <= y1; j += step)
                {
                    surface.Vertices.Add(new Vertex(i, (float)-(Math.Sin(i) * Math.Cos(j)), j));
                    ind++;
                    if (i != x0 && j != y0)
                        surface.Faces.Add(new Face(ind, ind - xLength, ind - xLength - 1, ind - 1));
                }
            }

            CalculateNormals(surface);

            return surface;
        }

        public static PolyHedron GetFunc4(float x0, float x1, float y0, float y1, float step)
        {
            if (x0 > x1)
            {
                float x = x0;
                x0 = x1;
                x1 = x;
            }
            if (y0 > y1)
            {
                float y = y0;
                y0 = y1;
                y1 = y;
            }

            var surface = new PolyHedron();
            int xLength = (int)((y1 - y0) / step + 1);
            int ind = -1;

            for (float i = x0; i <= x1; i += step)
            {
                for (float j = y0; j <= y1; j += step)
                {
                    double r = i * i + j * j + 1;
                    surface.Vertices.Add(new Vertex(i, (float)-(5 * (Math.Cos(r) / r + 0.1)), j));
                    ind++;
                    if (i != x0 && j != y0)
                        surface.Faces.Add(new Face(ind, ind - xLength, ind - xLength - 1, ind - 1));
                }
            }

            CalculateNormals(surface);

            return surface;
        }

        public Vertex GetFaceCenter(Face face)
        {
            float x = 0, y = 0, z = 0;

            foreach (int vertexIndex in face.Vertices)
            {
                x += Vertices[vertexIndex].X;
                y += Vertices[vertexIndex].Y;
                z += Vertices[vertexIndex].Z;
            }

            return new Vertex(x / face.Vertices.Length, y / face.Vertices.Length, z / face.Vertices.Length);
        }

        public PolyHedron Clone()
        {
            var newPoly = new PolyHedron(this.Faces, new List<Vertex>(this.Vertices), new List<Normal>(this.Normals));
            return newPoly;
        }

        public PolyHedron Reflected(string plane)
        {
            var newPoly = this.Clone();

            Matrix<float> reflectionMatrix;

            switch (plane.ToUpper())
            {
                case "XY":
                    reflectionMatrix = new float[4, 4]
                    {
                { 1,  0,  0,  0 },
                { 0,  1,  0,  0 },
                { 0,  0, -1,  0 },
                { 0,  0,  0,  1 }
                    };
                    break;

                case "YZ":
                    reflectionMatrix = new float[4, 4]
                    {
                { -1,  0,  0,  0 },
                { 0,  1,  0,  0 },
                { 0,  0,  1,  0 },
                { 0,  0,  0,  1 }
                    };
                    break;

                case "XZ":
                    reflectionMatrix = new float[4, 4]
                    {
                { 1,  0,  0,  0 },
                { 0, -1,  0,  0 },
                { 0,  0,  1,  0 },
                { 0,  0,  0,  1 }
                    };
                    break;

                default:
                    throw new ArgumentException("Invalid plane. Use 'XY', 'YZ', or 'XZ'.");
            }

            // Применяем матрицу отражения ко всем вершинам многогранника
            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= reflectionMatrix;
                newPoly.Normals[i] *= reflectionMatrix;
            }

            return newPoly;
        }

        public PolyHedron ScaledAroundCenter(float scaleX, float scaleY, float scaleZ)
        {
            var newPoly = this.Clone();

            // Шаг 1: Находим центр многогранника
            double centerX = 0, centerY = 0, centerZ = 0;
            FindCenter(newPoly.Vertices, ref centerX, ref centerY, ref centerZ);

            // Шаг 2: Перемещаем многогранник так, чтобы его центр оказался в начале координат
            Matrix<float> moveToOriginMatrix = new float[4, 4]
            {
        { 1, 0, 0, 0 },
        { 0, 1, 0, 0 },
        { 0, 0, 1, 0 },
        { (float)-centerX, (float)-centerY, (float)-centerZ, 1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= moveToOriginMatrix;
            }

            // Шаг 3: Выполняем масштабирование
            Matrix<float> scalingMatrix = new float[4, 4]
            {
        { scaleX, 0, 0, 0 },
        { 0, scaleY, 0, 0 },
        { 0, 0, scaleZ, 0 },
        { 0, 0, 0, 1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= scalingMatrix;
            }

            // Шаг 4: Перемещаем многогранник обратно в исходное положение
            Matrix<float> moveBackMatrix = new float[4, 4]
            {
        { 1, 0, 0, 0 },
        { 0, 1, 0, 0 },
        { 0, 0, 1, 0 },
        { (float)centerX, (float)centerY, (float)centerZ, 1 }
            };

            for (int i = 0; i < newPoly.Vertices.Count; i++)
            {
                newPoly.Vertices[i] *= moveBackMatrix;
            }

            return newPoly;
        }

        public void SaveToObj(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Сохраняем вершины
            foreach (var vertex in Vertices)
            {
                sb.AppendLine($"v {vertex.X} {vertex.Y} {vertex.Z}");
            }

            // Сохраняем нормали вершин
            foreach (var normal in Normals)
            {
                sb.AppendLine($"vn {normal.NX} {normal.NY} {normal.NZ}");
            }

            // Сохраняем грани
            foreach (var face in Faces)
            {
                sb.Append("f");
                foreach (var vertexIndex in face.Vertices)
                {
                    sb.Append($" {vertexIndex + 1}"); // OBJ индекс начинается с 1
                }
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public static PolyHedron LoadFromObj(string filePath)
        {
            var polyhedron = new PolyHedron();
            var lines = File.ReadAllLines(filePath);

            var vertices = new List<Vertex>();
            var normals = new List<Normal>();

            foreach (var line in lines)
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    
                    vertices.Add(new Vertex(x, y, z));
                }
                else if (line.StartsWith("vn "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    float nx = float.Parse(parts[1]);
                    float ny = float.Parse(parts[2]);
                    float nz = float.Parse(parts[3]);

                    normals.Add(new Normal(nx, ny, nz));
                }
                else if (line.StartsWith("f "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    int[] vertexIndices = new int[parts.Length - 1];
                    
                    for (int i = 1; i < parts.Length; i++)
                    {
                        var indices = parts[i].Split('/');
                        int vertexIndex = int.Parse(indices[0]) - 1;
                        vertexIndices[i - 1] = vertexIndex;

                        polyhedron.Vertices = new List<Vertex>(vertices);

                        polyhedron.Vertices[vertexIndex] = new Vertex(
                                vertices[vertexIndex].X,
                                vertices[vertexIndex].Y,
                                vertices[vertexIndex].Z
                            );

                        polyhedron.Normals = new List<Normal>(Enumerable.Repeat(new Normal(0, 0, 0), vertices.Count));

                        // Присваиваем нормаль, если она есть
                        if (indices.Length > 2 && int.TryParse(indices[2], out int normalIndex))
                        {
                            polyhedron.Normals = new List<Normal>(normals);

                            polyhedron.Normals[normalIndex - 1] = new Normal(
                                    normals[normalIndex - 1].NX,
                                    normals[normalIndex - 1].NY,
                                    normals[normalIndex - 1].NZ
                                );
                        }
                    }
                    
                    polyhedron.Faces.Add(new Face(vertexIndices));
                }
                else
                {
                    // Пропуск пока
                }
            }

            return polyhedron;
        }
    }
}