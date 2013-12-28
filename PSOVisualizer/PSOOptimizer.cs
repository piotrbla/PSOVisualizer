using System;
using System.Collections.Generic;
using System.Linq;

namespace PSOVisualizer
{
    class PSOOptimizer
    {
        public PSOOptimizer(PSOConfiguration config)
        {
            dimensions = config.DataLimits.Count;
            this.config = config;
        }

        public void Start()
        {
            randomizer = new Random(DateTime.Now.Millisecond);
            particles = new List<List<double>>(GetNumberOfParticles());
            particlesBest = new List<List<double>>(GetNumberOfParticles());
            particlesVelocities = new List<List<double>>(GetNumberOfParticles());
            particlesBestValue = new List<double>(dimensions);
            globalBest = new List<double>(dimensions);
            for (var i = 0; i < config.NumOfParticles; i++)
            {
                var particle = new List<double>(dimensions);
                var particleBest = new List<double>(dimensions);
                var particleVelocities = new List<double>(dimensions);
                for (var j = 0; j < dimensions; j++)
                {
                    var value = randomizer.NextDouble()*(config.DataLimits[j].Stop - config.DataLimits[j].Start) +
                                config.DataLimits[j].Start;
                    particle.Add(value);
                    particleBest.Add(value);
                    particleVelocities.Add(0);
                }
                particles.Add(particle);
                particlesBest.Add(particleBest);
                particlesVelocities.Add(particleVelocities);
                particlesBestValue.Add(MaxValue);
            }
            for (var j = 0; j < dimensions; j++)
            {
                globalBest.Add(0);
            }
            globalBestValue = MaxValue;
        }

        public int GetNumberOfParticles()
        {
            return config.NumOfParticles;
        }

        public void Step()
        {
            const double phiParticleParameter = 0.7;
            const double phiSwarmParameter = 0.9;
            const double omegaSwarmParameter = 0.85;
            for (var i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];
                var value = CalculateFitnessValue(particle);
                if (value < particlesBestValue[i])
                {
                    particlesBestValue[i] = value;
                    particlesBest[i]= particle.ToList();
                }
                if (value<globalBestValue)
                {
                    globalBestValue = value;
                    globalBest = particle.ToList();
                }
            }
            for (int j = 0; j < particles.Count; j++)
            {
                var particle = particles[j];
                //v[] = v[] + c1 * rand() * (pbest[] - present[]) + c2 * rand() * (gbest[] - present[]) http://www.swarmintelligence.org/tutorials.php
                for (var i = 0; i < dimensions; i++)
                {
                    var limitsDiff = config.DataLimits[i].Stop - config.DataLimits[i].Start;
                    particlesVelocities[j][i] = omegaSwarmParameter * particlesVelocities[j][i] +
                                                phiParticleParameter * randomizer.NextDouble() * (particlesBest[j][i] - particle[i]) +
                                                phiSwarmParameter*randomizer.NextDouble()*(globalBest[i] - particle[i]);
                    if (particlesVelocities[j][i] > maxVelocityPercent * limitsDiff)
                    {
                        particlesVelocities[j][i] = maxVelocityPercent * limitsDiff;
                        SetVelocitiesPenalties();
                    }
                    if (particlesVelocities[j][i] < -maxVelocityPercent * limitsDiff)
                    {
                        particlesVelocities[j][i] = -maxVelocityPercent * limitsDiff;
                        SetVelocitiesPenalties();
                    }

                    particle[i] += particlesVelocities[j][i];
                    if (particle[i] < config.DataLimits[i].Start)
                        particle[i] = config.DataLimits[i].Start;
                    if (particle[i] > config.DataLimits[i].Stop)
                        particle[i] = config.DataLimits[i].Stop;
                }
            }
            counter++;
        }

        private void SetVelocitiesPenalties()
        {
            maxVelocityPercent *= velocityPenaltyPercent;
            if (maxVelocityPercent < 0.1)
                velocityPenaltyPercent = 1.02;
            if (maxVelocityPercent > 0.9)
                velocityPenaltyPercent = 0.98;
        }

        private double CalculateFitnessValue(List<double> particle)
        {
            //testing fitness function
            var errorSum = 0D;
            for (int i = 1; i < 8; i++)
            {
                int j = 0;
                var xSum = 0D;
                foreach (var v in particle)
                {
                    var x = v*Math.Pow(i, j);
                    xSum += x;
                    j++;
                }
                const double spread = 0.00016;
                const double pip = spread / 2;
                j = 0;
                foreach (var d in new List<double> { 5 * pip, 19, 4, 34, 29 * spread, 17 * spread, 234, 6, 12, 7 * spread, 11 * spread })
                {
                    var x = d * Math.Pow(i, j);
                    xSum -= x;
                    j++;
                }
                errorSum += xSum*xSum;
                //0.0004, 19, 4, 34, 0.00464, 0.00272, 234, 6, 12, 0.00112, 0.00176
            }
            return Math.Sqrt(errorSum);
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

        const double MaxValue = 999999;
        private double maxVelocityPercent = 0.99;
        private readonly int dimensions = 0;
        private readonly PSOConfiguration config;
        private int counter = 0;
        private Random randomizer;
        private List<List<double>> particles;
        private List<List<double>> particlesBest;
        private List<List<double>> particlesVelocities;
        private List<double> globalBest;
        private List<double> particlesBestValue;
        private double globalBestValue;
        double velocityPenaltyPercent = 0.98;

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
