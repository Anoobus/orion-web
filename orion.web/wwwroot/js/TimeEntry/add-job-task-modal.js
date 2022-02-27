
  
let modalData = {}

let modalDefinition = {
    okId = 'ModalSubmit'
}


function removeAllOptions(selectBox) {
    while (selectBox.options.length > 0) {
        selectBox.remove(0);
    }
}
function setDisabled(selectBox) {
    console.log('setDisabled on', selectBox);
    //lock it all down
    removeAllOptions(selectBox);
    selectBox.setAttribute('disabled', '');

    //turn off the submit button too;   
    document.getElementById(modalDefinition.okId).style.display = "none";
    return M.FormSelect.init(selectBox, {})[0];
}

function setEnabled(selectBox, options, presetName, selectedValue) {
    console.log('setDisabled on', selectBox);

    removeAllOptions(selectBox);
    selectBox.add(new Option(presetName, "preset"));
  
    options.forEach((x, index) => {
        selectBox.add(x);
    });
    
    selectBox.removeAttribute('disabled');
    selectBox.value = selectedValue ?? "preset";
    return M.FormSelect.init(selectBox, {})[0];
}

document.addEventListener('DOMContentLoaded', function () {
  
    console.log("storing original modal options");
   

    function initializeFieldsOnDocLoad(){ 
        //store category options prior to clear
        const internalOptions = document.querySelectorAll('select.custom-drop-down.category option[internal-only="true"]');
        const otherOptions = document.querySelectorAll('select.custom-drop-down.category option[internal-only="false"]');
    
        //clear category
        let categorySelect = document.querySelectorAll('select.custom-drop-down.category')[0];
        removeAllOptions(categorySelect);
    
        //init materialize dd w/ no options
        catInstance = M.FormSelect.init(categorySelect, {})[0];
    
    
    
       //store task options prior to clear
        let taskOptions = document.querySelectorAll('select.custom-drop-down.task option');
        let optionGroup = {};
        taskOptions.forEach((x,index) => {
            
            let taskGroup = x.attributes['class'].value;
            if (!optionGroup.hasOwnProperty(taskGroup)) {
                optionGroup[taskGroup] = [];
            }
            optionGroup[taskGroup].push(x);
        })
        
        //clear task options 
        let taskSelect = document.querySelectorAll('select.custom-drop-down.task')[0];
        removeAllOptions(taskSelect);
        taskInstance = M.FormSelect.init(document.querySelectorAll('select.custom-drop-down.task'), {})[0];
        
        let submitBtn = document.getElementById('ModalSubmit');
        submitBtn.style.display = "none";

       
       
        return {
            internalCategoryOptions : internalOptions,
            otherCategoryOptions : otherOptions,
            groupedTaskOptoins : optionGroup,
            categorySelect: document.getElementById("NewEntry_SelectedTaskCategory"),
            taskSelect: document.getElementById("NewEntry_SelectedTaskId"),
            jobSelect: document.getElementById('NewEntry_SelectedJobId')
        };
    };
    modalData = initializeFieldsOnDocLoad();

    modalData.jobSelect.addEventListener('change', (event) => {
        const target = event.target;
        const selectedVal = target.value;
        if (selectedVal == "preset") {
            catInstance = setDisabled(modalData.categorySelect);
            taskInstance = setDisabled(modalData.taskSelect);
        }
        else {
            let internalOnly = selectedVal == 1004;
            let optionSet = internalOnly ? modalData.internalCategoryOptions : modalData.otherCategoryOptions;
            catInstance = setEnabled(modalData.categorySelect, optionSet, "Select Category");
            taskInstance = setDisabled(modalData.taskSelect);

        }
    });

   

    modalData.categorySelect.addEventListener('change', (event) => {
        console.log('catagory change detected!', event);
        const selectedVal = event.target.value;
     
        if (selectedVal == "preset") {
            taskInstance = setDisabled(modalData.taskSelect);
        }
        else {
            let thisCatsOptions = modalData.groupedTaskOptoins[selectedVal]
            taskInstance = setEnabled(modalData.taskSelect, thisCatsOptions, "Select Task");
        }
    })
       
    modalData.taskSelect.addEventListener('change', (event) => {
        console.log('task change detected!', event);
        const selectedVal = event.target.value;
     
        if (selectedVal == "preset") {
           document.getElementById('ModalSubmit').style.display = "none";
        }
        else {
            document.getElementById('ModalSubmit').style.display = "inline-block";
        }
    })
   
});

const AddClickTrigger = function (startingValue, triggerId, modalId, onPersistChanges)
{
    document.getElementById(triggerId).addEventListener("click", e => {
       
        var modalFinds = document.getElementById(modalId);
        console.log(startingValue);
        if(startingValue != undefined && startingValue != null){
            let job = startingValue.substring(0,startingValue.indexOf('.'));
            let task = startingValue.substring(job.length + 1);
            console.log(job,task);
            JobTaskModal(modalData, job,task, onPersistChanges)
        }
        else
        {
            JobTaskModal(modalData, null,null,onPersistChanges);
        }

        var instances = M.Modal.init(modalFinds, {});
        instances.open();
        e.preventDefault();
    });
   
}
const JobTaskModal = function(initialModalData, presetJobId, presetTaskId, onPersistChanges) {

   
    let modalData = initialModalData;
    let saveCallBack = onPersistChanges;

        function onOkClick( e){
            console.log('click is going here yo');
                e.preventDefault();
                saveCallBack();
            
        }

        if(presetJobId != null && presetTaskId != null)
        {
            console.log('selecting this job id')
            modalData.jobSelect.value = presetJobId;
            M.FormSelect.init(modalData.jobSelect, {})[0];
    
            let internalOnly = presetJobId == 1004;
            let optionSet = internalOnly ? modalData.internalCategoryOptions : modalData.otherCategoryOptions;
            let correctCat = '';
            
            Object.keys(modalData.groupedTaskOptoins).forEach((groupOpt, index) => {
                modalData.groupedTaskOptoins[groupOpt].forEach( (taskOpt, optIndex) => {
                    if(taskOpt.value == presetTaskId)
                        correctCat = groupOpt;
                });
            });
            modalData.categorySelect.value = correctCat;
            catInstance = setEnabled(modalData.categorySelect, optionSet, "Select Category", correctCat);
           
            let thisCatsOptions = modalData.groupedTaskOptoins[correctCat];
            setEnabled(modalData.taskSelect, thisCatsOptions, "Select Task", presetTaskId);
            modalData.taskSelect.value = presetTaskId;
          
        }

        document.getElementById('ModalSubmit').removeEventListener('click', onOkClick);
        document.getElementById('ModalSubmit').addEventListener('click', onOkClick);
}