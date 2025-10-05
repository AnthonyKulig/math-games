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
            // Only reset if user is submitting new instructions
            model.ResetGrid();
            model.InitializeStepState();
            model.GridStateJson = JsonSerializer.Serialize(model.Cells);
            return View(model);
        }

        [HttpPost]
        public IActionResult NextStep([FromBody] LoopingGridsIndexViewModel model)
        {
            // Do NOT reset grid or state here!
            model.NextStep();
            model.GridStateJson = JsonSerializer.Serialize(model.Cells);
            return PartialView("_GridPartial", model);
        }
    #endregion
    }
}
