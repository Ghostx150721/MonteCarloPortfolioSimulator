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
            var paths = new List<List<double>>(); 
            var drawdowns = new List<double>();

            for (int i = 0; i < parameters.SimulationCount; i++)
            {
                var path = RunSingleSimulationWithPath(parameters);

                paths.Add(path); 
                results.Add(path[^1]);

                double maxDd = CalculateMaxDrawdown(path);
                drawdowns.Add(maxDd);
            }

            var sorted = results.OrderBy(x => x).ToList();
            int count = sorted.Count;

            double p10 = sorted[(int)Math.Floor(0.10 * (count - 1))];
            double p50 = sorted[(int)Math.Floor(0.50 * (count - 1))];
            double p90 = sorted[(int)Math.Floor(0.90 * (count - 1))];

            return new SimulationResult
            {
                FinalPortfolioValues = results,
                Paths = paths,
                MaxDrawdowns = drawdowns,
                Average = results.Average(),
                Min = sorted.First(),
                Max = sorted.Last(),
                P10 = p10,
                P50 = p50,
                P90 = p90
            };
        }

        private List<double> RunSingleSimulationWithPath(SimulationParameters parameters)
        {
            double portfolio = parameters.InitialInvestment;

            int totalMonths = parameters.Years * 12;

            bool inCrash = false;

            var path = new List<double>();

            double monthlyCrashProb = parameters.CrashProbabilityPerYear / 12.0;

            for (int month = 0; month < totalMonths; month++)
            {
                if (!inCrash && _random.NextDouble() < monthlyCrashProb)
                {
                    inCrash = true;
                }

                double monthlyReturn;

                if (inCrash)
                {
                    // Convert yearly crash impact into monthly effect.............
                    double crashMonthly = parameters.CrashImpact / 6.0;

                    // Add some randomness around it
                    monthlyReturn = crashMonthly + (_random.NextDouble() * 0.02 - 0.01);

                    // Exit probability (still fine)
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

        private double CalculateMaxDrawdown(List<double> path)
        {
            double peak = path[0];
            double maxDrawdown = 0;

            foreach (var value in path)
            {
                if (value > peak)
                    peak = value;

                double drawdown = (value - peak) / peak;

                if (drawdown < maxDrawdown)
                    maxDrawdown = drawdown;
            }

            return maxDrawdown;
        }


    }
}
