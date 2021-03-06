using System;
using System.Diagnostics;
using System.Windows.Forms;
using pr5Lib;

namespace pr5
{
    public partial class Radius : Form
    {
        public delegate void RHandler(int r);

        public static event RHandler RChanged;

        private static int Rr;
        public Radius()
        {
            InitializeComponent();
            // ReSharper disable once VirtualMemberCallInConstructor
            DoubleBuffered = true;
            Opend = true;
            Rr = Shape.R;
            new R(0);
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (Do.Back.Peek().GetType().ToString() == "pr5Lib.R")
            {
                Do.Back.Peek().SetVal(Do.Back.Peek().GetVal() + trackBar1.Value - Shape.R);
            }
            else
            {
                new R(trackBar1.Value - Shape.R);
            }
            Debug.WriteLine(Do.Back.Peek().GetVal());
            RChanged?.Invoke(trackBar1.Value);
        }

        private void Radius_FormClosed(object sender, FormClosedEventArgs e)
        {
            Opend = false;
            if (Do.Back.Peek().GetType().ToString() == "pr5Lib.R")
                if (Do.Back.Peek().GetVal() == 0)
                    Do.Back.Pop();
        }

        private void Radius_Invalidate(object sender, InvalidateEventArgs e)
        {
            trackBar1.Value = Shape.R;
        }
    }
}