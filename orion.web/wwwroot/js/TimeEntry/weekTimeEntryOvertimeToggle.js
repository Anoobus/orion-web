


(function ($, weekTableSumDefinition) {
    if ($ === undefined) {
        throw "jQuery is not defined, but is required to run on this page. Client side processing will not continue.";
    }
    if (weekTableSumDefinition === undefined) {
        throw "window object is not defined, but is required to run on this page. Client side processing will not continue.";
    }    

    $(document).ready(function () {
        console.log('running overtime initial load');
        $('tr[' + weekTableSumDefinition.rowIdentifier + '] td div input[id$="OvertimeHours"]').each(function (index, el) {
            
            var testValue = $(el).val() | "0" ;
            if (Number(testValue) > 0) {
                var day = $(el).closest('td').attr(weekTableSumDefinition.dayIdentifier);
                var matchExpr = '[' + weekTableSumDefinition.dayIdentifier + '="' + day + '"]';
                console.log("showing cell", day);
                $('td' + matchExpr).show();
                $('th' + matchExpr).show();
            }            
        });
        if ($('td[' + weekTableSumDefinition.dayIdentifier + '^="ot-"]:visible').length) {
            var totalExpr = '[' + weekTableSumDefinition.dayIdentifier + '="ot-' + weekTableSumDefinition.columnTotoalIdentifier + '"]';
            $('td' + totalExpr).show();
            $('th' + totalExpr).show();

            console.log("showing header", totalExpr);
        }

        $('tr[' + weekTableSumDefinition.rowIdentifier + '] td a.ot-button').click(function (el) {
            var day = $(el.target).closest('td').attr(weekTableSumDefinition.dayIdentifier);
            var matchExpr = '[' + weekTableSumDefinition.dayIdentifier + '="ot-' + day + '"]';
            $('td' + matchExpr).toggle();
            $('th' + matchExpr).toggle();

            if ($('td[' + weekTableSumDefinition.dayIdentifier + '^="ot-"]:visible').length) {
                var totalExpr = '[' + weekTableSumDefinition.dayIdentifier + '="ot-' + weekTableSumDefinition.columnTotoalIdentifier + '"]';
                $('td' + totalExpr).show();
                $('th' + totalExpr).show();
            }
        });
    });

})($, weekTableSumDefinition);