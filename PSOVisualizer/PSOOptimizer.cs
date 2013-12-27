namespace PSOVisualizer
{
    class PSOOptimizer
    {
        public PSOOptimizer(int dimensions)
        {
            this.dimensions = dimensions;
        }

/*
 * For each particle
 *  Initialize particle
 * END

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
        private int dimensions=0;
    }
}
