// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function toastSuccess(data) {
    M.toast({ html: data , classes: "app-toast", displayLength: 7000});
}


function matchColHeight(shortEl, tallEl) {
    shortEl.style.height = tallEl.style.height;
}

function InitMaterializeComponents() {
    var dropdown = document.querySelectorAll('.dropdown-trigger');
    var ddInstances = M.Dropdown.init(dropdown, {});
    var selects = document.querySelectorAll('select:not(.custom-drop-down)');
    var selectInstances = M.FormSelect.init(selects, {});
    var datePicks = document.querySelectorAll('.datepicker');
    var dpInstsances = M.Datepicker.init(datePicks, {});

    var menuBtns = document.querySelectorAll('.fixed-action-btn');
    var menuBtnInstances = M.FloatingActionButton.init(menuBtns, {});
    M.updateTextFields();
}
document.addEventListener('DOMContentLoaded', function () {
    InitMaterializeComponents();
    axios
        .get('/api/Notifications')
        .then(response => {
            if (Object.keys(response.data).length !== 0) {
                
                response.data.forEach(x => toastSuccess(`<ul><li>${x}</li></ul>`));
               
            }
        });

});