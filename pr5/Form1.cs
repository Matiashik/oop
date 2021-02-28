using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using pr5Lib;

namespace pr5
{
    public partial class Form1 : Form
    {
        #region Locals

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once IdentifierTypo
        List<Shape> _splist = new List<Shape>(); //shapes

        private int _shapeType;
        private int _algorithmType;
        private Form _radius;
        private bool _play = false;
        private int _t = 10;
        private Task _playT;
        private string _fileN;
        private (int x, int y, bool t) _mouseS;
        private bool _changed = false;

        #endregion

        public Form1()
        {
            InitializeComponent();
            // ReSharper disable once VirtualMemberCallInConstructor
            DoubleBuffered = true;
            circleToolStripMenuItem.Checked = true;
            jarvisToolStripMenuItem.Checked = true;
            KeyPreview = true;
            Radius.RChanged += radius_Changed;
            Directory.CreateDirectory("saves");
            File.Create("saves/QuickSave.shp");
            Shape.InsideColor = DefaultBackColor;
            Shape.LineColor = Color.Black;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_splist.Count >= 3)
            {
                var spls = new List<Shape>();

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
                                spls.Add(_splist[i]);
                                spls.Add(_splist[j]);
                                e.Graphics.DrawLine(new Pen(Color.Black), t1.X, t1.Y, t2.X, t2.Y);
                            }
                        }
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
                }

                int m = 1;

                for (int f = 0; f < _splist.Count; f++)
                {
                    var sp = _splist[f];
                    if (!sp.IsTop && !sp.IsPressed)
                    {
                        if(!_mouseS.t) new Remove(sp, f);
                        _splist.RemoveAt(f);
                        m++;
                        continue;
                    }

                    sp.Draw(e.Graphics);
                    sp.IsTop = false;
                }

                if (m > 1) new Multi(m);
            }
            else
                foreach (var sp in _splist)
                    sp.Draw(e.Graphics);
        }

        #region MouseAndKeyEvents

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    foreach (var sp in _splist)
                        if (sp.IsInside(e.X, e.Y))
                        {
                            _mouseS = (e.X, e.Y, false);
                            for (int i = 0; i < _splist.Count; i++)
                                if (_splist[i].IsInside(e.X, e.Y))
                                {
                                    _splist[i].IsPressed = true;
                                    _splist[i].Dif = (e.X - _splist[i].X, e.Y - _splist[i].Y);
                                }

                            return;
                        }

                    var spls = _splist.ToList();
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

                    new Add(_splist[_splist.Count - 1], _splist.Count - 1);
                    Refresh();
                    if (spls.Count == _splist.Count)
                    {
                        var fl = true;
                        for (int i = 0; i < spls.Count; i++)
                        {
                            if (spls[i] != _splist[i])
                            {
                                fl = false;
                                break;
                            }
                        }

                        if (fl)
                        {
                            Do.Back.Peek().Undo(ref _splist);
                            _mouseS = (e.X, e.Y, true);
                            foreach (var sp in _splist)
                            {
                                sp.IsPressed = true;
                                sp.Dif = (e.X - sp.X, e.Y - sp.Y);
                            }
                        }
                    }
                    else _changed = true;
                    
                    break;

                case MouseButtons.Right:
                    for (int i = _splist.Count - 1; i >= 0; i--)
                        if (_splist[i].IsInside(e.X, e.Y))
                        {
                            new Remove(_splist[i], i);
                            _splist.RemoveAt(i);
                            break;
                        }

                    _changed = true;
                    Refresh();
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_mouseS.t) new DoMove(_mouseS.x - e.X, _mouseS.y - e.Y);
            int m = 0;
            foreach (var el in _splist)
            {
                if (el.IsPressed)
                {
                    if (!_mouseS.t)
                    {
                        new DoSinMove(_mouseS.x - e.X, _mouseS.y - e.Y, _splist.IndexOf(el));
                        m++;
                    }
                    el.IsPressed = false;
                }
            }

            if (!_mouseS.t && m > 0) new Multi(m);
            Refresh();
            _mouseS.t = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (var el in _splist)
                if (el.IsPressed)
                {
                    (el.X, el.Y) = (e.X - el.Dif.dx, e.Y - el.Dif.dy);
                    _changed = true;
                }

            Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            FileStream fs = null;
            var bf = new BinaryFormatter();
            switch (e.KeyCode)
            {
                case Keys.F5:
                    fs = new FileStream("saves/QuickSave.shp", FileMode.Create, FileAccess.Write);
                    bf.Serialize(fs, _splist);
                    bf.Serialize(fs, Shape.R);
                    bf.Serialize(fs, Shape.LineColor);
                    bf.Serialize(fs, Shape.InsideColor);
                    bf.Serialize(fs, _algorithmType);
                    bf.Serialize(fs, _shapeType);
                    bf.Serialize(fs, toolStripTextBox1.Text);
                    bf.Serialize(fs, _play);
                    fs.Close();
                    _changed = false;
                    break;

                case Keys.F6:
                    try
                    {
                        fs = new FileStream("saves/QuickSave.shp", FileMode.Open, FileAccess.Read);
                    }
                    catch (Exception ex)
                    {
                        return;
                    }

                    // ReSharper disable once AssignNullToNotNullAttribute

                    _splist = (List<Shape>) bf.Deserialize(fs);
                    Shape.R = (int) bf.Deserialize(fs);
                    Shape.LineColor = (Color) bf.Deserialize(fs);
                    Shape.InsideColor = (Color) bf.Deserialize(fs);
                    _algorithmType = (int) bf.Deserialize(fs);
                    _shapeType = (int) bf.Deserialize(fs);
                    toolStripTextBox1.Text = (string) bf.Deserialize(fs);
                    _play = (bool) bf.Deserialize(fs);

                    _radius?.Invalidate();
                    switch (_shapeType)
                    {
                        case 0:
                            circleToolStripMenuItem_Click(null, null);
                            break;
                        case 1:
                            triangleToolStripMenuItem_Click(null, null);
                            break;
                        case 2:
                            squareToolStripMenuItem_Click(null, null);
                            break;
                    }

                    switch (_algorithmType)
                    {
                        case 0:
                            jarvisToolStripMenuItem_Click(null, null);
                            break;
                        case 1:
                            byDefinitionToolStripMenuItem_Click(null, null);
                            break;
                    }

                    fs.Close();
                    _changed = false;
                    Refresh();
                    break;
            }
        }

        #endregion

        #region MenuEvents

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _shapeType = 0;
            circleToolStripMenuItem.Checked = true;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = false;
            Refresh();
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _shapeType = 1;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = true;
            squareToolStripMenuItem.Checked = false;
            Refresh();
        }

        private void squareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _shapeType = 2;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = true;
            Refresh();
        }

        private void byDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _algorithmType = 1;
            jarvisToolStripMenuItem.Checked = false;
            byDefinitionToolStripMenuItem.Checked = true;
            Refresh();
        }

        private void jarvisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _algorithmType = 0;
            jarvisToolStripMenuItem.Checked = true;
            byDefinitionToolStripMenuItem.Checked = false;
            Refresh();
        }

        private void linesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = Shape.LineColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Shape.LineColor = colorDialog1.Color;
                _changed = true;
                // ReSharper disable once ObjectCreationAsStatement
                new CColor(Shape.InsideColor, Shape.InsideColor, a, Shape.LineColor);
            }

            Refresh();
        }

        private void insideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = Shape.InsideColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _changed = true;
                Shape.InsideColor = colorDialog1.Color;
                // ReSharper disable once ObjectCreationAsStatement
                new CColor(a, Shape.InsideColor, Shape.LineColor, Shape.LineColor);
            }

            Refresh();
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shape.InsideColor = DefaultBackColor;
            Shape.LineColor = Color.Black;
            _changed = true;
            Refresh();
        }

        private void radiusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Radius.Opend)
            {
                _radius = new Radius();
                _radius.Show();
            }
            else
            {
                _radius.WindowState = FormWindowState.Normal;
                _radius?.Activate();
            }
        }

        private void toolStripPlay_Click(object sender, EventArgs e)
        {
            _changed = true;

            void Play()
            {
                var r = new Random();
                while (_play)
                {
                    foreach (var sp in _splist)
                    {
                        if (!sp.IsPressed)
                        {
                            sp.X += r.Next(-1, 2);
                            sp.Y += r.Next(-1, 2);
                        }
                    }

                    Refresh();
                    Thread.Sleep(_t);
                }
            }

            if (!_play)
            {
                try
                {
                    _playT.Dispose();
                }
                catch (Exception ex)
                {
                    // ignored
                }

                _play = true;
                _playT = new Task(Play);
                _playT.Start();
            }
        }

        private void toolStripStop_Click(object sender, EventArgs e)
        {
            _play = false;
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.SelectionStart = 0;
            toolStripTextBox1.SelectionLength = toolStripTextBox1.Text.Length;
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text == "") return;

            var arr = new List<char> {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};

            foreach (var i in toolStripTextBox1.Text)
            {
                if (!arr.Contains(i))
                {
                    toolStripTextBox1.Text = _t.ToString();
                    toolStripTextBox1.SelectionStart = toolStripTextBox1.Text.Length;
                    toolStripTextBox1.SelectionLength = 0;
                    return;
                }
            }

            _t = Convert.ToDouble(toolStripTextBox1.Text) > Int32.MaxValue
                ? Int32.MaxValue
                : Convert.ToInt32(toolStripTextBox1.Text);

            if (_play)
            {
                _play = false;
                toolStripPlay_Click(null, null);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fdg = new SaveFileDialog();
            fdg.InitialDirectory = Path.GetFullPath("saves");
            // ReSharper disable once LocalizableElement
            fdg.Filter = "Shape saves|*.shp";
            while (true)
            {
                var dres = fdg.ShowDialog();
                if (dres == DialogResult.OK)
                    break;
                if (dres == DialogResult.Cancel || dres == DialogResult.Abort)
                    return;
            }

            _fileN = fdg.FileName;
            this.Text = fdg.FileName;
            var fs = new FileStream(fdg.FileName, FileMode.Create, FileAccess.Write);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, _splist);
            bf.Serialize(fs, Shape.R);
            bf.Serialize(fs, Shape.LineColor);
            bf.Serialize(fs, Shape.InsideColor);
            bf.Serialize(fs, _algorithmType);
            bf.Serialize(fs, _shapeType);
            bf.Serialize(fs, toolStripTextBox1.Text);
            bf.Serialize(fs, _play);
            fs.Close();
            _changed = false;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fdg = new OpenFileDialog();
            fdg.InitialDirectory = Path.GetFullPath("saves");
            // ReSharper disable once LocalizableElement
            fdg.Filter = "Shape saves|*.shp";
            while (true)
            {
                var dres = fdg.ShowDialog();
                if (dres == DialogResult.OK)
                    break;
                if (dres == DialogResult.Cancel || dres == DialogResult.Abort)
                    return;
            }

            FileStream fs = null;
            var bf = new BinaryFormatter();
            try
            {
                fs = new FileStream(fdg.FileName, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                return;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            _fileN = fdg.FileName;
            this.Text = fdg.FileName;
            _splist = (List<Shape>) bf.Deserialize(fs);
            Shape.R = (int) bf.Deserialize(fs);
            Shape.LineColor = (Color) bf.Deserialize(fs);
            Shape.InsideColor = (Color) bf.Deserialize(fs);
            _algorithmType = (int) bf.Deserialize(fs);
            _shapeType = (int) bf.Deserialize(fs);
            toolStripTextBox1.Text = (string) bf.Deserialize(fs);
            _play = (bool) bf.Deserialize(fs);

            _radius?.Invalidate();
            switch (_shapeType)
            {
                case 0:
                    circleToolStripMenuItem_Click(null, null);
                    break;
                case 1:
                    triangleToolStripMenuItem_Click(null, null);
                    break;
                case 2:
                    squareToolStripMenuItem_Click(null, null);
                    break;
            }

            switch (_algorithmType)
            {
                case 0:
                    jarvisToolStripMenuItem_Click(null, null);
                    break;
                case 1:
                    byDefinitionToolStripMenuItem_Click(null, null);
                    break;
            }

            fs.Close();
            _changed = false;
            Refresh();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_fileN == null)
            {
                saveToolStripMenuItem_Click(null, null);
                return;
            }

            var fs = new FileStream(_fileN, FileMode.Create, FileAccess.Write);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, _splist);
            bf.Serialize(fs, Shape.R);
            bf.Serialize(fs, Shape.LineColor);
            bf.Serialize(fs, Shape.InsideColor);
            bf.Serialize(fs, _algorithmType);
            bf.Serialize(fs, _shapeType);
            bf.Serialize(fs, toolStripTextBox1.Text);
            bf.Serialize(fs, _play);
            fs.Close();
            _changed = false;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_changed) ChangeHandler();

            _fileN = null;
            _splist = new List<Shape>();
            jarvisToolStripMenuItem_Click(null, null);
            Shape.R = 30;
            circleToolStripMenuItem_Click(null, null);
            defaultToolStripMenuItem_Click(null, null);
            // ReSharper disable once LocalizableElement
            this.Text = "New";
            _changed = false;
            Refresh();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("{{{" + Do.Back.Peek().GetType() + "}}}");
                Do.Back.Peek().Undo(ref _splist);
            }
            catch (Exception exception)
            {
                // ignored
            }

            Refresh();
        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                Do.Forw.Peek().Redo(ref _splist);
            }
            catch (Exception exception)
            {
                // ignored
            }

            Refresh();
        }

        #endregion

        private void radius_Changed(int r)
        {
            Shape.R = r;
            _changed = true;
            Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changed) ChangeHandler();
        }

        private void ChangeHandler()
        {
            if (MessageBox.Show("Вы не сохранили изменения. Сохранить?", "Save changes",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (_fileN == null) saveToolStripMenuItem_Click(null, null);
                else saveToolStripMenuItem1_Click(null, null);
            }
        }
    }
}