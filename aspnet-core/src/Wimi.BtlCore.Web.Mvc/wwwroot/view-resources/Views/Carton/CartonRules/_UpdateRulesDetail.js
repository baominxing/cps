//# sourceURL=dynamic_UpdateRuleDetail.js
(function () {
    app.modals.UpdateRuleDetail = function () {
        var _modalManager,
            _args;
        var service = abp.services.app.cartonRule;
        var $createOrUpdateForm = null;
        var vmData = {
            typeList: [
                {
                    id: 0,
                    text: "Ascii"
                },
                {
                    id: 1,
                    text: "固定字符串"
                },
                {
                    id: 2,
                    text: "年"
                },
                {
                    id: 3,
                    text: "月"
                },
                {
                    id: 4,
                    text: "日"
                },
                {
                    id: 5,
                    text: "季度"
                },
                {
                    id: 6,
                    text: "周"
                },
                {
                    id: 7,
                    text: "班次"
                },
                {
                    id: 8,
                    text: "产线"
                },
                {
                    id: 9,
                    text: "流水号"
                },
                {
                    id: 10,
                    text: "校验码"
                },
                {
                    id: 11,
                    text: "特殊码"
                },
                {
                    id: 12,
                    text: "时间"
                }
            ],
            showControl: {
                show0: false,
                show1: false,
                show2: false,
                show3: false,
                show4: false,
                show5: false,
                show6: false,
                show7: false,
                show8: false,
                show9: false,
                show10: false,
                show11: false,
                show12: false
            }

        };
        var vue = new Vue({
            el: '#createOrUpdateForm',
            data: vmData,
            methods: {
                sequenceNoChanged(args) {
                    let _this = args.currentTarget;
                    var changeSequenceNo = $(_this).val();
                    var isCurrentDup = false;
                    $("input[name=SequenceNo]").each(function (index, element) {
                        if (_this != element && $(element).val() == changeSequenceNo) {
                            isCurrentDup = true;
                            abp.message.error(app.localize("DuplicateSequenceNo"));
                            $(_this).val('');
                        }
                    });
                    if (!isCurrentDup) {
                        var params = {
                            ruleId: _args.ruleId,
                            sequenceNo: changeSequenceNo,
                            ruleDetailId: _args.id
                        };
                        service.checkSequenceNo(params)
                            .done(function () { })
                            .fail(function () {
                                $($(_this).val(''));
                            });
                    }
                },
                shiftChange() {
                    var shiftId = $("#Shift").find("option:selected").val();
                    service.getShiftItemsById({ id: shiftId }).done(function (res) {
                        $("#shiftItemsDom").empty();
                        for (var i = 0; i < res.length; i++) {
                            var appendStr = '<div class="form-group"><div class="col-md-2 control-label"> <label>' + res[i].name + '</label></div>' + '<div class="col-md-4">' +
                                '<input type="hidden" class="form-control" name="ExpansionKey" value="' + res[i].id + '" />' +
                                ' <input type="text" class="form-control" required name="Value" value="" /> </div></div>';
                            $("#shiftItemsDom").append(appendStr);
                        }
                    });
                },
                importData() {
                    var files = $("#importFile")[0].files;
                    let _this = this;
                    if (files != undefined) {
                        $("#createOrUpdateForm").attr('method', 'POST');
                        $("#createOrUpdateForm").attr('onsubmit', 'return stopSubmit()');
                        $("#createOrUpdateForm").attr('action', '/CartonRules/ImportData');
                        $("#createOrUpdateForm").ajaxSubmit(function (message) {
                            if (message == "Success" || (message.result && message.result.value == "Success")) {
                                $("#importFile").val('');
                                abp.notify.success(app.localize("ImportSuccessfully"));
                            } else {
                                abp.message.error(message.result.value);
                            }
                            $("#createOrUpdateForm").removeAttr('method');
                            $("#createOrUpdateForm").removeAttr('onsubmit');
                            $("#createOrUpdateForm").removeAttr('action');
                        });

                        return false;
                    }
                }
            }
        });
        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");
            var type = _args.type.toString();
            $("#typeSelect").val(type).select2({ width: '80%' });
            switch (_args.type) {
                case 0: {
                    vmData.showControl.show0 = true;
                    break;
                }
                case 1: {
                    vmData.showControl.show1 = true;
                    break;
                }
                case 2: {
                    vmData.showControl.show2 = true;
                    break;
                }
                case 3: {
                    vmData.showControl.show3 = true;
                    break;
                }
                case 4: {
                    vmData.showControl.show4 = true;
                    break;
                }
                case 5: {
                    vmData.showControl.show5 = true;
                    break;
                }
                case 6: {
                    vmData.showControl.show6 = true;
                    break;
                }
                case 7: {
                    vmData.showControl.show7 = true;
                    break;
                }
                case 8: {
                    vmData.showControl.show8 = true;
                    break;
                }
                case 9: {
                    vmData.showControl.show9 = true;
                    break;
                }
                case 10: {
                    vmData.showControl.show10 = true;
                    break;
                }
                case 11: {
                    vmData.showControl.show11 = true;
                    break;
                }
                case 12: {
                    vmData.showControl.show12 = true;
                    break;
                }
                }
        };

        this.shown = function () {
            
            
        };

        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }

            $("input[name=SequenceNo]").each(function (index, element) {
                var sequenceNo = $(element).val();
                var params = {
                    ruleId: _args.ruleId,
                    sequenceNo: sequenceNo,
                    RuleDetailId:_args.id
                };
                service.checkSequenceNo(params)
                    .done(function () {
                        var parameters = $createOrUpdateForm.serializeFormToObject();

                        parameters.Id = _args.id;
                        parameters.CartonRuleId = _args.ruleId;

                        if (_args.type == 9) {
                            var isCheck = $("#expansionKey").is(':checked');
                            if (isCheck) {
                                parameters.ExpansionKey = 1;
                            } else {
                                parameters.ExpansionKey = 0;
                            }
                        }

                        _modalManager.setBusy(true);
                        service.updateRuleDetail(parameters).done(function (result) {
                            abp.notify.info(app.localize("SavedSuccessfully"));
                            _modalManager.setResult(result);
                            _modalManager.close();
                        }).always(function () {
                            _modalManager.setBusy(false);
                        });
                    })
                    .fail(function () {
                        $(element).val('');
                    });
            });
        };
    };
})();