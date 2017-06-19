using ICities;

namespace EnhancedDisastersMod
{
    public class EnhancedTornado : EnhancedDisaster
    {
        public EnhancedTornado()
        {
            Type = DisasterType.Tornado;
            CanOccurEverywhere = false;
            OccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
        }
    }
}
