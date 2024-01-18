let modalData = {}

let oneTimeClears = {};

function oneTimeClear(input) {
    if (!oneTimeClears[input.id]) {
        oneTimeClears[input.id] = true;
        input.value = '';
    }
}


 const AddClickTrigger = function (startingValue, triggerId, modalId, onPersistChanges, materialize) {
     
        document.getElementById(triggerId).onclick = (e) => {
            var modalFinds = document.getElementById(modalId);
            e.preventDefault();
            onShowModalClick(modalFinds, startingValue, onPersistChanges, materialize);            
        };
 };


function undoJobChange(btn) {
    let origInput = document.getElementById('original-job-text');
    let jobInput = document.getElementById('NewlySelectedJob-Name');
    jobInput.value = origInput.value;
    updateJobSelection(jobInput);
    oneTimeClears[jobInput.id] = false;
    let jobInputLabel = document.querySelector('label[for="NewlySelectedJob-Name"]').classList.add('active');
}

document.addEventListener('DOMContentLoaded', function () {
    modalData = initializeFieldsOnDocLoad(M);
   
    
   
    
});

function initializeFieldsOnDocLoad(materialize) {

    function updateNewJobId(input, targetId) {

        if (nameToIdMap.has(input.value)) {
            let newJobId = nameToIdMap.get(input.value);
            document.getElementById(targetId).value = newJobId;
        }
        else {
            let codeOnly = input.value.substring(0, 9);
            if (codeToIdMap.has(codeOnly)) {
                let newJobId = codeToIdMap.get(codeOnly);
                document.getElementById(targetId).value = newJobId;
            }
            else {
                console.log("skip set, can't find " + input.value + ' inside of our maps', nameToIdMap, codeToIdMap);
            }
        }


    }

    const AsMap = (idFilter, obj) => {
        const keys = Object.keys(obj);
        const map = new Map();
        for (let i = 0; i < keys.length; i++) {
            //inserting new key value pair inside map

            map.set(idFilter(keys[i]), obj[keys[i]]);
        };
        return map;
    };

    const nameToIdMap = AsMap((x) => x, jobDataBlob);
    const codeToIdMap = AsMap((x) => x.substring(0, 9), jobDataBlob);

    let autoCompleteObj = {};
    const jobMapIter = nameToIdMap.entries();
    let didAdd = false;
    do {
        let maybeValue = jobMapIter.next().value;
        if (maybeValue) {
            autoCompleteObj[maybeValue[0]] = null;
            didAdd = true;
        }
        else {
            didAdd = false;
        }

    } while (didAdd)

    

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

    let autoCompleteJob = document.getElementById('AddJobTaskAutoComplete');
   
  
    

    const okBtnId = 'ModalSubmit';
    let jobSelection = modalDropDown('NewEntry_SelectedJobId', materialize, okBtnId, (e) => addJobTaskOnJobChange(e.target.value, internalOptions, otherOptions));
    let categorySelection = modalDropDown('NewEntry_SelectedTaskCategory', materialize, okBtnId, (e) => onCategoryChange(e, optionGroup));
    let taskSelection = modalDropDown('NewEntry_SelectedTaskId', materialize, okBtnId, (e) => onTaskChange(e));

    categorySelection.setDisabled();
    taskSelection.setDisabled();
    jobSelection.setEnabled(jobOptions, "Select Job");

    let submitBtn = document.getElementById(okBtnId);
    submitBtn.style.display = "none";

    autoCompleteJob.onchange = autoCompleteUpdateJobSelection;
       

    var instances = M.Autocomplete.init(autoCompleteJob, {
        data: autoCompleteObj,
        limit: 15
    });

   

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

    
    
    
    

    


    function autoCompleteUpdateJobSelection(event, targetId) {

        let input = event.target;
     
        if (nameToIdMap.has(input.value)) {
            let newJobId = nameToIdMap.get(input.value);
            console.log(jobSelection);
            jobSelection.manuallySetValue(newJobId);
        }
        else {
            let codeOnly = input.value.substring(0, 9);
            if (codeToIdMap.has(codeOnly)) {
                let newJobId = codeToIdMap.get(codeOnly);
                console.log(jobSelection);
                jobSelection.manuallySetValue(newJobId);
            }
            else {
                console.log("skip set, can't find " + input.value + ' inside of our maps', nameToIdMap, codeToIdMap);
            }
        }
    }

    function addJobTaskOnJobChange(selectedVal, internalCategoryOptions, otherCategoryOptions) {
        
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


    document.getElementById('dd-job').onclick = updateSelectionType;
    document.getElementById('auto-job').onclick = updateSelectionType;

    function updateSelectionType(event) {
        console.log(event, "clicked");
        let selection = event.target.value;

        if (selection == "auto") {
            console.log("show auto");
            document.getElementById('job-by-quick-search').style.display = "block";
            document.getElementById('job-dd').style.display = "none";

            
                taskSelection.setDisabled();
                categorySelection.setDisabled();
                jobSelection.setEnabled(jobOptions, "Select Job");
            
            document.getElementById('dd-job').checked = false;
            document.getElementById('auto-job').checked = true;

        }
        else {
            console.log("show Drop down");
            document.getElementById('job-by-quick-search').style.display = "none";
            document.getElementById('job-dd').style.display = "block";

            document.getElementById('auto-job').checked = false;
            document.getElementById('dd-job').checked = true;
        }
    }



    function initializeModalForJobTaskCombo(jobId, taskId) {
        var event = document.createEvent('Event');
       
        console.log('sending event2 !!');
        event.initEvent('click', true, true);
        document.getElementById('dd-job').dispatchEvent(event);

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








