//Auto search by name
$('input[id="CompanyName"]').autocomplete({
    source: function (request, response) {
        $('#CompanyId').val("");
        $.ajax({
            url: "/Company/GetCompanyByName/",
            type: "POST",
            dataType: "json",
            data: { prefix: request.term },
            success: function (data) {
                if (data.result == false) {
                    toastr.error("DO not found");
                    return;
                }
                response($.map(data, function (item) {
                    return { label: item.Name, value: item.Name, ID: item.ID };
                }))
            }
        });
    },
    minLength: 0,
    select: function (event, ui) {
        $("#CompanyId").val(ui.item.ID);
    },
    maxShowItems: 5
}).focus(function () {
    $(this).autocomplete("search")
});




$('input[id="SizeName"]').autocomplete({
    source: function (request, response) {
        $('#SizeID').val("");
        $.ajax({
            url: "/Size/GetSizeByName/",
            type: "POST",
            dataType: "json",
            data: { prefix: request.term },
            success: function (data) {
                if (data.result == false) {
                    toastr.error("DO not found");
                    return;
                }
                response($.map(data, function (item) {
                    return { label: item.Name, value: item.Name, ID: item.ID };
                }))
            }
        });
    },
    minLength: 0,
    select: function (event, ui) {
        $("#SizeID").val(ui.item.ID);
    },
    maxShowItems: 5
}).focus(function () {
    $(this).autocomplete("search")
});

$('input[id="DiaSizeName"]').autocomplete({
    source: function (request, response) {
        $('#SizeID').val("");
        $.ajax({
            url: "/DiaSize/GetDiaSizeByName/",
            type: "POST",
            dataType: "json",
            data: { prefix: request.term },
            success: function (data) {
                if (data.result == false) {
                    toastr.error("DO not found");
                    return;
                }
                response($.map(data, function (item) {
                    return { label: item.Name, value: item.Name, ID: item.ID };
                }))
            }
        });
    },
    minLength: 0,
    select: function (event, ui) {
        $("#DiaSizeID").val(ui.item.ID);
    },
    maxShowItems: 5
}).focus(function () {
    $(this).autocomplete("search")
});

//$('input[id="Sizes"]').autocomplete({
//    source: function (request, response) {
//        $('#SizeID').val("");
//        $.ajax({
//            url: "/Size/GetSizeByName/",
//            type: "POST",
//            dataType: "json",
//            data: { prefix: request.term },
//            success: function (data) {
//                if (data.result == false) {
//                    toastr.error("DO not found");
//                    return;
//                }
//                response($.map(data, function (item) {
//                    return { label: item.Name, value: item.Name, ID: item.ID };
//                }))
//            }
//        });
//    },
//    minLength: 0,
//    select: function (event, ui) {
//        $("#SizeID").val(ui.item.ID);
//    },
//    maxShowItems: 5
//}).focus(function () {
//    $(this).autocomplete("search")
//});





$('input[id="CategoryName"]').autocomplete({
    source: function (request, response) {
        $('#CategoryId').val("");
        $.ajax({
            url: "/Category/GetCategoryByName/",
            type: "POST",
            dataType: "json",
            data: { prefix: request.term },
            success: function (data) {
                if (data.result == false) {
                    toastr.error("DO not found");
                    return;
                }
                response($.map(data, function (item) {
                    return { label: item.Name, value: item.Name, ID: item.ID };
                }))
            }
        });
    },
    minLength: 0,
    select: function (event, ui) {
        $("#CategoryId").val(ui.item.ID);
    },
    maxShowItems: 5
}).focus(function () {
    $(this).autocomplete("search")
});


