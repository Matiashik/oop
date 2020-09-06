using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pr4
{
    public partial class Form1 : Form
    {
        private int Rect;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Rect += 5;
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillPolygon(new SolidBrush(Color.Red),
                new Point[]
                {
                    new Point((ClientSize.Width + panel1.ClientSize.Width) / 2 - Rect, (ClientSize.Height + Rect) / 2),
                    new Point((ClientSize.Width + panel1.ClientSize.Width) / 2, (ClientSize.Height + Rect) / 2 - Rect),
                    new Point((ClientSize.Width + panel1.ClientSize.Width) / 2 + Rect, (ClientSize.Height + Rect) / 2)
                });
        }
    }
}