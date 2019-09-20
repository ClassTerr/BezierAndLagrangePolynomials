using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Lagrange_Ploynome
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Paint += new PaintEventHandler(Form1_Paint);
            DoubleBuffered = true;

            Random r = new Random();


            Points.Add(new PointD(1, 10));
            Points.Add(new PointD(3, 6));
            Points.Add(new PointD(4, 8));
            Points.Add(new PointD(6, 5));

            foreach (var item in Points)
            {
                item.Y -= 4.5;

                item.Y *= 50;
                item.X *= 50;

            }
        }

        int PointRadius = 5;

        PointD LERP(PointD p0, PointD p1, double t)
        {
            PointD res = new PointD();
            res.X = (1 - t) * p0.X + t * p1.X;
            res.Y = (1 - t) * p0.Y + t * p1.Y;
            return res;
        }

        double Dist(PointD p1, PointD p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }


        PointD LagInter(List<PointD> pts, double xk)
        {
            double yk = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                double p = 1;
                for (int j = 0; j < pts.Count; j++)
                {
                    if (j != i)
                        p = p * ((xk - pts[j].X) / (pts[i].X - pts[j].X + 0.000001));
                }
                yk = yk + p * pts[i].Y;
            }
            return new PointD(xk, yk);
        }

        List<PointD> Points = new List<PointD>();

        void Draw(Graphics g)
        {
            if (Points.Count > 0)
            {
                double min = Points.Min(i => i.X);
                double max = Points.Max(i => i.X);
                int steps = 100;
                var x = Enumerable.Range(0, steps)
                    .Select(i => min + (max - min) * ((double)i / (steps - 1)))
                    .ToArray();

                PointF[] pts = new PointF[steps];
                for (int i = 0; i < steps; i++)
                {
                    pts[i] = LagInter(Points, x[i]).Round();
                }
                try
                {
                    g.DrawLines(Pens.Blue, pts);

                    foreach (var item in Points)
                    {
                        g.FillEllipse(Brushes.Maroon,
                            (int)item.X - PointRadius,
                            (int)item.Y - PointRadius,
                            2 * PointRadius,
                            2 * PointRadius);
                    }

                    int xk = PointToClient(MousePosition).X;


                    double delta = 0.00001;
                    int len = 100;

                    double val = LagInter(Points, xk + delta).Y;
                    double deriv = (val - LagInter(Points, xk).Y) / (delta);
                    Text = "Интерполяционный многочлен Лагранжа. X: " + Math.Round(xk / 50.0, 4) + ", Derivative: " + Math.Round(deriv, 3);
                    double angle = Math.Atan(deriv);
                    double dy = len * Math.Sin(angle);
                    double dx = len * Math.Cos(angle);
                    g.DrawLine(Pens.Red, (float)(xk - dx), (float)(val - dy), (float)(xk + dx), (float)(val + dy));
                }
                catch { }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dragginObject == null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Points.Add(e.Location);
                }
            }
            else
            {
                dragginObject = null;
            }
        }
        int c = 0;
        int cc = 0;
        Stopwatch sw = Stopwatch.StartNew();

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Draw(e.Graphics);
            ++c;

            if (sw.ElapsedMilliseconds >= 1000)
            {
                cc = c;
                c = 0;
                sw.Restart();
            }
            e.Graphics.DrawString(cc.ToString(), SystemFonts.DefaultFont, Brushes.Red, 0, 0);
            Invalidate();
        }

        PointD dragginObject = null;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (var item in Points)
            {
                if (Dist(item, e.Location) <= PointRadius)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        dragginObject = item;
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        Points.Remove(item);
                    }

                    break;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragginObject != null)
            {
                dragginObject.X = Math.Min(ClientSize.Width, Math.Max(e.Location.X, 0));
                dragginObject.Y = Math.Min(ClientSize.Height, Math.Max(e.Location.Y, 0));
            }
        }
    }
}