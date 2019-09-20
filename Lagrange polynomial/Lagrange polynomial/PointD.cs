using System;
using System.Drawing;

namespace Lagrange_Ploynome
{
    public class PointD:IComparable<PointD>
    {
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
        public PointD()
        {
            X = 0;
            Y = 0;
        }
        public PointD(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public static implicit operator PointD(Point p)
        {
            return new PointD(p.X, p.Y);
        }

        public static implicit operator Point(PointD p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static implicit operator PointF(PointD p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }

        public Point Round()
        {
            return new Point((int)Math.Round(X), (int)Math.Round(Y));
        }

        public int CompareTo(PointD other)
        {
            return Math.Sign(other.X - X);
        }

        public double X, Y;

        public override string ToString()
        {
            return ((PointF)this).ToString(); 
        }
    }
}