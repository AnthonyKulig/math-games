using System.Text.Json;
using math_games.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace math_games.Controllers
{
    public class LoopingGridsController : Controller
    {
        #region public HTTP actions
        [HttpGet]
        public IActionResult Index()
        {
            var vm = LoopingGridsIndexViewModel.CreateDefault();
            vm.GridStateJson = JsonSerializer.Serialize(vm.Cells);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoopingGridsIndexViewModel model)
        {
            model.InitializeStepState();
            return View(model);
        }

        [HttpPost]
        public IActionResult NextStep([FromBody] LoopingGridsIndexViewModel model)
        {
            model.NextStep();
            return PartialView("_GridPartial", model);
        }
    #endregion
    }
}
