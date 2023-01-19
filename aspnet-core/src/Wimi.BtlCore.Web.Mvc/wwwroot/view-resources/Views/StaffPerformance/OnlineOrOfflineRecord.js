(function () {
    $(function () {
        var machineTree = new MachinesTree();
        machineTree.init($("div.machines-tree"), true);
        machineTree.initGroup($("div.machines-group-tree"));
        machineTree.setSelectAll();
        var $dateRange = $("#reservationtime");
        var $dataTable = null;
        var $table = $("#table");
        //员工姓名
        var $TextFilter = $("#TextFilter");
        var $query = $("#btnQuery");

        var page = {

            initDateRangePicker: function () {
                $dateRange.WIMIDaterangepicker();
            },
            loadTable: function () {
                var p = page.getQueryParameters();
                $dataTable = $table.WIMIDataTable({
                    order: [[3, "desc"]],
                    "ajax": {
                        url: abp.appAPIPath + "onlineOrOfflineRecord/queryRecords",
                        data: function (d) {
                            d.userName = p.userName;
                            d.machineIdList = p.machineIdList;
                            d.startTime = p.startTime;
                            d.endTime = p.endTime;
                        }
                    },
                    "columns": [
                        {
                            "data":
                                "userName",
                            "title":
                                app.localize("Name")
                        },
                        {
                            "data":
                                "machineName",
                            "title":
                                app.localize("MachineName")
                        },
                        {
                            "data":
                                "onlineTime",
                            "title":
                                app.localize("OnlineTime"),
                            "render": function (data) {
                                return moment(data).format("YYYY-MM-DD HH:mm");
                            }
                        },
                        {
                            "data":
                                "offlineTime",
                            "title":
                                app.localize("OfflineTime"),
                            "render": function (data) {
                                if (data == null) {
                                    return app.localize("InTheOnline");
                                }
                                return moment(data).format("YYYY-MM-DD HH:mm");
                            }
                        }
                    ]

                });
            },
            getQueryParameters: function () {
                var queryParameters = {};
                queryParameters.startTime = $dateRange.data("daterangepicker")
                    .startDate.format("YYYY-MM-DD HH:mm");
                queryParameters.endTime = $dateRange
                    .data("daterangepicker")
                    .endDate.format("YYYY-MM-DD HH:mm");
                queryParameters.userName = $.trim($TextFilter.val());

                queryParameters.machineIdList = machineTree.getSelectedMachineIds();
                return queryParameters;
            },
            initPage: function () {
                page.initDateRangePicker();

                $query.on("click",
                    function (e) {
                        e.preventDefault();

                        if ($dataTable != null) {
                            $dataTable.destroy();
                            $dataTable = null;
                            $table.empty();
                        }
                        page.loadTable();
                    });
                $query.click();

            }
        };


        page.initPage();

    });
})();