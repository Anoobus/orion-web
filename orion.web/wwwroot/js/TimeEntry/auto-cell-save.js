

document.addEventListener('DOMContentLoaded', function () {
    const curWeekId = document.getElementById('current-week-id').value;
    const curEmployeeId = document.getElementById('current-employee-id').value;

    const debouncedSaver = _.debounce(function (event) {
        if (event) {            
            let rowHoldingInput = event.target.parentElement.parentElement.parentElement;
            
            
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
                    console.log(isOverTime,day, txt, x);
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
                .patch('https://localhost:5001/orion-api/v1/employees/' + curEmployeeId + '/time-entry/week/' + curWeekId + '/efforts/' + job + '.' + task,allDays)
                .then(response => {
                    showToast("changes saved");
                    //window.location.reload(true)
                }).catch(function (error) {
                    if (error.response) {
                        // The request was made and the server responded with a status code
                        // that falls out of the range of 2xx
                        console.log(error.response.data);
                        console.log(error.response.status);
                        console.log(error.response.headers);
                        //toastSuccess(error.response.data.errors.title);
                        console.log(error.response.data.error.detail);
                    } else if (error.request) {
                        // The request was made but no response was received
                        // `error.request` is an instance of XMLHttpRequest in the browser and an instance of
                        // http.ClientRequest in node.js
                        console.log(error.request);
                    } else {
                        // Something happened in setting up the request that triggered an Error
                        console.log('Error', error.message);
                    }
                    console.log(error.config);

                });
        //"employees/{employee-id}/time-entry/week/{week-id:int}/efforts/{job-id:int}.{task-id:int}"


        }
        else {
            console.log('no event wtf?');
        }        
    }, 950);
    debouncedSaver();
  
   
   
    document.querySelectorAll('td.time-cell div input[type="text"]').forEach((x, index) => {        
        x.addEventListener('input', (event) => {
            debouncedSaver(event);
        });
    });

    

   
});
