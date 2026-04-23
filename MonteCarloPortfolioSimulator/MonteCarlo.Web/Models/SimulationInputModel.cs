namespace MonteCarlo.Web.Models
{
    public class SimulationInputModel
    {
        public double InitialInvestment { get; set; } = 10000;
        public double MonthlyContribution { get; set; } = 500;
        public double MeanAnnualReturn { get; set; } = 0.08;
        public double Volatility { get; set; } = 0.15;
        public int Years { get; set; } = 25;
        public int SimulationCount { get; set; } = 1000;
        public double CrashProbabilityPerYear { get; set; } = 0.02;
        public double CrashImpact { get; set; } = -0.4;
        public string ModelType { get; set; } = "Real";
    }
}
