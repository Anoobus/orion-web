
const AsMap = obj => {
    const keys = Object.keys(obj);
    const map = new Map();
    for (let i = 0; i < keys.length; i++) {
        //inserting new key value pair inside map
        map.set(keys[i], obj[keys[i]]);
    };
    return map;
};


const jobMap = AsMap(jobDataBlob);

let autoCompleteObj = {};
const jobMapIter = jobMap.entries();
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

let oneTimeClears = {};

function oneTimeClear(input) {
    if (!oneTimeClears[input.id]) {
        oneTimeClears[input.id] = true;
        input.value = '';
    }
}
function undoJobChange(btn) {
    let origInput = document.getElementById('original-job-text');
    let jobInput = document.getElementById('NewlySelectedJob-Name');
    jobInput.value = origInput.value;
    updateJobSelection(jobInput);
    oneTimeClears[jobInput.id] = false;
    let jobInputLabel = document.querySelector('label[for="NewlySelectedJob-Name"]').classList.add('active');
}

function initEditExpenseComponents() {
    var elems = document.querySelectorAll('.autocomplete');
    var instances = M.Autocomplete.init(elems, {
        data: autoCompleteObj,
        limit: 15
    });
    
}

document.addEventListener('DOMContentLoaded', function () {
    initEditExpenseComponents();
});

function updateJobSelection(input, targetId) {
    console.log('updateJobSelection', input)
    if (jobMap.has(input.value)) {
        let newJobName = input.value;
        let newJobId = jobMap.get(newJobName);
        console.log('setting updated value', newJobName, newJobId);
        document.getElementById(targetId).value = newJobId;
        console.log('after update', document.getElementById(targetId));
    }

}
function updateTargetEmployee(selectInput, targetId) {
    let newId = selectInput.value;
    document.getElementById(targetId).value = newId;
}
