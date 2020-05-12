using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pr2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double a;
            try
            {
                a = double.Parse(textBox3.Text);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            this.label4.Text = (a * 1.61).ToString();
        }
    }
}