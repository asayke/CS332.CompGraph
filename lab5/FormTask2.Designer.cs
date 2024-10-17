namespace CG_Lab
{
    partial class FormTask2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.PrevStep = new System.Windows.Forms.Button();
            this.NextStep = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Roughness = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.EndHeight = new System.Windows.Forms.NumericUpDown();
            this.StartHeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.MountainButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Roughness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox.Location = new System.Drawing.Point(12, 1);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1054, 491);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // PrevStep
            // 
            this.PrevStep.Location = new System.Drawing.Point(1120, 330);
            this.PrevStep.Name = "PrevStep";
            this.PrevStep.Size = new System.Drawing.Size(30, 30);
            this.PrevStep.TabIndex = 1;
            this.PrevStep.Text = "<";
            this.PrevStep.UseVisualStyleBackColor = true;
            this.PrevStep.Click += new System.EventHandler(this.PrevStep_Click);
            // 
            // NextStep
            // 
            this.NextStep.Location = new System.Drawing.Point(1210, 330);
            this.NextStep.Name = "NextStep";
            this.NextStep.Size = new System.Drawing.Size(30, 30);
            this.NextStep.TabIndex = 2;
            this.NextStep.Text = ">";
            this.NextStep.UseVisualStyleBackColor = true;
            this.NextStep.Click += new System.EventHandler(this.NextStep_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1116, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Шероховатость R:";
            // 
            // Roughness
            // 
            this.Roughness.DecimalPlaces = 2;
            this.Roughness.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Roughness.Location = new System.Drawing.Point(1120, 214);
            this.Roughness.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Roughness.Name = "Roughness";
            this.Roughness.Size = new System.Drawing.Size(120, 26);
            this.Roughness.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1116, 287);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 40);
            this.label2.TabIndex = 5;
            this.label2.Text = "Шаги построения\r\nгорного массива:";
            // 
            // EndHeight
            // 
            this.EndHeight.Location = new System.Drawing.Point(1120, 150);
            this.EndHeight.Maximum = new decimal(new int[] {
            491,
            0,
            0,
            0});
            this.EndHeight.Name = "EndHeight";
            this.EndHeight.Size = new System.Drawing.Size(120, 26);
            this.EndHeight.TabIndex = 6;
            // 
            // StartHeight
            // 
            this.StartHeight.Location = new System.Drawing.Point(1120, 95);
            this.StartHeight.Maximum = new decimal(new int[] {
            491,
            0,
            0,
            0});
            this.StartHeight.Name = "StartHeight";
            this.StartHeight.Size = new System.Drawing.Size(120, 26);
            this.StartHeight.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1106, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Начальная высота:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1116, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Конечная высота:";
            // 
            // MountainButton
            // 
            this.MountainButton.Location = new System.Drawing.Point(1120, 246);
            this.MountainButton.Name = "MountainButton";
            this.MountainButton.Size = new System.Drawing.Size(120, 38);
            this.MountainButton.TabIndex = 10;
            this.MountainButton.Text = "Построить";
            this.MountainButton.UseVisualStyleBackColor = true;
            this.MountainButton.Click += new System.EventHandler(this.MountainButton_Click);
            // 
            // FormTask2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1291, 493);
            this.Controls.Add(this.MountainButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.StartHeight);
            this.Controls.Add(this.EndHeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Roughness);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NextStep);
            this.Controls.Add(this.PrevStep);
            this.Controls.Add(this.pictureBox);
            this.Name = "FormTask2";
            this.Text = "FormTask2";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Roughness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button PrevStep;
        private System.Windows.Forms.Button NextStep;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown Roughness;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown EndHeight;
        private System.Windows.Forms.NumericUpDown StartHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button MountainButton;
    }
}