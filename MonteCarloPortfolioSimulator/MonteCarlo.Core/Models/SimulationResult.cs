namespace MonteCarlo.Core.Models
{
    public class SimulationResult
    {
        public List<double> FinalPortfolioValues { get; set; } = new();
        public double Average { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
