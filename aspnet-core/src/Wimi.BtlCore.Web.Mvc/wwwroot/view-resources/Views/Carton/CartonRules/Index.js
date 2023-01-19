(function () {
    $(function () {

        var createOrUpdateRulesModal = new app.ModalManager({
            viewUrl: abp.appPath + "CartonRules/CreateOrUpdateCartonRules",
            scriptUrl: abp.appPath + "view-resources/Views/Carton/CartonRules/_CreateOrUpdateCartonRules.js",
            modalClass: "CreateOrUpdateRule"
        });

        var createRuleDetailModal = new app.ModalManager({
            viewUrl: abp.appPath + "CartonRules/CreateRulesDetail",
            scriptUrl: abp.appPath + "view-resources/Views/Carton/CartonRules/_CreateRulesDetail.js",
            modalClass: "CreateRuleDetail"
        });

        var updateRuleDetailModal = new app.ModalManager({
            viewUrl: abp.appPath + "CartonRules/UpdateRulesDetail",
            scriptUrl: abp.appPath + "view-resources/Views/Carton/CartonRules/_UpdateRulesDetail.js",
            modalClass: "UpdateRuleDetail"
        });

        var examineModal = new app.ModalManager({
            viewUrl: abp.appPath + "CartonRules/Examine",
            scriptUrl: abp.appPath + "view-resources/Views/Carton/CartonRules/_Examine.js",
            modalClass: "ExamineModal"
        });

        var service = abp.services.app.cartonRule;

        var pageButtons = {
            init: function () {
                let _this = this;
                $(document)
                    .on('switchChange.bootstrapSwitch',
                        '.switch',
                        function (event, state) {
                            var targetData = $(this).data();
                            //是否启用
                            var flag = $(this).attr('flag');
                            if (flag === '1') {
                                service.updateRulesStatus({ id: targetData.id })
                                    .done(function () {
                                        _this.getTraceList();
                                    }).fail(function () {
                                        _this.getTraceList();
                                    });
                            }
                        });
            }
        };

        var vm = new Vue({
            el: '#traceCatalogQuery',
            computed: {
            },
            created: function () {
                var _this = this;
                
            },
            mounted: function () {
                let _this = this;

                //pageButtons.init();
                $(document)
                    .on('switchChange.bootstrapSwitch',
                        '.switch',
                    function (event, state) {
                        abp.ui.setBusy('#ruleTable');
                        var targetData = $(this).data();
                        //是否启用
                        var flag = $(this).attr('flag');
                        if (flag === '1') {
                            service.updateRulesStatus({ id: targetData.id })
                                .done(function () {
                                    abp.ui.clearBusy('#ruleTable');
                                    _this.getTraceList();
                                }).fail(function () {
                                    _this.getTraceList();
                                }).always(function () {
                                    abp.ui.clearBusy('#ruleTable');
                                });
                        }
                        });

                _this.getTraceList();

                $("#btnCreateRule").on("click", function () {
                    createOrUpdateRulesModal.open({ id: null }, function () {
                        _this.getTraceList();
                    });
                });

                $("#btnCreateRuleDetail").on("click", function () {
                    createRuleDetailModal.open({ ruleId: _this.currId}, function () {
                        _this.getRuleDetail(_this.currId);
                    });
                });

                //绑定表格每行的click事件
                $("#ruleTable").on("click",
                    "tr",
                    function () {
                        let $currentRow = $(this);
                        let tr = _this.ruleTable.row($currentRow).node();
                        $currentRow = $(tr);
                        if ($currentRow.hasClass("selected")) {
                            return;
                        } else {
                            _this.ruleTable.$("tr.selected").removeClass("selected");
                            $currentRow.addClass("selected");
                            let currentRow = _this.ruleTable.row($currentRow).data();
                            _this.currId = currentRow.id;
                            _this.getRuleDetail(currentRow.id);
                        }
                    });
            },
            methods: {
                getTraceList() {
                    let _this = this;
                    if (_this.ruleTable != null) {
                        _this.ruleTable.destroy();
                    }
                    service.listCartonRules({ async: false }).done(function (response) {
                        _this.ruleTable = $('#ruleTable').WIMIDataTable({
                            "data": response,
                            "scrollX": true,
                            "responsive": false,
                            //"initComplete": function () {
                            //    let trObj = _this.ruleTable.row(0).node();
                            //    if (trObj) {
                            //        //let trData = _this.ruleTable.row(0).data();
                            //        //$(trObj).addClass('selected');
                            //        //_this.currId = trData.partNo;
                            //        //_this.getRuleDetail(trData.partNo);
                            //    }
                            //},
                            "serverSide":false,
                            "stateSave": false,
                            order: [],
                            "columns": [
                                {
                                    "data": 'name',
                                    "title": app.localize("Name")
                                },
                                {
                                    "data": "isActive",
                                    "orderable": false,
                                    "title": app.localize("IsActive"),
                                    "className": "not-mobile",
                                    "render": function (data, type, full, meta) {
                                        return '<input class="switch" type="checkbox" ' + (data ? "checked" : "")
                                            + ' data-id="' + full.id + '"'
                                            + ' data-tag="isactive"'
                                            + 'flag="1"'
                                            + ' />';

                                    }
                                },
                                {
                                    "data": 'id',
                                    "visible":false
                                },
                                {
                                    "defaultContent": "",
                                    "title": app.localize("Actions"),
                                    "orderable": false,
                                    "width": "30px",
                                    "className": "text-center",
                                    "createdCell": function (td, cellData, rowData, row, col) {
                                        $(td)
                                            .buildActionButtons([
                                                {
                                                    title: app.localize("Editor"),
                                                    clickEvent: function () {
                                                        createOrUpdateRulesModal.open({ id: rowData.id }, function () {
                                                            _this.getTraceList();
                                                        });
                                                    },
                                                    isShow: true
                                                },
                                                {
                                                    title: app.localize("Delete"),
                                                    clickEvent: function () { _this.deleteCartonRule(rowData); },
                                                    isShow: true
                                                }
                                            ]);
                                    }
                                }
                            ],
                            drawCallback: function (settings) {
                                $('.switch').bootstrapSwitch({ "size": "mini", "onColor": "success" });
                            }
                        });
                        let trObj = _this.ruleTable.row(0).node();
                        if (trObj) {
                            let trData = _this.ruleTable.row(0).data();
                            $(trObj).addClass('selected');
                            _this.currId = trData.id;
                            _this.getRuleDetail(trData.id);
                        }
                    });
                },
                deleteCartonRule(rowData) {
                    let _this = this;
                    abp.message.confirm(
                        app.localize("DeleteCartonRulesConfirm{0}",rowData.name),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                service.deleteCartonRule({ id: rowData.id }).done(function () {
                                    abp.notify.success(app.localize("SuccessfullyRemoved"));
                                    _this.getTraceList();
                                });
                            }
                        }
                    );
                },
                searchFn() {
                    let _this = this;
                    ngPartCatlogId = 0;

                    _this.getRuleDetail(0);
                    _this.ruleTable.ajax.reload(_this.getFirst);
                },
                getFirst() {
                    let _this = this;
                    let trObj = _this.ruleTable.row(0).node();
                    if (trObj) {
                        let trData = _this.ruleTable.row(0).data();
                        _this.ruleTable.$("tr.selected").removeClass("selected");
                        $(trObj).addClass('selected');
                        _this.currId = trData.id;
                        _this.getRuleDetail(trData.id);
                    }
                },
                getRuleDetail(id) {
                    let _this = this;
                    let pid = id || _this.currId;

                    if (id === 0) {
                        pid = 0;
                    }
                    

                    service.getRuleDetailsByRuleId({ id: pid }).done(function(res) {

                        if (_this.ruleDetailTable) {
                            _this.ruleDetailTable.destroy();
                            $('#ruleDetailTable').empty();
                        }
                        //_this.partDetails = res.partDetails;
                        //_this.traceRecords = res.traceRecords;
                        _this.cartonNoLength = res.additionalInfo.cartonNoLenth;
                        _this.previewCartonNo = res.additionalInfo.previewCartonNo;
                        _this.bindPreviewHover();

                        
                        var shiftMerge = false;
                        var lineMerge = false;
                        _this.ruleDetailTable = $('#ruleDetailTable').WIMIDataTable({
                            "data": res.ruleDetailItems,
                            "scrollX": true,
                            "responsive": false,
                            //"initComplete": function () {
                            //    let trObj = _this.ruleTable.row(0).node();
                            //    if (trObj) {
                            //        //let trData = _this.ruleTable.row(0).data();
                            //        //$(trObj).addClass('selected');
                            //        //_this.currId = trData.partNo;
                            //        //_this.getRuleDetail(trData.partNo);
                            //    }
                            //},
                            "serverSide": false,
                            "stateSave": false,
                            order: [],
                            "columns": [
                                {
                                    "data": 'sequenceNo',
                                    "title": app.localize("SequenceNo")
                                },
                                {
                                    "data": "type",
                                    "orderable": false,
                                    "className":"typeTD",
                                    "title": app.localize("Type"),
                                    "render": function (data, type, full, meta) {
                                        switch (data) {
                                            case 0: {
                                                return app.localize("Ascii");
                                            }
                                            case 1: {
                                                return app.localize("FixedString");
                                            }
                                            case 2: {
                                                return app.localize("Year");
                                            }
                                            case 3: {
                                                return app.localize("Month");
                                            }
                                            case 4: {
                                                return app.localize("Day");
                                            }
                                            case 5: {
                                                return app.localize("Season");
                                            }
                                            case 6: {
                                                return app.localize("Week");
                                            }
                                            case 7: {
                                                return app.localize("Shift");
                                            }
                                            case 8: {
                                                return app.localize("ProductionLine");
                                            }
                                            case 9: {
                                                return app.localize("SerialNum");
                                            }
                                            case 10: {
                                                return app.localize("CalibratorCode");
                                            }
                                            case 11: {
                                                return app.localize("SpecialCode");
                                            }
                                            case 12: {
                                                return app.localize("Time");
                                            }
                                        }
                                    }
                                },
                                {
                                    "data": "length",
                                    "orderable": false,
                                    "title": app.localize("Length"),
                                    "render": function (data, type, full, meta) {
                                        return data;
                                    }
                                },
                                {
                                    "data": "value",
                                    "orderable": false,
                                    "title": app.localize("Value"),
                                    "render": function (data, type, full, meta) {
                                        switch (full.type) {
                                            case 3:
                                            case 5:
                                            case 6:
                                            case 9:
                                            case 11:{
                                                return "";
                                            }
                                            case 4: {
                                                if (data == 1) {
                                                    return app.localize("NaturalDay");
                                                } else {
                                                    return app.localize("DayOfYear");
                                                }

                                            }
                                            default: {
                                                return data;
                                            }

                                        }
                                    }
                                },
                                {
                                    "data": "remark",
                                    "orderable": false,
                                    "title": app.localize("Remark")
                                },
                                {
                                    "defaultContent": "",
                                    "title": app.localize("Actions"),
                                    "orderable": false,
                                    "width": "30px",
                                    "className": "text-center",
                                    "createdCell": function (td, cellData, rowData, row, col) {
                                        $(td)
                                            .buildActionButtons([
                                                {
                                                    title: app.localize("Editor"),
                                                    clickEvent: function () {
                                                        updateRuleDetailModal.open({ id: rowData.id, type: rowData.type, ruleId:_this.currId}, function () {
                                                            _this.getRuleDetail(_this.currId);
                                                        });
                                                    },
                                                    isShow: true
                                                },
                                                {
                                                    title: app.localize("Delete"),
                                                    clickEvent: function () {
                                                        _this.deleteRuleDetail(rowData);
                                                    },
                                                    isShow: true
                                                },
                                                {
                                                    title: app.localize("Examine"),
                                                    clickEvent: function () {
                                                        examineModal.open({ ruleId: _this.currId });
                                                    },
                                                    isShow: rowData.type == 10
                                                }
                                            ]);
                                    }
                                },
                                {
                                    "data": 'id',
                                    "visible": false
                                }
                            ],
                            drawCallback: function (settings) {
                                if (settings.aiDisplay.length === 0) {
                                    return;
                                }
                                var newGroupIndex = 0;
                                var rowSpan = 1;
                                var groupbytds = [0, 1, 2];//合并列依据索引
                                var grouptds = [0, 1, 2];//需要合并的列索引;
                                var pageLength = $('#ruleDetailTable').find("tbody tr").length;

                                $.each($('#ruleDetailTable').find("tbody tr"), function (index, value) {

                                    function isequalnexttd($table, groupbytds) {

                                        for (var i = 0; i < groupbytds.length; i++) {

                                            var currenttd = value.cells[groupbytds[i]];

                                            var nexttd = $table.find("tbody tr:eq(" + (index + 1) + ")")[0].cells[groupbytds[i]];

                                            if (currenttd.innerHTML !== nexttd.innerHTML) {
                                                return false;
                                            }
                                        }

                                        return true;
                                    }

                                    function isequalprevioustd($table, groupbytds) {

                                        for (var i = 0; i < groupbytds.length; i++) {

                                            var currenttd = value.cells[groupbytds[i]];

                                            var previoustd = $table.find("tbody tr:eq(" + (index - 1) + ")")[0].cells[groupbytds[i]];

                                            if (currenttd.innerHTML !== previoustd.innerHTML) {
                                                return false;
                                            }
                                        }

                                        return true;
                                    }

                                    if (index === pageLength - 1) {
                                        //最后一行特殊处理
                                        if (isequalprevioustd($('#ruleDetailTable'), groupbytds)) {
                                            rowSpan++;
                                        }

                                        for (i = 0; i < grouptds.length; i++) {
                                            $($('#ruleDetailTable').find("tbody tr:eq(" + newGroupIndex + ")")[0].cells[grouptds[i]]).attr("rowspan", rowSpan);
                                            $($('#ruleDetailTable').find("tbody tr:eq(" + newGroupIndex + ")")[0].cells[grouptds[i]]).addClass("groupedtd");
                                        }
                                    } else {

                                        if (isequalnexttd($('#ruleDetailTable'), groupbytds)) {
                                            rowSpan++;

                                            for (i = 0; i < grouptds.length; i++) {
                                                var node = $('#ruleDetailTable').find("tbody tr:eq(" + (index + 1) + ")")[0].cells[grouptds[i]];
                                                $(node).hide();
                                            }
                                        }
                                        else {
                                            for (i = 0; i < grouptds.length; i++) {
                                                $($('#ruleDetailTable').find("tbody tr:eq(" + newGroupIndex + ")")[0].cells[grouptds[i]]).attr("rowspan", rowSpan);
                                                $($('#ruleDetailTable').find("tbody tr:eq(" + newGroupIndex + ")")[0].cells[grouptds[i]]).addClass("groupedtd");
                                            }

                                            newGroupIndex = index + 1;
                                            rowSpan = 1;
                                        }
                                    }
                                });
                            }
                        });
                    }).fail(function(res) {
                        service.rerSetCalibratorCode({ id: pid }).done(function(res) {
                            abp.notify.success(app.localize("校验码已重置"));
                            console.log(_this.currId);
                            _this.getRuleDetail(_this.currId);

                        })
                    });

                },
                bindPreviewHover() {
                    let _this = this;
                    $("#barcodeDom").remove();
                    $("#preview").popover({
                        html: true,
                        placement: "auto",
                        container: 'body',
                        trigger: "hover",
                        title: app.localize("Preview"),
                        content: function () {
                            var html = '<div class="row" id="barcodeDom" style="width:800px;"><div class="col-xs-12 col-sm-4 col-md-4 order-barcode xs-mh-10"></div></div>';
                            var settings = {
                                output: "css",
                                bgColor: "#D2EAC2",
                                color: "#000000",
                                barWidth: 1,
                                barHeight: 70,
                                width:'auto'
                            };
                            $(html).appendTo('body');
                            return $('.order-barcode').html("").show().barcode(_this.previewCartonNo, "code128", settings); 
                            
                        }
                    });
                },
                deleteRuleDetail(rowData) {
                    let _this = this;
                    abp.message.confirm(
                        app.localize("DeleteRulesDetailConfirm"),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                service.deleteRuleDetailById({ id: rowData.id }).done(function () {
                                    abp.notify.success(app.localize("SuccessfullyRemoved"));
                                    _this.getRuleDetail(_this.currId);
                                });
                            }
                        }
                    );
                },
                openModal(row) {

                }   
            },
            filters: {
                formatData: function (time) {

                    if (time === null) {
                        return;
                    }

                    return moment(time).format("YYYY-MM-DD HH:mm:ss");
                }
            },
            data: {
                ruleTable: null,
                ruleDetailTable:null,
                currId:0,
                cartonNoLength: "",
                previewCartonNo:""
            }
        });
    });
})(jQuery);