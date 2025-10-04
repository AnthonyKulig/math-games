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
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoopingGridsIndexViewModel model)
        {
            List<int> distances = ParseInstructions(model);

            if (ModelState.IsValid && distances.Count > 0)
            {
                model.ResetGrid();
                model.ApplyInstructions(distances);
            }

            return View(model);
        }
        #endregion

        #region private helper methods
        private List<int> ParseInstructions(LoopingGridsIndexViewModel model)
        {
            // Validate and parse instructions: list of numbers separated by commas
            var distances = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.Instructions))
            {
                var parts = model.Instructions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var p in parts)
                {
                    if (int.TryParse(p, out var n))
                    {
                        distances.Add(n);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Instructions), "Instructions must be a list of integers separated by commas.");
                        break;
                    }
                }
            }
            else
            {
                ModelState.AddModelError(nameof(model.Instructions), "Instructions must be provided as a list of integers separated by commas.");
            }

            return distances;
        }
        #endregion
    }
}
