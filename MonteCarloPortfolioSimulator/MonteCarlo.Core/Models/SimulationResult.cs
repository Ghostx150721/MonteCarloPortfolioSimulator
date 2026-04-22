namespace MonteCarlo.Core.Models
{
    public class SimulationResult
    {
        public List<double> FinalPortfolioValues { get; set; } = new();
        public double Average { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double P10 { get; set; }
        public double P50 { get; set; }
        public double P90 { get; set; }
    }
}
