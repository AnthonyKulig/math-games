using System.Text.Json;
using math_games.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace math_games.Controllers
{
    public class LoopingGridsController : Controller
    {
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
            // Always start from a clean grid based on posted dimensions
            if (model.Rows <= 0 || model.Columns <= 0)
            {
                ModelState.AddModelError(nameof(model.Rows), "Invalid grid dimensions.");
                model = LoopingGridsIndexViewModel.CreateDefault();
            }
            else
            {
                model.EnsureGrid();
            }

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

            // If instructions valid, apply movement logic
            if (ModelState.IsValid && distances.Count > 0)
            {
                int currentRow = model.StartRow;
                int currentColumn = model.StartColumn;
                string direction = "left";

                // in-bounds guard for starting cell
                if (currentRow >= 0 && currentRow < model.Rows && currentColumn >= 0 && currentColumn < model.Columns)
                {
                    model.Cells[currentRow][currentColumn] = true;
                }

                for (int steps = 0; steps < 100; steps++)
                {
                    direction = GetNextDirection(direction);
                    int distance = distances[steps % distances.Count];

                    for (int i = 0; i < distance; i++)
                    {
                        switch (direction)
                        {
                            case "up":    currentRow -= 1; break;
                            case "right": currentColumn += 1; break;
                            case "down":  currentRow += 1; break;
                            case "left":  currentColumn -= 1; break;
                        }

                        // stop moving if out of bounds
                        if (currentRow < 0 || currentRow >= model.Rows || currentColumn < 0 || currentColumn >= model.Columns)
                            break;

                        model.Cells[currentRow][currentColumn] = true;
                    }
                }
            }

            // Keep hidden field in sync for subsequent renders
            model.GridStateJson = JsonSerializer.Serialize(model.Cells);

            return View(model);
        }

        string GetNextDirection(string direction)
        {
            if (direction == "up")
            {
                return "right";
            }
            else if (direction == "right")
            {
                return "down";
            }
            else if (direction == "down")
            {
                return "left";
            }
            else
            {
                return "up";
            }
        }
    }
}
