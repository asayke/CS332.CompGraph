using System;
using System.Windows.Forms;

namespace CG_Lab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormTask1 task1Form = new FormTask1();
            task1Form.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormTask2 task2Form = new FormTask2();
            task2Form.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormTask3 task2Form = new FormTask3();
            task2Form.Show();
        }
    }
}
