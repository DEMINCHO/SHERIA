
$(document).ready(function () { 
    App.init();

    PatientFormWizard.init();

    $('.selectpicker').select2({
        style: 'btn-white',
        size: 5
    });

    $("#client_type").bind("change", function () {

        var x = document.getElementById("ind_div");
        var xx = document.getElementById("confirm_ind_div");
        var y = document.getElementById("comp_div");
        var yy = document.getElementById("confirm_comp_div");

        var str = $("#client_type option:selected").val();

        if (str == 'IND') {
            x.style.display = "block";
            xx.style.display = "block";
            y.style.display = "none";
            yy.style.display = "none";
        }
        else {
            x.style.display = "none";
            xx.style.display = "none";
            y.style.display = "block";
            yy.style.display = "block";

        }
    });

});


var applicants = [];


var handlePatientWizards = function () {
    "use strict";
    $("#wizard").bwizard({
        validating: function (e, ui) {
            if (ui.index === 0) {
                //Validate Controls
                // step-1 client Details
                if (false === $('form[name="form-wizard"]').parsley().validate("wizard-step-1")) {
                    return false;
                } else {

                    /***** Applicant details confirmation *****/
                    document.getElementById('confirm_first_name').value = document.getElementById('first_name').value;
                    document.getElementById('confirm_middle_name').value = document.getElementById('middle_name').value;
                    document.getElementById('confirm_last_name').value = document.getElementById('last_name').value;
                    document.getElementById('confirm_id_number').value = document.getElementById('id_number').value;
                    document.getElementById('confirm_client_type').value = $("#client_type option:selected").text();
                    document.getElementById('confirm_id_number').value = document.getElementById('id_number').value;
                    document.getElementById('confirm_kra_pin').value = document.getElementById('kra_pin').value;
                    document.getElementById('confirm_nationality').value = document.getElementById('nationality').value;
                    document.getElementById('confirm_company_name').value = document.getElementById('company_name').value;
                    document.getElementById('confirm_cert_of_incoporation').value = document.getElementById('cert_of_incoporation').value;
                    document.getElementById('confirm_country').value = document.getElementById('country').value;

                    /***** Applicant details confirmation *****/

                }
            } else if ((ui.index === 1) && (ui.nextIndex > ui.index)) {
                // step-2 Address Details
                if (false === $('form[name="form-wizard"]').parsley().validate("wizard-step-2")) {
                    return false;
                } else {

                    /***** Applicant details confirmation *****/

                    document.getElementById('confirm_phone_number').value = document.getElementById('phone_number').value;
                    document.getElementById('confirm_email').value = document.getElementById('email').value;
                    document.getElementById('confirm_sec_phone_number').value = document.getElementById('sec_phone_number').value;
                    document.getElementById('confirm_postal_address').value = document.getElementById('postal_address').value;
                    document.getElementById('confirm_physical_address').value = document.getElementById('physical_address').value;

                    /***** Applicant details confirmation *****/

                }
            } else if ((ui.index === 2) && (ui.nextIndex > ui.index)) {
                // step-3 Uploads
                if (false === $('form[name="form-wizard"]').parsley().validate("wizard-step-3")) {
                    return false;
                } else {

                   
                }
            } else if ((ui.index === 3) && (ui.nextIndex > ui.index)) {
                //step-4 Confirm Details
                if (false === $('form[name="form-wizard"]').parsley().validate("wizard-step-4")) {
                    return false;
                } else {
                    var a = $(this).closest(".panel");

                    Swal.fire({
                        title: "Are you sure?",
                        text: "you want to proceed with onboarding?",
                        icon: "question",
                        showCancelButton: true,
                        confirmButtonText: "Proceed",
                        reverseButtons: true
                    }).then((result) => {
                        if (result.isConfirmed) {
                            var client_type = $("#client_type option:selected").text();;
                            var first_name = document.getElementById('first_name').value;
                            var middle_name = document.getElementById('middle_name').value;
                            var last_name = document.getElementById('last_name').value;
                            var id_number = document.getElementById('id_number').value;
                            var kra_pin = document.getElementById('kra_pin').value;
                            var nationality = document.getElementById('nationality').value;
                            var company_name = document.getElementById('company_name').value;
                            var cert_of_incoporation = document.getElementById('cert_of_incoporation').value;
                            var country = document.getElementById('country').value;
                            var remarks = document.getElementById('remarks').value;
                            var phone_number = document.getElementById('phone_number').value;
                            var sec_phone_number = document.getElementById('sec_phone_number').value;
                            var email = document.getElementById('email').value;
                            var physical_address = document.getElementById('physical_address').value;
                            var postal_address = document.getElementById('postal_address').value;

                           
                            var cnt = applicants.length;

                            var applicant =
                            {
                                id: cnt + 1,
                                client_type: client_type,
                                first_name: first_name,
                                middle_name: middle_name,
                                last_name: last_name,
                                id_number: id_number,
                                kra_pin: kra_pin,
                                nationality: nationality,
                                company_name: company_name,
                                cert_of_incoporation: cert_of_incoporation,
                                country: country,
                                remarks: remarks,
                                phone_number: phone_number,
                                sec_phone_number: sec_phone_number,
                                email: email,
                                physical_address: physical_address,
                                postal_address: postal_address 
                            };

                            //console.log(applicant);

                            //if (document.getElementById('applicantrecordid').value > 0) {
                            //    const index = applicants.findIndex(item => item.id === document.getElementById('applicantrecordid').value);
                            //    applicants.splice(index, 1);
                            //}

                            applicants.push(applicant);

                            const container = document.getElementById('uploadedFiles');
                            const client_files = container.textContent.trim();

                            console.log(client_files);

                            var parameters = {
                                applicant_details: applicants,
                                client_files: client_files
                            };
                            //console.log(parameters);
                            $.ajax({
                                url: "/ClientManagement/OnboardClient",
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
                                    document.getElementById("summary_system_reference").innerHTML = data.system_ref;
                                    document.getElementById("account_number").innerHTML = data.account_number;

                                    var buttons = document.getElementsByClassName("previous");

                                    for (var i = 0; i < buttons.length; i++) {
                                        buttons[i].setAttribute("aria-disabled", "true");
                                        buttons[i].setAttribute("class", "previous disabled");
                                    }

                                    if (data.error_code === '00') {
                                        document.getElementById("summary_status").innerHTML = "Success";
                                        document.getElementById("summary_status").classList = "label label-success";
                                        Swal.fire({
                                            title: "Success",
                                            text: data.error_desc,
                                            icon: "success",
                                            confirmButtonText: "Ok"
                                        });
                                    } else {
                                        document.getElementById("summary_status").innerHTML = "Failed";
                                        document.getElementById("summary_status").classList = "label label-danger";
                                        Swal.fire({
                                            title: "Failed",
                                            text: data.error_desc,
                                            icon: "error",
                                            confirmButtonText: "Ok"
                                        });
                                    }

                                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();
                                }
                            });
                        } else {
                            document.getElementById("summary_system_reference").innerHTML = "-";
                            document.getElementById("summary_status").innerHTML = "Cancelled";
                            document.getElementById("summary_status").classList = "label label-info";

                            Swal.fire({
                                title: "Cancelled",
                                text: "Registration has been cancelled",
                                icon: "info",
                                confirmButtonText: "Ok"
                            });
                        }
                    });
                }
            }
        }
    });
};

var PatientFormWizard = function () {
    "use strict";
    return {
        init: function () {
            handlePatientWizards();
        }
    };
}();