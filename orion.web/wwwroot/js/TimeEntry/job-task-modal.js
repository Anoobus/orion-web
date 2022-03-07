
//const JobTaskSelectors = {
//    category = 'select.custom-drop-down.category',
//    categoryId = 'NewEntry_SelectedTaskCategory',
//    task = 'select.custom-drop-down.task',
//    taskId = 'NewEntry_SelectedTaskId',
//    job = 'select.custom-drop-down.job',
//    jobId = 'NewEntry_SelectedJobId',
//    modalId = 'add-row-modal'
//};

//const JobTaskModal = class {

//    constructor(options) {
//        this.#options = options;
//        this.#modalData = this.#initializeFieldsOnDocLoad();
//        this.#addDropDownValidSelectionEnforcement();
//    }

//    #addDropDownValidSelectionEnforcement(){
//        this.#modalData.jobSelect.addEventListener('change', (event) => {
//            const target = event.target;
//            const selectedVal = target.value;
//            if (selectedVal == "preset") {
//                catInstance = this.#setDisabled(modalData.categorySelect);
//                taskInstance = this.#setDisabled(modalData.taskSelect);
//            }
//            else {
//                let internalOnly = selectedVal == 1004;
//                let optionSet = internalOnly ? modalData.internalCategoryOptions : modalData.otherCategoryOptions;
//                catInstance = this.#setEnabled(modalData.categorySelect, optionSet, "Select Category");
//                taskInstance = this.#setDisabled(modalData.taskSelect);
    
//            }
//        });
    
       
    
//        this.#modalData.categorySelect.addEventListener('change', (event) => {
//            console.log('catagory change detected!', event);
//            const selectedVal = event.target.value;
         
//            if (selectedVal == "preset") {
//                taskInstance = this.#setDisabled(modalData.taskSelect);
//            }
//            else {
//                let thisCatsOptions = modalData.groupedTaskOptoins[selectedVal]
//                taskInstance = this.#setEnabled(modalData.taskSelect, thisCatsOptions, "Select Task");
//            }
//        })
           
//        this.#modalData.taskSelect.addEventListener('change', (event) => {
//            console.log('task change detected!', event);
//            const selectedVal = event.target.value;
         
//            if (selectedVal == "preset") {
//               document.getElementById('ModalSubmit').style.display = "none";
//            }
//            else {
//                document.getElementById('ModalSubmit').style.display = "inline-block";
//            }
//        })
//    }
//    #initializeFieldsOnDocLoad(){ 
//        //store category options prior to clear
//        const internalOptions = document.querySelectorAll(JobTaskSelectors.category + ' option[internal-only="true"]');
//        const otherOptions = document.querySelectorAll(JobTaskSelectors.category + ' option[internal-only="false"]');
    
//        //clear category
//        let categorySelect = document.querySelectorAll(JobTaskSelectors.category )[0];
//        this.#removeAllOptions(categorySelect);
    
//        //init materialize dd w/ no options
//        catInstance = M.FormSelect.init(categorySelect, {})[0];
    
    
    
//       //store task options prior to clear
//        let taskOptions = document.querySelectorAll(JobTaskSelectors.task + ' option');
//        let optionGroup = {};
//        taskOptions.forEach((x,index) => {
            
//            let taskGroup = x.attributes['class'].value;
//            if (!optionGroup.hasOwnProperty(taskGroup)) {
//                optionGroup[taskGroup] = [];
//            }
//            optionGroup[taskGroup].push(x);
//        })
        
//        //clear task options 
//        let taskSelect = document.querySelectorAll(JobTaskSelectors.task )[0];
//        #removeAllOptions(taskSelect);
//        taskInstance = M.FormSelect.init(document.querySelectorAll(JobTaskSelectors.task), {})[0];
        
//        let submitBtn = document.getElementById('ModalSubmit');
//        submitBtn.style.display = "none";

       
       
//        return {
//            internalCategoryOptions : internalOptions,
//            otherCategoryOptions : otherOptions,
//            groupedTaskOptoins : optionGroup,
//            categorySelect: document.getElementById(JobTaskSelectors.categoryId),
//            taskSelect: document.getElementById(JobTaskSelectors.taskId),
//            jobSelect: document.getElementById(JobTaskSelectors.jobId)
//        };
//    };

//    #removeAllOptions(selectBox) {
//        while (selectBox.options.length > 0) {
//            selectBox.remove(0);
//        }
//    }
//    #setDisabled(selectBox) {
//        console.log('setDisabled on', selectBox);
//        //lock it all down
//        removeAllOptions(selectBox);
//        selectBox.setAttribute('disabled', '');
    
//        //turn off the submit button too;   
//        document.getElementById(modalDefinition.okId).style.display = "none";
//        return M.FormSelect.init(selectBox, {})[0];
//    }
//    #setEnabled(selectBox, options, presetName, selectedValue) {
//        console.log('setDisabled on', selectBox);
    
//        removeAllOptions(selectBox);
//        selectBox.add(new Option(presetName, "preset"));
      
//        options.forEach((x, index) => {
//            selectBox.add(x);
//        });
        
//        selectBox.removeAttribute('disabled');
//        selectBox.value = selectedValue | "preset";
//        return M.FormSelect.init(selectBox, {})[0];
//    }
//    Show(startingValue){
//        var modalFinds = document.getElementById(JobTaskSelectorys.modalId);
//        console.log(startingValue);
//        if(startingValue != undefined && startingValue != null){
//            let job = startingValue.substring(0,startingValue.indexOf('.'));
//            let task = startingValue.substring(job.length + 1);
//            console.log(job,task);
//            this.#showModal( job,task, onPersistChanges)
//        }
//        else
//        {
//            this.#showModal(null,null,onPersistChanges);
//        }

//        var instances = M.Modal.init(modalFinds, {});
//        instances.open();
//        e.preventDefault();
//    }

//    #showModal(presetJobId, presetTaskId, onSaveChanges){
       

//        function onOkClick( e){
//            console.log('click is going here yo');
//                e.preventDefault();
//                saveCallBack();
            
//        }

//        if(presetJobId != null && presetTaskId != null)
//        {
//            console.log('selecting this job id')
//            this.modalData.jobSelect.value = presetJobId;
//            M.FormSelect.init(modalData.jobSelect, {})[0];
    
//            let internalOnly = presetJobId == 1004;
//            let optionSet = internalOnly ? this.modalData.internalCategoryOptions : this.modalData.otherCategoryOptions;
//            let correctCat = '';
            
//            Object.keys(this.modalData.groupedTaskOptoins).forEach((groupOpt, index) => {
//                this.modalData.groupedTaskOptoins[groupOpt].forEach( (taskOpt, optIndex) => {
//                    if(taskOpt.value == presetTaskId)
//                        correctCat = groupOpt;
//                });
//            });
//            this.modalData.categorySelect.value = correctCat;
//            catInstance = this.#setEnabled(this.modalData.categorySelect, optionSet, "Select Category", correctCat);
           
//            let thisCatsOptions = this.modalData.groupedTaskOptoins[correctCat];
//            this.#setEnabled(this.modalData.taskSelect, thisCatsOptions, "Select Task", presetTaskId);
//            this.modalData.taskSelect.value = presetTaskId;
          
//        }

//        document.getElementById('ModalSubmit').removeEventListener('click', onOkClick);
//        document.getElementById('ModalSubmit').addEventListener('click', onOkClick);
//    }
//};