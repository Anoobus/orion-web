let modalDropDown = function (id, materialize, okId, onChange) {

    let selectBox = document.getElementById(id);

    function removeAllOptions() {
        while (selectBox.options.length > 0) {
            selectBox.remove(0);
        }
    }

    function setDisabled() {        
        //lock it all down
        removeAllOptions(selectBox);
        console.log(selectBox, "setting disabled for this item")
        selectBox.setAttribute('disabled', '');

        //turn off the submit button too;   
        document.getElementById(okId).style.display = "none";
        materialize.FormSelect.init(selectBox, {})[0];
    }

    function setEnabled(options, presetName, selectedValue) {
        removeAllOptions(selectBox);
        selectBox.add(new Option(presetName, "preset"));

        options.forEach((x, index) => {
            selectBox.add(x);
        });

        selectBox.removeAttribute('disabled');
        selectBox.value = selectedValue ?? "preset";
        materialize.FormSelect.init(selectBox, {})[0];
    }

    selectBox.addEventListener('change', onChange);
    
    return {
        removeAllOptions,
        setDisabled,
        setEnabled,
        value: () => selectBox.value,
        manuallySetValue: (selectedVal) => {
            selectBox.value = selectedVal;
            
            if (typeof (Event) === 'function') {
                var event = new Event('change');
            } else {  // for IE11
                var event = document.createEvent('Event');
                event.initEvent('change', true, true);
            }
            //emit the event so that materialize acks the change too
            selectBox.dispatchEvent(event);
        }                
    };
}
