//# sourceURL=dynamic_CreateRuleDetail.js
(function () {
    app.modals.CreateRuleDetail = function () {
        var _modalManager,
            _args;
        var _shiftFlag = true;
        var service = abp.services.app.cartonRule;
        var $createOrUpdateForm = null;
        var vmData = {
            typeSelected: [],
            showControl: {
                show0: false,
                show1:false,
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
       // $(".save-button").attr("disabled", "disabled");
        var vue = new Vue({
            el: '#createOrUpdateForm',
            data: vmData,
            methods: {
                //sequenceNoChanged(args) {
                //    let _this = args.currentTarget;
                //    var changeSequenceNo = $(_this).val();
                //    var isCurrentDup = false;
                //    $("input[name=SequenceNo]").each(function (index, element) {
                //        if (_this != element && $(element).val() == changeSequenceNo) {
                //            isCurrentDup = true;
                //            abp.message.error(app.localize("DuplicateSequenceNo"));
                //            $(_this).val('');
                //        }
                //    });
                //    if (!isCurrentDup) {
                //        var params = {
                //            ruleId: _args.ruleId,
                //            sequenceNo: changeSequenceNo
                //        };
                //        service.checkSequenceNo(params)
                //            .done(function () {
                //                $(".save-button").removeAttr("disabled");
                              
                //            })
                //            .fail(function () {
                //                $(".save-button").attr("disabled", "disabled");
                //               $(_this).val('');
                //            });
                //    }
                //},
                shiftChange() {
                    var shiftId = $("#Shift").find("option:selected").val();
                    service.getShiftItemsById({ id: shiftId }).done(function (res) {
                        $("#shiftItemsDom").empty();
                        for (var i = 0; i < res.length; i++) {
                            var appendStr = '<div class="form-group"><div class="col-md-3 control-label"> <label>' + res[i].name + '</label></div>' + '<div class="col-md-8">' +
                                '<input type="hidden" class="form-control" name="ExpansionKey" value="' + res[i].id+'" />'+
                                ' <input type="text" class="form-control" required name="Value' + i +'" value="" maxlength="100" /> </div></div>';
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
                            console.log(message);
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
                },
                stopSubmit() {
                    return false;
                }
            }
        });
        this.init = function (modalManager, args) {

            _modalManager = modalManager;
            _args = args;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");

            $("#typeSelect").select2({ width: '80%' });
            
            $("#add").on("click", function () {
                var selectVal = $("#typeSelect option:checked").val();
                switch (parseInt(selectVal)) {
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
            });
        };

        //this.shown = function () {
        //    _modalManager.getModal().find(".modal-body").keydown(function (e) {
        //        if (e.which * 1 === 13) {
        //            e.preventDefault();
        //            console.log(123);
        //        } else {
        //            console.log(e.which *1)
        //        }
        //    });
        //}

        this.save = function () {
          

            if (!$createOrUpdateForm.valid()) {
                return;
            }
           
            function buildParams(params) {
                var arr = [];
                var index = 0;
                var obj = {};
                var thisType = 0;
                params.map(function (x) {
                    if (x.name == "Type") {
                        thisType = x.value;
                        if (index == 0) {
                            obj[x.name] = x.value;
                        } else {
                            if (x.value != "7" && x.value != 8) {
                                obj.CartonRuleId = _args.ruleId;
                                arr.push(obj);
                            }
                            obj = {};
                            obj[x.name] = x.value;
                        }
                    } else {
                        obj[x.name] = x.value;
                        if (thisType == "7" && x.name.substr(0,5) == "Value") {
                            obj.CartonRuleId = _args.ruleId;
                            obj["Value"]=x.value
                            var copy1 = JSON.parse(JSON.stringify(obj));
                            arr.push(copy1);
                        }

                        if (thisType == "9" && x.name == "ExpansionKey") {
                            var checked = $("#resetDayly").is(":checked");
                            if (checked) {
                                obj[x.name] = 1;
                            } else {
                                obj[x.name] = 0;
                            }
                        }

                        if (thisType == "8" && x.name.substr(0, 5) == "Value") {
                            obj.CartonRuleId = _args.ruleId;
                            obj["Value"] = x.value
                            var copy2 = JSON.parse(JSON.stringify(obj));
                            arr.push(copy2);
                        }
                        if (x.name == "ValueHour" || x.name == "ValueNaturalDay" || x.name == "ValueYear") {
                            obj["Value"] = x.value;
                        }
                    }
                    _shiftFlag = true;
                    if (x.name == "Shift" && x.value == 0) {
                        _shiftFlag = false;
                        abp.message.warn("请选择班次");
                    }
                    index += 1;

                    //处理最后一笔
                    if (index == params.length - 1 && thisType != "7" && thisType != "8") {
                        obj.CartonRuleId = _args.ruleId;
                        arr.push(obj);
                    }
                });
                return arr;
            }

            var checkResult = true;

            $("input[name=SequenceNo]").each(function (index, element) {
                var sequenceNo = $(element).val();
                var params = {
                    ruleId: _args.ruleId,
                    sequenceNo: sequenceNo
                };
                console.log(index);
                service.checkSequenceNo(params, {async:false})
                    .done(function () {
                        if (checkResult) {
                            if ($("input[name=SequenceNo]").length == index + 1) {

                                var parameters = $createOrUpdateForm.serializeArray();


                                var finalParams = { ruleDetailInputItems: buildParams(parameters) };
                                if (_shiftFlag) {
                                    _modalManager.setBusy(true);
                                    service.createRuleDetail(finalParams).done(function (result) {
                                        abp.notify.info(app.localize("SavedSuccessfully"));
                                        _modalManager.setResult(result);
                                        _modalManager.close();
                                    }).always(function () {
                                        _modalManager.setBusy(false);
                                    });
                                }
                            }
                        }
                        
                    })
                    .fail(function () {
                        $(element).val('');
                        checkResult = false;
                    });
            });

        };
    };
})();