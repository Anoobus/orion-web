
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
    console.log(elems, "this is elems!");
    if (elems.length) {
        var instances = M.Autocomplete.init(elems, {
            data: autoCompleteObj,
            limit: 15
        });
    }
    
    
}

document.addEventListener('DOMContentLoaded', function () {
    initEditExpenseComponents();
});

function updateJobSelection(input, targetId) {
   
    if (nameToIdMap.has(input.value)){        
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

function updateTargetEmployee(selectInput, targetId) {
    let newId = selectInput.value;
    document.getElementById(targetId).value = newId;
}
function updateTargetVehicle(selectInput, targetId) {
    let newId = selectInput.value;
    document.getElementById(targetId).value = newId;
}