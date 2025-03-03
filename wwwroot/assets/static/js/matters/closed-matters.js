$(document).ready(function () {

    App.init();
    //TableManageResponsive.init();

    

    InitiateEditableDataTable.init();


    GetProductRecords();
    GetProductTypes();


});

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
                    { "data": "name", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "type_name", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "quantity", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "available", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "price_unit", "autoWidth": true, "sDefaultContent": "n/a" },
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
                $('.modal-body #name').val(json["name"]);
                $('.modal-body #type').val(json["type"]).trigger('change');
                $('.modal-body #total').val(json["quantity"]);
                $('.modal-body #price').val(json["price_unit"]);

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


function GetProductRecords() {
    $.get('GetRecords', { module: 'product_records' }, function (data) {
        getData(data);
    });
}
function GetProductTypes() {
    $.get('GetRecords', { module: 'product_type' }, function (data) {
        $("#type").get(0).options.length = 0;
        $("#type").get(0).options[0] = new Option("Please Select Product Type ", "-1");

        $.each(data, function (index, item) {
            $("#type").get(0).options[$("#type").get(0).options.length] = new Option(item.name, item.id);
        });

        $("#type").bind("change", function () {

        });
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


$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var name = document.getElementById('name').value;
    var type = document.getElementById('type').value;
    var total = document.getElementById('total').value;
    var price = document.getElementById('price').value;

    var parameters = {
        id: id,
        name: name,
        type: type,
        total: total,
        price: price

    };
    console.log(parameters);

    $.ajax({
        url: "/Management/CreateProduct",
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
                GetProductRecords();

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

