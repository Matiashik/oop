using System;
using pr5Lib;
using System.Drawing;


[Serializable]
public class Rect : Shape
{
    public Rect(int x, int y) : base(x, y)
    {
    }

    public Rect() : base(0, 0)
    {
    }

    public override Shape Copy()
    {
        return new Rect(x, y);
    }

    public override void Draw(Graphics graphics)
    {
        double len = Math.Sqrt(2 * R * R);
        PointF[] plist = new PointF[4];
        plist[0] = new PointF((float) (x - len), (float) (y + len / 2));
        plist[1] = new PointF((float) (x + len), (float) (y + len / 2));
        plist[2] = new PointF((float) (x + len), (float) (y - len / 2));
        plist[3] = new PointF((float) (x - len), (float) (y - len / 2));
        graphics.DrawPolygon(new Pen(lineColor, 2), plist);
        graphics.FillPolygon(new SolidBrush(insideColor), plist);
    }

    public override bool IsInside(int x1, int y1)
    {
        double len = Math.Sqrt(2 * R * R);
        if (x1 >= x - len && x1 <= x + len)
            if (y1 >= y - len / 2 && y1 <= y + len / 2)
                return true;
        return false;
    }
}