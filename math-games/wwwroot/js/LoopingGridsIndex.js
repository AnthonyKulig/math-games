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
        var mainForm = document.getElementById('mainForm');
        var submitBtn = document.getElementById('submitBtn');
        var instructionsStatus = document.getElementById('instructionsStatus');
        if (nextStepBtn && mainForm) {
            // Enable Next Step if instructions are loaded after postback
            if (instructionsStatus && instructionsStatus.classList.contains('alert-info')) {
                nextStepBtn.disabled = false;
            } else {
                nextStepBtn.disabled = true;
            }

            mainForm.addEventListener('submit', function(e) {
                // Let the form post normally, but after postback, Next Step will be enabled if instructions are valid
                // This is handled by the server-side rendering
            });

            nextStepBtn.addEventListener('click', function (e) {
                e.preventDefault();
                // Defensive: check all required fields exist
                var getVal = function(id) {
                    var el = document.getElementById(id);
                    return el ? el.value : '';
                };
                const data = {
                    Instructions: document.querySelector('[name="Instructions"]')?.value || '',
                    Rows: parseInt(document.querySelector('[name="Rows"]')?.value || '0'),
                    Columns: parseInt(document.querySelector('[name="Columns"]')?.value || '0'),
                    StartRow: parseInt(document.querySelector('[name="StartRow"]')?.value || '0'),
                    StartColumn: parseInt(document.querySelector('[name="StartColumn"]')?.value || '0'),
                    GridStateJson: getVal('GridStateJson'),
                    CurrentRow: parseInt(getVal('CurrentRow') || '0'),
                    CurrentColumn: parseInt(getVal('CurrentColumn') || '0'),
                    CurrentDirection: getVal('CurrentDirection'),
                    CurrentInstructionIndex: parseInt(getVal('CurrentInstructionIndex') || '0'),
                    CurrentStepInInstruction: parseInt(getVal('CurrentStepInInstruction') || '0'),
                    IsInitialized: getVal('IsInitialized') === 'true'
                };
                fetch('/LoopingGrids/NextStep', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(data)
                })
                .then(resp => resp.text())
                .then(html => {
                    document.getElementById('gridContainer').innerHTML = html;
                });
            });
        }
    });

    // Enable Next Step button after page load if instructions are loaded
    document.addEventListener('DOMContentLoaded', function() {
        var nextStepBtn = document.getElementById('nextStepBtn');
        var instructionsStatus = document.getElementById('instructionsStatus');
        if (nextStepBtn && instructionsStatus && instructionsStatus.classList.contains('alert-info')) {
            nextStepBtn.disabled = false;
        }
    });
})();
