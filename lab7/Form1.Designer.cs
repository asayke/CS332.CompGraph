namespace lab6
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Компоненты интерфейса
        private System.Windows.Forms.Button ScaleButton;
        private System.Windows.Forms.Button OffsetButton;
        private System.Windows.Forms.Button RotateXButton;
        private System.Windows.Forms.Button RotateYButton;
        private System.Windows.Forms.Button RotateZButton;
        private System.Windows.Forms.Button ReflectXYButton;
        private System.Windows.Forms.Button ReflectXZButton;
        private System.Windows.Forms.Button ReflectYZButton;
        private System.Windows.Forms.ComboBox ProjectionComboBox;
        private System.Windows.Forms.TextBox ScaleTextBox;
        private System.Windows.Forms.TextBox RotateTextBox;
        private System.Windows.Forms.TextBox OffsetXTextBox;
        private System.Windows.Forms.TextBox OffsetYTextBox;
        private System.Windows.Forms.TextBox OffsetZTextBox;
        private System.Windows.Forms.Button TetrahedronButton;
        private System.Windows.Forms.Button HexahedronButton;
        private System.Windows.Forms.Button OctahedronButton;

        /// <summary>
        /// Освободить используемые ресурсы
        /// </summary>
        /// <param name="disposing">true если управляемый ресурс должен быть удален; иначе false</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Метод для инициализации компонентов интерфейса
        /// </summary>
        private void InitializeComponent()
        {
            this.ScaleButton = new System.Windows.Forms.Button();
            this.OffsetButton = new System.Windows.Forms.Button();
            this.RotateXButton = new System.Windows.Forms.Button();
            this.RotateYButton = new System.Windows.Forms.Button();
            this.RotateZButton = new System.Windows.Forms.Button();
            this.ReflectXYButton = new System.Windows.Forms.Button();
            this.ReflectXZButton = new System.Windows.Forms.Button();
            this.ReflectYZButton = new System.Windows.Forms.Button();
            this.ProjectionComboBox = new System.Windows.Forms.ComboBox();
            this.ScaleTextBox = new System.Windows.Forms.TextBox();
            this.RotateTextBox = new System.Windows.Forms.TextBox();
            this.OffsetXTextBox = new System.Windows.Forms.TextBox();
            this.OffsetYTextBox = new System.Windows.Forms.TextBox();
            this.OffsetZTextBox = new System.Windows.Forms.TextBox();
            this.TetrahedronButton = new System.Windows.Forms.Button();
            this.HexahedronButton = new System.Windows.Forms.Button();
            this.OctahedronButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.axisComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.RotateAroundAxisCenterButton = new System.Windows.Forms.Button();
            this.x1TextBox = new System.Windows.Forms.TextBox();
            this.y1TextBox = new System.Windows.Forms.TextBox();
            this.z1TextBox = new System.Windows.Forms.TextBox();
            this.x1Coord = new System.Windows.Forms.Label();
            this.y1Coord = new System.Windows.Forms.Label();
            this.z1Coord = new System.Windows.Forms.Label();
            this.z2Coord = new System.Windows.Forms.Label();
            this.y2Coord = new System.Windows.Forms.Label();
            this.x2Coord = new System.Windows.Forms.Label();
            this.z2TextBox = new System.Windows.Forms.TextBox();
            this.y2TextBox = new System.Windows.Forms.TextBox();
            this.x2TextBox = new System.Windows.Forms.TextBox();
            this.RotateAroundLineButton = new System.Windows.Forms.Button();
            this.IcosahedronButton = new System.Windows.Forms.Button();
            this.DodecahedronButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.GeneratingPointsGrid = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AxisComboBox1 = new System.Windows.Forms.ComboBox();
            this.SegmentsNumericUpDown = new System.Windows.Forms.TextBox();
            this.BuildRevolutionFigureButton = new System.Windows.Forms.Button();
            this.txtX0 = new System.Windows.Forms.TextBox();
            this.txtX1 = new System.Windows.Forms.TextBox();
            this.txtY0 = new System.Windows.Forms.TextBox();
            this.txtY1 = new System.Windows.Forms.TextBox();
            this.txtDivisions = new System.Windows.Forms.TextBox();
            this.cmbFunction = new System.Windows.Forms.ComboBox();
            this.btnGenerateSurface = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.GeneratingPointsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ScaleButton
            // 
            this.ScaleButton.Location = new System.Drawing.Point(10, 10);
            this.ScaleButton.Name = "ScaleButton";
            this.ScaleButton.Size = new System.Drawing.Size(100, 23);
            this.ScaleButton.TabIndex = 0;
            this.ScaleButton.Text = "Масштаб";
            this.ScaleButton.Click += new System.EventHandler(this.ScaleButton_Click);
            // 
            // OffsetButton
            // 
            this.OffsetButton.Location = new System.Drawing.Point(10, 40);
            this.OffsetButton.Name = "OffsetButton";
            this.OffsetButton.Size = new System.Drawing.Size(100, 23);
            this.OffsetButton.TabIndex = 1;
            this.OffsetButton.Text = "Смещение";
            this.OffsetButton.Click += new System.EventHandler(this.OffsetButton_Click);
            // 
            // RotateXButton
            // 
            this.RotateXButton.Location = new System.Drawing.Point(10, 70);
            this.RotateXButton.Name = "RotateXButton";
            this.RotateXButton.Size = new System.Drawing.Size(100, 23);
            this.RotateXButton.TabIndex = 2;
            this.RotateXButton.Text = "Поворот X";
            this.RotateXButton.Click += new System.EventHandler(this.RotateXButton_Click);
            // 
            // RotateYButton
            // 
            this.RotateYButton.Location = new System.Drawing.Point(10, 100);
            this.RotateYButton.Name = "RotateYButton";
            this.RotateYButton.Size = new System.Drawing.Size(100, 23);
            this.RotateYButton.TabIndex = 3;
            this.RotateYButton.Text = "Поворот Y";
            this.RotateYButton.Click += new System.EventHandler(this.RotateYButton_Click);
            // 
            // RotateZButton
            // 
            this.RotateZButton.Location = new System.Drawing.Point(10, 130);
            this.RotateZButton.Name = "RotateZButton";
            this.RotateZButton.Size = new System.Drawing.Size(100, 23);
            this.RotateZButton.TabIndex = 4;
            this.RotateZButton.Text = "Поворот Z";
            this.RotateZButton.Click += new System.EventHandler(this.RotateZButton_Click);
            // 
            // ReflectXYButton
            // 
            this.ReflectXYButton.Location = new System.Drawing.Point(10, 160);
            this.ReflectXYButton.Name = "ReflectXYButton";
            this.ReflectXYButton.Size = new System.Drawing.Size(100, 23);
            this.ReflectXYButton.TabIndex = 5;
            this.ReflectXYButton.Text = "Отразить XY";
            this.ReflectXYButton.Click += new System.EventHandler(this.ReflectXYButton_Click);
            // 
            // ReflectXZButton
            // 
            this.ReflectXZButton.Location = new System.Drawing.Point(10, 190);
            this.ReflectXZButton.Name = "ReflectXZButton";
            this.ReflectXZButton.Size = new System.Drawing.Size(100, 23);
            this.ReflectXZButton.TabIndex = 6;
            this.ReflectXZButton.Text = "Отразить XZ";
            this.ReflectXZButton.Click += new System.EventHandler(this.ReflectXZButton_Click);
            // 
            // ReflectYZButton
            // 
            this.ReflectYZButton.Location = new System.Drawing.Point(10, 220);
            this.ReflectYZButton.Name = "ReflectYZButton";
            this.ReflectYZButton.Size = new System.Drawing.Size(100, 23);
            this.ReflectYZButton.TabIndex = 7;
            this.ReflectYZButton.Text = "Отразить YZ";
            this.ReflectYZButton.Click += new System.EventHandler(this.ReflectYZButton_Click);
            // 
            // ProjectionComboBox
            // 
            this.ProjectionComboBox.Items.AddRange(new object[] {
            "Perspective",
            "Axonometric"});
            this.ProjectionComboBox.Location = new System.Drawing.Point(10, 275);
            this.ProjectionComboBox.Name = "ProjectionComboBox";
            this.ProjectionComboBox.Size = new System.Drawing.Size(121, 21);
            this.ProjectionComboBox.TabIndex = 8;
            this.ProjectionComboBox.SelectedIndexChanged += new System.EventHandler(this.ProjectionComboBox_SelectedIndexChanged);
            // 
            // ScaleTextBox
            // 
            this.ScaleTextBox.Location = new System.Drawing.Point(213, 10);
            this.ScaleTextBox.Name = "ScaleTextBox";
            this.ScaleTextBox.Size = new System.Drawing.Size(50, 20);
            this.ScaleTextBox.TabIndex = 9;
            // 
            // RotateTextBox
            // 
            this.RotateTextBox.Location = new System.Drawing.Point(205, 103);
            this.RotateTextBox.Name = "RotateTextBox";
            this.RotateTextBox.Size = new System.Drawing.Size(50, 20);
            this.RotateTextBox.TabIndex = 10;
            // 
            // OffsetXTextBox
            // 
            this.OffsetXTextBox.Location = new System.Drawing.Point(149, 43);
            this.OffsetXTextBox.Name = "OffsetXTextBox";
            this.OffsetXTextBox.Size = new System.Drawing.Size(50, 20);
            this.OffsetXTextBox.TabIndex = 11;
            // 
            // OffsetYTextBox
            // 
            this.OffsetYTextBox.Location = new System.Drawing.Point(235, 43);
            this.OffsetYTextBox.Name = "OffsetYTextBox";
            this.OffsetYTextBox.Size = new System.Drawing.Size(50, 20);
            this.OffsetYTextBox.TabIndex = 12;
            // 
            // OffsetZTextBox
            // 
            this.OffsetZTextBox.Location = new System.Drawing.Point(331, 43);
            this.OffsetZTextBox.Name = "OffsetZTextBox";
            this.OffsetZTextBox.Size = new System.Drawing.Size(50, 20);
            this.OffsetZTextBox.TabIndex = 13;
            // 
            // TetrahedronButton
            // 
            this.TetrahedronButton.Location = new System.Drawing.Point(10, 305);
            this.TetrahedronButton.Name = "TetrahedronButton";
            this.TetrahedronButton.Size = new System.Drawing.Size(75, 23);
            this.TetrahedronButton.TabIndex = 14;
            this.TetrahedronButton.Text = "Тетраэдр";
            this.TetrahedronButton.Click += new System.EventHandler(this.TetrahedronButton_Click);
            // 
            // HexahedronButton
            // 
            this.HexahedronButton.Location = new System.Drawing.Point(10, 335);
            this.HexahedronButton.Name = "HexahedronButton";
            this.HexahedronButton.Size = new System.Drawing.Size(75, 23);
            this.HexahedronButton.TabIndex = 15;
            this.HexahedronButton.Text = "Гексаэдр";
            this.HexahedronButton.Click += new System.EventHandler(this.HexahedronButton_Click);
            // 
            // OctahedronButton
            // 
            this.OctahedronButton.Location = new System.Drawing.Point(10, 365);
            this.OctahedronButton.Name = "OctahedronButton";
            this.OctahedronButton.Size = new System.Drawing.Size(75, 23);
            this.OctahedronButton.TabIndex = 16;
            this.OctahedronButton.Text = "Октаэдр";
            this.OctahedronButton.Click += new System.EventHandler(this.OctahedronButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(125, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(210, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Z:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(125, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Масштаб:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(132, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Поворот:";
            // 
            // axisComboBox
            // 
            this.axisComboBox.FormattingEnabled = true;
            this.axisComboBox.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z"});
            this.axisComboBox.Location = new System.Drawing.Point(12, 425);
            this.axisComboBox.Name = "axisComboBox";
            this.axisComboBox.Size = new System.Drawing.Size(121, 21);
            this.axisComboBox.TabIndex = 22;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 402);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Ось:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 253);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Проекция:";
            // 
            // RotateAroundAxisCenterButton
            // 
            this.RotateAroundAxisCenterButton.Location = new System.Drawing.Point(10, 466);
            this.RotateAroundAxisCenterButton.Name = "RotateAroundAxisCenterButton";
            this.RotateAroundAxisCenterButton.Size = new System.Drawing.Size(253, 23);
            this.RotateAroundAxisCenterButton.TabIndex = 25;
            this.RotateAroundAxisCenterButton.Text = "Повернуть относительно центра";
            this.RotateAroundAxisCenterButton.UseVisualStyleBackColor = true;
            this.RotateAroundAxisCenterButton.Click += new System.EventHandler(this.RotateAroundAxisCenterButton_Click);
            // 
            // x1TextBox
            // 
            this.x1TextBox.Location = new System.Drawing.Point(33, 506);
            this.x1TextBox.Name = "x1TextBox";
            this.x1TextBox.Size = new System.Drawing.Size(51, 20);
            this.x1TextBox.TabIndex = 26;
            // 
            // y1TextBox
            // 
            this.y1TextBox.Location = new System.Drawing.Point(128, 506);
            this.y1TextBox.Name = "y1TextBox";
            this.y1TextBox.Size = new System.Drawing.Size(51, 20);
            this.y1TextBox.TabIndex = 27;
            // 
            // z1TextBox
            // 
            this.z1TextBox.Location = new System.Drawing.Point(233, 506);
            this.z1TextBox.Name = "z1TextBox";
            this.z1TextBox.Size = new System.Drawing.Size(51, 20);
            this.z1TextBox.TabIndex = 28;
            // 
            // x1Coord
            // 
            this.x1Coord.AutoSize = true;
            this.x1Coord.Location = new System.Drawing.Point(9, 512);
            this.x1Coord.Name = "x1Coord";
            this.x1Coord.Size = new System.Drawing.Size(23, 13);
            this.x1Coord.TabIndex = 32;
            this.x1Coord.Text = "X1:";
            // 
            // y1Coord
            // 
            this.y1Coord.AutoSize = true;
            this.y1Coord.Location = new System.Drawing.Point(104, 509);
            this.y1Coord.Name = "y1Coord";
            this.y1Coord.Size = new System.Drawing.Size(23, 13);
            this.y1Coord.TabIndex = 33;
            this.y1Coord.Text = "Y1:";
            // 
            // z1Coord
            // 
            this.z1Coord.AutoSize = true;
            this.z1Coord.Location = new System.Drawing.Point(209, 506);
            this.z1Coord.Name = "z1Coord";
            this.z1Coord.Size = new System.Drawing.Size(23, 13);
            this.z1Coord.TabIndex = 34;
            this.z1Coord.Text = "Z1:";
            // 
            // z2Coord
            // 
            this.z2Coord.AutoSize = true;
            this.z2Coord.Location = new System.Drawing.Point(209, 534);
            this.z2Coord.Name = "z2Coord";
            this.z2Coord.Size = new System.Drawing.Size(23, 13);
            this.z2Coord.TabIndex = 40;
            this.z2Coord.Text = "Z2:";
            // 
            // y2Coord
            // 
            this.y2Coord.AutoSize = true;
            this.y2Coord.Location = new System.Drawing.Point(104, 537);
            this.y2Coord.Name = "y2Coord";
            this.y2Coord.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.y2Coord.Size = new System.Drawing.Size(23, 13);
            this.y2Coord.TabIndex = 39;
            this.y2Coord.Text = "Y2:";
            // 
            // x2Coord
            // 
            this.x2Coord.AutoSize = true;
            this.x2Coord.Location = new System.Drawing.Point(9, 540);
            this.x2Coord.Name = "x2Coord";
            this.x2Coord.Size = new System.Drawing.Size(23, 13);
            this.x2Coord.TabIndex = 38;
            this.x2Coord.Text = "X2:";
            // 
            // z2TextBox
            // 
            this.z2TextBox.Location = new System.Drawing.Point(233, 534);
            this.z2TextBox.Name = "z2TextBox";
            this.z2TextBox.Size = new System.Drawing.Size(51, 20);
            this.z2TextBox.TabIndex = 37;
            // 
            // y2TextBox
            // 
            this.y2TextBox.Location = new System.Drawing.Point(128, 534);
            this.y2TextBox.Name = "y2TextBox";
            this.y2TextBox.Size = new System.Drawing.Size(51, 20);
            this.y2TextBox.TabIndex = 36;
            // 
            // x2TextBox
            // 
            this.x2TextBox.Location = new System.Drawing.Point(33, 534);
            this.x2TextBox.Name = "x2TextBox";
            this.x2TextBox.Size = new System.Drawing.Size(51, 20);
            this.x2TextBox.TabIndex = 35;
            // 
            // RotateAroundLineButton
            // 
            this.RotateAroundLineButton.Location = new System.Drawing.Point(10, 573);
            this.RotateAroundLineButton.Name = "RotateAroundLineButton";
            this.RotateAroundLineButton.Size = new System.Drawing.Size(280, 23);
            this.RotateAroundLineButton.TabIndex = 41;
            this.RotateAroundLineButton.Text = "Поворот вокруг произвольной прямой";
            this.RotateAroundLineButton.UseVisualStyleBackColor = true;
            this.RotateAroundLineButton.Click += new System.EventHandler(this.RotateAroundLineButton_Click);
            // 
            // IcosahedronButton
            // 
            this.IcosahedronButton.Location = new System.Drawing.Point(104, 305);
            this.IcosahedronButton.Name = "IcosahedronButton";
            this.IcosahedronButton.Size = new System.Drawing.Size(75, 23);
            this.IcosahedronButton.TabIndex = 42;
            this.IcosahedronButton.Text = "Икосаэдр";
            this.IcosahedronButton.Click += new System.EventHandler(this.IcosahedronButton_Click);
            // 
            // DodecahedronButton
            // 
            this.DodecahedronButton.Location = new System.Drawing.Point(104, 335);
            this.DodecahedronButton.Name = "DodecahedronButton";
            this.DodecahedronButton.Size = new System.Drawing.Size(89, 23);
            this.DodecahedronButton.TabIndex = 43;
            this.DodecahedronButton.Text = "Додекаэдр";
            this.DodecahedronButton.Click += new System.EventHandler(this.DodecahedronButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(834, 253);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 44;
            this.button1.Text = "load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.LoadModelButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(948, 253);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 45;
            this.button2.Text = "save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // GeneratingPointsGrid
            // 
            this.GeneratingPointsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GeneratingPointsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.GeneratingPointsGrid.Location = new System.Drawing.Point(445, 13);
            this.GeneratingPointsGrid.Name = "GeneratingPointsGrid";
            this.GeneratingPointsGrid.Size = new System.Drawing.Size(343, 150);
            this.GeneratingPointsGrid.TabIndex = 46;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "X";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Y";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Z";
            this.Column3.Name = "Column3";
            // 
            // AxisComboBox1
            // 
            this.AxisComboBox1.FormattingEnabled = true;
            this.AxisComboBox1.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z"});
            this.AxisComboBox1.Location = new System.Drawing.Point(478, 169);
            this.AxisComboBox1.Name = "AxisComboBox1";
            this.AxisComboBox1.Size = new System.Drawing.Size(121, 21);
            this.AxisComboBox1.TabIndex = 47;
            // 
            // SegmentsNumericUpDown
            // 
            this.SegmentsNumericUpDown.Location = new System.Drawing.Point(490, 192);
            this.SegmentsNumericUpDown.Name = "SegmentsNumericUpDown";
            this.SegmentsNumericUpDown.Size = new System.Drawing.Size(100, 20);
            this.SegmentsNumericUpDown.TabIndex = 48;
            // 
            // BuildRevolutionFigureButton
            // 
            this.BuildRevolutionFigureButton.Location = new System.Drawing.Point(647, 166);
            this.BuildRevolutionFigureButton.Name = "BuildRevolutionFigureButton";
            this.BuildRevolutionFigureButton.Size = new System.Drawing.Size(75, 23);
            this.BuildRevolutionFigureButton.TabIndex = 49;
            this.BuildRevolutionFigureButton.Text = "build";
            this.BuildRevolutionFigureButton.UseVisualStyleBackColor = true;
            this.BuildRevolutionFigureButton.Click += new System.EventHandler(this.BuildRevolutionFigureButton_Click);
            // 
            // txtX0
            // 
            this.txtX0.Location = new System.Drawing.Point(490, 614);
            this.txtX0.Name = "txtX0";
            this.txtX0.Size = new System.Drawing.Size(100, 20);
            this.txtX0.TabIndex = 50;
            // 
            // txtX1
            // 
            this.txtX1.Location = new System.Drawing.Point(647, 614);
            this.txtX1.Name = "txtX1";
            this.txtX1.Size = new System.Drawing.Size(100, 20);
            this.txtX1.TabIndex = 51;
            // 
            // txtY0
            // 
            this.txtY0.Location = new System.Drawing.Point(490, 664);
            this.txtY0.Name = "txtY0";
            this.txtY0.Size = new System.Drawing.Size(100, 20);
            this.txtY0.TabIndex = 52;
            // 
            // txtY1
            // 
            this.txtY1.Location = new System.Drawing.Point(647, 664);
            this.txtY1.Name = "txtY1";
            this.txtY1.Size = new System.Drawing.Size(100, 20);
            this.txtY1.TabIndex = 53;
            // 
            // txtDivisions
            // 
            this.txtDivisions.Location = new System.Drawing.Point(568, 706);
            this.txtDivisions.Name = "txtDivisions";
            this.txtDivisions.Size = new System.Drawing.Size(100, 20);
            this.txtDivisions.TabIndex = 54;
            // 
            // cmbFunction
            // 
            this.cmbFunction.FormattingEnabled = true;
            this.cmbFunction.Items.AddRange(new object[] {
            "Sin(x)*Cos(y)",
            "x^2 + y^2"});
            this.cmbFunction.Location = new System.Drawing.Point(790, 652);
            this.cmbFunction.Name = "cmbFunction";
            this.cmbFunction.Size = new System.Drawing.Size(121, 21);
            this.cmbFunction.TabIndex = 55;
            // 
            // btnGenerateSurface
            // 
            this.btnGenerateSurface.Location = new System.Drawing.Point(758, 706);
            this.btnGenerateSurface.Name = "btnGenerateSurface";
            this.btnGenerateSurface.Size = new System.Drawing.Size(75, 23);
            this.btnGenerateSurface.TabIndex = 56;
            this.btnGenerateSurface.Text = "generate";
            this.btnGenerateSurface.UseVisualStyleBackColor = true;
            this.btnGenerateSurface.Click += new System.EventHandler(this.btnGenerateSurface_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1076, 772);
            this.Controls.Add(this.btnGenerateSurface);
            this.Controls.Add(this.cmbFunction);
            this.Controls.Add(this.txtDivisions);
            this.Controls.Add(this.txtY1);
            this.Controls.Add(this.txtY0);
            this.Controls.Add(this.txtX1);
            this.Controls.Add(this.txtX0);
            this.Controls.Add(this.BuildRevolutionFigureButton);
            this.Controls.Add(this.SegmentsNumericUpDown);
            this.Controls.Add(this.AxisComboBox1);
            this.Controls.Add(this.GeneratingPointsGrid);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.DodecahedronButton);
            this.Controls.Add(this.IcosahedronButton);
            this.Controls.Add(this.RotateAroundLineButton);
            this.Controls.Add(this.z2Coord);
            this.Controls.Add(this.y2Coord);
            this.Controls.Add(this.x2Coord);
            this.Controls.Add(this.z2TextBox);
            this.Controls.Add(this.y2TextBox);
            this.Controls.Add(this.x2TextBox);
            this.Controls.Add(this.z1Coord);
            this.Controls.Add(this.y1Coord);
            this.Controls.Add(this.x1Coord);
            this.Controls.Add(this.z1TextBox);
            this.Controls.Add(this.y1TextBox);
            this.Controls.Add(this.x1TextBox);
            this.Controls.Add(this.RotateAroundAxisCenterButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.axisComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ScaleButton);
            this.Controls.Add(this.OffsetButton);
            this.Controls.Add(this.RotateXButton);
            this.Controls.Add(this.RotateYButton);
            this.Controls.Add(this.RotateZButton);
            this.Controls.Add(this.ReflectXYButton);
            this.Controls.Add(this.ReflectXZButton);
            this.Controls.Add(this.ReflectYZButton);
            this.Controls.Add(this.ProjectionComboBox);
            this.Controls.Add(this.ScaleTextBox);
            this.Controls.Add(this.RotateTextBox);
            this.Controls.Add(this.OffsetXTextBox);
            this.Controls.Add(this.OffsetYTextBox);
            this.Controls.Add(this.OffsetZTextBox);
            this.Controls.Add(this.TetrahedronButton);
            this.Controls.Add(this.HexahedronButton);
            this.Controls.Add(this.OctahedronButton);
            this.Name = "Form1";
            this.Text = "Polyhedron Transformations";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.GeneratingPointsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox axisComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button RotateAroundAxisCenterButton;
        private System.Windows.Forms.TextBox x1TextBox;
        private System.Windows.Forms.TextBox y1TextBox;
        private System.Windows.Forms.TextBox z1TextBox;
        private System.Windows.Forms.Label x1Coord;
        private System.Windows.Forms.Label y1Coord;
        private System.Windows.Forms.Label z1Coord;
        private System.Windows.Forms.Label z2Coord;
        private System.Windows.Forms.Label y2Coord;
        private System.Windows.Forms.Label x2Coord;
        private System.Windows.Forms.TextBox z2TextBox;
        private System.Windows.Forms.TextBox y2TextBox;
        private System.Windows.Forms.TextBox x2TextBox;
        private System.Windows.Forms.Button RotateAroundLineButton;
        private System.Windows.Forms.Button IcosahedronButton;
        private System.Windows.Forms.Button DodecahedronButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView GeneratingPointsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.ComboBox AxisComboBox1;
        private System.Windows.Forms.TextBox SegmentsNumericUpDown;
        private System.Windows.Forms.Button BuildRevolutionFigureButton;
        private System.Windows.Forms.TextBox txtX0;
        private System.Windows.Forms.TextBox txtX1;
        private System.Windows.Forms.TextBox txtY0;
        private System.Windows.Forms.TextBox txtY1;
        private System.Windows.Forms.TextBox txtDivisions;
        private System.Windows.Forms.ComboBox cmbFunction;
        private System.Windows.Forms.Button btnGenerateSurface;
    }
}
