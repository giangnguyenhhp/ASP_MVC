using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP_MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP_MVC.Controllers
{
    [Route("he-mat-troi/[action]")]
    public class PlanetController : Controller
    {
        private readonly PlanetService _planetService;
        private readonly ILogger<PlanetController> _logger;

        public PlanetController(PlanetService service, ILogger<PlanetController> logger)
        {
            _planetService = service;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //route : action
        [BindProperty(SupportsGet = true, Name = "action")]
        public string Name { get; set; } // NameAction ~ NamePlanetModel

        public IActionResult Mercury()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        public IActionResult Venus()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        public IActionResult Earth()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        public IActionResult Mars()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        [HttpGet("/sao-moc.html")]
        public IActionResult Jupiter()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        public IActionResult Saturn()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        public IActionResult Uranus()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        [Route("[controller]-[action]", Order = 3,Name = "neptune3")]
        [Route("sao/[action]", Order = 2,Name="neptune2")]
        [Route("sao/[controller]/[action]", Order = 1,Name = "neptune1")]
        public IActionResult Neptune()
        {
            var planet = _planetService.FirstOrDefault(x => x.Name == Name);
            return View("Details", planet);
        }

        [Route("thong-tin-hanh-tinh/{id:int}")]
        public IActionResult PlanetInfo(int id)
        {
            var planet = _planetService.FirstOrDefault(x => x.Id == id);
            return View("Details", planet);
        }
    }
}