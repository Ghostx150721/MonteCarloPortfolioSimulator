using Microsoft.AspNetCore.Mvc;
using MonteCarlo.Core.Models;
using MonteCarlo.Core.Services;

namespace MonteCarlo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly MonteCarloSimulator _simulator;

        public SimulationController()
        {
            _simulator = new MonteCarloSimulator();
        }

        [HttpPost("run")]
        public ActionResult<SimulationResult> RunSimulation([FromBody] SimulationParameters parameters)
        {
            var result = _simulator.Run(parameters);
            return Ok(result);
        }
    }
}
