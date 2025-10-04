
using System.Reflection;
using System.Text.Json;

namespace math_games.ViewModels
{
    public class LoopingGridsIndexViewModel
    {
        #region public properties
        public string? Instructions { get; set; }
        public int Rows { get; set; } = 50;
        public int Columns { get; set; } = 50;

        public int StartRow { get; set; } = 25;
        public int StartColumn { get; set; } = 25;
        // Hidden field to pass the current grid state as JSON on form submit
        public string? GridStateJson { get; set; }
        public List<List<bool>> Cells { get; set; } = new();
        #endregion

        #region public methods
        public void ResetGrid()
        {
            if (Rows <= 0 || Columns <= 0)
                throw new Exception($"Invalid gridsize:{Rows},{Columns}");

            Cells.Clear();
            for (int r = 0; r < Rows; r++)
            {
                var row = new List<bool>(Columns);
                for (int c = 0; c < Columns; c++)
                {
                    row.Add(false); // start inactive
                }

                Cells.Add(row);
            }

            GridStateJson = JsonSerializer.Serialize(Cells);
        }

        public void ApplyInstructions(List<int> distances)
        {
            int currentRow = StartRow;
            int currentColumn = StartColumn;
            string direction = "left";

            for (int steps = 0; steps < 100; steps++)
            {
                direction = GetNextDirection(direction);
                int distance = distances[steps % distances.Count];

                for (int i = 0; i < distance; i++)
                {
                    switch (direction)
                    {
                        case "up": currentRow -= 1; break;
                        case "right": currentColumn += 1; break;
                        case "down": currentRow += 1; break;
                        case "left": currentColumn -= 1; break;
                    }

                    if (currentRow < Rows && currentColumn < Columns
                        && currentRow > 0 && currentColumn > 0)
                    {
                        Cells[currentRow][currentColumn] = true;
                    }
                }
            }

            GridStateJson = JsonSerializer.Serialize(Cells);
        }
        #endregion

        #region private helper methods
        private string GetNextDirection(string direction)
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
        #endregion

        #region public factory methods
        public static LoopingGridsIndexViewModel CreateDefault()
        {
            var vm = new LoopingGridsIndexViewModel();
            vm.ResetGrid();
            return vm;
        }
        #endregion

    }
}
