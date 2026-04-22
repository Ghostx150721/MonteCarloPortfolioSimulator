using MonteCarlo.Core.Models;
using MonteCarlo.Core.Data;

namespace MonteCarlo.Core.Services
{
    public class MonteCarloSimulator
    {
        private readonly Random _random = new();
        private readonly List<double> _historicalReturns;

        public MonteCarloSimulator()
        {
            var dataService = new MarketDataService();

            _historicalReturns = dataService.LoadMonthlyReturns(Path.Combine(AppContext.BaseDirectory, "Data", "sp500.csv"));
        }

        public SimulationResult Run(SimulationParameters parameters)
        {
            var results = new List<double>();

            for (int i = 0; i < parameters.SimulationCount; i++)
            {
                double finalValue = RunSingleSimulation(parameters);
                results.Add(finalValue);
            }

            var sorted = results.OrderBy(x => x).ToList();

            int count = sorted.Count;

            double p10 = sorted[(int)(0.10 * count)];
            double p50 = sorted[(int)(0.50 * count)];
            double p90 = sorted[(int)(0.90 * count)];

            return new SimulationResult
            {
                FinalPortfolioValues = results,
                Average = results.Average(),
                Min = results.Min(),
                Max = results.Max(),
                P10 = p10,
                P50 = p50,
                P90 = p90
            };
        }

        private double RunSingleSimulation(SimulationParameters parameters)
        {
            double portfolio = parameters.InitialInvestment;

            int totalMonths = parameters.Years * 12;

            for (int month = 0; month < totalMonths; month++)
            {
                double monthlyReturns = GenerateMonthlyReturn(parameters);

                double monthlyCrashProbability = parameters.CrashProbabilityPerYear / 12;

                //if (_random.NextDouble() < monthlyCrashProbability)
                //{
                //    monthlyReturns += parameters.CrashImpact;
                //}

                portfolio *= Math.Exp(monthlyReturns);

                portfolio += parameters.MonthlyContribution;
            }

            return portfolio;
        }

        private double GenerateMonthlyReturn(SimulationParameters parameters)
        {
            int index = _random.Next(_historicalReturns.Count);
            return _historicalReturns[index];
        }
    }
}
