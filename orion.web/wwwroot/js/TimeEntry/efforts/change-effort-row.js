document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('a.action-change-row').forEach((link, index) => {
        
        const triggerId = link.attributes['id'].value;
        const presetJobTaskConfig = triggerId.replace("row-", "");
        AddClickTrigger(presetJobTaskConfig, triggerId, 'add-row-modal', () => console.log('onPersist Called from change-row on ' + presetJobTaskConfig),M);
    });
});