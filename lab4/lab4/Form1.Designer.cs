using System.Drawing;
namespace lab4
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.Button btnRotate;
        private System.Windows.Forms.Button btnScale;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtDx;
        private System.Windows.Forms.TextBox txtDy;
        private System.Windows.Forms.TextBox txtAngle;
        private System.Windows.Forms.TextBox txtScaleX;
        private System.Windows.Forms.TextBox txtScaleY;
        private System.Windows.Forms.Label lblDx;
        private System.Windows.Forms.Label lblDy;
        private System.Windows.Forms.Label lblAngle;
        private System.Windows.Forms.Label lblScaleX;
        private System.Windows.Forms.Label lblScaleY;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.btnRotate = new System.Windows.Forms.Button();
            this.btnScale = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtDx = new System.Windows.Forms.TextBox();
            this.txtDy = new System.Windows.Forms.TextBox();
            this.txtAngle = new System.Windows.Forms.TextBox();
            this.txtScaleX = new System.Windows.Forms.TextBox();
            this.txtScaleY = new System.Windows.Forms.TextBox();
            this.lblDx = new System.Windows.Forms.Label();
            this.lblDy = new System.Windows.Forms.Label();
            this.lblAngle = new System.Windows.Forms.Label();
            this.lblScaleX = new System.Windows.Forms.Label();
            this.lblScaleY = new System.Windows.Forms.Label();
            this.userXTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.userYTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.isPointInPolygon = new System.Windows.Forms.Label();
            this.vertexListLabel = new System.Windows.Forms.Label();
            this.vertexList = new System.Windows.Forms.ComboBox();
            this.EdgePoint1Label = new System.Windows.Forms.Label();
            this.EdgePoint2Label = new System.Windows.Forms.Label();
            this.EdgePoint1Value = new System.Windows.Forms.TextBox();
            this.EdgePoint2Value = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.LeftRightPosition = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.intersectionLabel = new System.Windows.Forms.Label();
            this.userInputEdgeCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(500, 400);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDoubleClick);
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(755, 34);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(100, 23);
            this.btnTranslate.TabIndex = 1;
            this.btnTranslate.Text = "Смещение";
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
            // 
            // btnRotate
            // 
            this.btnRotate.Location = new System.Drawing.Point(755, 98);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(100, 23);
            this.btnRotate.TabIndex = 2;
            this.btnRotate.Text = "Поворот";
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // btnScale
            // 
            this.btnScale.Location = new System.Drawing.Point(755, 158);
            this.btnScale.Name = "btnScale";
            this.btnScale.Size = new System.Drawing.Size(100, 23);
            this.btnScale.TabIndex = 3;
            this.btnScale.Text = "Масштаб";
            this.btnScale.Click += new System.EventHandler(this.btnScale_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(620, 296);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Очистить";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtDx
            // 
            this.txtDx.Location = new System.Drawing.Point(620, 20);
            this.txtDx.Name = "txtDx";
            this.txtDx.Size = new System.Drawing.Size(100, 22);
            this.txtDx.TabIndex = 5;
            this.txtDx.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // txtDy
            // 
            this.txtDy.Location = new System.Drawing.Point(620, 60);
            this.txtDy.Name = "txtDy";
            this.txtDy.Size = new System.Drawing.Size(100, 22);
            this.txtDy.TabIndex = 6;
            this.txtDy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // txtAngle
            // 
            this.txtAngle.Location = new System.Drawing.Point(620, 100);
            this.txtAngle.Name = "txtAngle";
            this.txtAngle.Size = new System.Drawing.Size(100, 22);
            this.txtAngle.TabIndex = 7;
            this.txtAngle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // txtScaleX
            // 
            this.txtScaleX.Location = new System.Drawing.Point(620, 140);
            this.txtScaleX.Name = "txtScaleX";
            this.txtScaleX.Size = new System.Drawing.Size(100, 22);
            this.txtScaleX.TabIndex = 8;
            this.txtScaleX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // txtScaleY
            // 
            this.txtScaleY.Location = new System.Drawing.Point(620, 180);
            this.txtScaleY.Name = "txtScaleY";
            this.txtScaleY.Size = new System.Drawing.Size(100, 22);
            this.txtScaleY.TabIndex = 9;
            this.txtScaleY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // lblDx
            // 
            this.lblDx.Location = new System.Drawing.Point(599, 20);
            this.lblDx.Name = "lblDx";
            this.lblDx.Size = new System.Drawing.Size(100, 23);
            this.lblDx.TabIndex = 10;
            this.lblDx.Text = "dx:";
            // 
            // lblDy
            // 
            this.lblDy.Location = new System.Drawing.Point(599, 60);
            this.lblDy.Name = "lblDy";
            this.lblDy.Size = new System.Drawing.Size(100, 23);
            this.lblDy.TabIndex = 11;
            this.lblDy.Text = "dy:";
            // 
            // lblAngle
            // 
            this.lblAngle.Location = new System.Drawing.Point(587, 100);
            this.lblAngle.Name = "lblAngle";
            this.lblAngle.Size = new System.Drawing.Size(100, 23);
            this.lblAngle.TabIndex = 12;
            this.lblAngle.Text = "Угол:";
            // 
            // lblScaleX
            // 
            this.lblScaleX.Location = new System.Drawing.Point(553, 140);
            this.lblScaleX.Name = "lblScaleX";
            this.lblScaleX.Size = new System.Drawing.Size(100, 23);
            this.lblScaleX.TabIndex = 13;
            this.lblScaleX.Text = "Масштаб X:";
            // 
            // lblScaleY
            // 
            this.lblScaleY.Location = new System.Drawing.Point(553, 180);
            this.lblScaleY.Name = "lblScaleY";
            this.lblScaleY.Size = new System.Drawing.Size(100, 23);
            this.lblScaleY.TabIndex = 14;
            this.lblScaleY.Text = "Масштаб Y:";
            // 
            // userXTextBox
            // 
            this.userXTextBox.Location = new System.Drawing.Point(620, 217);
            this.userXTextBox.Name = "userXTextBox";
            this.userXTextBox.Size = new System.Drawing.Size(100, 22);
            this.userXTextBox.TabIndex = 15;
            this.userXTextBox.TextChanged += new System.EventHandler(this.userXTextBox_TextChanged);
            this.userXTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(553, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "userX Input:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(553, 253);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 17;
            this.label2.Text = "userY input:";
            // 
            // userYTextBox
            // 
            this.userYTextBox.Location = new System.Drawing.Point(620, 250);
            this.userYTextBox.Name = "userYTextBox";
            this.userYTextBox.Size = new System.Drawing.Size(100, 22);
            this.userYTextBox.TabIndex = 18;
            this.userYTextBox.TextChanged += new System.EventHandler(this.userXTextBox_TextChanged);
            this.userYTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(518, 346);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(257, 16);
            this.label3.TabIndex = 19;
            this.label3.Text = "Находится ли точка внутри полигона: ";
            // 
            // isPointInPolygon
            // 
            this.isPointInPolygon.AutoSize = true;
            this.isPointInPolygon.Location = new System.Drawing.Point(712, 346);
            this.isPointInPolygon.Name = "isPointInPolygon";
            this.isPointInPolygon.Size = new System.Drawing.Size(0, 16);
            this.isPointInPolygon.TabIndex = 20;
            // 
            // vertexListLabel
            // 
            this.vertexListLabel.AutoSize = true;
            this.vertexListLabel.Location = new System.Drawing.Point(9, 426);
            this.vertexListLabel.Name = "vertexListLabel";
            this.vertexListLabel.Size = new System.Drawing.Size(197, 16);
            this.vertexListLabel.TabIndex = 21;
            this.vertexListLabel.Text = "координаты текущих вершин:";
            // 
            // vertexList
            // 
            this.vertexList.FormattingEnabled = true;
            this.vertexList.Location = new System.Drawing.Point(172, 426);
            this.vertexList.Name = "vertexList";
            this.vertexList.Size = new System.Drawing.Size(121, 24);
            this.vertexList.TabIndex = 22;
            // 
            // EdgePoint1Label
            // 
            this.EdgePoint1Label.AutoSize = true;
            this.EdgePoint1Label.Location = new System.Drawing.Point(548, 405);
            this.EdgePoint1Label.Name = "EdgePoint1Label";
            this.EdgePoint1Label.Size = new System.Drawing.Size(141, 16);
            this.EdgePoint1Label.TabIndex = 23;
            this.EdgePoint1Label.Text = "первая точка ребра:";
            // 
            // EdgePoint2Label
            // 
            this.EdgePoint2Label.AutoSize = true;
            this.EdgePoint2Label.Location = new System.Drawing.Point(548, 434);
            this.EdgePoint2Label.Name = "EdgePoint2Label";
            this.EdgePoint2Label.Size = new System.Drawing.Size(140, 16);
            this.EdgePoint2Label.TabIndex = 24;
            this.EdgePoint2Label.Text = "вторая точка ребра:";
            // 
            // EdgePoint1Value
            // 
            this.EdgePoint1Value.Location = new System.Drawing.Point(675, 402);
            this.EdgePoint1Value.Name = "EdgePoint1Value";
            this.EdgePoint1Value.Size = new System.Drawing.Size(100, 22);
            this.EdgePoint1Value.TabIndex = 25;
            this.EdgePoint1Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // EdgePoint2Value
            // 
            this.EdgePoint2Value.Location = new System.Drawing.Point(675, 434);
            this.EdgePoint2Value.Name = "EdgePoint2Value";
            this.EdgePoint2Value.Size = new System.Drawing.Size(100, 22);
            this.EdgePoint2Value.TabIndex = 26;
            this.EdgePoint2Value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userXTextBox_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(453, 468);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(264, 16);
            this.label4.TabIndex = 27;
            this.label4.Text = "Положение точки относительно ребра:";
            // 
            // LeftRightPosition
            // 
            this.LeftRightPosition.AutoSize = true;
            this.LeftRightPosition.Location = new System.Drawing.Point(675, 468);
            this.LeftRightPosition.Name = "LeftRightPosition";
            this.LeftRightPosition.Size = new System.Drawing.Size(0, 16);
            this.LeftRightPosition.TabIndex = 28;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(489, 501);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(218, 16);
            this.label5.TabIndex = 29;
            this.label5.Text = "Точка пересечения двух рёбер: ";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // intersectionLabel
            // 
            this.intersectionLabel.AutoSize = true;
            this.intersectionLabel.Location = new System.Drawing.Point(677, 501);
            this.intersectionLabel.Name = "intersectionLabel";
            this.intersectionLabel.Size = new System.Drawing.Size(0, 16);
            this.intersectionLabel.TabIndex = 30;
            this.intersectionLabel.Click += new System.EventHandler(this.label6_Click);
            // 
            // userInputEdgeCheckBox
            // 
            this.userInputEdgeCheckBox.AutoSize = true;
            this.userInputEdgeCheckBox.Location = new System.Drawing.Point(172, 480);
            this.userInputEdgeCheckBox.Name = "userInputEdgeCheckBox";
            this.userInputEdgeCheckBox.Size = new System.Drawing.Size(158, 20);
            this.userInputEdgeCheckBox.TabIndex = 31;
            this.userInputEdgeCheckBox.Text = "Режим ввода ребра";
            this.userInputEdgeCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.userInputEdgeCheckBox);
            this.Controls.Add(this.intersectionLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LeftRightPosition);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.EdgePoint2Value);
            this.Controls.Add(this.EdgePoint1Value);
            this.Controls.Add(this.EdgePoint2Label);
            this.Controls.Add(this.EdgePoint1Label);
            this.Controls.Add(this.vertexList);
            this.Controls.Add(this.vertexListLabel);
            this.Controls.Add(this.isPointInPolygon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.userYTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.userXTextBox);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.btnRotate);
            this.Controls.Add(this.btnScale);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.txtDx);
            this.Controls.Add(this.txtDy);
            this.Controls.Add(this.txtAngle);
            this.Controls.Add(this.txtScaleX);
            this.Controls.Add(this.txtScaleY);
            this.Controls.Add(this.lblDx);
            this.Controls.Add(this.lblDy);
            this.Controls.Add(this.lblAngle);
            this.Controls.Add(this.lblScaleX);
            this.Controls.Add(this.lblScaleY);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.TextBox userXTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox userYTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label isPointInPolygon;
        private System.Windows.Forms.Label vertexListLabel;
        private System.Windows.Forms.ComboBox vertexList;
        private System.Windows.Forms.Label EdgePoint1Label;
        private System.Windows.Forms.Label EdgePoint2Label;
        private System.Windows.Forms.TextBox EdgePoint1Value;
        private System.Windows.Forms.TextBox EdgePoint2Value;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label LeftRightPosition;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label intersectionLabel;
        private System.Windows.Forms.CheckBox userInputEdgeCheckBox;
    }
}


