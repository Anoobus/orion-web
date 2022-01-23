// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toastSuccess(data) {
    M.toast({ html: data });
}
function toastError(e) {
    M.toast({ html: "couldn't get notifications: " + e.status });
}

function matchColHeight(shortEl, tallEl) {
    console.log(shortEl, tallEl);
    shortEl.style.height = tallEl.style.height;
    console.log(shortEl, tallEl);
}

document.addEventListener('DOMContentLoaded', function () {
    var dropdown = document.querySelectorAll('.dropdown-trigger');
    var ddInstances = M.Dropdown.init(dropdown, {});
    var selects = document.querySelectorAll('select:not(.custom-drop-down)');
    var selectInstances = M.FormSelect.init(selects, {});
    var datePicks = document.querySelectorAll('.datepicker');
    var dpInstsances = M.Datepicker.init(datePicks, {});

    axios
        .get('/api/Notifications')
        .then(response => {
            console.log('got back notification call', response);
            if (Object.keys(response.data).length !== 0) {
                response.data.forEach(x => toastSuccess(x));
            }
        });

});