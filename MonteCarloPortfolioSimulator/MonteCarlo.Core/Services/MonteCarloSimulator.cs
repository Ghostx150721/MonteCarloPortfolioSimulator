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

            bool inCrash = false; // <-- ADD THIS

            for (int month = 0; month < totalMonths; month++)
            {
                // Chance to ENTER crash
                if (!inCrash && _random.NextDouble() < 0.02) // ~2% per month
                {
                    inCrash = true;
                }

                double monthlyReturn;

                if (inCrash)
                {
                    // Crash regime (bad returns)
                    monthlyReturn = -0.05 + (_random.NextDouble() * 0.02);

                    // Chance to EXIT crash
                    if (_random.NextDouble() < 0.2)
                        inCrash = false;
                }
                else
                {
                    // Normal regime (your existing model)
                    monthlyReturn = GenerateMonthlyReturn(parameters);
                }

                portfolio *= Math.Exp(monthlyReturn);
                portfolio += parameters.MonthlyContribution;
            }

            return portfolio;
        }

        private double GenerateMonthlyReturn(SimulationParameters parameters)
        {
            if (parameters.ModelType == "GBM")
            {
                double meanMonthly = parameters.MeanAnnualReturn / 12;
                double volatilityMonthly = parameters.Volatility / Math.Sqrt(12);

                double u1 = 1.0 - _random.NextDouble();
                double u2 = 1.0 - _random.NextDouble();

                double randStdNormal =
                    Math.Sqrt(-2.0 * Math.Log(u1)) *
                    Math.Cos(2.0 * Math.PI * u2);

                double drift = meanMonthly - 0.5 * volatilityMonthly * volatilityMonthly;
                double shock = volatilityMonthly * randStdNormal;

                return drift + shock;
            }
            else // Real data
            {
                int index = _random.Next(_historicalReturns.Count);
                return _historicalReturns[index];
            }
        }




    }
}
