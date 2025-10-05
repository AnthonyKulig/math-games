(function(){
    // Cell toggling logic
    const gridEl = document.getElementById('grid');
    if (gridEl) {
        const hidden = document.querySelector('input[name="GridStateJson"]');
        if (hidden) {
            let cells;
            try {
                cells = JSON.parse(hidden.value);
            } catch {
                cells = [];
            }
            function syncHidden() {
                hidden.value = JSON.stringify(cells);
            }
            gridEl.addEventListener('click', function(e) {
                const td = e.target.closest('td.cell');
                if (!td) return;
                const r = parseInt(td.getAttribute('data-r'));
                const c = parseInt(td.getAttribute('data-c'));
                const current = !!cells[r][c];
                cells[r][c] = !current;
                td.classList.toggle('active', !current);
                td.classList.toggle('inactive', current);
                syncHidden();
            });
        }
    }

    // Next Step AJAX logic
    document.addEventListener('DOMContentLoaded', function() {
        var nextStepBtn = document.getElementById('nextStepBtn');
        var playBtn = document.getElementById('playBtn');
        var mainForm = document.getElementById('mainForm');
        var maxItBox = document.getElementById('MaxIterations');
        var enableStepButtons = function() {
            var instructionsStatus = document.getElementById('instructionsStatus');
            var enable = instructionsStatus && instructionsStatus.classList.contains('alert-info');
            if (nextStepBtn) nextStepBtn.disabled = !enable;
            if (playBtn) playBtn.disabled = !enable;
        };
        enableStepButtons();

        // Utility to update ViewModelJson from MaxIterations textbox
        function syncMaxIterationsToJson() {
            var maxItBox = document.getElementById('MaxIterations');
            var jsonField = document.getElementById('ViewModelJson');
            if (maxItBox && jsonField) {
                try {
                    var vm = JSON.parse(jsonField.value);
                    vm.MaxIterations = parseInt(maxItBox.value) || 100;
                    jsonField.value = JSON.stringify(vm);
                } catch {}
            }
        }

        // Highlight the most recently updated cell
        function highlightCurrentCell(vm) {
            // Remove any previous .current class
            document.querySelectorAll('#grid td.current').forEach(td => td.classList.remove('current'));
            // Highlight the current cell if in bounds
            if (vm && typeof vm.CurrentRow === 'number' && typeof vm.CurrentColumn === 'number') {
                var td = document.querySelector(`#grid td[data-r="${vm.CurrentRow}"][data-c="${vm.CurrentColumn}"]`);
                if (td) td.classList.add('current');
            }
        }

        function afterGridUpdate() {
            // Re-enable/disable buttons based on new instructionsStatus
            var instructionsStatus = document.getElementById('instructionsStatus');
            var enable = instructionsStatus && instructionsStatus.classList.contains('alert-info');
            if (nextStepBtn) nextStepBtn.disabled = !enable;
            if (playBtn) playBtn.disabled = !enable;

            // Highlight the current cell
            var newJson = document.getElementById('ViewModelJson');
            if (newJson) {
                try { highlightCurrentCell(JSON.parse(newJson.value)); } catch {}
            }
        }

        if (maxItBox) {
            maxItBox.addEventListener('change', syncMaxIterationsToJson);
            maxItBox.addEventListener('input', syncMaxIterationsToJson);
        }

        mainForm && mainForm.addEventListener('submit', function(e) {
            setTimeout(enableStepButtons, 100);
        });

        // Next Step button logic
        if (nextStepBtn && mainForm) {
            nextStepBtn.addEventListener('click', function (e) {
                e.preventDefault();
                syncMaxIterationsToJson();
                var viewModelJson = document.getElementById('ViewModelJson').value;
                fetch('/LoopingGrids/NextStep', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: viewModelJson
                })
                .then(resp => resp.text())
                .then(html => {
                    document.getElementById('gridContainer').innerHTML = html;
                    var newJson = document.querySelector('#gridContainer #ViewModelJson');
                    if (newJson) {
                        document.getElementById('ViewModelJson').value = newJson.value;
                        try { highlightCurrentCell(JSON.parse(newJson.value)); } catch {}
                    }
                    enableStepButtons();
                });
            });
        }

        // Play button logic
        if (playBtn && mainForm) {
            playBtn.addEventListener('click', async function (e) {
                e.preventDefault();
                syncMaxIterationsToJson();
                var jsonField = document.getElementById('ViewModelJson');
                var vm = JSON.parse(jsonField.value);
                var iterations = parseInt(vm.MaxIterations) || 100;
                playBtn.disabled = true;
                nextStepBtn && (nextStepBtn.disabled = true);
                for (let i = 0; i < iterations; i++) {
                    await new Promise(res => setTimeout(res, 250)); // 0.25s pause
                    var viewModelJson = document.getElementById('ViewModelJson').value;
                    let resp = await fetch('/LoopingGrids/NextStep', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: viewModelJson
                    });
                    let html = await resp.text();
                    document.getElementById('gridContainer').innerHTML = html;
                    var newJson = document.querySelector('#gridContainer #ViewModelJson');
                    if (newJson) {
                        document.getElementById('ViewModelJson').value = newJson.value;
                        try { highlightCurrentCell(JSON.parse(newJson.value)); } catch {}
                    }
                }
                enableStepButtons();
            });
        }

        // Highlight on initial load if possible
        try {
            var jsonField = document.getElementById('ViewModelJson');
            if (jsonField) highlightCurrentCell(JSON.parse(jsonField.value));
        } catch {}
    });
})();
