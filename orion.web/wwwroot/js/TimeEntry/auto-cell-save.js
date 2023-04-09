

document.addEventListener('DOMContentLoaded', function () {
    const curWeekId = document.getElementById('current-week-id').value;
    const curEmployeeId = document.getElementById('current-employee-id').value;

    const debouncedSaver = _.debounce(function (event) {
        if (event) {

            let rowHoldingInput = event.target.parentElement.parentElement.parentElement;
            rowHoldingInput.querySelectorAll('td input[type="text"]').forEach(txtInput => {
                if (txtInput.value == "") {
                    txtInput.value = "0";
                }
            });
            let form = $("#main-time-entry-form");
            let validator = form.validate();


            if (form.valid()) {

                console.log(rowHoldingInput.attributes['effort-id'].value);
                let effortRaw = rowHoldingInput.attributes['effort-id'].value;
                let job = effortRaw.substring(0, effortRaw.indexOf('.'));
                let task = effortRaw.substring(job.length + 1);
                let allDays = {};
                for (let x of rowHoldingInput.children) {
                    if (x.classList.contains('time-cell')) {

                        let txt = x.children[0].children[0];
                        let isOverTime = x.attributes['day'].value.startsWith('ot-');
                        let day = x.attributes['day'].value.replace('ot-', '').replace('col-', '');
                        console.log(isOverTime, day, txt, x);
                        if (!allDays[day]) {
                            allDays[day] = {
                                overtimeHours: 0,
                                hours: 0
                            };
                        }
                        if (isOverTime) {
                            allDays[day].overtimeHours = Number(txt.value);
                        }
                        else {
                            allDays[day].hours = Number(txt.value);
                        }
                    }
                }



                axios
                    .patch('https://localhost:5001/orion-api/v1/employees/' + curEmployeeId + '/time-entry/week/' + curWeekId + '/efforts/' + job + '.' + task, allDays)
                    .then(response => {
                        console.log(response);
                        toastSuccess("changes saved");
                        $('div.week-errors').removeClass('validation-summary-errors');
                        $('div.week-errors').addClass('validation-summary-valid');
                        form.resetValidation();
                    }).catch(function (error) {

                        console.log('Error on call to save time', error);
                        $('div.week-errors ul').children().remove();
                        let toastContent = `<ul><li>${error.response.data.errors.title}</li><li>Changes where not saved!</li></ul>`;
                        $('div.week-errors ul').append(`<li>${error.response.data.errors.detail}</li>`)
                        $('div.week-errors').removeClass('validation-summary-valid');
                        $('div.week-errors').addClass('validation-summary-errors');
                        toastSuccess(toastContent);
                    });
            }
        }
    }, 950);
    debouncedSaver();



    document.querySelectorAll('td.time-cell div input[type="text"]').forEach((x, index) => {
        x.addEventListener('input', (event) => {
            debouncedSaver(event);
        });
    });




});
