//# sourceURL=dynamic_CreateOrUpdatePlan.js
(function () {
    app.modals.CreateOrUpdatePlan = function () {

        var _modalManager;
        var processPlanService = abp.services.app.processPlan;
        var basicDataAppService = abp.services.app.basicData;
        var productAppService = abp.services.app.product;
        var deviceGroupAppService = abp.services.app.deviceGroup;
        var $createOrUpdateForm = null;
        var $dateTimePicker;
        var targetType;
        var targetCount;
        var shiftCount;



        this.init = function (modalManager) {
            _modalManager = modalManager;

            hideTimeGroupEvent();
            selectTargetCountGroup();
            selectYieldCountGroup();
            timeCheckBox();

            $("#timeGroup").show();
            $("#targetCountGroupShift").hide();

            targetType = $("input[name='targetCount']:checked").val();
            targetCount = targetType == "4" ? 0 : $("#TargetAmount").val();
            shiftCount = getShiftInfo(targetType);
            productAppService.listProducts()
                .done(function (response) {
                    var data = _.map(response,
                        function (item) {
                            return { id: item.id, text: item.name };
                        });
                    $("#productName").select2({
                        data: data,
                        width: "265px",
                        multiple: false,
                        minimumResultsForSearch: -1,
                        placeholder: app.localize("PleaseChoose"),
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });
                });
            deviceGroupAppService.listFirstClassDeviceGroups()
                .done(function (response) {
                    var data = _.map(response,
                        function (item) {
                            return { id: item.id, text: item.displayName };
                        });
                    $("#deviceGroup").select2({
                        data: data,
                        width: "265px",
                        multiple: false,
                        minimumResultsForSearch: -1,
                        placeholder: app.localize("PleaseChoose"),
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });
                    getYieldCounterMachine();
                    changeDeviceGroup();
                });

            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");

            $dateTimePicker = $(".dateTimeRangePicker").daterangepicker({
                "singleDatePicker": true,
                "timePicker": true,
                "timePicker24Hour": true,
                "drops": "up",
                locale: {
                    format: 'YYYY-MM-DD HH:mm',
                    applyLabel: app.localize("Confirm"),
                    cancelLabel: app.localize("Cancel"),
                    monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                        '七月', '八月', '九月', '十月', '十一月', '十二月'],
                    daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
                }
            });

            if ($("#Status").val() === "Complete" || $("#Status").val() === "AutoComplete") {
                $createOrUpdateForm.find("input").prop("disabled", "disabled");
                $createOrUpdateForm.find("select").prop("disabled", "disabled");
                _modalManager.getModal().find(".save-button").hide();
            }
            if ($("#IsEdit").val() == "true" && targetType == "4") {
                $("#targetCountGroupShift").show();
            }

        };

        this.shown = function () {
            processPlanService.getPlanParameterByMachineId($("#Id").val()).done(function (response) {
                if (response.length != 0) {
                    $("#deviceGroup").val(response[0].deviceGroupId).trigger("change");
                    document.getElementById("machine").options.length = 0;
                    getYieldCounterMachine();
                    $("#machine").val(response[0].yieldCounterMachineId).trigger("change");
                    $("#productName").val(response[0].productId).trigger("change");
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

        var getYieldCounterMachine = function () {
            basicDataAppService.listMachineInDeviceGroup($("#deviceGroup").val())
                .done(function (response) {
                    var data = _.map(response,
                        function (item) {
                            return { id: item.value, text: item.name };
                        });
                    $("#machine").select2({
                        data: data,
                        multiple: false,
                        width: "265px",
                        minimumResultsForSearch: -1,
                        placeholder: app.localize("PleaseChoose"),
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    }).val($("#BindMachineId").val()).trigger("change");
                });
        }

        var changeDeviceGroup = function () {

            $("#deviceGroup").on("change",
                function () {
                    if ($('#table_wrapper').hasClass("has")) {
                        $('#table_wrapper').removeClass("has");
                    } else {
                        basicDataAppService.listMachineInDeviceGroup(document.getElementById("deviceGroup").value)
                            .done(function (response) {
                                $("#machine").empty();
                                var data = _.map(response,
                                    function (item) {
                                        return { id: item.value, text: item.name };
                                    });

                                $("#machine").select2({
                                    data: data,
                                    width: "265px",
                                    multiple: false,
                                    minimumResultsForSearch: -1,
                                    placeholder: app.localize("PleaseChoose"),
                                    language: {
                                        noResults: function () {
                                            return app.localize("NoMatchingData");
                                        }
                                    }
                                });
                            });
                    }

                    //if ($("#IsEdit").val() == "false") {
                    //	initShiftTab();
                    //}
                    initShiftTab();
                });
        };

        var hideTimeGroupEvent = function () {
            $("#timeSelect").click(function () {
                if ($("#timeSelect").prop('checked')) {
                    $("#timeGroups").show();
                    $("#timeSelect").val(true);
                } else {
                    $("#timeGroups").hide();
                    $("#timeSelect").val(false);
                }
            });
        };

        var timeCheckBox = function () {
            $("#IsAutoFinishCurrentPlan").click(function () {
                if ($("#IsAutoFinishCurrentPlan").prop('checked')) {
                    $("#IsAutoFinishCurrentPlan").val(true);
                } else {
                    $("#IsAutoFinishCurrentPlan").val(false);
                }
            });
            $("#IsAutoStartNextPlan").click(function () {
                if ($("#IsAutoStartNextPlan").prop('checked')) {
                    $("#IsAutoStartNextPlan").val(true);
                    $("#IsAutoFinishCurrentPlan").prop("checked", true);
                    $("#IsAutoFinishCurrentPlan").val(true);
                    $("#IsAutoFinishCurrentPlan").attr("disabled", true);
                } else {
                    $("#IsAutoStartNextPlan").val(false);
                    $("#IsAutoFinishCurrentPlan").attr("disabled", false);
                }
            });
        };

        var selectTargetCountGroup = function () {
            $("input[name='targetCount']").on("click",
                function () {
                    if ($(this).val() == 4) {
                        $("#TargetAmount").removeAttr("min");
                        $("#TargetAmount").removeAttr("required");
                        $("#targetCountGroup").hide();
                        $("#targetCountGroupShift").show();

                        if ($("#IsEdit").val() == "true") {
                            $("#TargetAmount").removeAttr("required");
                            $("#targetCountGroup").hide();
                            $("#targetCountGroupShift").show();
                        } else {

                            $("#targetCountShift").hide();
                            initShiftTab();
                        }


                    } else {
                        $("#targetCountGroup").show();
                        $("#targetCountGroupShift").hide();
                        $("#TargetAmount").attr("required", "required");
                        $("#TargetAmount").attr("min","0");
                    }
                });
        };

        var selectYieldCountGroup = function () {
            $("input[name='yieldCount']").on("click",
                function () {
                    if ($(this).val() == 0) {
                        $("#machines-div").hide();
                    } else {
                        $("#machines-div").show();
                    }
                });
        };

        var getShiftInfo = function (param) {
            if (param == "4") {
                //按照班次的方式
                var shifts = $("#targetCountShiftInCreate").find('input');
                var array = [];
                for (var i = 0; i < shifts.length; i++) {
                    var item = {};
                    item.ShiftName = $(shifts[i]).attr("shiftname");
                    item.ShiftId =  $.trim($(shifts[i]).attr("shiftid"));
                    item.SolutionId = $(shifts[i]).attr("solutionid");
                    item.ShiftTargetAmount = $.trim($(shifts[i]).val()) == "" ? 0 : $.trim($(shifts[i]).val());
                    array.push(item);
                }
                return JSON.stringify(array);
            }
            else {
                return "";
            }
        }

        var initShiftTab = function () {
            _modalManager.setBusy(true);
            var input = {
                deviceGroupId: $("#deviceGroup").val(),
                planStartTime: $("#timeSelect").val() == "true" ? $createOrUpdateForm.find(".startDateTime").val() : null
            };

            processPlanService.getShiftSolutionName(input).done(function (result) {
                if (result.length > 0) {
                    _modalManager.setBusy(false);
                    var tabData = [];
                    for (var i = 0; i < result.length; i++) {
                        var className = "";
                        if (i === 0) {
                            className = "active";
                        }
                        tabData.push({
                            className: className,
                            machineShiftSolutionName: result[i].name
                        });
                    }

                    var source = $("#shiftSolution-template").html();
                    var rendered = Handlebars.compile(source);
                    $("#shiftTap").html(rendered());
                    var tabDataSource = $("#tab-data-template").html();
                    var tabDataRender = Handlebars.compile(tabDataSource);

                    $("#tabData").html(tabDataRender({ datas: tabData }));
                    $("#tabData").scrollTabs(
                        {
                            click_callback: function () {

                                var param = {
                                    planId: $("#Id").val(),
                                    shiftSolutionName: $(this).text()
                                };

                                loadShiftItem(param);
                            }
                        });

                    var param = {
                        planId: $("#Id").val(),
                        shiftSolutionName: tabData[0].machineShiftSolutionName
                    };
                    loadShiftItem(param);
                } else {
                    $("#shiftTap").empty();
                    $("#tabData").empty();
                    $("#targetCountShiftInCreate").empty();
                    abp.message.error(app.localize("DeviceGroupHasNoShifts"));

                }
            });

        }

        var loadShiftItem = function (param) {

            processPlanService.getShiftInfo(param).done(function (result) {

                var source = $("#shift-template").html();
                var render = Handlebars.compile(source);

                $("#targetCountShiftInCreate").html(render({ shiftDatas: result }));

            })

            var body = _modalManager.getModal().find(".modal-body");
            var footer = _modalManager.getModal().find(".modal-footer");

            $('.modal-dialog').css("height", body.height() + footer.height());

        }

        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }

            var obj = $createOrUpdateForm.serializeFormToObject();

            var $product = $createOrUpdateForm.find(".productName");
            var $deviceGroup = $createOrUpdateForm.find(".deviceGroup");
            var $isTimeRangeSelect = $("#timeSelect").val();
            var $isAutoStartNextPlan = document.getElementById("IsAutoStartNextPlan").checked;
            var $isAutoFinishCurrentPlan = document.getElementById("IsAutoFinishCurrentPlan").checked;
            var $yieldCounterMachineId = $("#machine").val();
            var $targetAmount = $("#TargetAmount").val();
            var $targetType = $("input[name='targetCount']:checked").val();
            var $yieldSummaryType = $("input[name='yieldCount']:checked").val();
            var $planName = $("#PlanName").val();
            var $isEidt = $("#IsEdit").val();

            var parameters = {
                planName: $planName,
                productId: $product.val(),
                deviceGroupId: $deviceGroup.val(),
                productName: $product.find(":selected").text(),
                planAmount: $createOrUpdateForm.find(".planAmount").val(),
                planStartTime: $isTimeRangeSelect == "true" ? $createOrUpdateForm.find(".startDateTime").val() : null,
                planEndTime: $isTimeRangeSelect == "true" ? $createOrUpdateForm.find(".endDateTime").val() : null,
                isAutoFinishCurrentPlan: $isAutoFinishCurrentPlan,
                isTimeRangeSelect: $isTimeRangeSelect,//obj.IsTimeRangeSelect,
                isAutoStartNextPlan: $isAutoStartNextPlan,//obj.IsAutoStartNextPlan,
                yieldCounterMachineId: $yieldSummaryType == "0" ? 0 : $yieldCounterMachineId,
                targetAmount: $targetType == "4" ? 0 : $targetAmount,
                TargetType: $targetType,
                YieldSummaryType: $yieldSummaryType,
                ShiftTargetJson: getShiftInfo($targetType)
            };

            if (parameters.isTimeRangeSelect == "true") {
                if (parameters.startDateTime > parameters.endDateTime) {
                    abp.message.warn(app.localize("TheScheduledDeadlineMustBeGreaterThanTheStartTime"));
                    return;
                }
            }
            if (parameters.yieldSummaryType !== "0" && parameters.yieldCounterMachineId===null) {
                abp.message.error(app.localize("PleaseSelectEquipment"));
                return;
            }
            if ($createOrUpdateForm.data("isEdit")) {
                parameters.id = $createOrUpdateForm.find("#Id").val();
            }
            if ($isEidt == "true") {
                if (targetType != parameters.TargetType) {
                    abp.message.confirm(app.localize("DetermineTheDimensionsToChangeTheTargetQuantity"),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                processPlanService.updateProcessPlan(parameters).done(function (result) {
                                    abp.notify.info(app.localize("SavedSuccessfully"));
                                    _modalManager.setResult(result);
                                    _modalManager.close();
                                }).always(function () {
                                    _modalManager.setBusy(false);
                                }).fail(function () {
                                    status: $createOrUpdateForm.find(".status").val("New");
                                });
                            } else {
                                _modalManager.close();
                            }
                        });
                } else {
                    processPlanService.updateProcessPlan(parameters).done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _modalManager.setResult(result);
                        _modalManager.close();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    }).fail(function () {
                        status: $createOrUpdateForm.find(".status").val("New");
                    });
                }

            } else {
                processPlanService.createProcessPlan(parameters).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                }).fail(function () {
                    status: $createOrUpdateForm.find(".status").val("New");
                });
            }

        };
    };
})();