//$('input[id="SizeID"]').autocomplete({
//    source: function (request, response) {
//        $('#SizeId').val("");
//        $.ajax({
//            url: "/Size/GetSizeByName/",
//            type: "POST",
//            dataType: "json",
//            data: { prefix: request.term },
//            success: function (data) {
//                if (data.result == false) {
//                    toastr.error("DO not found");
//                    return;
//                }
//                response($.map(data, function (item) {
//                    return { label: item.SizeID, value: item.Code, ID: item.Description };
//                }))
//            }
//        });
//    },
//    minLength: 0,
//    select: function (event, ui) {
//        $("#CategoryId").val(ui.item.ID);
//    },
//    maxShowItems: 5
//}).focus(function () {
//    $(this).autocomplete("search")
//});

$('input[id*="_ColorName"]').autocomplete({
    source: function (request, response) {
        $('input[id*="_ColorId"]').val("");
        $.ajax({
            url: "/Color/GetColorByName/",
            type: "GET",
            dataType: "json",
            data: { prefix: request.term },
            success: function (data) {
                if (data.result == false) {
                    toastr.error("DO not found");
                    return;
                }
                response($.map(data, function (item) {
                    return { label: item.Name, value: item.Name, ID: item.ID };
                }))
            }
        });
    },
    minLength: 0,
    select: function (event, ui) {
        $('input[id*="_ColorId"]').val(ui.item.ID);
    },
    maxShowItems: 5
}).focus(function () {
    $(this).autocomplete("search")
});

$('input[id*="_GodownName"]').autocomplete({
    source: function (request, response) {
        $('input[id*="_GodownID"]').val("");
        $.ajax({
            url: "/Godown/GetGodownByName/",
            type: "GET",
            dataType: "json",
            data: { prefix: request.term },
            success: function (data) {
                if (data.result == false) {
                    toastr.error("DO not found");
                    return;
                }
                response($.map(data, function (item) {
                    return { label: item.Name, value: item.Name, ID: item.ID };
                }))
            }
        });
    },
    minLength: 0,
    select: function (event, ui) {
        $('input[id*="_GodownID"]').val(ui.item.ID);
    },
    maxShowItems: 5
}).focus(function () {
    $(this).autocomplete("search")
});

//Add New Product modal show
$(document).on('click', '#btnAddProduct', function () {
    $('#newProductAddModal').modal('show');
});

//Save product validation
$(document).on('click', '#btnSaveProduct', function (e) {
    var IsValid = true;
    var ProductName = $("#ProductName").val();
    if (ProductName == "") {
        e.preventDefault();
        toastr.error("Please enter product name");
        IsValid = false;
        $("#ProductName").focus()
        $('#ProductName').attr('style', 'border:1px solid red !important');
        return false;
    }
    else {
        $('#ProductName').attr('style', 'border:1px solid #c4daf1  !important');
    }
    if (IsValid) {
        var companyID = $("#CompanyId").val();
        if (companyID == "") {
            e.preventDefault();
            toastr.error("Please select company");
            IsValid = false;
            $("#CompanyName").focus()
            $('#CompanyName').attr('style', 'border:1px solid red !important');
            return false;
        } else {
            $('#CompanyName').prop('style', 'border:1px solid #c4daf1  !important');
        }
    }


    if (IsValid) {
        var SizeID = $("#SizeID").val();
        if (SizeID == "") {
            e.preventDefault();
            toastr.error("Please select Size");
            IsValid = false;
            $("#Size").focus()
            $('#Size').attr('style', 'border:1px solid red !important');
            return false;
        } else {
            $('#Size').prop('style', 'border:1px solid #c4daf1  !important');
        }
    }
    if (IsValid) {
        var SizeID = $("#DiaSizeID").val();
        if (SizeID == "") {
            e.preventDefault();
            toastr.error("Please select Dia. Size");
            IsValid = false;
            $("#DiaSize").focus()
            $('#DiaSize').attr('style', 'border:1px solid red !important');
            return false;
        } else {
            $('#DiaSize').prop('style', 'border:1px solid #c4daf1  !important');
        }
    }



    if (IsValid) {
        var CategoryId = $("#CategoryId").val();
        if (CategoryId == "") {
            e.preventDefault();
            toastr.error("Please select category");
            IsValid = false;
            $("#CategoryName").focus()
            $('#CategoryName').attr('style', 'border:1px solid red !important');
            return false;
        } else {
            $('#CategoryName').attr('style', 'border:1px solid #c4daf1  !important');
        }
    }


    if (IsValid) {
        var ProductType = $("#ProductType").val();
        if (ProductType == "") {
            e.preventDefault();
            toastr.error("Please select Product Type");
            IsValid = false;
            $("#ProductType").focus()
            $('#ProductType').attr('style', 'border:1px solid red !important');
            return false;
        } else {
            $('#ProductType').attr('style', 'border:1px solid #c4daf1  !important');
        }
    }


    if (IsValid) {
        var UnitType = $("#UnitType").val();
        if (UnitType == "") {
            e.preventDefault();
            toastr.error("Please select Unit Type");
            IsValid = false;
            $("#UnitType").focus()
            $('#UnitType').attr('style', 'border:1px solid red !important');
            return false;
        } else {
            $('#UnitType').attr('style', 'border:1px solid #c4daf1  !important');
        }
    }

    var btnid = $(this).data("id");
    if (btnid == "btnPOSaveProduct") {
        $('#btnSaveProduct').attr('disabled', true);
        SaveProduct();
    }
});

