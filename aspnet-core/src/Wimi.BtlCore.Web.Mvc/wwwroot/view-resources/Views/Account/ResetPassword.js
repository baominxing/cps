var CurrentPage = function() {

    var handleResetPassword = function() {

        $(".pass-reset-form").validate({
            errorElement: "span", //default input error message container
            errorClass: "help-block", // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {
                PasswordRepeat: {
                    equalTo: "#Password"
                }
            },

            messages: {
            
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
                if (element.closest(".input-icon").size() === 1) {
                    error.insertAfter(element.closest(".input-icon"));
                } else {
                    error.insertAfter(element);
                }
            },

            submitHandler: function(form) {
                form.submit();
            }
        });

        $(".pass-reset-form input").keypress(function(e) {
            if (e.which === 13) {
                if ($(".pass-reset-form").valid()) {
                    $(".pass-reset-form").submit();
                }
            }
        });
    };
    return {
        init: function() {
            handleResetPassword();
        }
    };

}();