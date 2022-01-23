let catInstance = {};
var taskInstance = {};

$(document).ready(function () {


    const approvalStatus = $('#ApprovalStatus').val();
    const isAdmin = $('#IsAdmin').val() == "true";
    //take copy of all options except for the "please select"
    const internalOptions = document.querySelectorAll('select.custom-drop-down.category option[internal-only="true"]');
    const otherOptions = document.querySelectorAll('select.custom-drop-down.category option[internal-only="false"]');
   

    let categorySelect = document.querySelectorAll('select.custom-drop-down.category')[0];
  
    removeAllOptions(categorySelect);
    catInstance = M.FormSelect.init(categorySelect, {})[0];



    let taskSelect = document.querySelectorAll('select.custom-drop-down.task')[0];
    let taskOptions = document.querySelectorAll('select.custom-drop-down.task option');
    removeAllOptions(taskSelect);

    let optionGroup = {};
    taskOptions.forEach(x => {
        
        let taskGroup = x.attributes['class'].value;
        if (!optionGroup.hasOwnProperty(taskGroup)) {
            optionGroup[taskGroup] = [];
        }
        optionGroup[taskGroup].push(x);
    })
    taskInstance = M.FormSelect.init(document.querySelectorAll('select.custom-drop-down.task'), {})[0];

    console.log(taskOptions);
  
   

  

  

   
 
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
        return M.FormSelect.init(selectBox, {})[0];
    }
    function setEnabled(selectBox, options, presetName) {
        console.log('setDisabled on', selectBox);

        removeAllOptions(selectBox);
        selectBox.add(new Option(presetName, "preset"));
      
        options.forEach(x => {
            selectBox.add(x);
        });
        
        selectBox.removeAttribute('disabled');
        selectBox.value = "preset";
        return M.FormSelect.init(selectBox, {})[0];
    }
    $('#NewEntry_SelectedJobId').change((event) => {
        console.log('job change detected!', event);
        const target = event.target;
        const selectedVal = target.value;
        const categorySelect = document.getElementById("NewEntry_SelectedTaskCategory");
        const taskSelect = document.getElementById("NewEntry_SelectedTaskId");
        if (selectedVal == "preset") {
            catInstance = setDisabled(categorySelect);
            taskInstance = setDisabled(taskSelect);
        }
        else {
            let internalOnly = selectedVal == 1004;
            let optionSet = internalOnly ? internalOptions : otherOptions;
            catInstance = setEnabled(categorySelect, optionSet, "Select Category");
        }
    });


    $('#NewEntry_SelectedTaskCategory').change((event) => {
        console.log('catagory change detected!', event);
        const selectedVal = event.target.value;
        const taskSelect = document.getElementById("NewEntry_SelectedTaskId");

        if (selectedVal == "preset") {
            taskInstance = setDisabled(taskSelect);
        }
        else {
            let thisCatsOptions = optionGroup[selectedVal]
            taskInstance = setEnabled(taskSelect, thisCatsOptions, "Select Task");
        }
    })


});

