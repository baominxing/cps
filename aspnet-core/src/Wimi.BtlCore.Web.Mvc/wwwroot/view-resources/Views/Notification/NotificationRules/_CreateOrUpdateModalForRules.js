//# sourceURL=dynamic_CreateOrUpdateModalForRules.js
(function () {
    app.modals.CreateOrUpdateModalForRules = function () {
        var TriggerWithTime = "TriggerWithTime";
        var _modalManager;
        var notificationAppService = abp.services.app.notification;
        var createOrUpdateForm = null;
        var isEditMode = $("#IsEditMode").val();
        var triggerType = $("#TriggerType").val();
        var noticeUserIds;

        this.init = function (modalManager) {
            jQuery.validator.addMethod("isNumber", function (value) {
                return isNaN(value);
            }, $.validator.format(app.localize("EnterNumber")));

            jQuery.validator.addMethod("gt", function (value, element, param) {
                return isNaN(value) && value > param;
            }, $.validator.format(app.localize("EnterNumberBigThanZero")));

            notificationAppService.getUserList()
                .done(function (result) {
                    if (result != null && result.length > 0) {

                        var data = _.map(result,
                            function (item) {
                                return { id: item.userId, text: item.userName }
                            });

                        $(".userList").select2({
                            data: data,
                            multiple: true,
                            placeholder: app.localize("PleaseSelect"),
                            language: {
                                noResults: function () {
                                    return app.localize("NoMatchingData");
                                }
                            }
                        });

                        if (isEditMode === "true") {
                            noticeUserIds = $("#NoticeUserIds").val().split(",");
                            $(".userList").val(noticeUserIds).trigger("change");
                            if (triggerType === TriggerWithTime) {
                                $(".triggerCondition").val($("#TriggerCondition").val());
                            } else {

                                $(".shiftSolutionList").val($("#ShiftSolutionId").val());
                                $(".shiftList").val($("#ShiftId").val());
                            }

                            $(".isEnabled").val($("#SelectIsEnabled").val());
                        }

                        $(".select2-container--default").css("width", "100%");
                    }
                });

            _modalManager = modalManager;
            createOrUpdateForm = _modalManager.getModal().find("form[name=CreateOrUpdateForm]");
        };


        this.shown = function () {
            
            $("#ShiftSolutionList").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });

            $("#ShiftList").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });

            $("#IsEnabled").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });

            $(".number").keydown(function (e) {
                var code = parseInt(e.keyCode);
                if (code >= 96 && code <= 105 || code >= 48 && code <= 57 || code == 8) {
                    return true;
                } else {
                    return false;
                }
            })
        };

        this.save = function () {
            if (!createOrUpdateForm.valid()) {
                return false;
            }

            //if (triggerType === TriggerWithTime && $(".triggerCondition").val() <= 0) {
            //    abp.message.error(app.localize("TriggerTimeShouldNotBeLessThanZero"));
            //    return false;
            //}


            var parameters = {
                notificationRuleId: $("#NotificationRuleId").val(),
                shiftSolutionId: $(".shiftSolutionList").val(),
                shiftId: $(".shiftList").val(),
                isEnabled: $(".isEnabled").val(),
                noticeUserIds: $(".userList").val().join(",")
            }

            if (triggerType === TriggerWithTime) {
                parameters.triggerCondition = $(".triggerCondition").val();
            } else {
                parameters.triggerCondition = $(".shiftList").val();
            }


            _modalManager.setBusy(true);

            if (isEditMode === "false") {
                notificationAppService.createNotificationRuleDetail(
                    parameters
                ).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                parameters.id = $("#Id").val();
                notificationAppService.updateNotificationRuleDetail(
                    parameters
                ).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }

        };

        $(".shiftSolutionList").change(function () {
            var id = $("#Id").val();

            notificationAppService.listShift({ id: id, shiftSolutionId: $(".shiftSolutionList").val() }).done(function (result) {
                $(".shiftList").empty();

                for (var i = 0; i < result.length; i++) {
                    $(".shiftList").append("<option value=" + result[i].value + ">" + result[i].name + "</option>");
                }
            });
        });
    };
})();