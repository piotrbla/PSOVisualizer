using System;
using System.Collections.Generic;
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

        private const int XFieldWidth = 530;
        private const int YFieldWidth = 530;
        private const int BorderFieldWidth = 50;
        private int x;
        private int y;
        private readonly Random randomizer;
        readonly List<Tuple<int, int>> points = new List<Tuple<int, int>>();

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval =50;
            timer1.Start();
            doubleBufferControl1.SetPoints(points);
        }

        private static Tuple<int, int> GetRangeStart(int rangeNumber)
        {
            var xStart = (rangeNumber % 3) * XFieldWidth + (rangeNumber % 3) * BorderFieldWidth ;
            var yStart = (rangeNumber / 3) * YFieldWidth + (rangeNumber / 3) * BorderFieldWidth;
            return new Tuple<int, int>(xStart, yStart);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            doubleBufferControl1.Invalidate();

            points.Clear();
            for (int i = 0; i < 6; i++)
            {
                var rangeStart = GetRangeStart(i);
                for (int j = 0; j < 6000; j++)
                {
                    x = randomizer.Next(XFieldWidth);
                    y = randomizer.Next(YFieldWidth);
                    points.Add(new Tuple<int, int>(x + rangeStart.Item1, y + rangeStart.Item2));
                }
                
            }
        }


    }
}
