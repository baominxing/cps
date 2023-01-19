//# sourceURL=dynamicCreateDeviceShiftSolutionModal.js

(function () {
    app.modals.CreateDeviceShiftSolutionModal = function () {

        var _modalManager;
        var _shiftAppService = abp.services.app.shift;
        var _$form = $('#ShiftInfoForm');;
        var _machineShiftSolutionTable = $('#MachineShiftSolutionTable');

        var _newTableTr =
            '<tr class="tr">' +
            '<td>' +
            '<input class="form-control input id" type="hidden" name="MachineShiftSolution[#].Id" value="0" required>' +
            '<button type="button" class="text-center btn btn-default btn-xs btn-delete" title="' + app.localize("Delete")+'"><i class="fa fa fa-trash-o" aria-hidden="true"></i></button>' +
            '</td>' +
            '<td>' +
            '<div class="input-group xs-ph-5 col-xs-12">' +
            '<select class="form-control name" name="MachineShiftSolution[#].Name" required>' +
            '</select>' +
            '</div>' +
            '</td>' +
            '<td>' +
            '<div class="input-group bootstrap-timepicker timepicker">' +
            '<input  type="text" data-type="calendar" class="form-control input-small startTime newStartTime" name="MachineShiftSolution[#].StartTime" readonly required>' +
            '<span class="input-group-addon"><i class="fa fa-calendar st"></i></span>' +
            '</div>' +
            '</td>' +
            '<td>' +
            '<div class="input-group bootstrap-timepicker timepicker">' +
            '<input type="text" data-type="calendar" class="form-control input-small endTime newEndTime" name="MachineShiftSolution[#].EndTime" readonly required>' +
            '<span class="input-group-addon"><i class="fa fa-calendar et"></i></span>' +
            '</div>' +
            '</td>' +
            '</tr>';

        var shifts = [];
        function getShiftSolutions() {
            if (shifts.length === 0) {
                _shiftAppService
                    .getShiftSolutions({ async: false })
                    .done(function (data) {
                        shifts = data.items;
                    });
            }
            return shifts;
        }
        var daterangepickerOption = function (date) {
            return {
                singleDatePicker: true,
                locale: {
                    format: 'YYYY-MM-DD',
                    monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                        '七月', '八月', '九月', '十月', '十一月', '十二月'],
                    daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
                },
                minDate: date,
                maxDate: moment().add(10, 'y')
            };
        };

        var initDateTimeRangePicker = function (timeInput, isStartTime) {
            var currentTime = moment().format('YYYY-MM-DD');
            $.each(timeInput,
                function (index, item) {
                    var time = $(item).val();
                    var minDate;
                    if (isStartTime === true) {
                        if (time === "") {
                            minDate = currentTime;
                        } else {
                            if (moment(time) > moment(currentTime)) {
                                minDate = currentTime;
                            } else {
                                minDate = moment(time);
                            }
                        }
                    }
                    else {
                        if (time === "") {
                            minDate = moment(currentTime);
                        } else {
                            minDate = moment(time);
                        }
                    }
                    $(item).WIMIDaterangepicker(daterangepickerOption(minDate));

                    if (time === "") {
                        $(item).val("");
                    }
                });
        }
        //新增班次方案按钮逻辑
        $("#AddShiftSolution").on("click", function (e) {
            e.preventDefault();
            var machineShiftSolutionTable = _machineShiftSolutionTable.find('.tbody');
            var $newTr = $(_newTableTr.replace(/#/g, _machineShiftSolutionTable.find('.tr').length));

            var $shiftselect = $newTr.find(".name");
            $shiftselect.append($("<option></option>"));
            for (var i = 0; i < shifts.length; i++) {
                $shiftselect.append($("<option></option>").val(shifts[i].id).html(shifts[i].name));
            }
            machineShiftSolutionTable.append($newTr);
            //初始化时间控件
            var newStartTime = _modalManager.getModal().find(".newStartTime");
            var newEndTime = _modalManager.getModal().find(".newEndTime");
            initDateTimeRangePicker(newStartTime, true);
            initDateTimeRangePicker(newEndTime, false);
        });

        //删除设备班次方案记录
        _machineShiftSolutionTable.on('click', '.btn-delete', function () {

            var $tr = $(this).closest('.tr');

            if ($tr.find(".id").val() * 1 !== 0) {
                //从数据库中删除这笔数据
                var id = $tr.find(".id").val();
                var machineId = $(".machineId").val();
                _shiftAppService.checkBeforeDeleteMachineShiftSolution({ id: id, machineId: machineId })
                    .done(function (result) {
                        if (result !== "") {
                            abp.message.confirm(result + ',' + app.localize("ContinueConfirm"),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _modalManager.setBusy(true);

                                        _shiftAppService.deleteMachineShiftSolution({ id: id, machineId: machineId })
                                            .done(function () {
                                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                                                $("#btnQuery").click();
                                            }).always(function () {
                                                _modalManager.setBusy(false);
                                            });
                                        endTimeRow($tr);
                                    }
                                }
                            );
                        } else {
                            abp.message.confirm(
                                app.localize("DeleteMachineShiftsConfirm"),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _modalManager.setBusy(true);

                                        _shiftAppService.deleteMachineShiftSolution({ id: id, machineId: machineId })
                                            .done(function () {
                                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                                                $("#btnQuery").click();
                                            }).always(function () {
                                                _modalManager.setBusy(false);
                                            });
                                        deleteTableRow($tr);
                                    }
                                }
                            );
                        }
                    });
            } else {
                deleteTableRow($tr);
            }

        });

        _machineShiftSolutionTable.on('change', '.startTime', function () {
            var $tr = $(this).closest('tr');
            var startTimeString = $(this).val();

            var trIndex = $tr.index();
            var $trs = _machineShiftSolutionTable.find('.tbody .tr');

            $.each($trs, function (index, item) {

                if (index < trIndex) {
                    if (new Date(startTimeString).getTime <= new Date($(item).find(".endTime")).getTime) {
                        $(this).val("");
                    }
                }
            });

        });
       
        _machineShiftSolutionTable.on('change', '.endTime', function () {
            var $tr = $(this).closest('.tr');
            var endTimeString = $(this).val();
            if (new Date(endTimeString).getTime() < new Date($tr.find(".startTime")).getTime) {
                abp.message.error(app.localize("EndTimeCannotSmallerThanStartTime"));
                $(this).val("");
            }
        });
        var startTimeInput;
        var endTimeInput;

        this.shown = function() {
            _$form = _modalManager.getModal().find("form[name=MachineShiftSolutionForm]");

            getShiftSolutions();
            initSelect();
        };



        this.init = function (modalManager) {
            _modalManager = modalManager;
            startTimeInput = _modalManager.getModal().find(".startTime");
            endTimeInput = _modalManager.getModal().find(".endTime");

            initDateTimeRangePicker(startTimeInput, true);
            initDateTimeRangePicker(endTimeInput, true);

        };
        this.save = function () {
            if (!_$form.valid()) {
                return;
            }
            var machine4ShiftSolutionInputDetail = [];
            var timeError = false;
            $('#MachineShiftSolutionTable .tbody .tr').each(function () {
                var startTime = $(this).find(".startTime").val();
                var endTime = $(this).find(".endTime").val();
                if (moment(endTime) < moment(startTime)) {
                    timeError = true;
                    return;
                }
                var detail = {
                    Id: $(this).find(".id").val(),
                    ShiftSolutionId: $(this).find(".name").val(),
                    StartTime: startTime,
                    EndTime: endTime
                };
                machine4ShiftSolutionInputDetail.push(detail);
            });
            //生效时间和失效时间统一为当前的0点
            if (timeError) {
                abp.message.error(app.localize("EndTimeCannotSmallerThanStartTime"));
                return;
            }
            var params = {
                Id: $("#MachineId").val(),
                Machine4ShiftSolutionInputDetail: machine4ShiftSolutionInputDetail
            };

            _modalManager.setBusy(true);
            _shiftAppService.updateMachineShiftSolution(
                params
            ).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });

        };

        //初始化下拉框控件
        function initSelect() {
            $(".name")
                .each(function (index, obj) {
                    var shiftSolutionId = $(obj).closest(".tr").find(".shiftSolutionId").val();
                    for (var i = 0; i < shifts.length; i++) {
                        $(obj).append($("<option></option>").val(shifts[i].id).html(shifts[i].name));
                    }
                    $(obj).val(shiftSolutionId);
                });
        }

        function deleteTableRow(tr) {
            var trIndex = tr.index();
            var orginalTrLength = _machineShiftSolutionTable.find('.tbody .tr').length;
            tr.remove();
            if (trIndex < orginalTrLength - 1) {
                var $trs = _machineShiftSolutionTable.find('.tbody .tr');
                $.each($trs, function (index, item) {
                    var $inputs = $(item).find('input');
                    $inputs.map(function () {
                        var orginal = $(this).attr('name');
                       // $(this).attr('name', orginal.replace(/\[\d\]/, '[' + index + ']'));
                    });
                });
            }
        }

        function endTimeRow(tr) {
            var currentTime = moment().format('YYYY-MM-DD');
            $(tr).find('.endTime').val(currentTime);
        }
    };
})();