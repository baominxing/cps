(function () {
    $(function () {
        var parameteresTable = $("#HistoryParametersTable"),
            parameteresDataTables = null,
            _$datepicker = $("#daterange-btn"),
            lastObjectId = $("#lastobjectId"),
            currentPageNo = 0,
            $tree = $("div.machines-tree"),
            $query = $("#btnQuery");

        var service = abp.services.app.paramters;
        var param = {};

        var comparisonModal = new app.ModalManager({
            viewUrl: abp.appPath + "HistoryParameters/ViewComparison",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/HistoryParameters/_ViewComparisonModal.js",
            modalClass: "ViewComparisonModalModal",
            modalSize: 'modal-lg'
        });

        $("#btnContrast").on("click",
            function () {
                var startTime = _$datepicker.data("daterangepicker").startDate.format("YYYYMMDDHHmmss");
                var endTime = _$datepicker.data("daterangepicker").endDate.format("YYYYMMDDHHmmss");

                var param = {
                    MachineId: machineTree.getSelectedMachineIds()[0],
                    startTime: startTime,
                    endTime: endTime
                }
                comparisonModal.open(param);
            });

        var machineTree = new MachinesTree();
        machineTree.init($tree, false);
        machineTree.selectFirst();
        machineTree.initMoreAndLess();

        var paramtersService = abp.services.app.paramters;
        var columnsList = [];

        _$datepicker.WIMIDaterangepicker({
            startDate: moment().subtract(1, "days"),
            endDate: moment(),
            timePicker: true
        });


        $tree.on("changed.jstree", function () {
            $query.click();
        });

        $query.on("click",
            function () {
                lastObjectId.val('');
                var startTime = _$datepicker.data("daterangepicker").startDate.format("YYYYMMDDHHmmss");
                var endTime = _$datepicker.data("daterangepicker").endDate.format("YYYYMMDDHHmmss");
                param = {
                    MachineId: machineTree.getSelectedMachineIds()[0],
                    StartTime: startTime,
                    EndTime: endTime
                };

                if ($.trim(param.MachineId).length === 0) {
                    return false;
                }
                parameteresTable.removeAttr("hidden");
                $("#prompt").attr("hidden", "hidden");

                if (parameteresDataTables != null) {
                    parameteresDataTables.destroy();
                    parameteresDataTables = null;
                    parameteresTable.empty();
                }

                var mediaTag = $.WIMI.getMediaTag();
                var responsive = true;
                if (mediaTag === "lg" || mediaTag === "md") {
                    responsive = false;
                    parameteresTable.parent().removeClass("table-responsive");
                } else {
                    parameteresTable.parent().addClass("table-responsive");
                }

                paramtersService.getHistoryParamtersColumns(param)
                    .done(function (data) {
                        if (data.paramColumns == null || data.paramColumns.length <= 1) {
                            abp.message.warn(app.localize("ConfigParameterTip"));
                            return false;
                        }
                        columnsList = data.paramColumns;
                        parameteresDataTables = parameteresTable.WIMIDataTable({
                            pagingType: "simple",
                            searching: false,
                            pageLength: 25,
                            destroy: true,
                            scrollX: true,
                            aLengthMenu: [10,20,25,50,100],
                            responsive: responsive,
                            ordering: false,
                            ajax: {
                                "url": abp.appAPIPath + "paramters/getHistoryParamtersList",
                                "type": "POST",
                                data: function (d) {
                                    param.PageDown = currentPageNo <= getDataTablesPageSize();
                                    $.extend(d, param);
                                },
                                dataFilter: function (data) {
                                    var json = $.parseJSON(data);
                                    var result = [];
                                    _.each(json.result.items,
                                        function (item) {
                                            result.push(item.paramData);
                                        });
                                    var last = _.last(json.result.items);
                                    if (last) {
                                        lastObjectId.val(last.objectId);
                                    } else {
                                        lastObjectId.val('');
                                    }
                                    json.result.data = result;

                                    currentPageNo = getDataTablesPageSize();
                                    return JSON.stringify(json.result);
                                }
                            },
                            columns: columnsList
                        });
                    });
            }).trigger("click");


        function getDataTablesPageSize() {
            if (parameteresDataTables) {
                return parameteresDataTables.page.info().page;
            }
            return currentPageNo;
        }

        $("#btnExport").on("click",
            function () {
                var paramter = param;

                service.export(paramter).done(function (result) {
                    app.downloadTempFile(result);
                });
            });
    });
})();