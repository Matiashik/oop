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
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            RChanged?.Invoke(trackBar1.Value);
        }
    }
}