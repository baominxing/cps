(function ($) {
    app.modals.ResetPasswordModal = function () {

        var _userService = abp.services.app.user;

        var _modalManager;
        var _$userInformationForm = null;

        function _findAssignedRoleNames() {
            var assignedRoleNames = [];

            _modalManager.getModal()
                .find(".user-role-checkbox-list input[type=checkbox]")
                .each(function () {
                    if ($(this).is(":checked")) {   
                        assignedRoleNames.push($(this).attr("name"));
                    }
                });

            return assignedRoleNames;
        }

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _$userInformationForm = _modalManager.getModal().find("form[name=UserInformationsForm]");
            //_$userInformationForm.validate();

            var passwordInputs = _modalManager.getModal().find("input[name=Password], input[name=PasswordRepeat]");
            var passwordInputGroups = passwordInputs.closest(".form-group");

            passwordInputGroups.slideDown("fast");
            if (!_modalManager.getArgs().id) {
                passwordInputs.attr("required", "required");
            }

            _modalManager.getModal()
                .find(".user-role-checkbox-list input[type=checkbox]")
                .change(function () {
                    $("#assigned-role-count").text(_findAssignedRoleNames().length);
                });
        };

        this.save = function () {
            if (!_$userInformationForm.valid()) {
                return;
            }

            var assignedRoleNames = _findAssignedRoleNames();
            var user = _$userInformationForm.serializeFormToObject();

            if (user.Password) {
                if (!user.PasswordRepeat) {
                    abp.message.error(app.localize("EnterPasswordAgain"));
                    return;
                }
                else if (user.PasswordRepeat != user.Password) {
                    abp.message.error(app.localize("PasswordNotSameRepeat"));
                    return;
                }
            }

            if (user.PasswordRepeat) {
                if (!user.Password) {
                    abp.message.error(app.localize("PleaseFirstEnterPassword"));
                    return;
                }
                else if (user.PasswordRepeat != user.Password) {
                    abp.message.error(app.localize("PasswordNotSameRepeat"));
                    return;
                }
            }

            //user.Password = null;
        
            _modalManager.setBusy(true);
            _userService.resetPassword({
                user: user,
            }).done(function () {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.close();
                abp.event.trigger("app.createOrEditUserModalSaved");
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);
