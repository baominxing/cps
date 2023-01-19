!(function () {
    // var _appService = abp.services.app.deviceGroup;
    var service = abp.services.app.trace;

    const relateDataSourceSettingsDefault = {
        source: 0,
        authModel: {
            dataLocation: "",
            authorisedAccount: "",
            authorisedPassword: ""
        }
    };
    const contentWriteIntoPlcDefault = { name: "", value: "0" };
    const offlineByQualityDefault = {
        offlineWhenOk: false,
        offlineWhenNg: false
    };

    var vm = new Vue({
        el: '#traceability-setting',
        directives: {
            select2: {
                inserted: function (el, binding, vnode) {
                    let options = binding.value || {};
                    $(el).select2(options).on("select2:select", (e) => {
                        el.dispatchEvent(new Event('change', { target: e.target }));
                    });
                },
                update: function (el, binding, vnode) {
                    for (var i = 0; i < vnode.data.directives.length; i++) {
                        if (vnode.data.directives[i].name == "model") {
                            $(el).val(vnode.data.directives[i].value);
                        }
                    }
                    $(el).trigger("change");
                }
            }
        },
        computed: {

        },
        created: function () {
            var _this = this;
            _this.getGroupList();
        },
        directives: {
            select2: {
                inserted: function (el, binding, vnode) {
                    let options = binding.value || {};
                    $(el).select2(options).on("select2:select", (e) => {
                        el.dispatchEvent(new Event('change', { target: e.target }));
                    });
                },
                update: function (el, binding, vnode) {
                    for (var i = 0; i < vnode.data.directives.length; i++) {
                        if (vnode.data.directives[i].name == "model") {
                            $(el).val(vnode.data.directives[i].value);
                        }
                    }
                    $(el).trigger("change");
                }
            }
        },
        methods: {
            getGroupList() {
                let _this = this;
                abp.ajax({
                    url: abp.appAPIPath + 'deviceGroup/ListRootDevices',
                    type: "POST"
                }).done(function (res) {
                    _this.listRootDevices = res;

                    if (_this.listRootDevices.length > 0) {
                        _this.rootDevice = _this.listRootDevices[0].value;
                        _this.initTable();
                    }
                });
            },
            onFlowTypeChange() {
                let _this = this;
                if (_this.flowType === 0) {
                    _this.triggerEndFlowStyle = 1;
                }
            },
            changeList() {
                let _this = this;
                _this.reloadTable();
                _this.getDaoxu();
                _this.getStationType();
                _this.getFlowTypeList();
                _this.clearInfo();
            },
            initTable() {
                let _this = this;
                //if (_this.rootDevice === '') return;
                _this.listLineTable = $('#listLineTable').WIMIDataTable({
                    "initComplete": function () {
                        _this.getFirstTableRow();
                    },
                    order: [],
                    "ajax": {
                        "url": abp.appAPIPath + "trace/ListLineFlowSettings",
                        "type": "POST",
                        "data": function (d) {
                            $.extend(d, {
                                "deviceGroupId": _this.rootDevice
                            });

                        }
                    },
                    "columns": [
                        {
                            "defaultContent": "",
                            "title": app.localize("Actions"),
                            "orderable": false,
                            "width": "20%",
                            "className": "text-center",
                            "createdCell": function (td, cellData, rowData, row, col) {
                                $(td).buildActionButtons([
                                    {
                                        title: app.localize("Editor"),
                                        clickEvent: function () { _this.settingForSelectedRow(rowData, td) },
                                        isShow: true
                                    },
                                    {
                                        title: app.localize("Delete"),
                                        clickEvent: function () { _this.deleteLine(rowData) },
                                        isShow: true
                                    }
                                ]);
                            }
                        },
                        {
                            "data": "code",
                            "title": app.localize("ProcessNumber")
                        },
                        {
                            "data": "displayName",
                            "title": app.localize("FlowName")
                        },
                        {
                            "data": "flowType",
                            "title": app.localize("ProcessCategory"),
                            "render": function (data) {

                                return _this.flowTypeDictionary['"' + data + '"'];
                            }
                        }
                    ]
                });
            },
            reloadTable() {
                let _this = this;
                _this.listLineTable.ajax.reload(_this.getFirstTableRow);
            },
            getFirstTableRow() {
                let _this = this;
                let trObj = _this.listLineTable.row(0).node();
                if (trObj) {
                    let trData = _this.listLineTable.row(0).data();
                    $(trObj).addClass('selected');
                    _this.getDetail(trData);
                    _this.isEdit = true;
                }
            },
            //选中行的相关设定
            settingForSelectedRow(currentRow, td) {
                let _this = this;
                if ($(td).parent().hasClass("selected")) {
                    return;
                }
                _this.listLineTable.$("tr.selected").removeClass("selected");
                $(td).parent().addClass('selected');
                _this.getDetail(currentRow);


            },
            //获取流程详情
            getDetail(currentRow) {
                let _this = this;
                _this.traceFlowSettingId = currentRow.id;
                _this.isEdit = true;
                _this.getDetailInfo(currentRow.id);

            },
            getDetailInfo(id) {
                let _this = this;
                //_this.vform.resetForm();
                service.getFlowSettingsDetail({ id: id }).then(function (res) {
                    let data = res.flowSetting;
                    //groupId
                    _this.rootDevice = data.deviceGroupId;
                    _this.code = data.code;
                    _this.displayName = data.displayName;
                    _this.flowSeq = data.flowSeq;
                    _this.preFlowId = data.preFlowId + '';
                    _this.nextFlowId = data.nextFlowId + '';
                    _this.flowType = data.flowType + '';
                    _this.triggerEndFlowStyle = data.triggerEndFlowStyle;
                    _this.stationType = data.stationType;    //工位类型

                    _this.writeIntoPlcViaFlow = data.writeIntoPlcViaFlow;
                    _this.writeIntoPlcViaFlowData = data.writeIntoPlcViaFlowData;
                    _this.contentWriteIntoPlcViaFlow = data.contentWriteIntoPlcViaFlow || $.extend({}, contentWriteIntoPlcDefault);
                    _this.contentWriteIntoPlcViaFlowData = data.contentWriteIntoPlcViaFlowData || $.extend({}, contentWriteIntoPlcDefault);
                    _this.qualityMakerFlowId = data.qualityMakerFlowId;
                    _this.offlineByQuality = data.offlineByQuality || $.extend({}, offlineByQualityDefault);
                    _this.relateDataSourceSettings = data.relateDataSourceSettings || $.extend({}, relateDataSourceSettingsDefault);
                    _this.sourceOfPartNo = data.sourceOfPartNo;
                    _this.needHandlerRelateData = data.needHandlerRelateData;

                    _this.eqList = res.relatedMachines;

                    service.listLineFlowSettingsByGroupId({ id: _this.rootDevice }).then(function (groupData) {
                        _this.orderOption = groupData;
                    });
                });
            },
            deleteLine(rowData) {
                let _this = this;
                abp.message.confirm(app.localize("BeSureToDeleteThisData")+"?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            service.deleteTraceFlowSetting({ id: rowData.id }).then(function (res) {
                                _this.clearInfo();
                                _this.reloadTable();
                                _this.getDaoxu();
                            });
                        }
                    });

            },
            //获取上道序   下道序
            getDaoxu() {
                let _this = this;

                if (_this.rootDevice == '') {
                    _this.orderOption = [];
                    return;
                }
                service.listLineFlowSettingsByGroupId({ id: _this.rootDevice }).then(function (data) {
                    _this.orderOption = data;
                });
            },
            //获取流程类型list
            getFlowTypeList() {
                let _this = this;
                service.listFlowType().then(function (data) {
                    _this.flowTypeList = data;

                    _.each(data, function (ele, index) {
                        _this.flowTypeDictionary['"' + ele.value + '"'] = ele.name;
                    });
                });
            },
            //获取工位类型
            getStationType() {
                let _this = this;
                service.listStationType().then(function (data) {
                    _this.stationTypeList = data;
                });
            },
            addProcess() {
                let _this = this;
                _this.isEdit = false;
                _this.listLineTable.$("tr.selected").removeClass("selected");
                _this.clearInfo();
                _this.getDaoxu();
            },
            clearInfo() {
                let _this = this;
                _this.isEdit = false;
                _this.traceFlowSettingId = '0';
                //_this.addId = -1;
                _this.code = '';
				_this.displayName = '';
				_this.flowSeq=0;
                _this.preFlowId = '';
                _this.nextFlowId = '';
                _this.triggerEndFlowStyle = 1;
                _this.stationType = '';    //产线类型
                _this.flowType = 0;
                _this.writeIntoPlcViaFlow = false;
                _this.writeIntoPlcViaFlowData = false;
                _this.contentWriteIntoPlcViaFlow = $.extend({}, contentWriteIntoPlcDefault);
                _this.contentWriteIntoPlcViaFlowData = $.extend({}, contentWriteIntoPlcDefault);
                _this.qualityMakerFlowId = "";
                _this.offlineByQuality = offlineByQualityDefault;
                _this.relateDataSourceSettings = relateDataSourceSettingsDefault;
                _this.sourceOfPartNo = 1;
                _this.needHandlerRelateData = false;

                _this.eqList = [];
                _this.vform.resetForm();
            },
            clearMachInfo() {
                let _this = this;
                _this.machineId = '';
                _this.workingStationCode = '';
                _this.workingStationDisplayName = '';
            },
            showMachineModal() {
                let _this = this;
                _this.clearMachInfo();
                _this.machineList = [];
                service.listMachines({ id: _this.rootDevice }).then(function (data) {
                    _this.machineList = data;
                });
            },
            addMachineFn() {   //增加设备
                let _this = this;

                let param = {
                    "traceFlowSettingId": _this.traceFlowSettingId,
                    "relatedMachineDto": {
                        "machineId": _this.machineId,
                        "traceFlowSettingId": _this.traceFlowSettingId,
                        "workingStationCode": _this.workingStationCode,
                        "workingStationDisplayName": _this.workingStationDisplayName
                    }
                };

                service.addMachineIntoTraceFlowSetting(param).then(function (data) {
                    _this.machineId = '';
                    _this.workingStationCode = '';
                    _this.workingStationDisplayName = '';
                    _this.getDetailInfo(_this.traceFlowSettingId);
                })


            },
            deleteMachine(row) {
                let _this = this;
                abp.message.confirm(app.localize("BeSureToDeleteThisData") + "?",
                    function (isConfirmed) {
                        if (isConfirmed) {

                            let param = {
                                "traceFlowSettingId": _this.traceFlowSettingId,
                                "relatedMachineDto": {
                                    "machineId": row.machineId,
                                    "machineName": row.machineName,
                                    "traceFlowSettingId": row.traceFlowSettingId,
                                    "workingStationCode": row.workingStationCode,
                                    "workingStationDisplayName": row.workingStationDisplayName
                                }
                            }


                            service.removeMachineFromTraceFlowSetting(param).then(function (data) {
                                _this.getDetailInfo(_this.traceFlowSettingId);
                            })
                        }
                    });

            },
            addFlowSettings() {
                let _this = this;
                if (_this.rootDevice === '') {
                    abp.message.warn(app.localize("PleaseSelectTheProductionLineFirst")+'!');
                    return;
                }
                let param = {
                    "deviceGroupId": _this.rootDevice,
                    "code": _this.code,
					"displayName": _this.displayName,
					"flowSeq": _this.flowSeq,
                    "preFlowId": _this.preFlowId,
                    "nextFlowId": _this.nextFlowId,
                    "flowType": _this.flowType,
                    "stationType": _this.stationType,
                    "triggerEndFlowStyle": _this.triggerEndFlowStyle,
                    "writeIntoPlcViaFlow": _this.writeIntoPlcViaFlow,
                    "writeIntoPlcViaFlowData": _this.writeIntoPlcViaFlowData,
                    "contentWriteIntoPlcViaFlow": _this.contentWriteIntoPlcViaFlow.value ? _this.contentWriteIntoPlcViaFlow : null,
                    "contentWriteIntoPlcViaFlowData": _this.contentWriteIntoPlcViaFlowData.value ? _this.contentWriteIntoPlcViaFlowData : null,
                    "qualityMakerFlowId": _this.qualityMakerFlowId,
                    "offlineByQuality": _this.offlineByQuality,
                    "relateDataSourceSettings": _this.relateDataSourceSettings,
                    "needHandlerRelateData": _this.needHandlerRelateData,
                    "sourceOfPartNo": _this.sourceOfPartNo
                };
                service.saveTraceFlowSetting(param).then(function (data) {
                    abp.message.success(app.localize("SavedSuccessfully"));
                    _this.rootDevice = data.flowSetting.deviceGroupId;
                    _this.traceFlowSettingId = data.flowSetting.id;
                    _this.reloadTable();
                });
            },
            updateFlowSettings() {
                let _this = this;
                let param = {
                    "id": _this.traceFlowSettingId,
                    "deviceGroupId": _this.rootDevice,
                    "code": _this.code,
					"displayName": _this.displayName,
					"flowSeq": _this.flowSeq,
                    "preFlowId": _this.preFlowId === "null" ? 0 : _this.preFlowId,
                    "nextFlowId": _this.nextFlowId === "null" ? 0 : _this.nextFlowId,
                    "flowType": _this.flowType,
                    "stationType": _this.stationType,
                    "triggerEndFlowStyle": _this.triggerEndFlowStyle,
                    "writeIntoPlcViaFlow": _this.writeIntoPlcViaFlow,
                    "writeIntoPlcViaFlowData": _this.writeIntoPlcViaFlowData,
                    "contentWriteIntoPlcViaFlow": _this.contentWriteIntoPlcViaFlow.value ? _this.contentWriteIntoPlcViaFlow : null,
                    "contentWriteIntoPlcViaFlowData": _this.contentWriteIntoPlcViaFlowData.value ? _this.contentWriteIntoPlcViaFlowData : null,
                    "qualityMakerFlowId": _this.qualityMakerFlowId,
                    "offlineByQuality": _this.offlineByQuality,
                    "relateDataSourceSettings": _this.relateDataSourceSettings,
                    "sourceOfPartNo": _this.sourceOfPartNo,
                    "needHandlerRelateData": _this.needHandlerRelateData
                };
                service.updateTraceFlowSetting(param).done(function (data) {
                    _this.reloadTable();
                });
            },
            //保存
            submit() {
                let _this = this;


                if (!$("#commentForm").valid()) {
                    return;
                }
                if (_this.isEdit) {
                    _this.updateFlowSettings();
                } else {
                    _this.addFlowSettings();
                }
            },

            initSelect2() {

                $("#rootDevice").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                }).trigger("change");
                $("#machineId").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                }).trigger("change");
                $("#stationType").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                });
                $("#flowType").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                });

                $("#qualityMakerFlowId").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                });
            }
        },
        mounted: function () {
            let _this = this;
            _this.getFlowTypeList();
            _this.getStationType();
            _this.getDaoxu();
            _this.initSelect2();

            _this.vform = $("#commentForm").validate({
                errorElement: "span",
                rules: {
                    processNumber: {
                        required: true,
                        maxlength: 40
                    },
                    processName: {
                        required: true,
                        maxlength: 100
                    },
                    stationType: {
                        required: true
                    }
                },
                messages: {
                    processNumber: {
                        required: app.localize("PleaseEnterTheProcessNumber"),
                        maxlength: app.localize("MaxLength")+"40"
                    },
                    processName: {
                        required: app.localize("PleaseEnterTheProcessName"),
                        maxlength: app.localize("MaxLength") + "100"
                    },
                    stationType: {
                        required: app.localize("PleaseEnterTheStationType")
                    }
                },
                showErrors: function (errorMap, errorList) {
                    $.each(errorList, function (i, v) {
                        layer.tips(v.message, v.element, {
                            time: 3000, tipsMore: true, tips: [3, '#d9534f']
                        });
                    });
                }
            });

            //绑定表格每行的click事件
            $(document).on("click", "#listLineTable tbody tr",
                function () {
                    let $currentRow = $(this);
                    let tr = _this.listLineTable.row($currentRow).node();
                    $currentRow = $(tr);
                    if ($currentRow.hasClass("selected")) {
                        return;
                    } else {
                        _this.listLineTable.$("tr.selected").removeClass("selected");
                        $currentRow.addClass("selected");
                        let currentRow = _this.listLineTable.row($currentRow).data();
                        _this.getDetail(currentRow);
                    }
                });

            //$("#stationType").change(function () {
            //    _this.stationType = $("#stationType").val();
            //});
        },
        data: {

                vform: null,
                addId: -1,
                rootDevice: '',
                listRootDevices: [],
                listLineTable: null,
                isEdit: false,
                isEditEq: false,
                //lineOPtion: [],
                //上道流程
                orderOption: [],
                upperOrderOption: [{
                    displayName: '',
                    value: '1'
                }],
                //下道流程
                nextOrderOption: [],
                //产线类型
                lineTypeOption: [],

                //编辑或添加
                code: '',
                displayName: '',
                flowSeq: 0,
                preFlowId: '0',
                nextFlowId: '0',
                triggerEndFlowStyle: 1,      //流程触发选择
                stationType: '3',          //工位类型
                flowType: 0,
                writeIntoPlcViaFlow: false,
                writeIntoPlcViaFlowData: false,
                contentWriteIntoPlcViaFlow: $.extend({}, contentWriteIntoPlcDefault),
                contentWriteIntoPlcViaFlowData: $.extend({}, contentWriteIntoPlcDefault),
                qualityMakerFlowId: "",
                offlineByQuality: offlineByQualityDefault,
                relateDataSourceSettings: relateDataSourceSettingsDefault,
                sourceOfPartNo: 1,
                needHandlerRelateData: false,
                //工位类型
                stationTypeList: [],
                feedingTypeList: [],
                flowTypeList: [],
                flowTypeDictionary: {},
                eqList: [
                ],

                //添加设备
                isEditMachine: false,
                traceFlowSettingId: '0',
                machineId: '0',

                workingStationCode: '',
                workingStationDisplayName: '',

                //addMachine: '',
                //addGongwei: '',
                //addName: '',
                //checkedItem: [],
                machineList: []
               
        }
    });
})();