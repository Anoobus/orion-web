let modalData = {}

 const AddClickTrigger = function (startingValue, triggerId, modalId, onPersistChanges, materialize) {
     console.log("AddClickTrigger to " + modalId + " when " + triggerId + " is clicked.", materialize);
        document.getElementById(triggerId).onclick = (e) => {
            var modalFinds = document.getElementById(modalId);
            e.preventDefault();
            onShowModalClick(modalFinds, startingValue, onPersistChanges, materialize);            
        };
 };

document.addEventListener('DOMContentLoaded', function () {
    modalData = initializeFieldsOnDocLoad(M);   
});

function initializeFieldsOnDocLoad(materialize) {
    //category options
    const internalOptions = document.querySelectorAll('select.custom-drop-down.category option[internal-only="true"]');
    const otherOptions = document.querySelectorAll('select.custom-drop-down.category option[internal-only="false"]');

    //task options
    let taskOptions = document.querySelectorAll('select.custom-drop-down.task option');
    let optionGroup = {};
    taskOptions.forEach((x, index) => {

        let taskGroup = x.attributes['class'].value;
        if (!optionGroup.hasOwnProperty(taskGroup)) {
            optionGroup[taskGroup] = [];
        }
        optionGroup[taskGroup].push(x);
    })

    //job options
    const jobOptions = document.querySelectorAll('select.custom-drop-down.job option');
    const curWeekId = document.getElementById('current-week-id').value;
    const curEmployeeId = document.getElementById('current-employee-id').value;
    let jobDropDown = document.getElementById('NewEntry_SelectedJobId');
    let taskDropDown = document.getElementById("NewEntry_SelectedTaskId");

    const okBtnId = 'ModalSubmit';
    let jobSelection = modalDropDown('NewEntry_SelectedJobId', materialize, okBtnId, (e) => onJobChange(e, internalOptions, otherOptions));
    let categorySelection = modalDropDown('NewEntry_SelectedTaskCategory', materialize, okBtnId, (e) => onCategoryChange(e, optionGroup));
    let taskSelection = modalDropDown('NewEntry_SelectedTaskId', materialize, okBtnId, (e) => onTaskChange(e));

    categorySelection.setDisabled();
    taskSelection.setDisabled();
    jobSelection.setEnabled(jobOptions, "Select Job");

    let submitBtn = document.getElementById(okBtnId);
    submitBtn.style.display = "none";



    let modalNotificationBlock = document.getElementById('notification-block');
    let notificationContent = document.getElementById('notification-block-content');
    function setNotificationBlockContent(content) {
        notificationContent.innerHTML = content;
        if (content == undefined || content == null || content == "") {
            modalNotificationBlock.style.display = "none";
        }
        else {
            modalNotificationBlock.style.display = "block";
            setTimeout(() => {
                console.log('remove pulse');
                notificationContent.classList.remove('pulse');
                console.log('pulse removed');
            }, 2500);
        }

    }

    function onJobChange(event, internalCategoryOptions, otherCategoryOptions) {
        const target = event.target;
        const selectedVal = target.value;
        document.getElementById('ModalSubmit').style.display = "none";
        if (selectedVal == "preset") {
            modalData.categorySelect.setDisabled();
            modalData.taskSelect.setDisabled();
        }
        else {
            let internalOnly = selectedVal == 1004;
            let optionSet = internalOnly ? internalCategoryOptions : otherCategoryOptions;
            modalData.categorySelect.setEnabled(optionSet, "Select Category", null);
            modalData.taskSelect.setDisabled();
        }
    };
    function onCategoryChange(event, groupedTaskOptions) {

        console.log('catagory change detected!', event);
        const selectedVal = event.target.value;
        document.getElementById('ModalSubmit').style.display = "none";
        if (selectedVal == "preset") {
            taskSelection.setDisabled();
        }
        else {
            let thisCatsOptions = groupedTaskOptions[selectedVal]
            taskSelection.setEnabled(thisCatsOptions, "Select Task", null);
        }
    }
    function onTaskChange(event) {
        console.log('task change detected!', event);
        const selectedVal = event.target.value;

        if (selectedVal == "preset") {
            document.getElementById('ModalSubmit').style.display = "none";
        }
        else {
            document.getElementById('ModalSubmit').style.display = "inline-block";
        }
    }


    function initializeModalForJobTaskCombo(jobId, taskId) {
        jobSelection.setEnabled(jobOptions, "Select Job", jobId);

        let internalOnly = jobId == 1004;
        let optionSet = internalOnly ? internalOptions : otherOptions;
        let correctCat = '';

        Object.keys(optionGroup).forEach((groupOpt, index) => {
            optionGroup[groupOpt].forEach((taskOpt, optIndex) => {
                if (taskOpt.value == taskId)
                    correctCat = groupOpt;
            });
        });
        categorySelection.setEnabled(optionSet, "Select Category", correctCat);

        let thisCatsOptions = optionGroup[correctCat];
        taskSelection.setEnabled(thisCatsOptions, "Select Task", taskId);

        document.getElementById('ModalSubmit').onclick =
            () => effortRepository.changeEffort(curEmployeeId,
                                                curWeekId,
                                                jobId,
                                                taskId,
                                                jobSelection.value(),
                                                taskSelection.value(),
                                                () => window.location.reload(true),
                                                toastSuccess);

    }

    function initializeModalForNewJobTaskCombo() {
        jobSelection.setEnabled(jobOptions, "Select Job");
        categorySelection.setDisabled();
        taskSelection.setDisabled();

        document.getElementById('ModalSubmit').onclick = () => effortRepository.saveNewEffortFor(curEmployeeId, curWeekId, jobSelection.value(), taskSelection.value(), toastSuccess);
    }

    const instance = {
        categorySelect: categorySelection,
        taskSelect: taskSelection,
        jobSelect: jobSelection,

        materialize: materialize,

        currentWeekId: curWeekId,
        currentEmployeeId: curEmployeeId,

        saveNewEffortFromCurrentValues: () => effortRepository.saveNewEffortFor(currentEmployeeId, currentWeekId, jobSelection.value(), taskSelection.value(), toastSuccess),

        setNotificationBlockContent: setNotificationBlockContent,
        initializeModalForJobTaskCombo: initializeModalForJobTaskCombo,
        initializeModalForNewJobTaskCombo: initializeModalForNewJobTaskCombo
    };
    return instance;

};

function onShowModalClick(modalToShow, startingValue, onPersistChanges, materialize) {
    console.log(startingValue);
    modalData.setNotificationBlockContent();
    if (startingValue != undefined && startingValue != null) {
        let job = startingValue.substring(0, startingValue.indexOf('.'));
        let task = startingValue.substring(job.length + 1);
        console.log("showing change-job modal for ....",job, task);
        
        modalData.initializeModalForJobTaskCombo(job, task);
    }
    else {
        console.log("showing add new job modal");
        modalData.initializeModalForNewJobTaskCombo();
    }

    let instance = materialize.Modal.init(modalToShow, {});
    instance.open();    
}








