using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace pr5Lib
{
    /// <summary>
    /// Плагин должен состоять из 1 файла, содержащего в себе описание класса, наследуемого от Shape.
    /// Название файла соответствует названию класса ровно как и общему названию плагина.
    /// </summary>
    [Serializable]
    public abstract class Shape
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

        [NonSerialized] protected bool isPressed = false;

        public bool IsPressed
        {
            get => isPressed;
            set => isPressed = value;
        }

        [NonSerialized] protected (int dx, int dy) dif;

        public (int dx, int dy) Dif
        {
            get => dif;
            set => dif = value;
        }

        [NonSerialized] protected bool isTop = false;

        public bool IsTop
        {
            get => isTop;
            set => isTop = value;
        }

        protected static int r;

        public static int R
        {
            get => r;
            set => r = value;
        }

        [NonSerialized] protected static Color lineColor;

        public static Color LineColor
        {
            get => lineColor;
            set => lineColor = value;
        }

        [NonSerialized] protected static Color insideColor;

        public static Color InsideColor
        {
            get => insideColor;
            set => insideColor = value;
        }

        public static bool operator ==(Shape a, Shape b) => (a.X == b.X) && (a.Y == b.Y);
        public static bool operator !=(Shape a, Shape b) => (a.X != b.X) || (a.Y != b.Y);

        protected Shape(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        static Shape()
        {
            R = 30;
        }

        public virtual void Draw(Graphics graphics)
        {
        }

        public virtual bool IsInside(int x1, int y1) => false;
        
        /// <summary>
        /// Возвращает условную копию объекта по его значениям.
        /// </summary>
        /// <returns>new [ShapeName](x, y).</returns>
        public abstract Shape Copy();

    }

    [Serializable]
    public class Circle : Shape
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

        public override void Draw(Graphics graphics)
        {
            graphics.DrawEllipse(new Pen(lineColor, 2), x - R, y - R, 2 * R, 2 * R);
            graphics.FillEllipse(new SolidBrush(insideColor), x - R, y - R, 2 * R, 2 * R);
        }

        public override Shape Copy()
        {
            return new Circle(x, y);
        }
    }

    [Serializable]
    public class Square : Shape
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
            graphics.DrawPolygon(new Pen(lineColor, 2), plist);
            graphics.FillPolygon(new SolidBrush(insideColor), plist);
        }

        public override bool IsInside(int x1, int y1)
        {
            double len = Math.Sqrt(2 * R * R);
            if (x1 >= x - len / 2 && x1 <= x + len / 2)
                if (y1 >= y - len / 2 && y1 <= y + len / 2)
                    return true;
            return false;
        }

        public override Shape Copy()
        {
            return new Square(x, y);
        }
    }

    [Serializable]
    public class Triangle : Shape
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
            graphics.DrawPolygon(new Pen(lineColor, 2), plist);
            graphics.FillPolygon(new SolidBrush(insideColor), plist);
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

        public override Shape Copy()
        {
            return new Triangle(x, y);
        }
    }
}