$(document).ready(function () {

    App.init();

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

    $('#due_date').datepicker({
        todayHighlight: true,
        startDate: '-6m',
        //endDate: '0',
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

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
                    "columnDefs": [
                        {
                            "targets": 5,
                            "render": function (data, type, row, meta) {
                                if (row.status === 'Pending') {
                                    return '<a href="#" class="btn btn-warning btn-xs flagclosed">Pending</a>';
                                } else if (row.status === 'Done') {
                                    return '<a href="#" class="btn btn-success btn-xs flagclosed">Done</a>';
                                } else
                                {
                                    return '<a href="#" class="btn btn-danger btn-xs flagunOpen"> Overdue</a>';
                                }
                            }
                        }
                    ],

                    "aoColumns": [
                        { "data": "task_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "start_date", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "due_date", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "priority", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "status", "autoWidth": true, "sDefaultContent": "n/a" },

                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs edit'><i class='fa fa-edit'></i> Edit</a>"
                        },
                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-danger btn-xs delete'><i class='fas fa-trash-alt'></i> Delete</a>"
                        }
                    ],

                   

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
                    $('.modal-body #task_name').val(json["task_name"]);
                    $('.modal-body #matter').val(json["matter_id"]);
                    $('.modal-body #start_date').val(json["start_date"]);
                    $('.modal-body #due_date').val(json["due_date"]);
                    $('.modal-body #priority').val(json["priority"]);
                    $('.modal-body #assigned_to').val(json["assigned_to"]);
                    $('.modal-body #task_status').val(json["status"]);
                    $('.modal-body #description').val(json["description"]);

                    $("#capture-tasks").appendTo("body").modal("show");
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
                            var parameters = { module: 'task_record', id: rec };
                            console.log(parameters);
                            $.ajax({
                                url: "/TasksManagement/Delete",
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

    GetTasks();
    GetMatter();
    GetLawyer();

});


function GetTasks() {
    $.get('GetRecords', { module: 'tasks' }, function (data) {
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


function GetMatter() {
    $.get('GetRecords', { module: 'matters' }, function (data) {
        $("#matter").get(0).options.length = 0;
        $("#matter").get(0).options[0] = new Option("Please Select matter ", "-1");

        $.each(data, function (index, item) {
            $("#matter").get(0).options[$("#matter").get(0).options.length] = new Option(item.matter_name, item.id);
        });

        $("#matter").bind("change", function () {

        });
    });
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



$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var task_name = document.getElementById('task_name').value;
    var matter_id = document.getElementById('matter').value;
    var start_date = document.getElementById('start_date').value;
    var due_date = document.getElementById('due_date').value;
    var priority = document.getElementById('priority').value;
    var task_status = document.getElementById('task_status').value;
    var assigned_to = document.getElementById('assigned_to').value;
    var description = document.getElementById('description').value;

    var parameters = {
        id: id,
        task_name: task_name,
        matter_id: matter_id,
        start_date: start_date,
        due_date: due_date,
        priority: priority,
        task_status: task_status,
        assigned_to: assigned_to,
        description: description
    };

    console.log(parameters);

    $.ajax({
        url: "/TasksManagement/CreateTasks",
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

                $("#capture-tasks").modal("hide").data("bs.modal", null);

                GetTasks();

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

$("#capture-tasks").on("hidden.bs.modal", function (e) {
    $('#recordid').val("");
    $('#task_name').val("");
    $('#matter').val("");
    $('#start_date').val("");
    $('#due_date').val("");
    $('#priority').val("");
    $('#assigned_to').val("");
    $('#task_status').val("");
    $('#description').val("");
});