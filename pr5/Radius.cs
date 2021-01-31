using System;
using System.Windows.Forms;

namespace pr5
{
    public partial class Radius : Form
    {
        public delegate void RHandler(int r);

        public static event RHandler RChanged;

        public Radius()
        {
            InitializeComponent();
            // ReSharper disable once VirtualMemberCallInConstructor
            DoubleBuffered = true;
            Opend = true;
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            RChanged?.Invoke(trackBar1.Value);
        }

        private void Radius_FormClosed(object sender, FormClosedEventArgs e)
        {
            Opend = false;
        }

        private void Radius_Invalidate(object sender, InvalidateEventArgs e)
        {
            trackBar1.Value = Shape.R;
        }
    }
}