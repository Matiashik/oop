using System;
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
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            RChanged?.Invoke(trackBar1.Value);
        }

        private void Radius_FormClosed(object sender, FormClosedEventArgs e)
        {
            Opend = false;
            new R(Rr - Shape.R);
        }

        private void Radius_Invalidate(object sender, InvalidateEventArgs e)
        {
            trackBar1.Value = Shape.R;
        }
    }
}