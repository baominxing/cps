//# sourceURL=dynamicProcessParametersModal.js
(function () {
    app.modals.processParametersModal = function () {
        var _modalManager,
            _args,
            service = abp.services.app.trace;

        this.init = function(modalManager, args) {
            _modalManager = modalManager;
            _args = args;

            var msg = abp.utils.formatString(app.localize("ProcessParametersOf{0}Processing{1}"), _args.machineName, _args.partNo);
            _modalManager.getModal().find(".modal-title span").text(msg);
        };
        this.shown = function () {
            var page= {
                $table: $("#parametersTable"),
                datatables: null,
                init: function() {
                    var param = {
                        PartNo: _args.partNo,
                        MachineId: _args.machineId,
                        MachineName: _args.machineName,
                        OperationTimeBegin: _args.operationTimeBegin,
                        OperationTimeEnd: _args.operationTimeEnd,
                        StepNo: _args.stepNo
                    };

                    service.listProcessParamters(param).done(function (result) {

                        if (result === null) {
                            return;
                        }

                        result.machineGatherList.push({ "code": "CreationTime", "name": app.localize("AcquisitionTime") });

                        var tablecolumns = _.map(result.machineGatherList,
                            function (item) {
                                return {
                                    title: item.name,
                                    data: item.code,
                                    orderable: false,
                                    render: function (data) {
                                        if (item.code === 'CreationTime') {
                                            return wimi.btl.dateTimeFormat(data.slice(0, 8) + "T" + data.slice(8));
                                        } else {
                                            return data;
                                        }
                                    }
                                };
                            });

                        var d = [];
                        _.each(result.paramList,
                            function(item) {
                              //  var obj = _.object(_.pluck(_.values(item), 'name'), _.pluck(_.values(item), 'value'));

                                var names = _.pluck(_.values(item), 'name');
                                var values = _.pluck(_.values(item), 'value');

                                //补全数据
                                var differenceCodes = _.difference(_.pluck(tablecolumns, 'data'), names);
                                _.each(differenceCodes,
                                    function (key) {
                                        names.push(key);
                                        values.push("");
                                    });

                                var obj = _.object(names, values);


                                d.push(obj);
                            });

                        var tablecolumns = _.map(result.machineGatherList,
                            function(item) {
                                return {
                                    title: item.name,
                                    data: item.code,
                                    orderable: false,
                                    render: function(data) {
                                        if (item.code === 'CreationTime') {
                                            return wimi.btl.dateTimeFormat(data.slice(0, 8) + "T" + data.slice(8));
                                        } else {
                                            return data;
                                        }
                                    }
                                };
                            });

                        var columns = tablecolumns;
                        if (_args.stepNo) {
                            // 清洗机参数拆分 根据Station
                            columns = _.filter(tablecolumns,
                                function(item) {
                                    return item.data.includes('Station' + _args.stepNo) ||
                                        item.data === 'CreationTime';
                                });
                        }

                        page.initdatatables(d, columns);
                    });
                },
                initdatatables: function (d, columns) {

                    if (this.datatables) {
                        parts.datatables.destroy();
                        parts.$table.empty();
                    }
                    this.datatables = this.$table.WIMIDataTable({
                        serverSide: false,
                        "scrollX": true,
                        "responsive": false,
                        ordering: false,
                        order: [],
                        data: d,
                        columns: columns
                    });
                }
            };
            page.init();
        };
    };
})();