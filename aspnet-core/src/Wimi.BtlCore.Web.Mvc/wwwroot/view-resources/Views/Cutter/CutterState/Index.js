(function () {
    $(function () {
        var cutterAppService = abp.services.app.cutter,
            $btnCreate = $("#btnCreate"),
            $btnBatchUnloadCutter = $("#btnBatchUnloadCutter"),
            $btnUpdateRate = $("#btnUpdateRate"),
            $btnQuery = $("#btnQuery"),
            $btnExport = $("#btnExport");

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Cutter.CutterState.Manage")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "CutterState/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Cutter/CutterState/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        var loadOrUnloadModal = new app.ModalManager({
            viewUrl: abp.appPath + "CutterState/LoadOrUnLoadCutterModal",
            scriptUrl: abp.appPath + "view-resources/Views/Cutter/CutterState/_LoadOrUnLoadCutterModal.js",
            modalClass: "LoadOrUnloadModal"
        });

        var updateRateModal = new app.ModalManager({
            viewUrl: abp.appPath + "CutterState/UpdateRateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Cutter/CutterState/_UpdateRateModal.js",
            modalClass: "UpdateRateModal"
        });


        //查询
        var queryObject = {
            $type: $("#cutterType"),
            $modal: $("#cutterModel"),
            $cutterTypeTree: null,
            modalSelect2: null,
            $enumDropDown: $(".enumDropDown"),
            init: function () {
                var option = {
                    $typeSelector: this.$type,
                    istypeMultiple: true,
                    istypeDisabled: false,
                    $modalselector: this.$modal,
                    ismodalMultiple: true,
                    ismodalDisabled: false
                }
                this.$enumDropDown.select2({
                    minimumResultsForSearch: -1,
                    multiple: true
                });
                this.cutterTypeInit(option);
                this.cutterModalInit(option);
            },
            cutterModalInit: function (option) {
                cutterAppService.findCutterModal({ CutterModelIds: _.pluck(option.$typeSelector.select2('data'), 'id') }, { async: false })
                    .done(function (response) {
                        if (queryObject.modalSelect2) {
                            option.$modalselector.empty();
                        }

                        var data = _.map(response,
                            function(item) {
                                return { id: item.value, text: item.name };
                            });

                        queryObject.modalSelect2 = option.$modalselector.select2({
                            data: data,
                            multiple: option.ismodalMultiple,
                            placeholder: app.localize("PleaseSelect"),
                            language: {
                                noResults: function () {
                                    return app.localize("NoMatchingData");
                                }
                            }
                        });

                        option.$modalselector.prop("disabled", option.ismodalDisabled);
                        if (option.hasOwnProperty("modalSelectedValue")) {
                            option.$modalselector.select2({ minimumResultsForSearch: -1 })
                                .val(option.modalSelectedValue)
                                .trigger("change");
                        }
                    });
            },
            cutterTypeInit(option) {
                if (option.$typeSelector) {
                    option.$typeSelector.select2({
                        data: [],
                        multiple: option.istypeMultiple,
                        placeholder: app.localize("PleaseSelect"),
                        language: {
                            noResults: function () {
                                return app.localize("MaintainCutterType");
                            }
                        }
                    });

                    option.$typeSelector.prop("disabled", option.istypeDisabled);
                    var $tree = $("div.cutter-type-tree");
                    var defaultHtml = '<div class="alert alert-danger alert-dismissible"><h3>' + app.localize("MaintainToolType") + '</h3></div>';
                    var html = $tree.find('li').length === 0 ? defaultHtml : $tree.html();
                    option.$typeSelector.on("select2:open",
                        function () {
                            option.$typeSelector.select2("close");
                            layer.open({
                                type: 1,
                                title: app.localize("PleaseChooseCutterType"),
                                closeBtn: false,
                                area: '350px;',
                                shade: 0.8,
                                id: 'layuipro',
                                resize: false,
                                btn: [app.localize("Confirm"), app.localize("Cancel")],
                                btnAlign: 'r',
                                moveType: 1, //拖拽模式，0或者1
                                content: html,
                                success: function(layero) {
                                    if ($tree.find('li').length > 0) {
                                        //加载树
                                        queryObject.cutterTypeTreeInit($("#layuipro"), option.istypeMultiple);

                                        //将select2已选择的节点 在树上也选中
                                        var selectedNodeId = _.pluck(option.$typeSelector.select2('data'), 'id');
                                        queryObject.$cutterTypeTree.jstree().select_node(selectedNodeId);

                                        //确定按钮:将选中的节点加入select2
                                        layero.find('.layui-layer-btn0')
                                            .click(function() {
                                                option.$typeSelector.empty();
                                                var selectedNode =
                                                    queryObject.$cutterTypeTree.jstree().get_selected(true);
                                                _.each(selectedNode,
                                                    function(item) {
                                                        option.$typeSelector
                                                            .append(new Option($.trim(item.text),
                                                                item.id,
                                                                false,
                                                                true));
                                                    });
                                                queryObject.cutterModalInit(option);
                                            });
                                    }
                                },
                                end: function () {
                                    queryObject.$cutterTypeTree = null;
                                }
                            });
                        });
                }
            },
            cutterTypeTreeInit: function ($treeContainer, isneedCheckbox) {
                var plugins = ["types"];
                if (isneedCheckbox) {
                    plugins.push("checkbox");
                }
                queryObject.$cutterTypeTree = $treeContainer
                    .jstree({
                        'checkbox': {
                            keep_selected_style: true,
                            three_state: false,
                            cascade: ""
                        },
                        plugins: plugins
                    })
                    .on('ready.jstree',
                        function () {
                            queryObject.$cutterTypeTree.jstree('open_all');
                        });
            },
            callbackFunction: function (option) {
                if (option) {
                    queryObject.cutterTypeInit(option);
                    queryObject.cutterModalInit(option);
                }
            }
        }

        //表格
        var tableObject = {
            $table: $("#cuttertable"),
            dataTables: null,
            load: function () {
                var param = {
                    CutterNo: $.trim($("#cutterNo").val()),
                    CutterTypeIds: _.pluck($("#cutterType").select2('data'), 'id'),
                    CutterModelIds: _.pluck($("#cutterModel").select2('data'), 'id'),
                    CutterUsedStateses: _.pluck($("#UsedStates").select2('data'), 'id'),
                    CutterLifeStateses: _.pluck($("#LifeStates").select2('data'), 'id'),
                    MachineNo: $.trim($("#machineNo").val()),
                    CutterTValue: $.trim($("#cutterTValue").val())
                };

                if (this.dataTables) {
                    tableObject.dataTables.destroy();
                    tableObject.$table.empty();
                }

                if (_.contains(param.CutterUsedStateses,"0")) {
                    param.CutterUsedStateses = [];
                }

                if (_.contains(param.CutterLifeStateses, "0")) {
                    param.CutterLifeStateses = [];
                }

                cutterAppService.getCutterStatesColumns()
                    .done(function (response) {
                        tableObject.dataTables = tableObject.$table.WIMIDataTable({
                            "ajax": {
                                url: abp.appAPIPath + "cutter/getCutterStatesList",
                                data: param
                            },
                            'columnDefs': [{
                                'targets': 0,
                                'searchable': false,
                                'orderable': false,
                                'className': 'dt-body-center',
                                'render': function (data, type, full, meta) {
                                    return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(data).html() + '">';
                                }
                            }],
                            serverSide: true,
                            retrieve: true,
                            responsive: false,
							ordering: false,
                            scrollX: true,
                            columns: tableObject.getDataTablesColumns(response),
                            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                                var lifeState = aData.lifeStatusName;
                                if (lifeState === "Alarm") {
                                    $(nRow).css("background", "red");
                                }
                                if (lifeState === "Warning") {
                                    $(nRow).css("background", "yellow");
                                }
                            }
                        });

                        tableObject.bindCheckbox();
                    });
            },
            reload: function () {
                this.dataTables.ajax.reload(null);
                $('#select_all').prop('checked', false);
            },
            bindCheckbox: function () {
                $('#select_all').on('click', function () {
                    var rows = tableObject.dataTables.rows({ 'search': 'applied' }).nodes();
                    $('input[type="checkbox"]', rows).prop('checked', this.checked);
                });

                $('#cuttertable tbody').on('change', 'input[type="checkbox"]', function () {
                    if (!this.checked) {
                        var el = $('#select_all').get(0);
                        if (el && el.checked && ('indeterminate' in el)) {
                            el.indeterminate = true;
                        }
                    }
                });
            },
            getDataTablesColumns: function (data) {
                var colunm = [
                    {
                        "title": "<input  type='checkbox' id='select_all'/>"
                    },
                    {
                        "defaultContent": "",
                        "title": app.localize("Actions"),
                        "orderable": false,
                        "width": "10%",
                        "className": "text-center",
                        "createdCell": function (td, cellData, rowData, row, col) {
                            $(td)
                                .buildActionButtons([
                                    {
                                        title: app.localize('Edit'),
                                        clickEvent: function() { tableObject.updateRow(rowData) },
                                        isShow: permissions.manage &&
                                        (rowData.cutterUsedStatus === app.consts.cutterUsedStates.New ||
                                            rowData.cutterUsedStatus === app.consts.cutterUsedStates.NotLoad) //新增 和 未装刀
                                    }, {
                                        title: app.localize("CutterLoadAndUnload"),
                                        clickEvent: function() { tableObject.loadOrUnloadCutter(rowData); },
                                        isShow: permissions.manage &&
                                            rowData.cutterUsedStatus !== app.consts.cutterUsedStates.UnLoad
                                    },
                                    {
                                        title: app.localize('Delete'),
                                        className:"btn-danger",
                                        clickEvent: function() { tableObject.deleteRow(rowData); },
                                        isShow: permissions.manage &&
                                        (rowData.cutterUsedStatus === app.consts.cutterUsedStates.New ||
                                            rowData.cutterUsedStatus === app.consts.cutterUsedStates.NotLoad ||
                                            rowData.cutterUsedStatus ===app.consts.cutterUsedStates.UnLoad) //新增 、未装刀 和已拆卸 
                                    },
                                    {
                                        title: app.localize('Reset'),
                                        clickEvent: function () { tableObject.resetRow(rowData); },
                                        isShow: permissions.manage 
                                    }
                                ]);
                        }
                    },
                    {
                        title: app.localize("CutterNo"),
                        data: "cutterNo"
                    },
                    {
                        title: app.localize("ToolType"),
                        data: "cutterTypeName"
                    }, {
                        title: app.localize("CutterType"),
                        data: "cutterModelName"
                    }, {
                        title: app.localize("MachineCode"),
                        data: "machineNo"
                    },
                    {
                        title: app.localize("MachineName"),
                        data: "machineName"
                    }, {
                        title: app.localize("CutterTValue"),
                        data: "cutterTValue"
                    }, {
                        title: app.localize("CutterUsedStatus"),
                        data: "usedStatusName",
                        render: function (d) {
                            return app.localize(d);
                        }
                    }, {
                        title: app.localize("CutterLifeStatus"),
                        data: "lifeStatusName",
                        render: function (d) {
                            return app.localize(d);
                        }
                    }, {
                        title: app.localize("CountingMethod"),
                        data: "countingMethodName",
                        render: function (d) {
                            return app.localize(d);
                        }
                    }, {
                        title: app.localize("OriginalLife"),
                        data: "originalLife"
                    }, {
                        title: app.localize("UsedLife"),
                        data: "usedLife"
                    }, {
                        title: app.localize("RestLife"),
                        data: "restLife"
                    }, {
                        title: app.localize("WarningLife"),
                        data: "warningLife"
                    }
                ];

                _.each(data.parameterMap,
                    function (item) {
                        if (_.contains(_.pluck(data.dataTablesColumns, 'data'), item.name.toLowerCase())) {
                            colunm.push({
                                title: item.value,
                                data: item.name.toLowerCase()
                            });
                        }
                    });

                return colunm;
            },
            //删除
            deleteRow: function (rowData) {
                abp.message.confirm(app.localize("WhetherToDeleteTheRecord"),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            cutterAppService.deleteCutterStates({ id: rowData.id })
                                .done(function () {
                                    abp.notify.success(app.localize("SuccessfullyDeleted"));
                                    tableObject.reload();
                                });
                        }
                    });
            },
            //修改
            updateRow: function (rowData) {
                createOrUpdateModal.open({ Id: rowData.id, callback: queryObject.callbackFunction }, function (data) {
                    tableObject.reload();
                });
            },
            //重置寿命
            resetRow: function (rowData) {
                cutterAppService.resetCutterLife({ id: rowData.id })
                    .done(function () {
                        abp.notify.success(app.localize("OperationSuccess"));
                        tableObject.reload();
                    });
            },
            //装卸刀
            loadOrUnloadCutter: function (rowData) {
                loadOrUnloadModal.open({ Id: rowData.id }, function () {
                    tableObject.reload();
                });
            },
            export: function () {
                var param = {
                    CutterNo: $.trim($("#cutterNo").val()),
                    CutterTypeIds: _.pluck($("#cutterType").select2('data'), 'id'),
                    CutterModelIds: _.pluck($("#cutterModel").select2('data'), 'id'),
                    CutterUsedStateses: _.pluck($("#UsedStates").select2('data'), 'id'),
                    CutterLifeStateses: _.pluck($("#LifeStates").select2('data'), 'id'),
                    MachineNo: $.trim($("#machineNo").val()),
                    CutterTValue: $.trim($("#cutterTValue").val())
                };

                if (_.contains(param.CutterUsedStateses, "0")) {
                    param.CutterUsedStateses = [];
                }

                if (_.contains(param.CutterLifeStateses, "0")) {
                    param.CutterLifeStateses = [];
                }
                cutterAppService.exportCutterStatesList(param).done(function (result) {
                    app.downloadTempFile(result);
                });
            }
        }


        queryObject.init();


        //新建
        $btnCreate.click(function (e) {
            e.preventDefault();
            createOrUpdateModal.open({ callback: queryObject.callbackFunction }, function () {
                tableObject.reload();
            });
        });

        //批量卸刀
        $btnBatchUnloadCutter.click(function (e) {
            e.preventDefault();

            var selected = [];
            var illegalselected = [];
            _.each(tableObject.$table.find("input[type='checkbox']:checked"),
                function (key) {
                    var rowData = tableObject.dataTables.rows($(key).closest('tr')).data()[0];
                    if (rowData) {
                        selected.push(rowData.id);
                        if (rowData.cutterUsedStatus !==app.consts.cutterUsedStates.Loading) {
                            illegalselected.push(rowData.cutterNo);
                        }
                    }
                });

            if (selected.length > 0 && illegalselected.length > 0) {
                var msg = "";
                _.each(illegalselected,
                    function(key) {
                        msg += "[" + key + "] ";
                    });
                abp.message.error(msg, app.localize("FollowingCuttersCannotBeUnloaded"));
                return false;
            }

            if (selected.length > 0) {
                cutterAppService.bulkUnLoadCutters({ SelectedIds: selected })
                    .done(function () {
                        abp.notify.success(app.localize("OperationSuccess"));
                        tableObject.reload();
                    });
            } else {
                abp.message.error(app.localize("PleaseSelectAtLeastOneRecord"));
            }
        });

        $btnUpdateRate.click(function (e) {
            e.preventDefault();
            updateRateModal.open({ }, function () {
                tableObject.reload();
            });
        });

        //查询
        $btnQuery.click(function(e) {
                e.preventDefault();
                tableObject.load();
            })
            .trigger('click');

        //导出
        $btnExport.click(function (e) {
            e.preventDefault();
            tableObject.export();
        });

        var querySelect2Change = function (selectEventParam) {
            var selectData = selectEventParam.params.data;
            var $thisSelect = $(selectEventParam.target);

            var selectedStates = _.pluck($thisSelect.select2('data'), 'id');
            if (selectData.id * 1 > 0) {
                var filterVals = _.filter(selectedStates,
                    function (id) {
                        return id !== "0";
                    });
                $thisSelect.val(filterVals).trigger("change");
            } else {
                $thisSelect.val(["0"]).trigger("change");
            }
        }

        $('#UsedStates').on('select2:select', querySelect2Change);
        $('#LifeStates').on('select2:select', querySelect2Change);
    });
})();