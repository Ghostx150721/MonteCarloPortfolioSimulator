using Microsoft.AspNetCore.Mvc;
using MonteCarlo.Core.Models;
using MonteCarlo.Core.Services;
using MonteCarlo.Web.Models;
using MonteCarlo.Web.Services;

namespace MonteCarlo.Web.Controllers
{
    public class SimulationController : Controller
    {
        private readonly SimulationApiService _service;

        public SimulationController(SimulationApiService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View(new SimulationInputModel());
        }

        [HttpPost]
        public async Task<IActionResult> Run(SimulationInputModel input)
        {
            var parameters = new SimulationParameters
            {
                InitialInvestment = input.InitialInvestment,
                MonthlyContribution = input.MonthlyContribution,
                MeanAnnualReturn = input.MeanAnnualReturn,
                Volatility = input.Volatility,
                Years = input.Years,
                SimulationCount = input.SimulationCount,
                CrashProbabilityPerYear = input.CrashProbabilityPerYear,
                CrashImpact = input.CrashImpact,
                ModelType = input.ModelType
            };

            var result = await _service.RunSimulation(parameters);

            return View("Result", result);
        }

        [HttpPost]
        public async Task<IActionResult> Compare(SimulationInputModel input)
        {
            // Strategy A
            var paramsA = new SimulationParameters
            {
                InitialInvestment = input.InitialInvestment,
                MonthlyContribution = input.MonthlyContribution,
                Years = input.Years,
                MeanAnnualReturn = input.MeanAnnualReturn,
                Volatility = input.Volatility,
                SimulationCount = input.SimulationCount,
                ModelType = input.ModelType,
                CrashProbabilityPerYear = input.CrashProbabilityPerYear,
                CrashImpact = input.CrashImpact
            };

            // Strategy B
            var paramsB = new SimulationParameters
            {
                InitialInvestment = input.InitialInvestment,
                MonthlyContribution = input.MonthlyContribution * 2,
                Years = input.Years / 2,
                MeanAnnualReturn = input.MeanAnnualReturn,
                Volatility = input.Volatility,
                SimulationCount = input.SimulationCount,
                ModelType = input.ModelType,
                CrashProbabilityPerYear = input.CrashProbabilityPerYear,
                CrashImpact = input.CrashImpact
            };

            var taskA = _service.RunSimulation(paramsA);
            var taskB = _service.RunSimulation(paramsB);

            await Task.WhenAll(taskA, taskB);

            var result = new ComparisonResult
            {
                StrategyA = await taskA,
                StrategyB = await taskB
            };

            return View("CompareResult", result);
        }
    }
}
