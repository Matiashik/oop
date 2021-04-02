using System;
using pr5Lib;
using System.Drawing;


[Serializable]
public class RectInv : Shape
{
    public RectInv(int x, int y) : base(x, y)
    {
    }

    public RectInv() : base(0, 0)
    {
    }

    public override Shape Copy()
    {
        return new RectInv(x, y);
    }

    public override void Draw(Graphics graphics)
    {
        double len = Math.Sqrt(2 * R * R);
        PointF[] plist = new PointF[4];
        plist[0] = new PointF((float) (x - len / 2), (float) (y + len));
        plist[1] = new PointF((float) (x + len / 2), (float) (y + len));
        plist[2] = new PointF((float) (x + len / 2), (float) (y - len));
        plist[3] = new PointF((float) (x - len / 2), (float) (y - len));
        graphics.DrawPolygon(new Pen(lineColor, 2), plist);
        graphics.FillPolygon(new SolidBrush(insideColor), plist);
    }

    public override bool IsInside(int x1, int y1)
    {
        double len = Math.Sqrt(2 * R * R);
        if (x1 >= x - len / 2 && x1 <= x + len / 2)
            if (y1 >= y - len && y1 <= y + len)
                return true;
        return false;
    }
}