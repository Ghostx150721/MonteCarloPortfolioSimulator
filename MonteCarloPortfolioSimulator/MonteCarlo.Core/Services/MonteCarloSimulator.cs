using MonteCarlo.Core.Models;

namespace MonteCarlo.Core.Services
{
    public class MonteCarloSimulator
    {
        private readonly Random _random = new();

        public SimulationResult Run(SimulationParameters parameters)
        {
            var results = new List<double>();

            for (int i = 0; i < parameters.SimulationCount; i++)
            {
                double finalValue = RunSingleSimulation(parameters);
                results.Add(finalValue);
            }

            return new SimulationResult
            {
                FinalPortfolioValues = results,
                Average = results.Average(),
                Min = results.Min(),
                Max = results.Max()
            };
        }

        private double RunSingleSimulation(SimulationParameters parameters)
        {
            double portfolio = parameters.InitialInvestment;

            int totalMonths = parameters.Years * 12;

            for (int month = 0; month < totalMonths; month++)
            {
                double monthlyReturns = GenerateMonthlyReturn(parameters);

                portfolio *= Math.Exp(monthlyReturns);

                portfolio += parameters.MonthlyContribution;
            }

            return portfolio;
        }

        private double GenerateMonthlyReturn(SimulationParameters parameters)
        {
            // Convert annual return to monthly
            double meanMonthly = parameters.MeanAnnualReturn / 12;
            double volatilityMonthly = parameters.Volatility / Math.Sqrt(12);

            // Box-Muller transform for normal distribution
            double u1 = 1.0 - _random.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - _random.NextDouble();

            double randStdNormal =
                Math.Sqrt(-2.0 * Math.Log(u1)) *
                Math.Cos(2.0 * Math.PI * u2);

            double randomReturn = meanMonthly + volatilityMonthly * randStdNormal;

            // GBM formula
            double drift = meanMonthly - 0.5 * volatilityMonthly * volatilityMonthly;
            double shock = volatilityMonthly * randStdNormal;

            return drift + shock;
        }
    }
}
