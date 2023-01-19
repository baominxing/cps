(function () {
    app.modals.ProcessSelectMachineModal = function () {
        var service = abp.services.app.feedbackCalendarDetaill;
        var _modalManager,_args;

        var selectMachineTable = null;
        var pcdMachineTable = null;

        var _$filterInput;
        var selectedArray = [];

        selectMachineTable = {
            $table: $("#ProcessSelectMachineTable"),
            $dataTable: null,

            initializeDataTable: function () {
                var cols = [
                    {
                        defaultContent: "",
                        title: '<input type="checkbox" class="checkbox-all">',
                        orderable: false,
                        width: "30px",
                        className: "text-center",
                        createdCell: function (td, cellData, rowData, row, col) {
                            var $controler = $('<input type="checkbox" class="checkbox-item" name="selected">');
                            $controler.appendTo($(td))
                                .click(function () {
                                    selectItem($(this), rowData);
                                });
                        }
                    },                  
                    {
                        data: "name",
                        orderable: false,
                        title: "设备名称"
                    },
                    {
                        data: "value",
                        orderable: false,
                        visible: false,
                        title: "id"
                    }
                ];

                selectMachineTable.$dataTable = selectMachineTable.$table.WIMIDataTable({
                    serverSide: false,
                    order: [],
                    autoWidth: false,
                    ajax: {
                        url: abp.appAPIPath + "feedbackCalendarDetaill/listMachines",
                        data: function (d) {
                            d.FeedbackCalendarId = _args.id;

                            //todo: need support regex search?
                            d.search = {
                                value: _$filterInput.val(),
                                regex: false
                            };
                        },
                        type: "POST"
                    },
                    columns: cols
                });
            },

            load: function () {        
                if (selectMachineTable.$dataTable != null) {
                    selectMachineTable.$dataTable.ajax.reload(null);
                } else {
                    selectMachineTable.initializeDataTable();
                }
            },

            addMachines: function () {
                if (selectedArray.length > 0) {
                    var parameters = {
                        FeedbackCalendarId: _args.id,
                        machineIdList: _.pluck(selectedArray, "value")
                    };

                    console.log(parameters);

                    service.addMachinesToDetail(parameters).done(function () {
                        pcdMachineTable.$dataTable.ajax.reload(null);
                        selectMachineTable.$dataTable.ajax.reload(null);
                    }).always(function () {
                        selectedArray = [];
                    });

                } else {
                    abp.message.info("请选择数据！");
                }
            }
        };

        pcdMachineTable = {
            $table: $("#PcdMachineTable"),
            $dataTable: null,

            initializeDataTable: function () {
                var cols = [
                    {                                              
                        title: "操作",
                        data: null,
                        className: "text-center",
                        orderable: false,
                        render: function () {
                            return "";
                        },
                        createdCell: function (td, cellData, rowData, row, col) {
                            $('<button class="btn btn-default btn-xs" title="' +
                                app.localize("Delete") +
                                '"><i class="fa fa-trash"></i></button>')
                                .appendTo($(td))
                                .click(function () {
                                    pcdMachineTable.removeMachineRelated(rowData);
                                });
                        }
                    },
                    {
                        data: "machineCode",
                        orderable: false,
                        title: "设备编号"
                    },
                    {
                        data: "machineName",
                        orderable: false,
                        title: "设备名称"
                    },
                    {
                        data: "creationTime",
                        orderable: false,
                        title: "添加时间",
                        render: function (data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    }
                ];

                pcdMachineTable.$dataTable = pcdMachineTable.$table.WIMIDataTable({
                    serverSide: true,
                    responsive: false,
                    retrieve: true,
                    paging: true,
                    order: [],
                    ajax: {
                        url: abp.appAPIPath + "feedbackCalendarDetaill/listSelectedMachines",
                        data: function (d) {
                            d.FeedbackCalendarId = _args.id;
                        },
                        type: "POST"
                    },
                    columns: cols
                });
            },

            removeMachineRelated: function (rowData) {
                abp.message.confirm(
                    "你确定要解除与设备【" + rowData.machineName + "】的关联？",
                    function(isConfirmed) {
                        if (isConfirmed) {
                            service.delete({ id: rowData.id }).done(function() {
                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                                pcdMachineTable.$dataTable.ajax.reload(null);
                                selectMachineTable.$dataTable.ajax.reload(null);
                            }).fail(function(err) {
                                setTimeout(function() { abp.message.error(err.message); }, 500);
                            });
                        }
                    });
            },

            load: function () {
                if (pcdMachineTable.$dataTable != null) {
                    pcdMachineTable.$dataTable.ajax.reload(null);
                } else {
                    pcdMachineTable.initializeDataTable();
                }
            }
        };

        $(document).on("click", ".checkbox-all", function () {
            var $self = $(this);
            selectedArray = [];

            if ($self.prop("checked")) {
                var data = $("#ProcessSelectMachineTable").DataTable().rows().data();
                _.each(data, function (item) {
                    selectedArray.push(item);
                });
            }
        });

        function selectItem($this, item) {
            if ($this.prop("checked")) {
                if (!_.contains(selectedArray, item)) {
                    selectedArray.push(item);
                }
            } else {
                selectedArray = _.reject(selectedArray, function (one) { return one.value === item.value });
            }
        }

        this.init = function (modalManager, args) {
            _args = args;
            _modalManager = modalManager;
            _$filterInput = _modalManager.getModal().find(".lookup-filter-text");

            _modalManager.getModal()
                .find(".lookup-filter-button")
                .click(function (e) {
                    e.preventDefault();
                    selectMachineTable.load();
                });

            _modalManager.getModal()
                .find(".lookup-filter-save")
                .click(function (e) {
                    e.preventDefault();
                    selectMachineTable.addMachines();
                });

            _modalManager.getModal()
                .find(".modal-body")
                .keydown(function (e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        selectMachineTable.load();
                    }
                });

        };

        this.shown = function() {
            selectMachineTable.load();
            pcdMachineTable.load();
        };

        this.save = function () {
            
        };
    };
})();