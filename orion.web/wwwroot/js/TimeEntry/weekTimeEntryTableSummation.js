

(function ($, weekTableSumDefinition) {
    if ($ === undefined) {
        throw "jQuery is not defined, but is required to run on this page. Client side processing will not continue.";
    }
    if (weekTableSumDefinition === undefined) {
        throw "window object is not defined, but is required to run on this page. Client side processing will not continue.";
    }

    function totalRow(row) {
        console.log('totaling row: ', row);
        var total = 0;
        row.find('td[autosum] input[type="text"]').each(function (index, inputVal) {
            total = total + Number($(inputVal).val());
        });

        var otTotal = 0;
        row.find('td[autosum][day^="ot-"] input[type="text"]').each(function (index, inputVal) {
            otTotal = otTotal + Number($(inputVal).val());
        });
        row.find('td[day=ot-' + weekTableSumDefinition.columnTotoalIdentifier + ']').html(otTotal.toFixed(1));
        row.find('td[day=' + weekTableSumDefinition.columnTotoalIdentifier + ']').html(total.toFixed(1));
    }

    function totalColumn(dayIdentifier, colName) {
        var total = 0;
        $('td[' + dayIdentifier + '="' + colName + '"] input[type="text"]').each(function (index, inputVal) {
            total = total + Number($(inputVal).val());
        });
        $('tr:last th[' + dayIdentifier + '="' + colName + '"]').html(total.toFixed(1));
    }

    function applyGrandTotal() {
        var total = 0;
        if ($('tr[' + weekTableSumDefinition.rowIdentifier + ']').length) {
            $('td[' + weekTableSumDefinition.dayIdentifier + '="' + weekTableSumDefinition.columnTotoalIdentifier + '"]').each(function (index, totalCell) {
                var innerText = $(totalCell).text();
                total += Number(innerText);
            });
            $('tr:last th[' + weekTableSumDefinition.dayIdentifier + '="' + weekTableSumDefinition.columnTotoalIdentifier + '"]').html(total.toFixed(1));

            var otTotal = 0;
            $('td[' + weekTableSumDefinition.dayIdentifier + '="ot-' + weekTableSumDefinition.columnTotoalIdentifier + '"]').each(function (index, totalCell) {
                var innerText = $(totalCell).text();
                otTotal += Number(innerText);
            });
            $('tr:last th[' + weekTableSumDefinition.dayIdentifier + '="ot-' + weekTableSumDefinition.columnTotoalIdentifier + '"]').html(otTotal.toFixed(1));
        }
    }

    $(document).ready(function () {
        $('tr[' + weekTableSumDefinition.rowIdentifier + ']').each(function (index, row) {
            totalRow($(row));
        });

        $('tr[' + weekTableSumDefinition.rowIdentifier + ']:first td[' + weekTableSumDefinition.dayIdentifier + ']').each(function (index, tdItem) {
            totalColumn(weekTableSumDefinition.dayIdentifier, $(tdItem).attr(weekTableSumDefinition.dayIdentifier));
        });

        applyGrandTotal();

        $('td[' + weekTableSumDefinition.columnIdentifier + '] input[type="text"]').change(function (el) {
            var row = $(el.currentTarget).closest('tr');
            totalRow(row);
            var columnId = $(el.currentTarget).closest('td').attr(weekTableSumDefinition.dayIdentifier);
            totalColumn(weekTableSumDefinition.dayIdentifier, columnId);
            applyGrandTotal();
        });
    });

})($, weekTableSumDefinition);