(function(){
    const gridEl = document.getElementById('grid');
    if (!gridEl) return;
    const hidden = document.querySelector('input[name="GridStateJson"]');
    if (!hidden) return;

    try {
        var cells = JSON.parse(hidden.value);
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
})();