//Product Save function
function SaveProduct() {
    var ProductName = $("#ProductName").val();
    var companyID = $("#CompanyId").val();
    var SizeID = $("#SizeID").val();
    var DiaSizeID = $("#DiaSizeID").val();
    var CategoryId = $("#CategoryId").val();
    var ProductType = $("#ProductType").val();
    var UnitType = $("#UnitType").val();
    var MRP = getDefaultFloatIfEmpty($("#MRP").val());
    var model = {
        ProductName: ProductName,
        CategoryId: CategoryId,
        CompanyId: companyID,
        SizeID: SizeID,
        DiaSizeID: DiaSizeID,
        UnitType: UnitType,
        MRP: MRP,
        ProductType: ProductType,
        Code: "0000"
    };

    $.ajax({
        url: "/Product/AddProduct/",
        type: "POST",
        dataType: "json",
        data: { "newProduct": model },
        success: function (data) {
            if (data.result == false) {
                toastr.error(data.msg);
            }
            else {
                toastr.info("save success.");
                $('#ProductsId').val(data.data.ProductID);
                $('#ProductsName').val(data.data.ProductName);
                $('#ProductsCode').val(data.data.Code);
                $('#ProductsType').val(data.data.ProductType);
                $('#PODetail_MRPRate').val(data.data.MRP).trigger('input');
                $('#newProductAddModal').modal('hide');
                ClearProductField();
            }
            $('#btnSaveProduct').attr('disabled', false);
        },
        error: function () {
            $('#btnSaveProduct').attr('disabled', false);
        }
    });
}

function ClearProductField() {
    $("#ProductName").val("");
    $("#CompanyId").val("");
    $("#CompanyName").val("");
    $("#CategoryName").val("");
    $("#CategoryId").val("");
    $("#ProductType").val("");
    $("#UnitType").val("");
    $("#MRP").val("");
    $("#SizeId").val("");
    $("#Size").val("");
    $("#DiaSizeId").val("");
    $("#DiaSize").val("");
}

//Add New Company
$(document).on('click', '#btnAddCompany', function () {
    ShowNewEntryModal("Add New Company", "Company Name", "company");
});




//Add New Category
$(document).on('click', '#btnAddCategory', function () {
    ShowNewEntryModal("Add New Category", "Category Name", "category");
});



//Add New Size
$(document).on('click', '#btnAddSize', function () {
    ShowNewEntryModal("Add New Size", "Size Name", "Sizes");
});

//Add New Dia. Size
$(document).on('click', '#btnAddDiaSize', function () {
    ShowNewEntryModal("Add New Dia. Size", "Dia. Size Name", "DiaSizes");
});
//Add New Godown
$(document).on('click', '#btnAddGodown', function () {
    ShowNewEntryModal("Add New Godown", "Godown Name", "godown");
});

