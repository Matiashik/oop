using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pr5
{
    abstract class Shape
    {
        protected int x;
        public int X { get; set; }
        protected int y;
        public int Y { get; set; }
        protected static int r;
        public static int R { get; set; }
        protected static Color color;
        public static Color Color { get; set; }

        protected Shape(int x, int y)
        {
            X = x;
            Y = y;
        }

        static Shape()
        {
            color = Color.Black;
            R = 3;
        }

        public abstract void Draw(Graphics graphics);

        public abstract bool IsInside(int x1, int y1);
    }

    class Circle : Shape
    {
        public Circle(int x, int y) : base(x, y)
        {
        }

        public override bool IsInside(int x1, int y1)
        {
            int xx = Math.Abs(x1 - X);
            int yy = Math.Abs(y1 - Y);
            return Math.Sqrt(xx * xx + yy * yy) <= R;
        }

        public override void Draw(Graphics graphics) => graphics.DrawEllipse(new Pen(color), x, y, 2 * R, 2 * R);
    }

    class Square : Shape
    {
        public Square(int x, int y) : base(x, y)
        {
        }

        public override void Draw(Graphics graphics)
        {
            double len = Math.Sqrt(2 * R * R);
            PointF[] plist = new PointF[4];
            plist[0] = new PointF((float) (x - len / 2), (float) (y + len / 2));
            plist[1] = new PointF((float) (x + len / 2), (float) (y + len / 2));
            plist[2] = new PointF((float) (x + len / 2), (float) (y - len / 2));
            plist[3] = new PointF((float) (x - len / 2), (float) (y - len / 2));
            graphics.DrawPolygon(new Pen(color), plist);
        }

        public override bool IsInside(int x1, int y1)
        {
            double len = Math.Sqrt(2 * R * R);
            if (x1 >= x - len / 2 && x1 <= x + len / 2)
                if (y1 >= y - len / 2 && y1 <= y + len / 2)
                    return true;
            return false;
        }
    }

    class Triangle : Shape
    {
        public Triangle(int x, int y) : base(x, y)
        {
        }

        public override void Draw(Graphics graphics)
        {
            PointF[] plist = new PointF[3];
            plist[0] = new PointF(x, y - R);
            plist[1] = new PointF(x - R * (float) Math.Sin(60), y + R / 2);
            plist[2] = new PointF(x + R * (float) Math.Sin(60), y + R / 2);
            graphics.DrawPolygon(new Pen(color), plist);
        }

        public override bool IsInside(int x1, int y1)
        {
            double dis((float x, float y) a, (float x, float y) b) =>
                Math.Sqrt(Math.Abs(a.x - b.x) * Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) * Math.Abs(a.y - b.y));

            double p((float x, float y) a, (float x, float y) b, (float x, float y) c) =>
                (dis(a, b) + dis(b, c) + dis(a, c)) / 2;

            double sqr((float x, float y) a, (float x, float y) b, (float x, float y) c) =>
                Math.Sqrt(p(a, b, c) * (p(a, b, c) - dis(a, b)) * (p(a, b, c) - dis(b, c)) * (p(a, b, c) - dis(a, c)));

            return sqr((x, y - R), (x - R * (float) Math.Sin(60), y + R / 2), (x1, y1)) +
                   sqr((x, y - R), (x + R * (float) Math.Sin(60), y + R / 2), (x1, y1)) +
                   sqr((x + R * (float) Math.Sin(60), y + R / 2), (x - R * (float) Math.Sin(60), y + R / 2),
                       (x1, y1)) ==
                   sqr((x + R * (float) Math.Sin(60), y + R / 2), (x - R * (float) Math.Sin(60), y + R / 2),
                       (x, y - R));
            
        }
    }
}