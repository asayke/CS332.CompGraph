using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace CG_Lab
{
    public partial class FormTask1 : Form
    {
        private LSystem lSystem;
        private LSystemGenerator generator;
        private FractalDrawer drawer;

        private int iterationsCount = 5;

        private int[] iterationsCounts;

        private float stepDecreasePercent = 0f;
        private int colorChangeValue = 0;
        private Pen pen = Pens.Black;
        private float penThicknessDecreasePercent = 0f;

        public FormTask1()
        {
            InitializeComponent();
            iterationsCounts = new int[] { 5, 5, 3, 5, 5, 5, 9, 3, 7, 12 };
            comboBox1.SelectedIndex = 0;
        }

        private void LoadLSystem(string filePath)
        {
            lSystem = new LSystem(filePath);
            generator = new LSystemGenerator(lSystem);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var sequence = generator.GenerateSequence(iterationsCount);

            drawer = new FractalDrawer(e.Graphics, this.ClientSize, pen, new PointF(0, 0), lSystem.StartDirection);

            drawer.Draw(sequence, lSystem.Angle, 10, stepDecreasePercent, colorChangeValue, penThicknessDecreasePercent);
            stepDecreasePercent = 0f;
            colorChangeValue = 0;
            pen = Pens.Black;
            penThicknessDecreasePercent = 0f;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLSystem(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")) + comboBox1.SelectedItem.ToString() + ".txt");

            iterationsCount = iterationsCounts[comboBox1.SelectedIndex];

            if (comboBox1.SelectedIndex == 9)
            {
                stepDecreasePercent = 15f;
                colorChangeValue = 18;
                pen = new Pen(Color.Black, 20);
                penThicknessDecreasePercent = 15f;
            }

            Invalidate();
        }
    }

    public class LSystem
    {
        public string Axiom { get; set; }
        public double Angle { get; set; }
        public double StartDirection { get; set; }
        public Dictionary<char, string> Rules { get; set; }

        public LSystem(string filePath)
        {
            Rules = new Dictionary<char, string>();
            ParseFile(filePath);
        }

        private void ParseFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            var firstLine = lines[0].Split(' ');
            Axiom = firstLine[0];
            Angle = double.Parse(firstLine[1]);
            StartDirection = double.Parse(firstLine[2]);

            for (int i = 1; i < lines.Length; i++)
            {
                var rule = lines[i];
                if (!string.IsNullOrEmpty(rule))
                {
                    var parts = rule.Split('>');
                    Rules[parts[0][0]] = parts[1];
                }
            }
        }
    }

    public class LSystemGenerator
    {
        private readonly LSystem lSystem;
        public LSystemGenerator(LSystem lsystem)
        {
            lSystem = lsystem;
        }

        public string GenerateSequence(int iterations)
        {
            var current = lSystem.Axiom;

            for (int i = 0; i < iterations; i++)
            {
                var next = new StringBuilder();

                foreach (var symbol in current)
                {
                    if (lSystem.Rules.ContainsKey(symbol))
                    {
                        next.Append(lSystem.Rules[symbol]);
                    }
                    else
                    {
                        next.Append(symbol);
                    }
                }

                current = next.ToString();
            }

            return current;
        }
    }


    public class FractalDrawer
    {
        private readonly Graphics graphics;
        private readonly Size windowSize;
        private Pen pen;
        private PointF currentPosition;
        private double currentDirection;

        private double minX, minY, maxX, maxY;
        private double scaleCoef;

        private readonly Random random = new Random();
        private Queue<double> angles = new Queue<double>();

        public FractalDrawer(Graphics graphics, Size windowSize, Pen pen, PointF startPosition, double startDirection)
        {
            this.graphics = graphics;
            this.windowSize = windowSize;
            this.pen = pen;
            currentPosition = startPosition;
            currentDirection = startDirection;
        }

        public void CalculateBounds(string sequence, double angleIncrement, float stepLength, float stepDecreasePercent = 0f)
        {
            var stack = new Stack<(PointF position, double direction, float stepLength)>();

            minX = maxX = 0;
            minY = maxY = 0;

            PointF currentPosition = this.currentPosition;
            double currentDirection = this.currentDirection;

            double initialAngle = angleIncrement;

            foreach (var symbol in sequence)
            {
                if (char.IsLetter(symbol))
                {
                    stepLength -= stepLength * (stepDecreasePercent / 100);
                    var nextPosition = CalculateNextPosition(stepLength, currentPosition, currentDirection);

                    minX = Math.Min(minX, nextPosition.X);
                    minY = Math.Min(minY, nextPosition.Y);
                    maxX = Math.Max(maxX, nextPosition.X);
                    maxY = Math.Max(maxY, nextPosition.Y);

                    currentPosition = nextPosition;
                }
                else
                {
                    switch (symbol)
                    {
                        case '+':
                            currentDirection += angleIncrement;
                            break;

                        case '-':
                            currentDirection -= angleIncrement;
                            break;

                        case '[':
                            stack.Push((currentPosition, currentDirection, stepLength));
                            break;

                        case ']':
                            var savedState = stack.Pop();
                            currentPosition = savedState.position;
                            currentDirection = savedState.direction;
                            stepLength = savedState.stepLength;
                            break;
                        case '@':
                            angleIncrement = random.NextDouble() * initialAngle;
                            angles.Enqueue(angleIncrement);
                            break;
                    }
                }
                
            }

            double width = maxX - minX;
            double height = maxY - minY;
            double scaleX = (windowSize.Width - 80) / width;
            double scaleY = (windowSize.Height - 80) / height;

            scaleCoef = Math.Min(scaleX, scaleY);
        }

        public void Draw(string sequence, double angleIncrement, float stepLength, float stepDecreasePercent = 0f, int colorChangeValue = 0, float penThicknessDecreasePercent = 0f)
        {
            Pen initialPen = pen;

            CalculateBounds(sequence, angleIncrement, stepLength, stepDecreasePercent);

            double offsetX = (windowSize.Width - (maxX - minX) * scaleCoef) / 2;
            double offsetY = (windowSize.Height - (maxY - minY) * scaleCoef) / 2;

            double x = -minX * scaleCoef + offsetX;
            double y = -minY * scaleCoef + offsetY;

            currentPosition = new PointF((float)x, (float)y);

            var stack = new Stack<(PointF position, double direction, float stepLength, Pen pen)>();

            stepLength *= (float)scaleCoef;

            foreach (var symbol in sequence)
            {
                if (char.IsLetter(symbol))
                {
                    int newColorValue = pen.Color.R + colorChangeValue;
                    pen = new Pen(Color.FromArgb(newColorValue, newColorValue, newColorValue), pen.Width - pen.Width * (penThicknessDecreasePercent / 100));
                    stepLength -= stepLength * (stepDecreasePercent / 100);
                    
                    var nextPosition = CalculateNextPosition(stepLength, currentPosition, currentDirection);
                    graphics.DrawLine(pen, currentPosition, nextPosition);
                    currentPosition = nextPosition;

                }
                else
                {
                    switch (symbol)
                    {
                        case '+':
                            currentDirection += angleIncrement;
                            break;

                        case '-':
                            currentDirection -= angleIncrement;
                            break;

                        case '[':
                            stack.Push((currentPosition, currentDirection, stepLength, pen));
                            break;

                        case ']':
                            var savedState = stack.Pop();
                            currentPosition = savedState.position;
                            currentDirection = savedState.direction;
                            stepLength = savedState.stepLength;
                            pen = savedState.pen;
                            break;
                        case '@':
                            angleIncrement = angles.Dequeue();
                            break;
                    }
                }
            }
            pen = initialPen;
        }

        private PointF CalculateNextPosition(float stepLength, PointF position, double direction)
        {
            var radianAngle = direction * (Math.PI / 180.0);
            var nextX = position.X + (float)(stepLength * Math.Cos(radianAngle));
            var nextY = position.Y - (float)(stepLength * Math.Sin(radianAngle));
            return new PointF(nextX, nextY);
        }
    }

}