//Add New Color
$(document).on('click', '#btnAddColor', function () {
    ShowNewEntryModal("Add New Color", "Color Name", "color");
});

function ShowNewEntryModal(HeaderName, LabelName, EntryType) {
    $('#addEntryNewHeader').text(HeaderName);
    $('#lblEntryName').text(LabelName);
    $('#entryType').val(EntryType);
    $('#newName').attr('style', 'border:1px solid #c4daf1  !important');
    $('#newEntryModal').modal('show');
}


$(document).on('click', '#btnAddCompanyCategory', function (e) {
    var newName = $('#newName').val();
    if (newName == "") {
        toastr.error("Please enter name");
        $('#newName').attr('style', 'border:1px solid red !important');
        e.preventDefault();
        return false;
    }
    else {
        $('#newName').attr('style', 'border:1px solid #c4daf1  !important');
        AddNewEntity(newName);
    }
});

$(document).on('click', '#btnAddSize', function (e) {
    var newName = $('#newName').val();
    if (newName == "") {
        toastr.error("Please enter name");
        $('#newName').attr('style', 'border:1px solid red !important');
        e.preventDefault();
        return false;
    }
    else {
        $('#newName').attr('style', 'border:1px solid #c4daf1  !important');
        AddNewEntity(newName);
    }
});

$(document).on('click', '#btnAddDiaSize', function (e) {
    var newName = $('#newName').val();
    if (newName == "") {
        toastr.error("Please enter name");
        $('#newName').attr('style', 'border:1px solid red !important');
        e.preventDefault();
        return false;
    }
    else {
        $('#newName').attr('style', 'border:1px solid #c4daf1  !important');
        AddNewEntity(newName);
    }
});

function AddNewEntity(newName) {
    $('#btnAddCompanyCategory').attr('disabled', true);
    var type = $('#entryType').val();
    var _URL = "";
    if (type == "company") {
        _URL = "/Company/AddCompany/";
    }
    else if (type == "category") {
        _URL = "/Category/AddCategory/";
    }
    else if (type == "Sizes") {
        _URL = "/size/AddSize/";
    }
    else if (type == "DiaSizes") {
        _URL = "/diasize/AddDiaSize/";
    }
    else if (type == "godown") {
        _URL = "/Godown/AddGodown/";
    }
    else if (type == "color") {
        _URL = "/Color/AddColor/";
    }
    $.ajax({
        url: _URL,
        type: "POST",
        dataType: "json",
        data: { "Name": newName },
        success: function (data) {
            if (data.result == false) {
                toastr.error(data.msg);
                return;
            }
            else {
                toastr.info("save success.");
                $('#newName').val("");
                SetupFieldValue(type, data);
                $('#newEntryModal').modal('hide');
            }
        },
        error: function (err) {
            console.error(err);
        }
    });

    $('#btnAddCompanyCategory').attr('disabled', false);
}


function SetupFieldValue(type, data) {
    if (type == "company") {
        $('#CompanyId').val(data.data.CompanyID);
        $('#CompanyName').val(data.data.Name);
    }
    else if (type == "category") {
        $('#CategoryId').val(data.data.CategoryID);
        $('#CategoryName').val(data.data.Description);
    }

    else if (type == "Sizes") {
        $('#SizeID').val(data.data.SizeID);
        $('#Sizes').val(data.data.Description);
    }

    else if (type == "DiaSizes") {
        $('#DiaSizeID').val(data.data.DiaSizeID);
        $('#DiaSizes').val(data.data.Description);
    }

    else if (type == "godown") {
        $('input[id*="_GodownID"]').val(data.data.GodownID);
        $('input[id*="_GodownName"]').val(data.data.Name);
    }
    else if (type == "color") {
        $('input[id*="_ColorId"]').val(data.data.ColorID);
        $('input[id*="_ColorName"]').val(data.data.Name);
    }
}



