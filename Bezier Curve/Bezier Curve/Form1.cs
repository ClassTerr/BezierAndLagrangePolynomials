using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Beizer_Curve
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
			Paint += new PaintEventHandler(Form1_Paint);
            DoubleBuffered = true;

            Random r = new Random();

            for (int i = 0; i < 4; i++)
            {
                BeizerPoints.Add(new PointD(r.Next(0, ClientSize.Width), r.Next(0, ClientSize.Height)));
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

        PointD BeizerCurve(double alpha, List<PointD> pts)
        {
            if (pts.Count == 0 || alpha < 0 || alpha > 1)
                throw new ArgumentException();

            for (int j = 0; j < pts.Count - 1; j++)
            {
                for (int i = 0; i < pts.Count - 1; i++)
                {
                    pts[i] = LERP(pts[i], pts[i + 1], alpha);
                }
            }
            return pts[0];
        }

        List<PointD> BeizerPoints = new List<PointD>();

        void Draw(Graphics g)
        {
            if (BeizerPoints.Count > 0)
            {
                int stepCount = 100;
                Point[] pts = new Point[stepCount + 1];
                for (int i = 0; i <= stepCount; i++)
                {
                    pts[i] = BeizerCurve((double)i / stepCount, BeizerPoints.ToList()).Round();
                }
                g.DrawLines(Pens.Blue, pts);

                foreach (var item in BeizerPoints)
                {
                    g.FillEllipse(Brushes.Maroon,
                        (int)item.X - PointRadius,
                        (int)item.Y - PointRadius,
                        2 * PointRadius,
                        2 * PointRadius);
                }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dragginObject == null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    BeizerPoints.Add(e.Location);
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
            foreach (var item in BeizerPoints)
            {
                if (Dist(item, e.Location) <= PointRadius)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        dragginObject = item;
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        BeizerPoints.Remove(item);
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