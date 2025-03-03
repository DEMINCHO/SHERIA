$(document).ready(function () {

    App.init();
    //TableManageResponsive.init();

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
                        { "data": "matter_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "matter_number", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "assigned_to", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "client_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "start_date", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "close_date", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "practice_area", "autoWidth": true, "sDefaultContent": "n/a" },

                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs edit'><i class='fa fa-edit'></i> Edit</a>"
                        },
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


                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #matter_name').val(json["matter_name"]);
                    $('.modal-body #matter_number').val(json["matter_number"]);
                    $('.modal-body #assigned_to').val(json["assigned_to"]);
                    $('.modal-body #client_id').val(json["client_id"]);
                    $('.modal-body #start_date').val(json["start_date"]);
                    $('.modal-body #close_date').val(json["close_date"]);
                    $('.modal-body #practice_area').val(json["practice_area"]);
                    $('.modal-body #matter_status').val(json["matter_status"]);
                    $('.modal-body #matter_billing').val(json["matter_billing"]);
                    $('.modal-body #description').val(json["description"]);

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
                            var parameters = { module: 'open_matters', id: rec };
                            console.log(parameters);
                            $.ajax({
                                url: "/Matters/Delete",
                                type: "GET",
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

    $('#close_date').datepicker({
        todayHighlight: true,
        startDate: '-6m',
        //endDate: '0',
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

    GetMattersRecords();
    GetLawyer();
    GetClient()

});


function GetMattersRecords() {
    $.get('GetRecords', { module: 'open_matters_record' }, function (data) {
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


function GetLawyer() {
    $.get('GetRecords', { module: 'portal_users' }, function (data) {
        $("#assigned_to").get(0).options.length = 0;
        $("#assigned_to").get(0).options[0] = new Option("Please Select Lawyer ", "-1");

        $.each(data, function (index, item) {
            $("#assigned_to").get(0).options[$("#assigned_to").get(0).options.length] = new Option(item.name, item.id);
        });

        $("#assigned_to").bind("change", function () {

        });
    });
}

function GetClient(){
    $.get('GetRecords', { module: 'client_record' }, function (data) {
        $("#client_id").get(0).options.length = 0;
        $("#client_id").get(0).options[0] = new Option("Please Select Client", "-1");

        $.each(data, function (index, item) {
            $("#client_id").get(0).options[$("#client_id").get(0).options.length] = new Option(item.client_name, item.id);
        });

        $("#client_id").bind("change", function () {

        });
    });

}

$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var matter_name = document.getElementById('matter_name').value;
    var matter_number = document.getElementById('matter_number').value;
    var assigned_to = document.getElementById('assigned_to').value;
    var client_id = document.getElementById('client_id').value;
    var start_date = document.getElementById('start_date').value;
    var close_date = document.getElementById('close_date').value;
    var practice_area = document.getElementById('practice_area').value;
    var matter_status = document.getElementById('matter_status').value;
    var matter_billing = document.getElementById('matter_billing').value;
    var description = document.getElementById('description').value;

    var parameters = {
        id: id,
        matter_name: matter_name,
        matter_number: matter_number,
        assigned_to: assigned_to,
        client_id: client_id,
        start_date: start_date,
        close_date: close_date,
        practice_area: practice_area,
        matter_status: matter_status,
        matter_billing: matter_billing,
        description: description
    };
    console.log(parameters);

    $.ajax({
        url: "/Matters/CreateMatters",
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

            if (data.error_code === '00') {
                Swal.fire({
                    title: "Success",
                    text: data.error_desc,
                    icon: "success",
                    confirmButtonText: "Ok"
                });


                $("#capture-matters").modal("hide").data("bs.modal", null);
                GetMattersRecords();

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
