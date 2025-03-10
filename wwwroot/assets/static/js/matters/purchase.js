$(document).ready(function () {
    App.init();
    GetPurchaseRecords();
    GetProducts();
    GetClients();

    InitiateEditableDataTable.init();

    // Initialize datepickers
    $('#start_date').datepicker({
        todayHighlight: true,
        startDate: '-6m',
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

    $('#return_date').datepicker({
        todayHighlight: true,
        startDate: '-6m',
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

    $('#actual_return_date').datepicker({
        todayHighlight: true,
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

    $('#bulk_return_date').datepicker({
        todayHighlight: true,
        format: 'yyyy-mm-dd',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        todayBtn: 'linked'
    });

    // Initialize select2 for better dropdown experience
    $("#product").select2({
        placeholder: "Select a product",
        allowClear: true
    });

    $("#bulk_client").select2({
        placeholder: "Select a client",
        allowClear: true
    });

    $("#return_condition").select2({
        minimumResultsForSearch: -1 // Hide search box
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
            var productId = $(this).val();
            if (productId != "-1") {
                // Get product price if needed
                GetProductPrice(productId);
            }
        });
    });
}

function GetProductPrice(productId) {
    $.get('GetRecords', { module: 'product_price', id: productId }, function (data) {
        if (data && data.price) {
            $('#price').val(data.price);
        }
    });
}

function GetClients() {
    $.get('GetRecords', { module: 'client_list' }, function (data) {
        $("#bulk_client").get(0).options.length = 0;
        $("#bulk_client").get(0).options[0] = new Option("Please Select Client ", "-1");

        $.each(data, function (index, item) {
            $("#bulk_client").get(0).options[$("#bulk_client").get(0).options.length] = new Option(item.name, item.id);
        });

        $("#bulk_client").bind("change", function () {
            var clientId = $(this).val();
            if (clientId != "-1") {
                GetClientProducts(clientId);
            } else {
                $("#client_products tbody").empty();
            }
        });
    });
}

function GetClientProducts(clientId) {
    $.get('GetRecords', { module: 'client_products', client_id: clientId }, function (data) {
        $("#client_products tbody").empty();

        $.each(data, function (index, item) {
            var row = '<tr data-id="' + item.id + '">' +
                '<td>' + item.product_name + '</td>' +
                '<td>' + item.quantity + '</td>' +
                '<td><input type="number" class="form-control return-qty" max="' + item.quantity + '" value="' + item.quantity + '"></td>' +
                '<td><select class="form-control product-condition">' +
                '<option value="Good">Good</option>' +
                '<option value="Damaged">Damaged</option>' +
                '<option value="Needs Repair">Needs Repair</option>' +
                '</select></td>' +
                '</tr>';

            $("#client_products tbody").append(row);
        });
    });
}

// Get return details for a specific product
function GetReturnDetails(id) {
    $.get('GetRecords', { module: 'return_details', id: id }, function (data) {
        if (data) {
            // Populate the return details modal
            $('#detail_client_name').text(data.client_name);
            $('#detail_product_name').text(data.product_name);
            $('#detail_original_quantity').text(data.original_quantity);
            $('#detail_returned_quantity').text(data.returned_quantity);
            $('#detail_return_date').text(data.return_date);
            $('#detail_condition').text(data.condition);
            $('#detail_processed_by').text(data.processed_by || 'System');
            $('#detail_notes').text(data.notes || 'No notes provided');

            // Show the modal
            $("#return-details").modal("show");
        } else {
            Swal.fire({
                title: "Error",
                text: "Could not retrieve return details",
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
}

// Calculate total price based on quantity and unit price
function calculateTotalPrice() {
    var quantity = parseInt($('#product_total').val()) || 0;
    var unitPrice = parseFloat($('#unit_price').val()) || 0;
    var totalPrice = quantity * unitPrice;

    $('#price').val(totalPrice.toFixed(2));
}

// Attach event listeners for price calculation
$('#product_total, #unit_price').on('input', function () {
    calculateTotalPrice();
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
                    { "data": "client_name", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "product_name", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "start_date", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "end_date", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "quantity", "autoWidth": true, "sDefaultContent": "n/a" },
                    { "data": "price", "autoWidth": true, "sDefaultContent": "n/a" },
                    {
                        "data": "is_returned",
                        "render": function (data, type, row) {
                            if (data === true || data === "1" || data === 1) {
                                return '<span class="label label-success">Returned</span>';
                            } else {
                                return '<span class="label label-warning">Active</span>';
                            }
                        },
                        "autoWidth": true
                    },
                    {
                        "bSortable": false,
                        "render": function (data, type, row) {
                            if (row.is_returned === true || row.is_returned === "1" || row.is_returned === 1) {
                                return '<a href="#" class="btn btn-success btn-xs view-details2"></i> Returned</a>';
                            } else {
                                return '<a href="#" class="btn btn-info btn-xs return"><i class="fa fa-undo"></i> Return</a> ' +
                                    '<a href="#" class="btn btn-danger btn-xs delete"><i class="fas fa-trash-alt"></i> Delete</a>';
                            }
                        }
                    }
                ],
                "order": [[2, "desc"]], // Sort by assigned date by default
                "language": {
                    "emptyTable": "No products assigned to clients",
                    "zeroRecords": "No matching records found",
                    "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                    "infoEmpty": "Showing 0 to 0 of 0 entries",
                    "infoFiltered": "(filtered from _MAX_ total entries)",
                    "search": "Search:",
                    "paginate": {
                        "first": "First",
                        "last": "Last",
                        "next": "Next",
                        "previous": "Previous"
                    }
                }
            });

            // Return Product Button Click
            $('#editabledatatable').on("click", 'a.return', function (e) {
                e.preventDefault();

                nRow = $(this).parents('tr')[0];
                var aData = oTable.fnGetData(nRow);
                var json = JSON.parse(JSON.stringify(aData));

                // Populate the return modal with data
                $('.modal-body #return_record_id').val($(nRow).attr("recid"));
                $('.modal-body #return_client_name').val(json["client_name"]);
                $('.modal-body #return_product_name').val(json["product_name"]);
                $('.modal-body #return_quantity').val(json["quantity"]);
                $('.modal-body #actual_return_date').val(new Date().toISOString().split('T')[0]);

                // Show the return modal
                $("#return-product").appendTo("body").modal("show");
            });

            // View Return Details Button Click
            $('#editabledatatable').on("click", 'a.view-details', function (e) {
                e.preventDefault();

                nRow = $(this).parents('tr')[0];
                var recordId = $(nRow).attr("recid");

                // Get and display return details
                GetReturnDetails(recordId);
            });

            // Delete Button Click
            $('#editabledatatable').on("click", 'a.delete', function (e) {
                e.preventDefault();

                var a = $(this).closest(".panel");
                var nRow = $(this).parents('tr')[0];
                var rec = $(this).parents('tr').attr("recid");

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
                                GetPurchaseRecords();
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

// Save Product Issue
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

    // Validation
    if (!client_name) {
        Swal.fire({
            title: "Warning",
            text: "Please enter client name",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    if (product == "-1") {
        Swal.fire({
            title: "Warning",
            text: "Please select a product",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    if (!product_total || product_total <= 0) {
        Swal.fire({
            title: "Warning",
            text: "Please enter a valid quantity",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    if (!start_date) {
        Swal.fire({
            title: "Warning",
            text: "Please select a pick date",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

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

// Save Product Return
$('#save_return').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('return_record_id').value;
    var return_quantity = document.getElementById('return_quantity').value;
    var actual_return_date = document.getElementById('actual_return_date').value;
    var return_condition = document.getElementById('return_condition').value;
    var return_notes = document.getElementById('return_notes').value;

    // Validation
    if (!return_quantity || return_quantity <= 0) {
        Swal.fire({
            title: "Warning",
            text: "Please enter a valid return quantity",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    if (!actual_return_date) {
        Swal.fire({
            title: "Warning",
            text: "Please select a return date",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    var parameters = {
        id: id,
        return_quantity: return_quantity,
        actual_return_date: actual_return_date,
        return_condition: return_condition,
        return_notes: return_notes
    };
    console.log(parameters);

    $.ajax({
        url: "/ClientManagement/ReturnProduct",
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
            $("#return-product").modal("hide").data("bs.modal", null);
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
                text: "Product return could not be processed " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
});

// Save Bulk Product Return
$('#save_bulk_return').click(function () {
    var a = $(this).closest(".panel");
    var clientId = $('#bulk_client').val();
    var returnDate = $('#bulk_return_date').val();
    var notes = $('#bulk_return_notes').val();

    if (clientId == "-1") {
        Swal.fire({
            title: "Warning",
            text: "Please select a client",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    if (!returnDate) {
        Swal.fire({
            title: "Warning",
            text: "Please select a return date",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    var returnItems = [];
    $('#client_products tbody tr').each(function () {
        var productId = $(this).data('id');
        var returnQty = $(this).find('.return-qty').val();
        var condition = $(this).find('.product-condition').val();

        if (returnQty > 0) {
            returnItems.push({
                id: productId,
                return_quantity: returnQty,
                return_condition: condition
            });
        }
    });

    if (returnItems.length === 0) {
        Swal.fire({
            title: "Warning",
            text: "No products selected for return",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    var parameters = {
        client_id: clientId,
        return_date: returnDate,
        notes: notes,
        return_items: JSON.stringify(returnItems)
    };
    console.log(parameters);

    $.ajax({
        url: "/ClientManagement/BulkReturnProducts",
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
            $("#return-product-bulk").modal("hide").data("bs.modal", null);
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
                text: "Bulk product return could not be processed " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
});

// Payment Processing
$('#stk_payment').click(function () {
    var a = $(this).closest(".panel");

    var m_amount = document.getElementById('m_amount').value;
    var m_mobile = document.getElementById('m_mobile').value;

    // Validation
    if (!m_amount || m_amount <= 0) {
        Swal.fire({
            title: "Warning",
            text: "Please enter a valid amount",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    if (!m_mobile) {
        Swal.fire({
            title: "Warning",
            text: "Please enter a mobile number",
            icon: "warning",
            confirmButtonText: "Ok"
        });
        return;
    }

    // Format mobile number if needed
    if (m_mobile.length === 10 && m_mobile.startsWith('0')) {
        m_mobile = '254' + m_mobile.substring(1);
    }

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
            $("#payment").modal("hide").data("bs.modal", null);
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
                text: "Payment could not be processed " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
});

// Export to Excel
$('#export_excel').click(function () {
    window.location.href = '/ClientManagement/ExportProductsToExcel';
});

// Print Report
$('#print_report').click(function () {
    window.open('/ClientManagement/PrintProductReport', '_blank');
});

// Reset forms when modals are closed
$("#capture-matters").on("hidden.bs.modal", function (e) {
    $('#recordid').val("");
    $('#client_name').val("");
    $('#mobile').val("");
    $('#email').val("");
    $('#id_no').val("");
    $('#product').val("-1").trigger('change');
    $('#product_total').val("");
    $('#price').val("");
    $('#start_date').val("");
    $('#return_date').val("");
});

$("#return-product").on("hidden.bs.modal", function (e) {
    $('#return_record_id').val("");
    $('#return_client_name').val("");
    $('#return_product_name').val("");
    $('#return_quantity').val("");
    $('#actual_return_date').val("");
    $('#return_condition').val("Good").trigger('change');
    $('#return_notes').val("");
});

$("#return-product-bulk").on("hidden.bs.modal", function (e) {
    $('#bulk_client').val("-1").trigger('change');
    $('#bulk_return_date').val("");
    $('#bulk_return_notes').val("");
    $("#client_products tbody").empty();
});

$("#payment").on("hidden.bs.modal", function (e) {
    $('#m_amount').val("");
    $('#m_mobile').val("");
});

// Search functionality
$('#search_button').click(function () {
    var searchTerm = $('#search_input').val().toLowerCase();

    $('#editabledatatable').DataTable().search(searchTerm).draw();
});

// Clear search
$('#clear_search').click(function () {
    $('#search_input').val('');
    $('#editabledatatable').DataTable().search('').draw();
});

// Filter by date range
$('#filter_date_range').click(function () {
    var startDate = $('#filter_start_date').val();
    var endDate = $('#filter_end_date').val();

    if (startDate && endDate) {
        // Custom filtering function
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var assignedDate = new Date(data[2]); // Index 2 is the assigned date column
                var start = new Date(startDate);
                var end = new Date(endDate);

                if (assignedDate >= start && assignedDate <= end) {
                    return true;
                }
                return false;
            }
        );

        // Apply filter
        $('#editabledatatable').DataTable().draw();

        // Remove the filter after it's applied
        $.fn.dataTable.ext.search.pop();
    }
});

// Filter by return status
$('#filter_status').change(function () {
    var status = $(this).val();

    if (status === 'all') {
        $('#editabledatatable').DataTable().column(6).search('').draw();
    } else if (status === 'returned') {
        $('#editabledatatable').DataTable().column(6).search('Returned').draw();
    } else if (status === 'active') {
        $('#editabledatatable').DataTable().column(6).search('Active').draw();
    }
});

// Refresh data
$('#refresh_data').click(function () {
    GetPurchaseRecords();
});

// Handle quantity change to update price
$('#product_total').on('input', function () {
    var quantity = parseInt($(this).val()) || 0;
    var productId = $('#product').val();

    if (productId != "-1") {
        $.get('GetRecords', { module: 'product_price', id: productId }, function (data) {
            if (data && data.unit_price) {
                var unitPrice = parseFloat(data.unit_price);
                var totalPrice = quantity * unitPrice;
                $('#price').val(totalPrice.toFixed(2));
            }
        });
    }
});

// Initialize tooltips
$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

// Initialize popovers
$(function () {
    $('[data-toggle="popover"]').popover();
});

// Handle client search in the issue product modal
$('#search_client').on('input', function () {
    var searchTerm = $(this).val().toLowerCase();

    if (searchTerm.length >= 3) {
        $.get('GetRecords', { module: 'search_client', term: searchTerm }, function (data) {
            if (data && data.length > 0) {
                var client = data[0]; // Take the first match
                $('#client_name').val(client.name);
                $('#mobile').val(client.phone_number);
                $('#email').val(client.email);
                $('#id_no').val(client.id_no);
                $('#client_id').val(client.id);
            }
        });
    }
});

// Handle product barcode scanning
$('#barcode_scanner').on('input', function () {
    var barcode = $(this).val();

    if (barcode.length > 0) {
        $.get('GetRecords', { module: 'product_by_barcode', barcode: barcode }, function (data) {
            if (data) {
                $('#product').val(data.id).trigger('change');
                $('#barcode_scanner').val('');
            }
        });
    }
});

// Handle bulk selection of products for return
$('#select_all_products').change(function () {
    if ($(this).is(':checked')) {
        $('.product-checkbox').prop('checked', true);
    } else {
        $('.product-checkbox').prop('checked', false);
    }
});

// Generate receipt after successful purchase
function generateReceipt(purchaseId) {
    window.open('/ClientManagement/GenerateReceipt?id=' + purchaseId, '_blank');
}

// Generate return receipt
function generateReturnReceipt(returnId) {
    window.open('/ClientManagement/GenerateReturnReceipt?id=' + returnId, '_blank');
}

// Send SMS notification to client
function sendSmsNotification(clientPhone, message) {
    var parameters = {
        phone: clientPhone,
        message: message
    };

    $.ajax({
        url: "/ClientManagement/SendSms",
        type: "POST",
        data: parameters,
        success: function (data) {
            if (data.error_code === '00') {
                Swal.fire({
                    title: "Success",
                    text: "SMS notification sent successfully",
                    icon: "success",
                    confirmButtonText: "Ok"
                });
            } else {
                Swal.fire({
                    title: "Failed",
                    text: "SMS notification could not be sent: " + data.error_desc,
                    icon: "error",
                    confirmButtonText: "Ok"
                });
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            Swal.fire({
                title: "Failed",
                text: "SMS notification could not be sent: " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
}

// Send email notification to client
function sendEmailNotification(clientEmail, subject, message) {
    var parameters = {
        email: clientEmail,
        subject: subject,
        message: message
    };

    $.ajax({
        url: "/ClientManagement/SendEmail",
        type: "POST",
        data: parameters,
        success: function (data) {
            if (data.error_code === '00') {
                Swal.fire({
                    title: "Success",
                    text: "Email notification sent successfully",
                    icon: "success",
                    confirmButtonText: "Ok"
                });
            } else {
                Swal.fire({
                    title: "Failed",
                    text: "Email notification could not be sent: " + data.error_desc,
                    icon: "error",
                    confirmButtonText: "Ok"
                });
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            Swal.fire({
                title: "Failed",
                text: "Email notification could not be sent: " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
}

// Generate overdue returns report
$('#overdue_returns_report').click(function () {
    window.open('/ClientManagement/OverdueReturnsReport', '_blank');
});

// Generate inventory status report
$('#inventory_status_report').click(function () {
    window.open('/ClientManagement/InventoryStatusReport', '_blank');
});

// Handle automatic SMS reminder for overdue returns
function setupOverdueReminders() {
    $.get('GetRecords', { module: 'overdue_returns' }, function (data) {
        if (data && data.length > 0) {
            $.each(data, function (index, item) {
                var message = "Dear " + item.client_name + ", please note that your product '" +
                    item.product_name + "' was due for return on " + item.end_date +
                    ". Please return it as soon as possible to avoid additional charges.";

                sendSmsNotification(item.phone_number, message);
            });
        }
    });
}

// Check for product availability before issuing
$('#product').change(function () {
    var productId = $(this).val();

    if (productId != "-1") {
        $.get('GetRecords', { module: 'product_availability', id: productId }, function (data) {
            if (data && data.available_quantity !== undefined) {
                if (data.available_quantity <= 0) {
                    Swal.fire({
                        title: "Warning",
                        text: "This product is out of stock!",
                        icon: "warning",
                        confirmButtonText: "Ok"
                    });
                    $('#product_total').attr('max', 0);
                } else {
                    $('#product_total').attr('max', data.available_quantity);
                    if (data.available_quantity < 5) {
                        Swal.fire({
                            title: "Low Stock",
                            text: "Only " + data.available_quantity + " units available",
                            icon: "info",
                            confirmButtonText: "Ok"
                        });
                    }
                }
            }
        });
    }
});

// Document ready function to initialize additional features
$(document).ready(function () {
    // Set up periodic check for overdue returns
    setInterval(function () {
        setupOverdueReminders();
    }, 24 * 60 * 60 * 1000); // Check once a day

    // Initialize date range picker for reports
    $('.daterange').daterangepicker({
        opens: 'left',
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear',
            format: 'YYYY-MM-DD'
        }
    });

    $('.daterange').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('YYYY-MM-DD') + ' - ' + picker.endDate.format('YYYY-MM-DD'));
    });

    $('.daterange').on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
    });
});