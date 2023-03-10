var CurrentPage = function() {

    var handleEmailActivation = function() {

        var $form = $(".email-activation-form");

        $form.validate({
            errorElement: "span", //default input error message container
            errorClass: "help-block", // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {
            
            },

            invalidHandler: function(event, validator) {

            },

            highlight: function(element) {
                $(element).closest(".form-group").addClass("has-error");
            },

            success: function(label) {
                label.closest(".form-group").removeClass("has-error");
                label.remove();
            },

            errorPlacement: function(error, element) {
                error.insertAfter(element);
            },

            submitHandler: function(form) {
                $form.find(".alert-danger").hide();
            }
        });

        $form.find("input").keypress(function(e) {
            if (e.which === 13) {
                if ($(".forget-form").valid()) {
                    $(".forget-form").submit();
                }
            }
        });

        $form.submit(function(e) {
            e.preventDefault();

            if (!$form.valid()) {
                return;
            }

            abp.ui.setBusy(
                null,
                abp.ajax({
                    contentType: app.consts.contentTypes.formUrlencoded,
                    url: $form.attr("action"),
                    data: $form.serialize()
                }).done(function() {
                    abp.message.success(app.localize("ActivationMailSentMessage"), app.localize("MailSent"))
                    //abp.message.success("已向您的邮箱发送了一封邮箱地址验证邮件，请查收.", "邮件已发送")
                        .done(function() {
                            location.href = abp.appPath + "Account/Login";
                        });
                })
            );
        });
    };
    return {
        init: function() {
            handleEmailActivation();
        }
    };
}();