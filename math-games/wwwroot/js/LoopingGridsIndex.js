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
        if (nextStepBtn && mainForm) {
            // Enable Next Step if instructions are loaded after postback
            var enableNextStepIfLoaded = function() {
                var instructionsStatus = document.getElementById('instructionsStatus');
                if (instructionsStatus && instructionsStatus.classList.contains('alert-info')) {
                    nextStepBtn.disabled = false;
                } else {
                    nextStepBtn.disabled = true;
                }
            };
            enableNextStepIfLoaded();

            mainForm.addEventListener('submit', function(e) {
                // Let the form post normally, but after postback, Next Step will be enabled if instructions are valid
                setTimeout(enableNextStepIfLoaded, 100); // re-check after postback
            });

            nextStepBtn.addEventListener('click', function (e) {
                e.preventDefault();
                var viewModelJson = document.getElementById('ViewModelJson').value;
                fetch('/LoopingGrids/NextStep', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: viewModelJson
                })
                .then(resp => resp.text())
                .then(html => {
                    document.getElementById('gridContainer').innerHTML = html;
                    // Enable Next Step if instructions are loaded in the new partial
                    var instructionsStatus = document.getElementById('instructionsStatus');
                    if (instructionsStatus && instructionsStatus.classList.contains('alert-info')) {
                        nextStepBtn.disabled = false;
                    } else {
                        nextStepBtn.disabled = true;
                    }
                    // Update the main form's hidden field with the new JSON from the partial
                    var newJson = document.querySelector('#gridContainer #ViewModelJson');
                    if (newJson) {
                        document.getElementById('ViewModelJson').value = newJson.value;
                    }
                });
            });
        }
    });
})();
