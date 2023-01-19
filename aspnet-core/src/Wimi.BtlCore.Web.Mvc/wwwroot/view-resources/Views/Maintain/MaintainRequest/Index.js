(function () {
    $(function () {
        var repairRequestAppService = abp.services.app.repairRequest;
        var commonService = abp.services.app.commonLookup;


        var createOrUpdateRequestModal = new app.ModalManager({
            viewUrl: abp.appPath + "MaintainRequest/CreateOrUpdateRepaireRequest",
            scriptUrl: abp.appPath + "view-resources/Views/Maintain/MaintainRequest/_CreateOrUpdateRepaireRequest.js",
            modalClass: "CreateOrUpdateRepaireRequest"
        });
        var lookRequestModal = new app.ModalManager({
            viewUrl: abp.appPath + "MaintainRequest/LookRepaireRequest",
        
            modalClass: "LookRepaireRequest"
        });

        var page = {
            $table: $("#MaintainRequestTable"),
            $dataTable: null,
           
            initMachineSelect2: function () {
                commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                    var machines = _.chain(response.machines)
                        .filter(
                        function (item) {
                            return _.contains(response.grantedGroupIds, item.deviceGroupId);
                        })
                        .map(function (item) {
                            var machine = { id: item.id, text: item.name };
                            return machine;
                        });

                    var machinesData = [{ id: 0, text: app.localize("All") }];
                    //_.each(_.groupBy(machines._wrapped, "id"),
                    //    function (item) {
                    //        machinesData.push(item[0]);
                    //    });
                    for (var n = 0; n < machines._wrapped.length; n++) {
                        machinesData.push(machines._wrapped[n]);
                    }
                    $("#MachineId").select2({
                        data: machinesData,
                        multiple: false,
                        minimumResultsForSearch: -1,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainTheEquipment");
                            }
                        }
                    }).val(0).trigger('change');
                });

         
            },

            initStatusSelect2: function () {
                $("#Status").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                });
            },
            initDataPicker: function () {
                var dataOption = {
                    singleDatePicker: true,
                    maxDate: moment().add(360, 'years')    
                };

                $("#StartTime").WIMIDaterangepicker(dataOption);
                $("#EndTime").WIMIDaterangepicker($.extend({ "endTimeOnly": true }, dataOption));
            },
            initTable: function () {
                if (!$.fn.DataTable.isDataTable("#MaintainRequestTable")) {

                    page.$datatable = page.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "repairRequest/listRequest",
                            data: function(d) {
                                var formobj = $("#RequestForm").serializeFormToObject();
                                formobj.MachineId = $("#MachineId").val();
                                return _.extend(d, formobj);

                            }
                        },
                        "responsive": false,
                        "scrollX": true,
                        "columns": [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "150px",
                                "className": "text-center not-mobile",
                                "createdCell": function(td, cellData, rowData, row, col) {
                                    $(td).buildActionButtons([
                                        {
                                            //待维修
                                            title: app.localize("Edit"),
                                            clickEvent: function() {
                                                page.editTableItem(rowData);
                                            },
                                            isShow: rowData.status === 0
                                        },
                                        {
                                            //待维修
                                            title: app.localize("View"),
                                            clickEvent: function() {
                                                page.lookTableItem(rowData);
                                            },
                                            isShow: true
                                        },
                                        {
                                            title: app.localize("Delete"),
                                            clickEvent: function() {
                                                page.deleteTableItem(rowData);
                                            },
                                            isShow: rowData.status === 0,
                                            className: "btn-danger"
                                        },
                                    ]);
                                }
                            },
                            {
                                "data": "code",
                                "title": app.localize("RequestNo")
                            },
                            {
                                "data": "status",
                                "title": app.localize("State"),
                                "width": "120px",
                                "render": function(data, type, row) {
                                    var statu;
                                    switch (data) {
                                        case 0:
                                            statu = app.localize("PendingMaintance");
                                        break;
                                        case 1:
                                            statu = app.localize("InMaintenance");
                                        break;
                                        case 2:
                                            statu = app.localize("Repaired");
                                        break;
                                    default:
                                        statu = "";
                                        break;
                                    }
                                    return statu;

                                }
                            },
                            {
                                "data": "requestDate",
                                "title": app.localize("PlanFixDate"),
                                "width": "100px",
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateTimeNoSecondFormat(data);
                                }
                            },
                            {
                                "data": "machineCode",
                                "title": app.localize("MachineCode")

                            },
                            {
                                "data": "machineName",
                                "title": app.localize("MachineName")


                            },
                            {
                                "data": "machineType",
                                "title": app.localize("MachineType")
                            },
                            {
                                "data": "isShutdown",
                                "title": app.localize("IsItDowntime"),
                                "render": function (data, type, row) {
                                    var statu = app.localize("Yes");
                                    if (data == false) {
                                        statu = app.localize("No");
                                    }
                                    return statu;

                                }

                            },
                            {
                                "data": "requestUserName",
                                "title": app.localize("Declarant")

                            },
                            {
                                "data": "creationTime",
                                "title": app.localize("ReportingTime"),
                                "width": "100px",
                                "render": function(data, type, full, meta) {
                                    return wimi.btl.dateTimeNoSecondFormat(data);
                                }
                            }
                        ]
                    });
                } else {

                    page.$datatable.ajax.reload();
                }
            },
            init: function () {
                page.initDataPicker();
                page.initTable();
                page.initMachineSelect2();
                page.initStatusSelect2();
            },
            editTableItem: function (rowdata) {
                createOrUpdateRequestModal.open({ id: rowdata.id }, function () {
                    page.initTable();
                });
            },
            deleteTableItem: function (rowdata) {
                abp.message.confirm(
                    app.localize("WillDeleteOrder{0}", rowdata.code),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            repairRequestAppService
                                .deleteRequest({id: rowdata.id })
                                .done(function (result) {
                                    abp.notify.info(app.localize("SuccessfullyRemoved"));
                                    page.initTable();
                                });
                        }
                    }
                );
            },
            lookTableItem:function(rowdata) {
                lookRequestModal.open({ id: rowdata.id }, function () {
                  
                });
            },
        }

        $("#btnQuery").click(function () {
            page.initTable();
        });

        $("#btnRequest").click(function () {
            createOrUpdateRequestModal.open({id:0}, function () {
                page.initTable();
            });
        });

        page.init();
    });
})();