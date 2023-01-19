(function () {
    $(function () {
        var service = abp.services.app.carton;

        var currentCartonSetting = {};  //当前装箱设定
        var currentCartonNo = ""; //当前箱码
        var nextIsSwitchCartonNo = false; //是否需要生成箱码

        var selectDefectsModal = new app.ModalManager({
            viewUrl: abp.appPath + "CartonPrinting/SelectDefectsModal",
            scriptUrl: abp.appPath + "view-resources/Views/Carton/CartonPrinting/_SelectDefectsModal.js",
            modalClass: "SelectDefectsModal"
        });

        //包装箱码模块
        var cartonNoBox = {
            $_Carton: $("#CartonNo"),
            $_Restoration: $("#Restoration"),
            init: function () {
                this.bindCartonNoEvent();
                this.bindRestorationEvent();
            },
            bindCartonNoEvent: function () {
                //箱码回车事件
                this.$_Carton.keypress(function (e) {
                    if (e.charCode === 13) {
                        var cartonNo = cartonNoBox.$_Carton.val();
                        cartonNoBox.$_Carton.select();
                        service.getCartonAndSettingByCartonNo({ Id: cartonNo }).done(function (res) {
                            if (page.checkCurrentCartonSetting(res.cartonSetting)) {
                                currentCartonSetting = res.cartonSetting
                                currentCartonNo = cartonNo;
                                cartonNoBox.lockCartonNo();
                                cartonListBox.reload();
                                packingSettingBox.setPrintBotton();
                                packingSettingVue.cartonInfo = res.cartonInfo;
                                partScanningBox.$_PartNo.select();
                            } else {
                                abp.message.confirm(app.localize("ConfirmSwitchCurrentCartonSetting"), app.localize(L("ConfirmSwitchCurrentCartonSettingTitle")), function (isConfirmed) {
                                    if (isConfirmed) {
                                        currentCartonSetting = res.cartonSetting
                                        currentCartonNo = cartonNo;
                                        cartonNoBox.lockCartonNo();
                                        cartonListBox.reload();
                                        packingSettingBox.setPrintBotton();
                                        packingSettingVue.cartonInfo = res.cartonInfo;
                                        partScanningBox.$_PartNo.select();
                                    } else {
                                        return;
                                    }
                                });
                            };
                        })
                    }
                });
            },
            bindRestorationEvent: function () {
                this.$_Restoration.click(function () {
                    cartonNoBox.clearCarton();
                    partScanningBox.hiddenFinalInspectionDiv();
                })
            },
            clearCarton: function () {
                this.$_Carton.removeAttr("readonly");
                currentCartonNo = "";
                currentCartonSetting = {};
                this.$_Carton.val(currentCartonNo);
                cartonListBox.reload();
                packingSettingBox.setPrintBotton();
                packingSettingVue.cartonInfo = {};
                partInfoVue.partInfo = {};
                this.$_Carton.focus();
            },
            lockCartonNo: function () {
                cartonNoBox.$_Carton.val(currentCartonNo);
                cartonNoBox.$_Carton.attr("readonly", "readonly");
            }
        }

        //装箱列表模块
        var cartonListBox = {
            $_CartonListBoxBody: $("#CartonListBoxBody"),
            $table: $("#PartListTable"),
            datatables: null,
            init: function () {
                if (this.datatables) {
                    cartonListBox.datatables.destroy();
                    cartonListBox.$table.empty();
                }
                this.datatables = this.$table.WIMIDataTable({
                    "ajax": {
                        url: abp.appAPIPath + "carton/listCartonRecords",
                        data: function (d) {
                            $.extend(d, cartonListBox.getParams());
                        }
                    },
                    serverSide: true,
                    retrieve: true,
                    responsive: false,
                    ordering: false,
                    order: [],
                    scrollCollapse: true,
                    "sScrollY": "100%",
                    columns: cartonListBox.getColumns()
                });

                this.$table.on("click",
                    "tr",
                    function () {
                        let $currentRow = $(this);
                        let tr = cartonListBox.datatables.row($currentRow).node();
                        $currentRow = $(tr);
                        if ($currentRow.hasClass("selected")) {
                            return;
                        } else {
                            cartonListBox.datatables.$("tr.selected").removeClass("selected");
                            $currentRow.addClass("selected");
                            let currentRow = cartonListBox.datatables.row($currentRow).data();                         
                            cartonListBox.getCartonRecord(currentRow.id);
                        }
                    });
            },
            getParams: function () {
                return {
                    CartonNo: currentCartonNo
                };
            },
            getColumns: function () {
                return [
                    {
                        "data": "partNo",
                        "title": app.localize("WorkpieceQRCode")
                    },
                    {
                        "data": "shiftDay",
                        "title": app.localize("ShiftDay"),
                        "render": function (data) {
                            return wimi.btl.dateFormat(data);
                        }
                    },
                    {
                        "data": "shiftName",
                        "title": app.localize("ShiftName")                      
                    },
                    {
                        "data": "cartonTime",
                        "title": app.localize("CartonTime"),
                        "render": function (data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    }
                ];
            },
            getCartonRecord: function (cartonId) {
                service.getCartonRecord({ Id: cartonId }).done(function (res) {
                    partInfoVue.partInfo = res;
                });
            },
            getFirst() {
                let trObj = cartonListBox.datatables.row(0).node();
                if (trObj) {
                    let trData = cartonListBox.datatables.row(0).data();
                    cartonListBox.datatables.$("tr.selected").removeClass("selected");
                    $(trObj).addClass('selected');
                    cartonListBox.getCartonRecord(trData.id);
                }
            },
            reload: function () {
                cartonListBox.datatables.ajax.reload(cartonListBox.getFirst);
            }
        }
        
        //装箱设置模块
        var packingSettingBox = {
            $_PackingSettingBoxBody: $("#PackingSettingBoxBody"),
            $_PrinterSelect: $("#PrinterSelect"),
            $_PrintButton: $("#Print"),
            $_MaxCountText: $("#MaxCountText"),
            $_SaveMaxCount: $("#SaveMaxCount"),
            init: function () {
                this.setPrintBotton();
                this.bindPrintEvent();
                this.bindMaxCountEvent();
            },
            setPrintBotton: function () {
                if (currentCartonSetting.isPrint === true) {
                    packingSettingBox.loadPrint();
                    $("#Print").attr("disabled", false);
                } else {
                    $("#Print").attr("disabled", true);
                    $("#PrinterSelect").empty();
                }     
            },
            bindPrintEvent: function () {
                $("#Print").click(function () {
                    packingSettingBox.print();
                });
            },
            print: function () {
                debugger
                //if (currentCartonNo === "") {
                //    abp.notify.error(app.localize("CantPrint"));
                //    return;
                //}
                
                service.print({ PrinterName: $("#PrinterSelect").val(), CartonNo: currentCartonNo }).done(function () {
                    abp.notify.success(app.localize("PrintSuccess"));
                    page.clear();
                })
            },
            bindMaxCountEvent: function () {

                $("#SaveMaxCount").click(function () {
                    packingSettingBox.updateMaxCount($("#MaxCountText").val());
                });

                $("#MaxCountText").keypress(function (e) {
                    if (e.charCode === 13) {
                        packingSettingBox.updateMaxCount($("#MaxCountText").val());
                    }
                });
            },
            updateMaxCount: function (count) {
                if (currentCartonNo === "") {
                    abp.notify.error(app.localize("CantUpdateMaxCount"));
                    return;
                }

                if (parseInt(count) < packingSettingVue.cartonInfo.realPackingCount) {
                    abp.notify.error(app.localize("UpdateMaxCountError"));
                    packingSettingBox.getSettingByCurrentCartonNo();
                    return;
                }
                service.updateMaxCount({ MaxCount: count, CartonNo: currentCartonNo, CartonSettingId: currentCartonSetting.id }).done(function () {
                    abp.notify.success(app.localize("SuccessfullySaved"));
                    packingSettingBox.getSettingByCurrentCartonNo();
                })       
            },
            getSettingByCurrentCartonNo: function () {
                service.getCartonAndSettingByCartonNo({ Id: currentCartonNo }).done(function (res) {
                    cartonListBox.reload();
                    packingSettingVue.cartonInfo = res.cartonInfo;
                    partScanningBox.$_PartNo.select();
                })
            },
            loadPrint() {
                service.getInstalledPrinters()
                    .done(function (response) {
                        $("#PrinterSelect").empty();
                        var data = _.map(response,
                            function (item) {
                                return { id: item, text: item };
                            });

                        $("#PrinterSelect").select2({
                            data: data,
                            multiple: false,
                            minimumResultsForSearch: -1,
                            placeholder: app.localize("PleaseChoose"),
                            language: {
                                noResults: function () {
                                    return app.localize("NoMatchingData");
                                }
                            }
                        });

                        if ($.inArray(currentCartonSetting.printerName, response) >= 0) {
                            $("#PrinterSelect").val(currentCartonSetting.printerName).trigger("change");
                        }
                    });
            }
        }

        //装箱设置数据源
        var packingSettingVue = new Vue({
            el: '#PackingSettingVue',
            data: {
                "cartonInfo": {}
            }
        });

        //工件信息模块
        var partInfoBox = {
            $_PartInfoBox: $("#PartInfoBox"),
        }

        //工件信息数据源
        var partInfoVue = new Vue({
            el: '#PartInfoVue',
            methods: {
                dateFormat: function (data) {
                    if (data === undefined) {
                        return "";

                    } else {
                        return wimi.btl.dateTimeFormat(data);
                    }
                   
                },
                qualifiedFormat: function (data) {
                    if (data === undefined) {
                        return "";
                    } else {
                        if (data === true) {
                            return app.localize("Qualified");
                        } else if (data === false) {
                            return app.localize("Unqualified");
                        } else {
                            return app.localize("Unknown");
                        }
                    }
                }
            },
            data: {
                "partInfo": {}
            }
        });

        //工件扫描模块
        var partScanningBox = {
            $_PartNo: $("#PartNo"),  //工件二维码
            $_Submit: $("#Submit"),  //提交按钮
            $_ClearNo: $("#ClearNo"), //清空按钮
            $_FinalInspectionDiv: $("#FinalInspectionDiv"),  //合格/不合格按钮DIV
            $_FQualified: $("#FQualified"),  //合格按钮
            $_FUnQualified: $("#FUnQualified"), //不合格按钮
            init: function () {
                this.bindSubmitEvent();
                this.bindFinalIsQualifiedEvent();
                this.bindClearEvent();
            },
            bindClearEvent: function () {
                //提交按钮事件
                this.$_ClearNo.click(function () {
                    partScanningBox.clearPart();
                });

                //二维码回车事件
                this.$_ClearNo.keypress(function (e) {
                    if (e.charCode === 13) {
                        partScanningBox.clearPart();
                    }
                });
            },
            clearPart: function () {
                this.hiddenFinalInspectionDiv();
                this.$_PartNo.val("");
                this.$_PartNo.select();
            },
            bindSubmitEvent: function () {
                //提交按钮事件
                this.$_Submit.click(function () {
                    partScanningBox.submitPart();
                });

                //二维码回车事件
                this.$_PartNo.keypress(function (e) {
                    if (e.charCode === 13) {
                        partScanningBox.submitPart();
                    }
                });
            },
            //提交工件获取装箱设置
            submitPart: function () {
                var partNo = this.$_PartNo.val();
                partScanningBox.$_PartNo.select();
                //工件编号非空校验
                if (partNo.replace(/(^s*)|(s*$)/g, "").trim().length == 0) {
                    abp.notify.error(app.localize("PartNoCantBeNull"));
                    return;
                }

                //满箱检验
                if (page.checkIsFullCarton()) {
                    abp.message.confirm(app.localize("ConfirmFullCartonContinue"), app.localize("ConfirmFullCartonContinueTitle"), function (isConfirmed) {
                        if (isConfirmed) {
                            //满箱继续打包
                            page.clearAndSelectPartNo();
                            partScanningBox.getCartonSettingAndPacking(partNo);
                        } else {
                            return;
                        }
                    })
                }
                else {
                    partScanningBox.getCartonSettingAndPacking(partNo);
                }
            },
            getCartonSettingAndPacking: function (partNo) {
                service.getCartonSettingByParNo({ Id: partNo }).done(function (res) {
                    //规则校验
                    if (!page.checkCurrentCartonSetting(res)) {
                        abp.message.confirm(app.localize("ConfirmSwitchCurrentCartonSetting"), app.localize("ConfirmSwitchCurrentCartonSettingTitle"), function (isConfirmed) {
                            if (isConfirmed) {
                                page.clearAndSelectPartNo();
                                currentCartonSetting = res;

                                packingSettingBox.setPrintBotton();
                                if (currentCartonSetting.isFinalTest) {
                                    //终检,显示合格/不合格按钮
                                    partScanningBox.showFinalInspectionDiv();
                                }
                                else {
                                    //不终检，直接装箱操作
                                    partScanningBox.hiddenFinalInspectionDiv();
                                    partScanningBox.packing(partNo, currentCartonSetting.id);
                                }
                            } else {
                                return;
                            }
                        });
                    } else {
                        currentCartonSetting = res
                        packingSettingBox.setPrintBotton();
                        if (currentCartonSetting.isFinalTest) {
                            //终检,显示合格/不合格按钮
                            partScanningBox.showFinalInspectionDiv();
                        }
                        else {
                            //不终检，直接装箱操作
                            partScanningBox.hiddenFinalInspectionDiv();
                            partScanningBox.packing(partNo, currentCartonSetting.id);
                        }
                    };
                });
            },
            showFinalInspectionDiv: function () {
                this.$_FinalInspectionDiv.show();
                this.$_Submit.hide();
                this.$_PartNo.attr("readonly","readonly");
            },
            hiddenFinalInspectionDiv: function () {
                this.$_FinalInspectionDiv.hide();
                this.$_Submit.show();
                this.$_PartNo.removeAttr("readonly");
            },
            bindFinalIsQualifiedEvent: function () {
                this.$_FQualified.click(function () {
                    //合格下线
                    partScanningBox.$_PartNo.select();
                    service.finalInspec({ PartNo: partScanningBox.$_PartNo.val(), Qualified: true }).done(function () {
                        abp.notify.success(app.localize("FinalSpcOkOffline"));
                        //打包
                        partScanningBox.packing(partScanningBox.$_PartNo.val(), currentCartonSetting.id);
                        partScanningBox.hiddenFinalInspectionDiv();
                    }).always(function () {
                       
                    })               
                });

                this.$_FUnQualified.click(function () {
                    //不合格下线
                    selectDefectsModal.open({}, function (reasonIds) {
                        partScanningBox.$_PartNo.select();
                        service.finalInspec({ PartNo: partScanningBox.$_PartNo.val(), Qualified: false, DefectivePartReasonIds: reasonIds }).done(function () {
                            if (currentCartonSetting.isGoodOnly) {
                                abp.notify.success(app.localize("FinalSpcNgOffline"));
                            } else {
                                //打包
                                partScanningBox.packing(partScanningBox.$_PartNo.val(), currentCartonSetting.id);
                            }
                            partScanningBox.hiddenFinalInspectionDiv();
                        }).always(function () {
                            
                        });
                    });
                });
            },
            //提交装箱
            packing: function (partNo, cartonSettingId) {
                if (page.checkIsSwitch() === false) {
                    cartonNoBox.$_Carton.select();
                    abp.message.error(app.localize("PleaseSetCarton"));
                    return;
                };
                service.packing({ PartNo: partNo, CartonSettingId: cartonSettingId, CartonNo: currentCartonNo, IsSwitchNo: nextIsSwitchCartonNo }).done(function (res) {
                    packingSettingVue.cartonInfo = res; 
                    currentCartonNo = res.cartonNo;
                    cartonNoBox.lockCartonNo();
                    cartonListBox.reload();
                    abp.notify.success(app.localize("CartonSuccess"));
                    page.autoPrintCarton();
                }).always(function () {
                    nextIsSwitchCartonNo = false;
                })
            }

        }

        //整体页面
        var page = {       
            init: function () {
                this.initBoxHeight();
                this.initFocus();
                cartonNoBox.init();
                cartonListBox.init();
                packingSettingBox.init();
                partScanningBox.init();
            },
            initBoxHeight: function () {
                var height = packingSettingBox.$_PackingSettingBoxBody.outerHeight() + partInfoBox.$_PartInfoBox.outerHeight(true);
                cartonListBox.$_CartonListBoxBody.css("height", height);
            },
            initFocus: function () {
                partScanningBox.$_PartNo.focus();
            },
            checkCurrentCartonSetting: function (setting) {
                if (currentCartonSetting.id === undefined) {
                    return true;
                }
                else if (currentCartonSetting.id === setting.id) {
                    return true;
                }
                else if (currentCartonSetting.id != undefined && currentCartonSetting.id != setting.id) {
                    return false;
                }
                else {
                    return true;
                }
            },
            checkIsSwitch() {
                if (currentCartonSetting.autoCartonNo === false) {
                    nextIsSwitchCartonNo = false;
                    var cartonNo = cartonNoBox.$_Carton.val();
                    if (cartonNo.replace(/(^s*)|(s*$)/g, "").trim().length == 0) {
                        return false;
                    } else {
                        currentCartonNo = cartonNo;
                        return true;
                    }
                }
                else {
                    if (currentCartonNo === "") {
                        nextIsSwitchCartonNo = true;
                    }
                    return true;
                }                
            },
            autoPrintCarton: function () {
                if (currentCartonSetting.isPrint) {
                    if (currentCartonSetting.isAutoPrint) {
                        if (packingSettingVue.cartonInfo.maxPackingCount === packingSettingVue.cartonInfo.realPackingCount) {
                            $("#Print").click();
                        }
                    } else {
                        if (packingSettingVue.cartonInfo.maxPackingCount === packingSettingVue.cartonInfo.realPackingCount) {
                            abp.message.confirm(app.localize("ConfirmAutoPrintCarton"), app.localize("ConfirmAutoPrintCartonTitle"), function (isConfirmed) {
                                if (isConfirmed) {
                                    $("#Print").click();
                                }
                            })
                        }
                    }
                }        
            },
            clear: function () {
                cartonNoBox.clearCarton();
                partScanningBox.clearPart();
            },
            clearAndSelectPartNo: function () {
                cartonNoBox.clearCarton();
                partScanningBox.$_PartNo.select();
            },
            checkIsFullCarton: function () {
                if (packingSettingVue.cartonInfo.maxPackingCount != undefined
                    && packingSettingVue.cartonInfo.realPackingCount != undefined
                    && packingSettingVue.cartonInfo.maxPackingCount === packingSettingVue.cartonInfo.realPackingCount) {
                    return true;
                } else {
                    return false;
                }
            }
        };

        page.init();     
        
    });
})();