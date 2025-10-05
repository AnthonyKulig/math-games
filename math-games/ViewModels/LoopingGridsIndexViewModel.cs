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

        #region state for step-by-step execution
        public int CurrentRow { get; set; } = 25;
        public int CurrentColumn { get; set; } = 25;
        public string CurrentDirection { get; set; } = "left";
        public int CurrentInstructionIndex { get; set; } = 0;
        public int CurrentStepInInstruction { get; set; } = 0;
        public List<int> ParsedInstructions { get; set; } = new();
        public bool IsInitialized { get; set; } = false;
        public int Steps { get; set; } = 1;
        #endregion

        #region public methods
        public void InitializeStepState()
        {
            ResetInstructions();
            CurrentRow = StartRow;
            CurrentColumn = StartColumn;
            CurrentDirection = "left";
            CurrentInstructionIndex = 0;
            CurrentStepInInstruction = 0;
            IsInitialized = true;
            ResetGrid();
            GridStateJson = JsonSerializer.Serialize(Cells);
        }

        public bool NextStep()
        {
            RefreshCellsFromJson();
            if (ParsedInstructions == null || ParsedInstructions.Count == 0)
                ResetInstructions();
            ApplyOneStep();
            return true;
        }

        public void ApplyInstructions(List<int> distances)
        {
            int currentRow = StartRow;
            int currentColumn = StartColumn;
            string direction = "left";

            for (Steps = 1; Steps <= 100; Steps++)
            {
                ApplyOneStep();
            }

            GridStateJson = JsonSerializer.Serialize(Cells);
        }
        #endregion

        #region private helper methods
        private void ResetInstructions()
        {
            ParsedInstructions.Clear();
            if (!string.IsNullOrWhiteSpace(Instructions))
            {
                var parts = Instructions.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
                foreach (var p in parts)
                {
                    if (int.TryParse(p, out var n))
                        ParsedInstructions.Add(n);
                }
            }
        }

        private void RefreshCellsFromJson()
        {
            if (!string.IsNullOrWhiteSpace(GridStateJson))
            {
                List<List<bool>> parsed = JsonSerializer.Deserialize<List<List<bool>>>(GridStateJson);

                if (parsed is not null)
                    Cells = parsed;
            }
        }

        private void ApplyOneStep()
        {
            if (ParsedInstructions == null || ParsedInstructions.Count == 0)
                return;

            // Get current distance
            int distance = ParsedInstructions[CurrentInstructionIndex % ParsedInstructions.Count];

            RefreshCellsFromJson();

            // Move one step in the current direction
            switch (CurrentDirection)
            {
                case "up": CurrentRow -= 1; break;
                case "right": CurrentColumn += 1; break;
                case "down": CurrentRow += 1; break;
                case "left": CurrentColumn -= 1; break;
            }

            if (CurrentRow >= 0 && CurrentRow < Rows && CurrentColumn >= 0 && CurrentColumn < Columns)
            {
                Cells[CurrentRow][CurrentColumn] = true;
            }

            CurrentStepInInstruction++;
            if (CurrentStepInInstruction >= distance)
            {
                // Move to next instruction and direction
                CurrentStepInInstruction = 0;
                CurrentInstructionIndex++;
                CurrentDirection = GetNextDirection(CurrentDirection);
            }

            GridStateJson = JsonSerializer.Serialize(Cells);
        }

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

        private void ResetGrid()
        {
            if (Rows <= 0)
                Rows = 1;
            if (Columns <= 0)
                Columns = 1;

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
