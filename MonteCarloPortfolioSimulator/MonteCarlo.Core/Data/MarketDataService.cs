using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo.Core.Data
{
    public class MarketDataService
    {
        public List<double> LoadMonthlyReturns(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1);

            var data = new List<(DateTime date, double price)>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length < 2)
                    continue;

                if (DateTime.TryParse(parts[0], out var date) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double price) &&
                    price > 0)
                {
                    data.Add((date, price));
                }
            }

            var ordered = data.OrderBy(x => x.date).ToList();

            var returns = new List<double>();

            for (int i = 1; i < ordered.Count; i++)
            {
                double r = Math.Log(ordered[i].price / ordered[i - 1].price);
                returns.Add(r);
            }

            return returns;
        }
    }
}
