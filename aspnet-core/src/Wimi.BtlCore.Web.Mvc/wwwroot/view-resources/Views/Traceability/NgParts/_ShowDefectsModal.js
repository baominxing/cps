//# sourceURL=dynamicProcessParametersModal.js
(function () {
    app.modals.ShowDefectsModal = function () {
        var _modalManager,
            _args,
            service = abp.services.app.trace;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;

            var msg = abp.utils.formatString(app.localize("Workpiece{0}NGCauseView"), _args.PartNo);
            _modalManager.getModal().find(".modal-title span").text(msg);
        };
        this.shown = function () {
            var page = { 
                $table: $("#partNgInfoTable"),
                datatables: null,
                init: function () {
                    if (this.datatables) {
                        page.datatables.destroy();
                        page.$table.empty();
                    }
                    this.datatables = this.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "trace/listDefectiveInfos",
                            data: function (d) {

                                $.extend(d, page.getParams(_args));
                            }
                        },
                        serverSide: true,
                        retrieve: true,
                        responsive: false,
                        ordering: false,
                        order: [],
                        scrollCollapse: true,
                        scrollX: true,
                        columns: page.getColumns()
                    })
                },

                getParams: function (args) {
                    return {
                        partNo: args.PartNo,
                        defectiveMachineId: args.DefectiveMachineId
                    }
                },
                getColumns: function () {
                    return [
                        {
                            "data": "defectivePartName",
                            "title": app.localize("DefectivePart")
                        },
                        {
                            "data": "defectiveReasonName",
                            "title": app.localize("BadReason")
                        },
                        {
                            "data": "creationTime",
                            "title": app.localize("InputTime"),
                            "render": function (data) {
                                if (data !== null) {
                                    return moment(data).format("YYYY-MM-DD HH:mm:ss");
                                } else {
                                    return "";
                                }
                            }
                        },
                        {
                            "data": "creatorUserName",
                            "title": app.localize("EntryPerson")
                        }
                    ];
                }
            };
            page.init();
        };
    };
})();