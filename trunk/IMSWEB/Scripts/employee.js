var employee = (function () {

    var getEmployees = (function (EmployeeID) {
        $('input[id="EmployeeName"]').autocomplete({
            source: function (request, response) {
                $('#EmployeeName').val("");

                ihelper.get("/Employee/GetEmployeesByName", { prefix: request.term }, function (data) {
                    if (data == false) {
                        toastr.error("No employee found!");
                        return;
                    }
                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Name, ID: item.ID };
                    }))
                });
            },
            minLength: 0,
            select: function (event, ui) {
                $("#" + EmployeeID).val(ui.item.ID);
            },
            maxShowItems: 5
        }).focus(function () {
            $(this).autocomplete("search")
        });
    });

    return {
        getEmployees: getEmployees
    }
})();

