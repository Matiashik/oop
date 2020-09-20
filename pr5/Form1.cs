using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pr5
{
    public partial class Form1 : Form
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        List<Shape> SPLIST = new List<Shape>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (var sp in SPLIST) if (sp.IsInside(e.X, e.Y)) return;
                SPLIST.Add(new Circle(e.X, e.Y));
                Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < SPLIST.Count; i++)
                    if (SPLIST[i].IsInside(e.X, e.Y))
                        SPLIST.RemoveAt(i);
                Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var sp in SPLIST)
            {
                sp.Draw(e.Graphics);
            }
        }
    }
}