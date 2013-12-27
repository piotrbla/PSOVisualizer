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

        private int dimensions=0;
        private PSOConfiguration config;
        private double counter=0;

        public void Start()
        {
            /*
             * For each particle
             *  Initialize particle
             * END
             */
        }
        public void Step()
        {
            /*
             * Do
             *   For each particle
             *       Calculate fitness value
             *       If the fitness value is better than the best fitness value (pBest) in history
             *           set current value as the new pBest
             *   End For
             *   Choose the particle with the best fitness value of all the particles as the gBest
             *   For each particle
             *       Calculate particle velocity according equation (a)
             *       Update particle position according equation (b)
             *   End
             * While maximum iterations or minimum error criteria is not attained
             */
            counter++;
        }
        public bool IsDone()
        {
            //While maximum iterations or minimum error criteria is not attained
            return counter > config.BestPositionTimeout;
        }
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

        private void AddDataLimit(double start, double stop, string name)
        {
            DataLimits.Add(new RangeDefinition(start, stop, name));
        }

        private void AddTypicalVelocityLimit()
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
        public double NumOfParticles { get; set; }
        public List<RangeDefinition> VelocityLimits { get; set; }
        public double MaxIterNum { get; set; }
        public double BestPositionTimeout { get; set; }
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
