document.addEventListener('DOMContentLoaded', function () {
    const curWeekId = document.getElementById('current-week-id').value;
    const curEmployeeId = document.getElementById('current-employee-id').value;
    document.querySelectorAll('a.action-remove-row').forEach((link, index) => {
        let presetJobTaskConfig = link.attributes['row'].value;
        let job = presetJobTaskConfig.substring(0, presetJobTaskConfig.indexOf('.'));
        let task = presetJobTaskConfig.substring(job.length + 1);
        link.onclick = (e) => {            
            effortRepository.removeEffortFor(curEmployeeId, curWeekId, job, task, toastSuccess);
        }        
    });
});

