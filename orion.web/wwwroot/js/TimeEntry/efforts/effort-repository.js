document.addEventListener('DOMContentLoaded', function () {

    const effortRepository = (function (axios, showToast) {

        function saveNewEffortFor(curEmployeeId, curWeekId, jobId, taskId, onError) {
            axios
                .post('https://localhost:5001/orion-api/v1/employees/' + curEmployeeId + '/time-entry/week/' + curWeekId + '/efforts',
                    {
                        selectedTaskId: taskId,
                        selectedJobId: jobId,
                    })
                .then(response => {
                    showToast("New effort added!");
                    window.location.reload(true)
                }).catch(function (error) {
                    if (error.response) {
                        // The request was made and the server responded with a status code
                        // that falls out of the range of 2xx
                        console.log(error.response.data);
                        console.log(error.response.status);
                        console.log(error.response.headers);
                        //toastSuccess(error.response.data.errors.title);
                        onError(error.response.data.errors.detail);

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
        }
        function removeEffortFor(curEmployeeId, curWeekId, jobId, taskId,  onError) {
            axios.delete('https://localhost:5001/orion-api/v1/employees/' + curEmployeeId + '/time-entry/week/' + curWeekId + '/efforts/' +  jobId  + '.' +  taskId )
                .then(response => {
                    showToast("Effort removed");                    
                    window.location.reload(true)
                }).catch(function (error) {
                    if (error.response) {
                        // The request was made and the server responded with a status code
                        // that falls out of the range of 2xx
                        console.log(error.response.data);
                        console.log(error.response.status);
                        console.log(error.response.headers);

                        onError(error.response.data.errors.detail);

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
        }
        function changeEffort(curEmployeeId, curWeekId, oldJobId, oldTaskId, newJobId, newTaskId, onError) {
            axios.post('https://localhost:5001/orion-api/v1/employees/' + curEmployeeId + '/time-entry/week/' + curWeekId + '/efforts/switch',
                {
                    newTaskId,
                    oldTaskId,
                    newJobId,
                    oldJobId
                }            )
                .then(response => {
                    showToast("Effort modified");
                    window.location.reload(true)
                }).catch(function (error) {
                    if (error.response) {
                        // The request was made and the server responded with a status code
                        // that falls out of the range of 2xx
                        console.log(error.response.data);
                        console.log(error.response.status);
                        console.log(error.response.headers);

                        onError(error.response.data.errors.detail);

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
        }

        return {
            saveNewEffortFor: saveNewEffortFor,
            removeEffortFor: removeEffortFor,
            changeEffort: changeEffort

        }
    })(axios, toastSuccess);
    
    window.effortRepository = effortRepository;
});

