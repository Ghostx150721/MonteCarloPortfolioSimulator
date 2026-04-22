using Microsoft.AspNetCore.Mvc;
using MonteCarlo.Core.Models;
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
                CrashImpact = input.CrashImpact
            };

            var result = await _service.RunSimulation(parameters);

            return View("Result", result);
        }
    }
}
