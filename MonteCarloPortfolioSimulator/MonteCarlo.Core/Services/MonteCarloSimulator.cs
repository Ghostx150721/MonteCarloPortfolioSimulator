using MonteCarlo.Core.Data;
using MonteCarlo.Core.Models;
using System.Globalization;

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
            var paths = new List<List<double>>(); // ✅ store paths

            for (int i = 0; i < parameters.SimulationCount; i++)
            {
                var path = RunSingleSimulationWithPath(parameters);

                paths.Add(path); // ✅ keep full path
                results.Add(path[^1]); // cleaner than Last()
            }

            var sorted = results.OrderBy(x => x).ToList();
            int count = sorted.Count;

            // ✅ safer percentile calculation
            double p10 = sorted[(int)Math.Floor(0.10 * (count - 1))];
            double p50 = sorted[(int)Math.Floor(0.50 * (count - 1))];
            double p90 = sorted[(int)Math.Floor(0.90 * (count - 1))];

            return new SimulationResult
            {
                FinalPortfolioValues = results,
                Paths = paths, 
                Average = results.Average(),
                Min = sorted.First(),
                Max = sorted.Last(),
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

        private List<double> RunSingleSimulationWithPath(SimulationParameters parameters)
        {
            double portfolio = parameters.InitialInvestment;

            int totalMonths = parameters.Years * 12;

            bool inCrash = false;

            var path = new List<double>();

            for (int month = 0; month < totalMonths; month++)
            {
                if (!inCrash && _random.NextDouble() < 0.02)
                {
                    inCrash = true;
                }

                double monthlyReturn;

                if (inCrash)
                {
                    monthlyReturn = -0.05 + (_random.NextDouble() * 0.02);

                    if (_random.NextDouble() < 0.2)
                        inCrash = false;
                }
                else
                {
                    monthlyReturn = GenerateMonthlyReturn(parameters);
                }

                portfolio *= Math.Exp(monthlyReturn);
                portfolio += parameters.MonthlyContribution;

                path.Add(portfolio);
            }

            return path;
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

        public List<double> LoadMonthlyReturns(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1); // skip header

            var prices = new List<double>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                double close = double.Parse(parts[1], CultureInfo.InvariantCulture);
                prices.Add(close);
            }

            var returns = new List<double>();

            for (int i = 1; i < prices.Count; i++)
            {
                double r = Math.Log(prices[i] / prices[i - 1]);
                returns.Add(r);
            }

            return returns;
        }

    }
}
