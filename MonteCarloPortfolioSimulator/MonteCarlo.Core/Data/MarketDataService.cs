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

            var prices = new List<double>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length < 2)
                    continue;

                if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                {
                    prices.Add(price);
                }
            }

            var returns = new List<double>();

            for (int i = 1; i < prices.Count; i++)
            {
                double r = Math.Log(prices[i] / prices[i - 1]); // log return
                returns.Add(r);
            }

            return returns;
        }
    }
}
