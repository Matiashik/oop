using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pr5
{
    abstract class Shape
    {
        protected int x;

        public int X
        {
            get => x;
            set => x = value;
        }

        protected int y;

        public int Y
        {
            get => y;
            set => y = value;
        }

        protected static int r;

        public static int R
        {
            get => r;
            set => r = value;
        }

        protected static Color color;

        public static Color Color
        {
            get => color;
            set => color = value;
        }

        protected Shape(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        static Shape()
        {
            color = Color.Black;
            R = 15;
        }

        public virtual void Draw(Graphics graphics)
        {
        }

        public virtual bool IsInside(int x1, int y1) => false;
    }

    class Circle : Shape
    {
        public Circle(int x, int y) : base(x, y)
        {
        }

        public override bool IsInside(int x1, int y1)
        {
            int xx = Math.Abs(x1 - x);
            int yy = Math.Abs(y1 - y);
            return Math.Sqrt(xx * xx + yy * yy) <= R;
        }

        public override void Draw(Graphics graphics) =>
            graphics.DrawEllipse(new Pen(color), x - R, y - R, 2 * R, 2 * R);
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
            plist[1] = new PointF(x - R * (float) Math.Sin(1.0472), y + R / 2);
            plist[2] = new PointF(x + R * (float) Math.Sin(1.0472), y + R / 2);
            graphics.DrawPolygon(new Pen(color), plist);
        }

        public override bool IsInside(int x1, int y1)
        {
            double Dis((float x, float y) a, (float x, float y) b) =>
                Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));

            double Sqr(double s1, double s2, double s3)
            {
                double p = (s1 + s2 + s3) / 2;
                return Math.Sqrt(p * (p - s1) * (p - s2) * (p - s3));
            }

            return Math.Abs(Sqr(Dis((x, y - R), (x1, y1)),
                                Dis((x - R * (float) Math.Sin(1.0472), y + R / 2), (x1, y1)),
                                Dis((x, y - R), (x - R * (float) Math.Sin(1.0472), y + R / 2))) +
                            Sqr(Dis((x, y - R), (x1, y1)),
                                Dis((x + R * (float) Math.Sin(1.0472), y + R / 2), (x1, y1)),
                                Dis((x, y - R), (x + R * (float) Math.Sin(1.0472), y + R / 2))) +
                            Sqr(Dis((x + R * (float) Math.Sin(1.0472), y + R / 2), (x1, y1)),
                                Dis((x - R * (float) Math.Sin(1.0472), y + R / 2), (x1, y1)),
                                Dis((x + R * (float) Math.Sin(1.0472), y + R / 2),
                                    (x - R * (float) Math.Sin(1.0472), y + R / 2))) -
                            Sqr(Dis((x + R * (float) Math.Sin(1.0472), y + R / 2), (x, y - R)),
                                Dis((x - R * (float) Math.Sin(1.0472), y + R / 2), (x, y - R)),
                                Dis((x + R * (float) Math.Sin(1.0472), y + R / 2),
                                    (x - R * (float) Math.Sin(1.0472), y + R / 2)))) < 0.0001;
        }
    }
}