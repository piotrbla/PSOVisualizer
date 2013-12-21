using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace PSOVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            randomizer = new Random(DateTime.Now.Millisecond);
            InitializeComponent();
        }

        private Graphics g;
        private int x;
        private int y;
        private int direction = -1;
        private readonly Random randomizer;
        private readonly Pen pen = new Pen(Color.Black);
        readonly List<Tuple<int, int>> points = new List<Tuple<int, int>>();

        private void Form1_Load(object sender, EventArgs e)
        {
            g = pictureBox1.CreateGraphics();
            timer1.Interval =2;
            timer1.Start();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            g.DrawLine(pen, 10, 10, 1234, 1000 );
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            direction = randomizer.Next(5) < 4 ? 1 : -1;
            x += randomizer.Next(3) * direction;
            direction = randomizer.Next(5) < 4 ? 1 : -1;
            y += randomizer.Next(3) * direction;
            points.Add(new Tuple<int, int>(x,y));

            if (points.Count % 50 == 0 )
                g.Clear(Color.Beige);
            g.DrawString(points.Count.ToString(),new Font(new FontFamily(GenericFontFamilies.SansSerif),12), new SolidBrush(Color.Black), 500, 100);
            foreach (var point in points)
            {
                g.DrawLine(pen, point.Item1, point.Item2, point.Item1+2, point.Item2+2 );
            }
        }


    }
}
