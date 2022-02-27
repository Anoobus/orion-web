


(function ($, weekTableSumDefinition) {
    if ($ === undefined) {
        throw "jQuery is not defined, but is required to run on this page. Client side processing will not continue.";
    }
    if (weekTableSumDefinition === undefined) {
        throw "window object is not defined, but is required to run on this page. Client side processing will not continue.";
    }    

    $(document).ready(function () {
        $('tr[' + weekTableSumDefinition.rowIdentifier + '] td div input[id$="OvertimeHours"]').each(function (index, el) {
            
            var testValue = $(el).val() | "0";
            if (Number(testValue) > 0) {
                var day = $(el).closest('td').attr(weekTableSumDefinition.dayIdentifier);
                var matchExpr = '[' + weekTableSumDefinition.dayIdentifier + '="' + day + '"]';
                $('td' + matchExpr).show();
                $('th' + matchExpr).show();
            }            
        });

        if ($('td[' + weekTableSumDefinition.dayIdentifier + '^="ot-"]:visible').length) {
            var totalExpr = '[' + weekTableSumDefinition.dayIdentifier + '="ot-' + weekTableSumDefinition.columnTotoalIdentifier + '"]';
            $('td' + totalExpr).show();
            $('th' + totalExpr).show();
        }

        $('th a.ot-button').click(function (el) {
            el.preventDefault();
            var day = $(el.target).closest('th').attr(weekTableSumDefinition.dayIdentifier);
            var matchExpr = '[' + weekTableSumDefinition.dayIdentifier + '="ot-' + day + '"]';
            console.log("trying to toggle " + matchExpr + " becuase it was matched the dayIdentifier of the closest th of the target", el.target);

            let otColShow = $('th' + matchExpr)[0].style.display == "none";
            $('td' + matchExpr).toggle();
            $('th' + matchExpr).toggle();

            console.log("checking if any ot cols are up, if so we'll show the total col");

            let visibleOtCells = $('td[' + weekTableSumDefinition.dayIdentifier + ']')
                .filter((index, cell) => cell.attributes['day'].value.startsWith('ot-')
                    && cell.style.display != "none"
                    && cell.attributes['day'].value != "ot-" + day
                );


            if (otColShow || visibleOtCells > 0) {
                var totalExpr = '[' + weekTableSumDefinition.dayIdentifier + '="ot-' + weekTableSumDefinition.columnTotoalIdentifier + '"]';
                console.log('trying to show td/th with expr ' + totalExpr);
                $('td' + totalExpr).show();
                $('th' + totalExpr).show();
            }
            else {
                var totalExpr = '[' + weekTableSumDefinition.dayIdentifier + '="ot-' + weekTableSumDefinition.columnTotoalIdentifier + '"]';
                console.log('trying to close td/th with expr ' + totalExpr);
                $('td' + totalExpr).hide();
                $('th' + totalExpr).hide();
            }
        });
    });

})($, weekTableSumDefinition);