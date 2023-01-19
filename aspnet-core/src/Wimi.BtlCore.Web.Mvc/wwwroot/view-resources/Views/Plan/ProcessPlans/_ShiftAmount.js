//# sourceURL=dynamicProcessParametersModal.js
(function () {
    app.modals.Shift = function () {
        var _modalManager,
            _args

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;

            var msg = abp.utils.formatString(app.localize("ShiftTarget"));
            _modalManager.getModal().find(".modal-title span").text(msg);
        };
        this.shown = function() {
            var page = {
                $table: $("#shiftAmountTable"),
                datatables: null,
                query: function() {
                    if (!$.fn.DataTable.isDataTable("#shiftAmountTable")) {
                        page.$datatable = page.$table.WIMIDataTable({
                            "scrollX": true,
                            "responsive": false,
                            ordering: false,
                            "ajax": {
                                url: abp.appAPIPath + "processPlan/getPlanShiftItem",
                                data: function(d) {
                                    d.id = $("#Id").val();
                                }
                            },
                            "columns": [
                                {
                                    "data": "shiftName",
                                    "title": app.localize("ShiftName")
                                },
                                {
                                    "data": "shiftTargetAmount",
                                    "title": app.localize("ShiftTarget")
                                }
                            ]
                        });
                    } else {
                        page.$datatable.ajax.reload();
                    }
                },
                init: function() {
                    page.query();
                }
            };
            page.init();
        };
       
    };
})();