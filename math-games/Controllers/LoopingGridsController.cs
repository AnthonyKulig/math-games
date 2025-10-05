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
            model.GridStateJson = JsonSerializer.Serialize(model.Cells);
            return View(model);
        }

        [HttpPost]
        public IActionResult NextStep([FromBody] JsonElement json)
        {
            var model = JsonSerializer.Deserialize<LoopingGridsIndexViewModel>(json.GetRawText());
            model?.NextStep();
            return PartialView("_GridPartial", model);
        }
        #endregion
    }
}
