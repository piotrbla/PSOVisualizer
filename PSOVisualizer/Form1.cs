using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PSOVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval =50;
            timer1.Start();
            doubleBufferControl1.SetPoints(points);
            for (var i = 0; i < 6; i++)
            {
                var rangeStart = GetRangeStart(i);
                borders.Add(new Tuple<int, int, int, int>(rangeStart.Item1, rangeStart.Item2, rangeStart.Item1 + XFieldWidth, rangeStart.Item2));
                borders.Add(new Tuple<int, int, int, int>(rangeStart.Item1, rangeStart.Item2, rangeStart.Item1, rangeStart.Item2 + YFieldWidth));
                borders.Add(new Tuple<int, int, int, int>(rangeStart.Item1 + XFieldWidth, rangeStart.Item2, rangeStart.Item1 + XFieldWidth, rangeStart.Item2 + YFieldWidth));
                borders.Add(new Tuple<int, int, int, int>(rangeStart.Item1, rangeStart.Item2 + YFieldWidth, rangeStart.Item1 + XFieldWidth, rangeStart.Item2 + YFieldWidth));
            }
            doubleBufferControl1.SetBorders(borders);
            CheckNotSoSimplePSO();
        }

        private void CheckNotSoSimplePSO()
        {
            const double spread = 0.00016;
            const double pip = spread/2;
            var configuration = new PSOConfiguration(11) {Spread = spread, BestPositionTimeout = 1000, NumOfParticles = 6000};
            configuration.AddDataLimit(-20*pip, 20*pip, "");
            configuration.AddDataLimit(17, 24, "");
            configuration.AddDataLimit(1, 7, "");
            configuration.AddDataLimit(4, 50, "");
            configuration.AddDataLimit(8*spread, 48*spread, "");
            configuration.AddDataLimit(8*spread, 48*spread, "");
            configuration.AddDataLimit(100, 500, "");
            configuration.AddDataLimit(4, 9, "");
            configuration.AddDataLimit(3, 18, "");
            configuration.AddDataLimit(5*spread, 10*spread, "");
            configuration.AddDataLimit(3*spread, 15*spread, "");

            optimizer = new PSOOptimizer(configuration);//TODO: Add fitness function (ie strategy definition)
            optimizer.Start();
        }

        private static Tuple<int, int> GetRangeStart(int rangeNumber)
        {
            var xStart = (rangeNumber % 3) * XFieldWidth + (rangeNumber % 3) * BorderFieldWidth ;
            var yStart = (rangeNumber / 3) * YFieldWidth + (rangeNumber / 3) * BorderFieldWidth;
            return new Tuple<int, int>(xStart, yStart);
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(timerWorks)
                return;
            timerWorks = true;
            doubleBufferControl1.Invalidate();

            points.Clear();
            for (var i = 0; i < 6; i++)
            {
                var rangeStart = GetRangeStart(i);
                for (var j = 0; j < optimizer.GetNumberOfParticles(); j++)
                {
                    var point = optimizer.Get2DPoint(j, i*2, i*2 + 1);
                    points.Add(new Tuple<int, int>(
                        (int) (point.Item1 * XFieldWidth + rangeStart.Item1), 
                        (int) (point.Item2 * YFieldWidth + rangeStart.Item2))
                    );
                }
                
            }
            optimizer.Step();
            if (optimizer.IsDone())
                timer1.Stop();
            timerWorks = false;
        }

        private const int XFieldWidth = 530;
        private const int YFieldWidth = 530;
        private const int BorderFieldWidth = 50;
        private PSOOptimizer optimizer;
        private bool timerWorks = false;
        readonly List<Tuple<int, int>> points = new List<Tuple<int, int>>();
        readonly List<Tuple<int, int, int, int>> borders = new List<Tuple<int, int, int, int>>();


    }
}
