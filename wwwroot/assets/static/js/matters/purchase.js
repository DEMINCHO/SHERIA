$(document).ready(function () {

    App.init();
    //TableManageResponsive.init();
    GetPurchaseRecords();
    GetProducts();
    
    InitiateEditableDataTable.init();


    $('#start_date').datepicker({
        todayHighlight: true,
        startDate: '-6m',
        //endDate: '0',
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

    $('#return_date').datepicker({
        todayHighlight: true,
        startDate: '-6m',
        //endDate: '0',
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

});


function GetPurchaseRecords() {
    $.get('GetRecords', { module: 'purchase_list' }, function (data) {
        getData(data);
    });
}

function getData(jsonstring) {
    table = $('#editabledatatable').dataTable();
    oSettings = table.fnSettings();
    table.fnClearTable(this);

    var json = $.parseJSON(JSON.stringify(jsonstring));
    //var json = JSON.parse(jsonstring);
    for (var i = 0; i < json.length; i++) {
        var item = json[i];
        table.oApi._fnAddData(oSettings, item);
    }
    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
    table.fnDraw();
}


function GetProducts() {
    $.get('GetRecords', { module: 'product_list' }, function (data) {
        $("#product").get(0).options.length = 0;
        $("#product").get(0).options[0] = new Option("Please Select Product ", "-1");

        $.each(data, function (index, item) {
            $("#product").get(0).options[$("#product").get(0).options.length] = new Option(item.name, item.id);
        });

        $("#product").bind("change", function () {

        });
    });
}

var InitiateEditableDataTable = function () {
    return {
        init: function () {
            //Datatable Initiating
            var oTable = $('#editabledatatable').dataTable({
                "responsive": true,
                "createdRow": function (row, data, dataIndex) {
                    $(row).attr("recid", data.id);
                },
                "aoColumns": [
                    { "data": "client_name", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "product_name", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "start_date", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "end_date", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "quantity", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "price", "autoWidth": true, "sDefaultContent": "n/a" },
                    //{
                    //    "bSortable": false,
                    //    "sDefaultContent": "<a href='#' class='btn btn-info btn-xs edit'><i class='fa fa-edit'></i> Edit</a>"
                    //},
                    {
                        "bSortable": false,
                        "sDefaultContent": "<a href='#' class='btn btn-danger btn-xs delete'><i class='fas fa-trash-alt'></i> Delete</a>"
                    }
                ]
            });

            $('#editabledatatable').on("click", 'a.flagclosed', function (e) {
                e.preventDefault();

                nRow = $(this).parents('tr')[0];

                var aData = oTable.fnGetData(nRow);

                var json = JSON.parse(JSON.stringify(aData));
                console.log(json);

                var matter_id = json["id"];

                console.log(matter_id);

                //ajax call to update debit_credit_note table - paid = 1

                Swal.fire({
                    title: "Are you sure?",
                    text: "You want to Change Matters Status ?",
                    icon: "question",
                    showCancelButton: true,
                    confirmButtonText: "YES!",
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        //$.blockUI();

                        oTable.fnDeleteRow(nRow);
                        //Ajax to flag as deleted
                        var parameters = { module: 'open_matter_status', id: matter_id };
                        $.ajax({
                            url: "/Matters/UpdateMatters",
                            type: "POST",
                            data: parameters,
                            success: function (data) {
                                Swal.fire({
                                    title: "Confirmed",
                                    text: "Matter has been Closed",
                                    icon: "success",
                                    confirmButtonText: "Ok"
                                });

                                GetMattersRecords();
                            },
                            error: function (xhr, textStatus, errorThrown) {
                                //$.unblockUI();

                                Swal.fire({
                                    title: "Failed",
                                    text: "Matter could not be updated " + errorThrown,
                                    icon: "error",
                                    confirmButtonText: "Ok"
                                });
                            }
                        });
                    } else {
                        e.preventDefault();
                    }
                });
            });


            var isEditing = null;

            //Edit
            $('#editabledatatable').on("click", 'a.edit', function (e) {
                e.preventDefault();

                nRow = $(this).parents('tr')[0];

                if (isEditing !== null && isEditing != nRow) {
                    //restoreRow(oTable, isEditing);
                    editRow(oTable, nRow);
                    isEditing = nRow;
                } else {
                    editRow(oTable, nRow);
                    isEditing = nRow;
                }
            });

            function editRow(oTable, nRow) {
                var aData = oTable.fnGetData(nRow);
                var jqTds = $('>td', nRow);

                var json = JSON.parse(JSON.stringify(aData));
                console.log(json);

                $('.modal-body #recordid').val($(nRow).attr("recid"));
                $('.modal-body #client_id').val(json["client_id"]);
                $('.modal-body #client_name').val(json["client_name"]);
                $('.modal-body #mobile').val(json["phone_number"]);
                $('.modal-body #email').val(json["email"]);
                $('.modal-body #id_no').val(json["id_no"]);
                $('.modal-body #product').val(json["product_id"]).trigger("change");
                $('.modal-body #product_total').val(json["quantity"]);
                $('.modal-body #price').val(json["price"]);
                $('.modal-body #start_date').val(json["start_date"]);
                $('.modal-body #return_date').val(json["end_date"]);

                $("#capture-matters").appendTo("body").modal("show");
            }

            //Delete an Existing Row
            $('#editabledatatable').on("click", 'a.delete', function (e) {
                e.preventDefault();

                var a = $(this).closest(".panel");

                var nRow = $(this).parents('tr')[0];

                var rec = $(this).parents('tr').attr("recid");

                //console.log($(this).parents('tr').attr("recid"));
                Swal.fire({
                    title: "Are you sure?",
                    text: "You want to delete this record",
                    icon: "question",
                    showCancelButton: true,
                    confirmButtonText: "Proceed!",
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {

                        oTable.fnDeleteRow(nRow);
                        //Ajax to flag as deleted
                        var parameters = { module: 'purchase_product', id: rec };
                        console.log(parameters);
                        $.ajax({
                            url: "/Management/Delete",
                            type: "POST",
                            data: parameters,
                            beforeSend: function () {
                                if (!$(a).hasClass("panel-loading")) {
                                    var t = $(a).find(".panel-body"),
                                        i = '<div class="panel-loader"><span class="spinner-small"></span></div>';

                                    $(a).addClass("panel-loading"), $(t).prepend(i);
                                }
                            },
                            success: function (data) {
                                $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                                Swal.fire({
                                    title: "Deleted",
                                    text: "Record has been deleted",
                                    icon: "success",
                                    confirmButtonText: "Ok"
                                });
                                GetTopics();
                            },
                            error: function (xhr, textStatus, errorThrown) {
                                $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                                Swal.fire({
                                    title: "Failed",
                                    text: "Operation could not be completed " + errorThrown,
                                    icon: "error",
                                    confirmButtonText: "Ok"
                                });
                            }
                        });
                    } else {
                        e.preventDefault();
                    }
                });
            });
        }
    };
}();
$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var client_name = document.getElementById('client_name').value;
    var mobile = document.getElementById('mobile').value;
    var email = document.getElementById('email').value;
    var id_no = document.getElementById('id_no').value;


    var product = document.getElementById('product').value;
    var product_total = document.getElementById('product_total').value;
    var price = document.getElementById('price').value;
    var start_date = document.getElementById('start_date').value;
    var return_date = document.getElementById('return_date').value;

    var parameters = {
        id: id,
        client_name: client_name,
        mobile: mobile,
        email: email,
        id_no: id_no, 

        product: product,
        product_total: product_total,
        price: price,
        start_date: start_date,
        return_date: return_date
    };
    console.log(parameters);

    $.ajax({
        url: "/ClientManagement/ProductPurchase",
        type: "POST",
        data: parameters,
        beforeSend: function () {
            if (!$(a).hasClass("panel-loading")) {
                var t = $(a).find(".panel-body"),
                    i = '<div class="panel-loader"><span class="spinner-small"></span></div>';

                $(a).addClass("panel-loading"), $(t).prepend(i);
            }
        },
        success: function (data) {
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();
            $("#capture-matters").modal("hide").data("bs.modal", null);
            GetPurchaseRecords();
            if (data.error_code === '00') {
                Swal.fire({
                    title: "Success",
                    text: data.error_desc,
                    icon: "success",
                    confirmButtonText: "Ok"
                });


                

            } else {
                Swal.fire({
                    title: "Failed",
                    text: data.error_desc,
                    icon: "error",
                    confirmButtonText: "Ok"
                });
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

            Swal.fire({
                title: "Failed",
                text: "Record could not be saved " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
});

$('#stk_payment').click(function () {
    var a = $(this).closest(".panel");

    var m_amount = document.getElementById('m_amount').value;
    var m_mobile = document.getElementById('m_mobile').value;

    var parameters = {
        amount: m_amount,
        mobile: m_mobile
    };
    console.log(parameters);

    $.ajax({
        url: "/Management/DownloadAction",
        type: "POST",
        data: parameters,
        beforeSend: function () {
            if (!$(a).hasClass("panel-loading")) {
                var t = $(a).find(".panel-body"),
                    i = '<div class="panel-loader"><span class="spinner-small"></span></div>';

                $(a).addClass("panel-loading"), $(t).prepend(i);
            }
        },
        success: function (data) {
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();
            $("#capture-matters").modal("hide").data("bs.modal", null);
            if (data.error_code === '00') {
                Swal.fire({
                    title: "Success",
                    text: data.error_desc,
                    icon: "success",
                    confirmButtonText: "Ok"
                });


                

            } else {
                Swal.fire({
                    title: "Failed",
                    text: data.error_desc,
                    icon: "error",
                    confirmButtonText: "Ok"
                });
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

            Swal.fire({
                title: "Failed",
                text: "Record could not be saved " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
});

$("#capture-matters").on("hidden.bs.modal", function (e) {
    $('#recordid').val("");
    $('#matter_name').val("");
    $('#matter_number').val("");
    $('#assigned_to').val("");
    $('#client_id').val("");
    $('#start_date').val("");
    $('#close_date').val("");
    $('#practice_area').val("");
    $('#matter_status').val("");
    $('#matter_billing').val("");
    $('#description').val("");
});
