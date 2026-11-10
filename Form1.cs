using Microsoft.VisualBasic.ApplicationServices;
using System.Reflection;

namespace Jarvis_march
{
    public partial class Form1 : Form
    {
        private List<PointF> points = new List<PointF>();
        private List<PointF> hull = new List<PointF>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            BuildConvexHull();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            points.Clear();
            hull.Clear();
            Draw();
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                points.Add(new PointF(e.X, e.Y));
                Draw();
            }
            else if (e.Button == MouseButtons.Right && points.Count > 0) 
            {
                int index = 0;
                double best_dist = double.MaxValue;
                for (int i = 0; i < points.Count; i++)
                {
                    double dx = points[i].X - e.X;
                    double dy = points[i].Y - e.Y;
                    double d = dx * dx + dy * dy;
                    if (d < best_dist) { best_dist = d; index = i; }
                }
                points.RemoveAt(index);
                Draw();
            }
        }

        private void BuildConvexHull()
        {
            Console.WriteLine("built");
            hull = JarvisMarch(points);
            Draw();
            
        }

        private List<PointF> JarvisMarch(List<PointF> pts)
        {
            int n = pts.Count;
            var result = new List<PointF>();

            // 1. Находим самую левую нижнюю точку
            int start = 0;
            for (int i = 1; i < n; i++)
            {
                if (pts[i].X < pts[start].X || (pts[i].X == pts[start].X && pts[i].Y < pts[start].Y))
                    start = i;
            }

            int current = start;
            do
            {
                result.Add(pts[current]);
                int next = (current + 1) % n;

                // 2. Ищем следующую точку через сравнение углов
                for (int i = 0; i < n; i++)
                {
                    if (i == current) continue;

                    // Если точка i находится правее (или на одной линии но дальше),
                    // чем кандидат next — обновляем next
                    double cross = (pts[i].X - pts[current].X) * (pts[next].Y - pts[current].Y) -
                                   (pts[i].Y - pts[current].Y) * (pts[next].X - pts[current].X);

                    if (cross > 0 || (cross == 0 &&
                        Math.Abs(pts[i].X - pts[current].X) + Math.Abs(pts[i].Y - pts[current].Y) >
                        Math.Abs(pts[next].X - pts[current].X) + Math.Abs(pts[next].Y - pts[current].Y)))
                    {
                        next = i;
                    }
                }
                current = next;
            } while (current != start);

            return result;
        }

        private void Draw()
        {
            pictureBox.Invalidate();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Рёбра оболочки
            if (hull.Count >= 2)
            {
                using (var pen = new Pen(Color.Red, 2))
                {
                    for (int i = 0; i < hull.Count; i++)
                    {
                        var a = hull[i];
                        var b = hull[(i + 1) % hull.Count];
                        g.DrawLine(pen, a, b);
                    }
                }
            }

            // Обычные точки — чёрные
            using (var brush = new SolidBrush(Color.Black))
            {
                foreach (var p in points)
                {
                    g.FillEllipse(brush, p.X - 2, p.Y - 2, 4, 4);
                }
            }

            // Вершины оболочки — жёлтые с красным контуром
            using (var brush = new SolidBrush(Color.Yellow))
            using (var pen = new Pen(Color.Red, 2))
            {
                foreach (var p in hull)
                {
                    g.FillEllipse(brush, p.X - 5, p.Y - 5, 10, 10);
                    g.DrawEllipse(pen, p.X - 5, p.Y - 5, 10, 10);
                }
            }
        }
     }
}
