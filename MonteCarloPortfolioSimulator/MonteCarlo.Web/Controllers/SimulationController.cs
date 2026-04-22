using Microsoft.AspNetCore.Mvc;
using MonteCarlo.Core.Models;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Run()
        {
            var parameters = new SimulationParameters
            {
                InitialInvestment = 10000,
                MonthlyContribution = 500,
                MeanAnnualReturn = 0.08,
                Volatility = 0.15,
                Years = 25,
                SimulationCount = 1000,
                CrashProbabilityPerYear = 0.02,
                CrashImpact = -0.4
            };

            var result = await _service.RunSimulation(parameters);

            return View("Result", result);
        }
    }
}
