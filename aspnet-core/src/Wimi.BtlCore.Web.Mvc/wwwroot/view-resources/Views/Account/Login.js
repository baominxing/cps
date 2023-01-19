var CurrentPage = function () {

    var handleLogin = function () {

        var $loginForm = $(".login-form");

        $loginForm.validate({
            errorElement: "span", //default input error message container
            errorClass: "help-block", // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                username: {
                    required: true
                },
                password: {
                    required: true
                }
            },

            invalidHandler: function (event, validator) {
                $loginForm.find(".alert-danger").show();
            },

            highlight: function (element) {
                $(element).closest(".form-group").addClass("has-error");
            },

            success: function (label) {
                label.closest(".form-group").removeClass("has-error");
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element.closest(".input-icon"));
            },

            submitHandler: function (form) {
                $loginForm.find(".alert-danger").hide();
            }
        });

        $loginForm.find("input").keypress(function (e) {
            if (e.which === 13) {
                if ($(".login-form").valid()) {
                    $(".login-form").submit();
                }
            }
        });

        $loginForm.submit(function (e) {
            e.preventDefault();

            if (!$(".login-form").valid()) {
                return;
            }

            abp.ui.setBusy(
                null,
                abp.ajax({
                    contentType: app.consts.contentTypes.formUrlencoded,
                    url: $loginForm.attr("action"),
                    data: $loginForm.serialize()
                })
            );
        });

        $loginForm.find("input[name=returnUrlHash]").val(location.hash);

        $("input[type=text]").first().focus();
    };
    return {
        init: function () {

            handleLogin();
        }
    };

}();