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
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        List<Shape> _splist = new List<Shape>(); //shapes

        bool _mousePressed = true;
        List<(int i, int dx, int dy)> _pindex = new List<(int i, int dx, int dy)>(); //pressed shapes

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    foreach (var sp in _splist)
                        if (sp.IsInside(e.X, e.Y))
                            return;
                    _splist.Add(new Circle(e.X, e.Y));
                    Invalidate();
                    break;

                case MouseButtons.Right:
                    for (int i = _splist.Count - 1; i >= 0; i--)
                        if (_splist[i].IsInside(e.X, e.Y))
                        {
                            _splist.RemoveAt(i);
                            break;
                        }

                    Invalidate();
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var sp in _splist)
            {
                sp.Draw(e.Graphics);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mousePressed = true;
                for (int i = 0; i < _splist.Count; i++)
                    if (_splist[i].IsInside(e.X, e.Y))
                        _pindex.Add((i, e.X - _splist[i].X, e.Y - _splist[i].Y));
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            _mousePressed = false;
            _pindex = new List<(int i, int dx, int dy)>();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePressed && _pindex.Count != 0)
            {
                foreach (var el in _pindex)
                {
                    (_splist[el.i].X, _splist[el.i].Y) = (e.X - el.dx, e.Y - el.dy);
                }

                Invalidate();
            }
        }
    }
}