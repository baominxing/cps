"use strict";
(function ($, abp) {
    $(function () {
        var oldPutVolume = 0;
        var oldAimVolume = 0;

        var _planService = abp.services.app.productionPlan;
        
        var _$datatables = null;

        var saveId = null;

        var vmData = {
            filter: {
                productCode: null,
                planCode: null
            },
            selectedPlan: {}
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + "ProductionPlan/CreateOrEditModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/ProductionPlan/_CreateOrEditModal.js",
            modalClass: "CreateOrEditProductionPlanModal"
        });

        function initTables() {
            _$datatables = $("#ProductionPlanTable").WIMIDataTable({
                    "searching": false,
                    "scrollX": true,
                    "responsive": false,
                    "ajax": {
                        "url": abp.appAPIPath + "productionPlan/listProductionPlan",
                        data: function(param) {
                            return $.extend(param, vmData.filter);
                        }
                    },
                    "rowCallback": function(row, data, displayIndex) {

                    },
                    "order": [[ 1, "desc" ]],
                    "columns": [
                        {
                            "defaultContent": "",
                            "title": app.localize("Actions"),
                            "width": "25px",
                            "orderable": false,
                            "createdCell": function(td, cellData, rowData, row, col) {

                                // [{title, clickEvent, permission}]
                                $(td).buildActionButtons([
                                    {
                                        title: app.localize("Edit"),
                                        clickEvent: function() {
                                            _createOrEditModal.open({ id: rowData.id });
                                        },
                                        isShow:  rowData.canDelete
                                    }, {
                                        title: app.localize("Delete"),
                                        clickEvent: function() { deletePlan(rowData) },
                                        isShow:  rowData.canDelete
                                    }
                                ]);
                            }
                        },
                        {
                            "data": "code",
                            "title": app.localize("ProductionPlanCode"),
                            "className": "not-mobile"
                        },
                        {
                            "data": "orderCode",
                            "title": app.localize("OrderCode"),
                            "className": "not-mobile"
                        },
                        {
                            "data": "displayState",
                            "title": app.localize("PlanState"),
                            "className": "not-mobile",
                            "orderable": false,
                            "render": function (data, type, full, meta) {

                                switch (data) {
                                    case "Prepared": return '<span class="label label-warning">' + app.localize(data) + '</span>';
                                    case "Underway": return '<span class="label label-success">' + app.localize(data) + '</span>';
                                    case "Completed": return '<span class="label label-success">' + app.localize(data) + '</span>';
                                    case "BreakOff": return '<span class="label label-danger">' + app.localize(data) + '</span>';
                                    default: return '<span class="label label-primary">' + app.localize(data) + '</span>';
                                }

                            }

                        },
                        {
                            "data": "productCode",
                            "title": app.localize("ProductCode"),
                            "orderable": false,
                            "className": "not-mobile"
                        },
                        {
                            "data": "productName",
                            "title": app.localize("ProductName"),
                            "orderable": false,
                            "className": "not-mobile"
                        },
                        {
                            "data": "craftName",
                            "title": app.localize("CraftName"),
                            "orderable": false,
                            "className": "not-mobile"
                        },
                        {
                            "data": "putVolume",
                            "title": app.localize("PutVolume"),
                            "orderable": false,
                            "className": "not-mobile"
                        },
                        {
                            "data": "aimVolume",
                            "title": app.localize("TargetQuantity"),
                            "orderable": false,
                            "className": "not-mobile"
                        },
                        {
                            "data": "unit",
                            "title": app.localize("Unit"),
                            "className": "not-mobile",
                            "orderable": false,
                            "render": function(data, type, full, meta) {
                                if (data == null) {
                                    return app.localize("NotAssociated");
                                }
                                return data;
                            }
                        },
                        {
                            "data": "startDate",
                            "className": "not-mobile",
                            "title": app.localize("StartDate"),
                            "orderable": false,
                            "render": function(data, type, full, meta) {
                                return wimi.btl.dateFormat(data);
                            }
                        }, {
                            "data": "endDate",
                            "className": "not-mobile",
                            "title": app.localize("EndDate"),
                            "orderable": false,
                            "render": function(data, type, full, meta) {
                                return wimi.btl.dateFormat(data);
                            }
                        },
                        {
                            "data": "actualStartDate",
                            "className": "not-mobile",
                            "title": app.localize("ActualStartDate"),
                            "orderable": false,
                            "render": function(data, type, full, meta) {
                                return wimi.btl.dateTimeFormat(data);
                            }
                        }, {
                            "data": "actualEndDate",
                            "className": "not-mobile",
                            "title": app.localize("ActualEndDate"),
                            "orderable": false,
                            "render": function(data, type, full, meta) {
                                return wimi.btl.dateTimeFormat(data);
                            }
                        } ,{
                            "data": "qualifiedCount",
                            "title": app.localize("ActualPositiveNumber"),
                            "orderable": false,
                            "className": "not-mobile"
                        },
                        {
                            "data": "defectiveCount",
                            "title": app.localize("ActualDefectiveNumber"),
                            "orderable": false,
                            "className": "not-mobile"
                        },
                        {
                            "data": "memo",
                            "title": app.localize("Note"),
                            "orderable": false,
                            "className": "not-mobile"
                        }
                    ]
                })
                .on('draw', function () {
                    
                    // 用 saveId 判别是新增还是编辑，svaeId 为 null 表示新增，否则为编辑
                    // 如果是编辑 并且 数据中存在对应的行数据，那么选中改行
                    // 如果是编辑 但是 数据中不存在对应的行数据，那么选中第一行(删除数据的场景)
                    // 如果是新增 选中第一行
                    if (saveId != null) {

                        var rows = _$datatables.rows();
                        
                        var selectedRow = _.find(rows[0], function (item) {
                            return _$datatables.row(item).data().id === saveId;
                        });
                        if (selectedRow != null) {
                            // 行中有数据，至为选中
                            
                            $(_$datatables.row(selectedRow).node()).trigger('click');

                            return;
                        }
                    }

                    var trObj = _$datatables.row(0).node();
                    if (trObj == null) {
                        vmData.selectedPlan = {};
                        return;
                    }
                    $(trObj).trigger('click');

                });

            $('#ProductionPlanTable tbody').on('click', 'tr', function () {

                if ($(this).hasClass("selected")) {

                } else {
                    _$datatables.$("tr.selected")
                        .css("background-color", "")
                        .removeClass("selected");
                    $(this).css("background-color", "#a8e4ff")
                        .addClass("selected");
                }

                if (_$datatables.row($(this)).data()) {
                    var data = _$datatables.row($(this)).data();

                    data.workOrders = _.sortBy(data.workOrders, "processOrderSeq");

                    _.each(data.workOrders, function (item) {
                        item.edited = false;
                    });

                    vmData.selectedPlan = data;
                }


            });
        }

        function getPlans() {
            _$datatables.ajax.reload();
          //  _$datatables.draw(false);
           
        }

        function deletePlan(plan) {
            
            abp.message.confirm(
                abp.utils.formatString(app.localize("WhetherToDeleteTheProductionPlanNumbered{0}"),plan.code),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _planService.delete({
                            id: plan.id
                            })
                            .done(function() {
                                getPlans(true);
                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                            });
                    }
                }
            );
        }

        vmData.create = function () {

            abp.ui.setBusy();

            _planService.canCreate()
                .done(function() {
                    _createOrEditModal.open({id:null});
                })
                .always(function() {
                    abp.ui.clearBusy();
                });
        }
        vmData.search = function () {
            saveId = null;
            getPlans();
        }
        vmData.reset= function() {
            vmData.filter.productCode = null;
            vmData.filter.planCode = null;
            vmData.search();
        }
        vmData.changeWorkOrder = function (item) {
            if (item.putVolume <= 0) {
                abp.notify.error(app.localize("PutVolumeNeedMoreThenZero"),);
                return;
            }
            if (item.aimVolume <= 0) {
                abp.notify.error(app.localize("AimVolumeNeedMoreThenZero"),);
                return;
            }
            _planService.changeWorkOrderVolume({
                workOrderId: item.id,
                putVolume: item.putVolume,
                aimVolume:item.aimVolume
                })
                .done(function() {
                    item.edited = false;
                });
        }

        vmData.modifyWorkOrder = function(item) {

            oldPutVolume = item.putVolume;
            oldAimVolume = item.aimVolume;
            item.edited = true;
        }

        vmData.unchangeWorkOrder = function(item) {

            item.putVolume = oldPutVolume;
            item.aimVolume = oldAimVolume;
            item.edited = false;
        }

        var vue = new Vue({
            el: '#app-productionplan',
            data: vmData,
            mounted: function () {
                initTables();
            }
        });

        abp.event.on("app.CreateOrEditProductionPlanModalSaved", function (edata) {
            
            saveId = edata;
            getPlans();
        });
    });
})($,abp);