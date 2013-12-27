using System;
using System.Collections.Generic;

namespace PSOVisualizer
{
    class PSOOptimizer
    {
        public PSOOptimizer(PSOConfiguration config)
        {
            dimensions = config.DataLimits.Count;
            this.config = config;
        }

        private readonly int dimensions=0;
        private readonly PSOConfiguration config;
        private int counter=0;
        private Random randomizer;

        public void Start()
        {
            randomizer = new Random(DateTime.Now.Millisecond);
            particles = new List<List<double>>(config.NumOfParticles);
            for (var i = 0; i < config.NumOfParticles; i++)
            {
                var particle = new List<double>(dimensions);
                for (var j = 0; j < dimensions; j++)
                    particle.Add(randomizer.NextDouble() * (config.DataLimits[j].Stop - config.DataLimits[j].Start) + config.DataLimits[j].Start);
                particles.Add(particle);
            }
        }

        public int GetNumberOfParticles()
        {
            return config.NumOfParticles;
        }

        public void Step()
        {
            var index = 0;
            foreach (var particle in particles)
            {
                for (var i = 0; i < dimensions; i++)
                {
                    var startDiff = Math.Abs(config.DataLimits[i].Start - particle[i]);
                    var stopDiff = Math.Abs(config.DataLimits[i].Stop - particle[i]);
                    var limitsDiffPercent = Math.Abs(config.DataLimits[i].Stop - config.DataLimits[i].Start) / 100;
                    if (startDiff > stopDiff)
                        particle[i] -= limitsDiffPercent * (index % 5);
                    else
                        particle[i] += limitsDiffPercent * (index % 5);
                }
                index++;
            }
            /* Do
             *   For each particle
             *       Calculate fitness value
             *       If the fitness value is better than the best fitness value (pBest) in history
             *           set current value as the new pBest
             *   End For
             *   Choose the particle with the best fitness value of all the particles as the gBest
             *   For each particle
             *       Calculate particle velocity according equation (a)
             *          v[] = v[] + c1 * rand() * (pbest[] - present[]) + c2 * rand() * (gbest[] - present[]) (a)
             *       Update particle position according equation (b)
             *          present[] = present[] + v[] (b)
             *   End
             * While maximum iterations or minimum error criteria is not attained
             */
            counter++;
        }
        
        public Tuple<double,double> Get2DPoint(int particleIndex, int dimensionX, int dimensionY)
        {
            if (dimensionX >= dimensions) dimensionX = dimensions - 1;
            if (dimensionY >= dimensions) dimensionY = dimensions - 1;//TODO: think about odd number of dimensions
            return new Tuple<double, double>
                (
                    GetValueScaled(particleIndex, dimensionX), 
                    GetValueScaled(particleIndex,dimensionY)
                );
        }

        private double GetValueScaled(int particleIndex, int dimension)
        {
            return 
                (particles[particleIndex][dimension] - config.DataLimits[dimension].Start) 
                / 
                (config.DataLimits[dimension].Stop - config.DataLimits[dimension].Start);
        }

        public bool IsDone()
        {
            //While maximum iterations or minimum error criteria is not attained
            return counter > config.BestPositionTimeout;
        }

        private List<List<double>> particles;
        private List<List<double>> particlesBest;
        private List<double> globalBest;
    }

    internal class PSOConfiguration
    {
        public PSOConfiguration()
        {
            SetTestingValues();
        }

        public PSOConfiguration(int dimensions)
        {
            this.dimensions = dimensions;
            DataLimits = new List<RangeDefinition>(dimensions);
            VelocityLimits = new List<RangeDefinition>(dimensions);
        }

        public void AddDataLimit(double start, double stop, string name)
        {
            DataLimits.Add(new RangeDefinition(start, stop, name));
        }

        public void AddTypicalVelocityLimit()
        {
            VelocityLimits.Add(GetTypicalRangeDefinition());
        }
        
        private void SetTestingValues()
        {
            dimensions = 11;
            Spread = 0.00016;
            Pip = Spread/2;
            Omega = 0.85;
            PhiSwarm  = 0.65;
            PhiParticle = 0.45;
            DataLimits = new List<RangeDefinition>
                {
                    new RangeDefinition(-20*Pip, 20*Pip, ""),
                    new RangeDefinition(17, 24, ""),
                    new RangeDefinition(1, 7,""),
                    new RangeDefinition(4, 50,""),
                    new RangeDefinition(8*Spread, 48*Spread,""),
                    new RangeDefinition(8*Spread, 48*Spread,""),
                    new RangeDefinition(100, 500,""),
                    new RangeDefinition(4, 9,""),
                    new RangeDefinition(3, 18,""),
                    new RangeDefinition(5*Spread, 10*Spread,""),
                    new RangeDefinition(3*Spread,15*Spread,"")

                };
            NumOfParticles = 250;
            VelocityLimits  = new List<RangeDefinition>
                {
                    GetTypicalRangeDefinition(), GetTypicalRangeDefinition(), GetTypicalRangeDefinition(), 
                    GetTypicalRangeDefinition(), GetTypicalRangeDefinition(), GetTypicalRangeDefinition(),
                    GetTypicalRangeDefinition(), GetTypicalRangeDefinition(), GetTypicalRangeDefinition(), 
                    GetTypicalRangeDefinition(), GetTypicalRangeDefinition()
                };
            MaxIterNum = 110;
            BestPositionTimeout = 9999999;
        }

        private static RangeDefinition GetTypicalRangeDefinition()
        {
            return new RangeDefinition(0.1, 0.9,"");
        }

        public double Spread { get; set; }
        public double Pip { get; set; }
        public double Omega { get; set; }
        public double PhiSwarm { get; set; }
        public double PhiParticle { get; set; }
        public List<RangeDefinition> DataLimits { get; set; }
        public int NumOfParticles { get; set; }
        public List<RangeDefinition> VelocityLimits { get; set; }
        public double MaxIterNum { get; set; }
        public int BestPositionTimeout { get; set; }
        private int dimensions = 0;
    }
    
    public class RangeDefinition //TODO: maybe Internal Range for other algorithms? (PSO can pass entire Range)
    {
        public RangeDefinition(double start, double stop, double step, string name)
        {
            Start = start;
            Stop = stop;
            Step = step;
            Name = name;
        }

        public string Name {get; set;}

        public RangeDefinition(double start, double stop, string name) : this(start, stop, 1, name) { }
        public double Start { get; set; }
        public double Stop { get; set; }
        public double Step { get; set; }
    }
}
