//# sourceURL=dynamicCreateShiftInfoModal.js

(function () {
    app.modals.CreateShiftInfoModal = function () {

        var _modalManager;
        var shiftAppService = abp.services.app.shift;
        var $form = $('#ShiftInfoForm');
        var shiftInfoTable = $('#ShiftInfoTableEdit');
        var hasZeroDuration = false;
        var largerDuration = false;
        var isNumber = true;

        var daterangepickerOption = function (date) {
            return {
                singleDatePicker: true,
                locale: {
                    format: 'YYYY-MM-DD'
                },
                minDate: date,
                maxDate: moment().add(10, 'y')
            };
        };

        $('.startTime').timepicker({
            showMeridian: false,
            minuteStep: 30,
            defaultTime: '00:00',
            showInputs: false
        });

        $('.endTime').timepicker({
            showMeridian: false,
            minuteStep: 30,
            defaultTime: '00:00',
            showInputs: false

        });

        //$('.startTime').daterangepicker({
        //    "singleDatePicker": true,
        //    locale: {
        //        format: 'YYYY-MM-DD'
        //    },
        //    minDate: moment()
        //});

        //$('.endTime').daterangepicker({
        //    "singleDatePicker": true,
        //    locale: {
        //        format: 'YYYY-MM-DD'
        //    },
        //    minDate: moment()
        //});

        //新增班次按钮逻辑
        var newTableTr;
        $("#AddShiftInfo")
            .on("click",
                function (e) {
                    e.preventDefault();
                    var shiftInfoTableBody = shiftInfoTable.find('.tbody');
                    shiftInfoTableBody.append(newTableTr.replace(/#/g, shiftInfoTable.find('.tr').length));
                    $('#ShiftInfoForm').validate($.WIMI.options.validate);

                    $('.startTime')
                        .timepicker({
                            showMeridian: false,
                            minuteStep: 30,
                            defaultTime: '00:00',
                            showInputs: false

                        });
                    $('.endTime')
                        .timepicker({
                            showMeridian: false,
                            minuteStep: 30,
                            defaultTime: '00:00',
                            showInputs: false
                        });

                    //
                    var body = _modalManager.getModal().find(".modal-body");
                    var footer = _modalManager.getModal().find(".modal-footer");
                    $('.modal-dialog').css("height", body.height() + footer.height());
                });

        //删除班次记录
        shiftInfoTable.on("click", ".btn-delete", function () {
            var $tr = $(this).closest('.tr');
            if ($tr.find(".id").val() !== 0) {
                abp.message.confirm("",app.localize("ConfirmToDeleteThisShift") + "？",
                    function (result) {
                        if (result === true) {

                            //从数据库中删除这笔数据
                            //var param = { Id: $tr.find(".id").val() };

                            //shiftAppService.deleteShiftSolutionItem(param).done(function () {
                            //    abp.notify.success(app.localize("SuccessfullyDeleted"));
                            //});
                            var trIndex = $tr.index();
                            var orginalTrLength = shiftInfoTable.find('.tbody .tr').length;
                            $tr.remove();

                            if (trIndex < orginalTrLength - 1) {
                                var $trs = shiftInfoTable.find('.tbody .tr');

                                $.each($trs, function (index, item) {

                                    var $inputs = $(item).find('input');
                                    $inputs.map(function () {
                                        var orginal = $(this).attr('name');
                                        $(this).attr('name', orginal.replace(/\[\d\]/, '[' + index + ']'));
                                    });
                                });
                            }

                        } else {
                            return;
                        }

                    });
            }
        });

        //根据开始时间和结束时间自动计算有效时长
        shiftInfoTable.on("change", ".startTime", function () {
            var $tr = $(this).closest('tr');
            var startTimeString = $(this).val();
            var endTimeString = $tr.find(".endTime").val();
            var st = new Date();
            st.setHours(startTimeString.split(':')[0]);
            st.setMinutes(startTimeString.split(':')[1]);
            var et = new Date();
            et.setHours(endTimeString.split(':')[0]);
            et.setMinutes(endTimeString.split(':')[1]);

            var shiftTime = (et - st) / 1000 / 3600;
            var value = shiftTime < 0 ? 24 + shiftTime : shiftTime;
            $tr.find(".shiftTime").val(value.toFixed(2));
        });

        //根据开始时间和结束时间自动计算有效时长
        shiftInfoTable.on("change", ".endTime", function () {
            var $tr = $(this).closest('.tr');
            var startTimeString = $tr.find(".startTime").val();
            var endTimeString = $(this).val();
            var st = new Date();
            st.setHours(startTimeString.split(':')[0]);
            st.setMinutes(startTimeString.split(':')[1]);
            var et = new Date();
            et.setHours(endTimeString.split(':')[0]);
            et.setMinutes(endTimeString.split(':')[1]);

            var shiftTime = (et - st) / 1000 / 3600;
            var value = shiftTime < 0 ? 24 + shiftTime : shiftTime;
            $tr.find(".shiftTime").val(value.toFixed(2));
        });

        //check班次名称是否有输入重复
        var names = [];
        shiftInfoTable.on("change", ".name", function () {
            var $trs = shiftInfoTable.find('.tbody .tr');
            $.each($trs, function (index, item) {
                names.push($(item).find(".name").val());
            });
            var name = this.value;
            if ($.grep(names, function (item) { return item === name; }).length > 1) {
                this.value = "";
                abp.message.warn(app.localize("ShiftNameCannotBeRepeated"));
            }
            names.length = 0;
        });
        this.init = function (modalManager) {
            _modalManager = modalManager;

            $form = _modalManager.getModal().find("form[name=ShiftInfoForm]");

        };

        this.save = function () {
            if (!$form.valid()) {
                return;
            }
            hasZeroDuration = false;
            largerDuration = false;
            isNumber = true;
            $("#ShiftInfoTableEdit .tbody .tr").each(function () {

                var duration = $(this).find(".duration").val();
                if (!$.isNumeric(duration)) {
                    isNumber = false;
                    return false;
                }
                if (duration <= 0) {
                    hasZeroDuration = true;
                    return false;
                }
                var shiftTime = $(this).find(".shiftTime").val();
                if (parseFloat(shiftTime) < duration) {
                    largerDuration = true;
                    return false;
                }
            });

            if (!isNumber) {
                abp.message.error(app.localize("ActualWorkingHoursMustBeaValidNumber"));
                return;
            }
            if (hasZeroDuration) {
                abp.message.error(app.localize("TheActualWorkingHoursMustBeGreaterThan0"));
                return;
            }
            if (largerDuration) {
                abp.message.error(app.localize("TheActualWorkingHoursShouldNotBeLongerThanTheShiftHours"));
                return;
            }
            var shiftSolutionItems = [];
            $("#ShiftInfoTableEdit .tbody .tr").each(function () {
                var detail = {
                    ShiftSolutionId: $(this).find(".shiftSolutionId").val(),
                    Id: $(this).find(".id").val(),
                    Name: $(this).find(".name").val(),
                    StartTime: $(this).find(".startTime").val(),
                    EndTime: $(this).find(".endTime").val(),
                    Duration: $(this).find(".duration").val(),
                    IsNextDay: $(this).find(".isnextday").prop('checked'),
                    CreationTime: $(this).find(".creationTime").val()
                };
                shiftSolutionItems.push(detail);
            });

            for (var i = 0; i < shiftSolutionItems.length - 1; i++) {
                var firstStartTime = shiftSolutionItems[i].StartTime.split(":");
                var firstEndTime = shiftSolutionItems[i].EndTime.split(":");
                var secondStartTime = shiftSolutionItems[i + 1].StartTime.split(":");
                var secondEndTime = shiftSolutionItems[i + 1].EndTime.split(":");
                var isNextDayFirst = shiftSolutionItems[i].IsNextDay;
                var isNextDaySecond = shiftSolutionItems[i + 1].IsNextDay;
                if (firstStartTime.length > 1 && firstEndTime.length > 1
                    && secondStartTime.length > 1 && secondEndTime.length > 1) {
                    var firstStartTimeNum = parseFloat(firstStartTime[0]) + parseFloat(firstStartTime[1]) / 60;
                    var firstEndTimeNum = parseFloat(firstEndTime[0]) + parseFloat(firstEndTime[1]) / 60;
                    var secondStartTimeNum = parseFloat(secondStartTime[0]) + parseFloat(secondStartTime[1]) / 60;
                    var secondEndTimeNum = parseFloat(secondEndTime[0]) + parseFloat(secondEndTime[1]) / 60;
                    if (firstEndTimeNum < firstStartTimeNum) {
                        firstEndTimeNum += 24;
                        isNextDayFirst = false;
                    }
                    if (secondEndTimeNum < secondStartTimeNum) {
                        secondStartTimeNum += 24;
                        isNextDaySecond = false;
                    }
                    if (isNextDaySecond === true) {
                        secondStartTimeNum += 24;
                    }
                    if (isNextDayFirst === true) {
                        firstEndTimeNum += 24;
                    }
                    if (firstEndTimeNum > secondStartTimeNum) {
                        abp.message.error(app.localize("ShiftsMustBeSetInChronologicalOrderAndTimeMustNotBeRepeated") + "！");
                        return;
                    }
                }
            }
            var params = {
                shiftSolutionId: $("#ShiftSolutionId").val(),
                shiftSolutionItems: shiftSolutionItems
            };

            if ($("#State").val() === "Create") {
                _modalManager.setBusy(true);
                shiftAppService.createShiftInfo(
                    params
                ).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
            else {
                _modalManager.setBusy(true);
                shiftAppService.updateShiftInfo(
                    params
                ).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }

        };
        newTableTr = '<tr class="tr">' +
            '<td>' +
            '<input class="form-control input shiftSolutionId" type="hidden" name="ShiftInfo[#].ShiftSolutionId" value="' + $("#ShiftSolutionId").val() + '" required>' +
            '<input class="form-control input id" type="hidden" name="ShiftInfo[#].Id" value="0" required>' +
            '<button type="button" class="text-center btn btn-delete btn-xs btn-delete" title="删除"><i class="fa fa fa-trash-o" aria-hidden="true"></i></button>' +
            '</td>' +
            '<td>' +
            '<div class="form-group"><input class="form-control name" type="text" name="ShiftInfo[#].Name" required maxlength="@DeviceGroup.MaxDisplayNameLength">' +
            '</div></td>' +
            '<td>' +
            '<div class="input-group bootstrap-timepicker timepicker">' +
            '<input type="text" class="form-control input-small startTime" name="ShiftInfo[#].StartTime" readonly>' +
            '<span class="input-group-addon"><i class="glyphicon glyphicon-time"></i></span>' +
            '</div>' +
            '</td>' +
            '<td>' +
            '<div class="input-group bootstrap-timepicker timepicker">' +
            '<input type="text" class="form-control input-small endTime" name="ShiftInfo[#].EndTime" readonly>' +
            '<span class="input-group-addon"><i class="glyphicon glyphicon-time"></i></span>' +
            '</div>' +
            '</td>' +
            '<td class="text-center">' +
            '<input class="form-control input shiftTime" type="text"  value="0" readonly>' +
            '</td>' +
            '<td class="text-center">' +
            '<input class="form-control input shiftTime duration" type="number" name="ShiftInfo[#].Duration" value="0" >' +
            '</td>' +
            '<td class="text-center">' +
            '<input type="checkbox" class="input isnextday" style="vertical-align:-webkit-baseline-middle" name="ShiftInfo[0].IsNextDay" />' +
            '</td>' +
            '</tr>';
    };
})();