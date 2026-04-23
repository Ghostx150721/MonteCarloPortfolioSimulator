namespace MonteCarlo.Core.Models
{
    public class SimulationParameters
    {
        public double InitialInvestment { get; set; }
        public double MonthlyContribution { get; set; }
        public double MeanAnnualReturn { get; set; } // e.g. 0.08
        public double Volatility { get; set; } // e.g. 0.15
        public int Years { get; set; }
        public int SimulationCount { get; set; }
        public double CrashProbabilityPerYear { get; set; } // e.g. 0.02 (2%)
        public double CrashImpact { get; set; } // e.g. -0.4 (-40%)
        public string ModelType { get; set; } = "Real";
    }
}
