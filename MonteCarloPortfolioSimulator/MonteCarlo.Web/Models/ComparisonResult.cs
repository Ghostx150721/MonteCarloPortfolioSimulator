using MonteCarlo.Core.Models;

namespace MonteCarlo.Web.Models
{
    public class ComparisonResult
    {
        public SimulationResult StrategyA { get; set; }
        public SimulationResult StrategyB { get; set; }
    }
}
