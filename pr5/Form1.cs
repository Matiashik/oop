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
using System.Windows.Forms.VisualStyles;

namespace pr5
{
    public partial class Form1 : Form
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        List<Shape> _splist = new List<Shape>(); //shapes

        private int _shapeType;

        public Form1()
        {
            InitializeComponent();
            // ReSharper disable once VirtualMemberCallInConstructor
            DoubleBuffered = true;
            circleToolStripMenuItem.Checked = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_splist.Count >= 3)
            {
                for (int i = 0; i < _splist.Count; i++)
                {
                    for (int j = i + 1; j < _splist.Count; j++)
                    {
                        var t1 = _splist[i];
                        var t2 = _splist[j];
                        double k = ((double) t1.Y - t2.Y) / (t1.X - t2.X);
                        double b = t1.Y - k * t1.X;
                        var a = true;

                        for (int f = 0; f < _splist.Count; f++)
                        {
                            if (f == i || f == j) continue;
                            if (_splist[f].Y >= _splist[f].X * k + b)
                            {
                                a = false;
                                break;
                            }
                        }

                        if (!a)
                        {
                            a = true;
                            for (int f = 0; f < _splist.Count; f++)
                            {
                                if (f == i || f == j) continue;
                                if (_splist[f].Y <= _splist[f].X * k + b)
                                {
                                    a = false;
                                    break;
                                }
                            }
                        }

                        if (a)
                        {
                            _splist[i].IsTop = true;
                            _splist[j].IsTop = true;
                            e.Graphics.DrawLine(new Pen(Color.Black), t1.X, t1.Y, t2.X, t2.Y);
                        }
                    }
                }

                for (int f = 0; f < _splist.Count; f++)
                {
                    var sp = _splist[f];
                    if (!sp.IsTop && !sp.IsPressed)
                    {
                        _splist.RemoveAt(f);
                        continue;
                    }

                    sp.Draw(e.Graphics);
                    sp.IsTop = false;
                }
            }
            else
                foreach (var sp in _splist)
                    sp.Draw(e.Graphics);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    foreach (var sp in _splist)
                        if (sp.IsInside(e.X, e.Y))
                        {
                            for (int i = 0; i < _splist.Count; i++)
                                if (_splist[i].IsInside(e.X, e.Y))
                                {
                                    _splist[i].IsPressed = true;
                                    _splist[i].Dif = (e.X - _splist[i].X, e.Y - _splist[i].Y);
                                }

                            return;
                        }

                    switch (_shapeType)
                    {
                        case 1:
                            _splist.Add(new Triangle(e.X, e.Y));
                            break;
                        case 2:
                            _splist.Add(new Square(e.X, e.Y));
                            break;
                        default:
                            _splist.Add(new Circle(e.X, e.Y));
                            break;
                    }

                    Refresh();
                    break;

                case MouseButtons.Right:
                    for (int i = _splist.Count - 1; i >= 0; i--)
                        if (_splist[i].IsInside(e.X, e.Y))
                        {
                            _splist.RemoveAt(i);
                            break;
                        }

                    Refresh();
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (var el in _splist)
                if (el.IsPressed)
                    el.IsPressed = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (var el in _splist)
                if (el.IsPressed)
                    (el.X, el.Y) = (e.X - el.Dif.dx, e.Y - el.Dif.dy);

            Refresh();
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _shapeType = 0;
            circleToolStripMenuItem.Checked = true;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = false;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _shapeType = 1;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = true;
            squareToolStripMenuItem.Checked = false;
        }

        private void squareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _shapeType = 2;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = true;
        }
    }
}