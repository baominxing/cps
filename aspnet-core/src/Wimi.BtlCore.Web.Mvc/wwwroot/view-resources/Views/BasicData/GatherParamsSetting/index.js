(function () {
    $(function () {
        var $machineTree = $("div.machines-tree");
        var params,
            pageButtons;
        var basicDataService = abp.services.app.basicData;

        var permissions = {
            manage: abp.auth.hasPermission('Pages.BasicData.GatherParamsSetting.Manage')
        };
        var editParamModal = new app.ModalManager({
            viewUrl: abp.appPath + 'GatherParamsSetting/EditModal',
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/GatherParamsSetting/_EditModal.js",
            modalClass: 'GatherParamsSettingEditModal'
        });
        var selectGroupModal = new app.ModalManager({
            viewUrl: abp.appPath + 'GatherParamsSetting/SelectGroupModal',
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/GatherParamsSetting/_SelectGroupModal.js",
            modalClass: 'SelectGroupModal'
        });

        var batchSwitchModal = new app.ModalManager({
            viewUrl: abp.appPath + 'GatherParamsSetting/BatchSwitchModal',
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/GatherParamsSetting/_BatchSwitchModal.js",
            modalClass: 'BatchSwitchModal'
        });


        var tree = new MachinesTree();

        function checkHasTreeNodeSelected() {
            var selectedMachineNode = tree.getSelectedNode().length > 0 &&
                _.pluck(tree.getSelectedNode(), "data")[0].hasOwnProperty('machineid');
            if (!selectedMachineNode) {
                abp.message.error(app.localize("ChooseMachineNode"));
                return false;
            }
            return true;
        }

        pageButtons = {
            init: function () {

                $("#BulkSetButton").click(function (e) {
                        e.preventDefault();

                        if (!checkHasTreeNodeSelected()) {
                            return false;
                        }
                        params.bulkSet();
                });

                $("#selectTenants").on("change",
                    function () {
                        $(this).closest('form').submit();
                    });

                $("#BulkDefaultSetButton").click(function (e) {
                    e.preventDefault();

                    if (!checkHasTreeNodeSelected()) {
                        return false;
                    }

                    params.bulkDefaultSet();
                });

                $("#btn-refresh").click(function () {
                    if (!checkHasTreeNodeSelected()) {
                        return;
                    }
                    params.refreshSettings();
                });

                $(document)
                    .on('switchChange.bootstrapSwitch',
                        '.switch',
                        function (event, state) {
                            var targetData = $(this).data();
                            if (targetData.tag === "priorshow") {
                                // 看板
                                basicDataService
                                    .updateMachineGatherParamPriorShow({ id: targetData.id })
                                    .done(function () {
                                        params.loadtable();
                                    });
                            } else if (targetData.tag === "showstate") {
                                // 实时状态
                                basicDataService
                                    .updateMachineGatherParamShowState({ id: targetData.id })
                                    .done(function () {
                                        params.loadtable();
                                    });
                            } else {
                                // 运行参数
                                basicDataService
                                    .updateMachineGatherParamShowParam({ id: targetData.id })
                                    .done(function () {
                                        params.loadtable();
                                    });
                            }

                        });
            }
        };

        params = {
            $paramsTable: $("#ParamsTable"),
            $paramsDataTable: null,
            editParam: function (args) {
                editParamModal.open(args, function () {
                    params.loadtable();
                });
            },
            bulkSet: function () {
                selectGroupModal.open({ id: tree.getSelectedMachineIds()[0] }, function (result) {
                    params.loadtable();
                });
            },
            bulkDefaultSet: function () {
                basicDataService.bulkSetGatherParamsViaHost({ id: tree.getSelectedMachineIds()[0] })
                    .done(function () {
                        params.loadtable();
                    });
            },
            refreshSettings: function () {
                if (checkHasTreeNodeSelected()) {
                    params.loadtable();
                }
            },
            loadtable: function () {
                if ($.fn.DataTable.isDataTable('#ParamsTable')) {
                    params.$paramsDataTable.ajax.reload(null);
                } else {
                    params.$paramsDataTable = params.$paramsTable.WIMIDataTable({
                        "ajax": {
                            "url": abp.appAPIPath + "machineGatherParam/getMachineGatherParams",
                            "type": "POST",
                            data: function (d) {
                                if (tree.getSelectedNode().length > 0) {
                                    d.machineId = tree.getSelectedMachineIds()[0];
                                }
                                $("#select_all").prop("checked", false);
                            }
                        },
                        columnDefs: [{
                            'targets': 0,
                            'searchable': false,
                            'orderable': false,
                            'className': 'dt-body-center',
                            'render': function () {
                                return '<input type="checkbox" class="multiselect"  >';
                            }
                        }],
                        serverSide: true,
                        retrieve: true,
                        responsive: false,
                        ordering: true,
                        scrollCollapse: true,
                        order: [],
                        scrollX: true,
                        searching: true,
                        "pageLength": 50,
                        "columns": [
                            {
                                "orderable": false,
                                "title": "<input  type='checkbox' id='select_all'/>",
                                "width": "30px"
                            },
                            {
                                "defaultContent": "",
                                "title": app.localize('Actions'),
                                "orderable": false,
                                "width": "30px",
                                "createdCell": function (td, cellData, rowData, row, col) {

                                    var $td = $(td);

                                    if (permissions.manage) {
                                        $('<button class="btn btn-default btn-xs">' + app.localize('Edit') + '</button> ')
                                            .appendTo($td)
                                            .click(function () {
                                                params.editParam({ id: rowData.id, dataType: rowData.dataType });
                                            });
                                    }
                                }
                            },
                            {
                                "orderable": false,
                                "data": "code",
                                "className": "not-mobile",
                                "title": app.localize("ParameterCode")
                            },
                            {
                                "orderable": false,
                                "data": "name",
                                "title": app.localize("ParameterName")
                            },
                            {
                                "data": "dataType",
                                "orderable": false,
                                "title": app.localize("DataType")
                            },
                            {
                                "data": "unit",
                                "orderable": false,
                                "title": app.localize("Unit")
                            },
                            {
                                "orderable": false,
                                "data": "displayStyleString",
                                "title": app.localize("DisplayMode")
                            },
                            {
                                "data": "sortSeq",
                                "className": "not-mobile",
                                "title": app.localize("SortSeq")
                            },
                            {
                                "data": "hexcode",
                                "orderable": false,
                                "title": app.localize("BackgroundColor"),
                                "width": "60px",
                                "className": "not-mobile text-center",
                                "render": function (data, type, full, meta) {
                                    if (!data) {
                                        data = "#204D74";
                                    }
                                    return '<span class="label" style="background:' + data + ';display:inline-block;width:30px;">&nbsp;</span>';
                                }
                            },
                            {
                                "data": "isShowForStatus",
                                "orderable": false,
                                "width": "80px",
                                "title": app.localize("ShowState"),
                                "render": function (data, type, full, meta) {
                                    return '<input class="switch" type="checkbox" ' + (data ? "checked" : "")
                                        + ' data-id="' + full.id + '"'
                                        + ' data-tag="showstate"'
                                        + ' />';
                                }
                            },
                            {
                                "data": "isShowForParam",
                                "orderable": false,
                                "width": "80px",
                                "title": app.localize("ShowParam"),
                                "render": function (data, type, full, meta) {
                                    return '<input class="switch" type="checkbox" ' + (data ? "checked" : "")
                                        + ' data-id="' + full.id + '"'
                                        + ' data-tag="showparam"'
                                        + ' />';
                                }
                            },
                            {
                                "data": "isShowForVisual",
                                "className": "not-mobile",
                                "orderable": false,
                                "width": "80px",
                                "title": app.localize("ShowVision"),
                                "render": function (data, type, full, meta) {
                                    return '<input class="switch" type="checkbox" ' + (data ? "checked" : "")
                                        + ' data-id="' + full.id + '"'
                                        + ' data-tag="priorshow"'
                                        + ' />';
                                }
                            }
                        ],
                        "drawCallback": function (settings) {
                            $('.switch').bootstrapSwitch({ "size": "mini", "onColor": "success" });
                        }
                    });
                
                }
            }
        }


        tree.init($machineTree,
            false,
            function (d) {
                if (d && d.node && d.node.data.hasOwnProperty("machineid")) {
                    params.loadtable();
                }
            });
        $machineTree.jstree().open_node('ul > li:first');
        $machineTree.jstree().select_node('ul ul > li:first');

        pageButtons.init();


        $(document).on('click', '#select_all', function () {
            var rows = params.$paramsDataTable.rows({ 'search': 'applied' }).nodes();
            $('input[type="checkbox"]', rows).prop('checked', this.checked);
        })

        $('#documentTable tbody').on('change',
            'input[type="checkbox"]',
            function () {
                if (!this.checked) {
                    var el = $("#select_all").get(0);
                    if (el && el.checked && ('indeterminate' in el)) {
                        el.indeterminate = true;
                    }
                }
            });

        $("#btnBatchSend").click(function (e) {

            var selected = [];
            _.each(params.$paramsTable.find("input.multiselect[type='checkbox']:checked"),
                function (key) {
                    var rowData = params.$paramsDataTable.rows($(key).closest('tr')).data()[0];
                    if (rowData) {
                        selected.push(rowData.id);
                    }
                });

            if (selected.length > 0) {
                batchSwitchModal.open({ Id: selected },
                    function () {
                        $("#select_all").prop("checked", false);
                        params.loadtable();
                    });
            } else {
                abp.message.error(app.localize("PleaseSelectAtLeastOneRecord"));
            }
        });
    });

})();
