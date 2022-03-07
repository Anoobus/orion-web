let modalDropDown = function (id, materialize, okId, onChange) {

    let selectBox = document.getElementById(id);

    function removeAllOptions() {
        while (selectBox.options.length > 0) {
            selectBox.remove(0);
        }
    }

    function setDisabled() {
        console.log('setDisabled on', selectBox);
        //lock it all down
        removeAllOptions(selectBox);
        selectBox.setAttribute('disabled', '');

        //turn off the submit button too;   
        document.getElementById(okId).style.display = "none";
        materialize.FormSelect.init(selectBox, {})[0];
    }

    function setEnabled(options, presetName, selectedValue) {
        console.log('setEnabled on', selectBox);

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
        value: () => selectBox.value
    };
}
