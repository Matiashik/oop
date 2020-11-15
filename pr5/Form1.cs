﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private int _algorithmType;

        public Form1()
        {
            InitializeComponent();
            // ReSharper disable once VirtualMemberCallInConstructor
            DoubleBuffered = true;
            circleToolStripMenuItem.Checked = true;
            jarvisToolStripMenuItem.Checked = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_splist.Count >= 3)
            {
                if (_algorithmType == 1)
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

                if (_algorithmType == 0)
                {
                    double Dist(Shape a, Shape b) => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

                    (double X, double Y) Vector(Shape str, Shape end) => (end.X - str.X, end.Y - str.Y);

                    double Angle(Shape zr, Shape nw, Shape nx) =>
                        nw == nx
                            ? 0
                            : Math.Acos((Vector(nw, zr).X * Vector(nw, nx).X + Vector(nw, zr).Y * Vector(nw, nx).Y) /
                                        (Dist(nw, zr) * Dist(nw, nx)));

                    var spls = new List<Shape>();

                    {
                        var p = _splist[0];
                        foreach (var sp in _splist)
                        {
                            if (sp.Y < p.Y) p = sp;
                            else if (sp.Y == p.Y)
                                if (p.X > sp.X)
                                    p = sp;
                        }

                        p.IsTop = true;
                        spls.Add(p);
                        foreach (var sp in _splist)
                        {
                            if (Angle(new Circle(0, spls[0].Y), spls[0], sp) >
                                Angle(new Circle(0, spls[0].Y), spls[0], p)) p = sp;
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            else if (Angle(new Circle(0, spls[0].Y), spls[0], sp) ==
                                     Angle(new Circle(0, spls[0].Y), spls[0], p))
                                if (Dist(spls[0], sp) > Dist(spls[0], p))
                                    p = sp;
                        }

                        p.IsTop = true;
                        spls.Add(p);
                    }

                    e.Graphics.DrawLine(new Pen(Color.Black), spls[0].X, spls[0].Y, spls[1].X, spls[1].Y);

                    {
                        var i = 1;
                        do
                        {
                            var nx = spls[i];
                            foreach (var sp in _splist)
                            {
                                if (spls.Contains(sp) && sp != spls[0]) continue;
                                if (Angle(spls[i - 1], spls[i], sp) >
                                    Angle(spls[i - 1], spls[i], nx)) nx = sp;
                                // ReSharper disable once CompareOfFloatsByEqualityOperator
                                else if (Angle(spls[i - 1], spls[i], sp) ==
                                         Angle(spls[i - 1], spls[i], nx))
                                    if (Dist(spls[i], sp) > Dist(spls[i], nx))
                                        nx = sp;
                            }

                            nx.IsTop = true;
                            spls.Add(nx);
                            i++;
                            e.Graphics.DrawLine(new Pen(Color.Black), spls[i - 1].X, spls[i - 1].Y, spls[i].X,
                                spls[i].Y);
                        } while (spls[0] != spls[i]);
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


        private void byDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _algorithmType = 1;
            jarvisToolStripMenuItem.Checked = false;
            byDefinitionToolStripMenuItem.Checked = true;
        }

        private void jarvisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _algorithmType = 0;
            jarvisToolStripMenuItem.Checked = true;
            byDefinitionToolStripMenuItem.Checked = false;
        }
    }
}