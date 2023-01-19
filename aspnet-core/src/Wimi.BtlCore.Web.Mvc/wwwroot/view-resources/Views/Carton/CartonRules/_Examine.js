//# sourceURL=dynamicExamineModal.js
(function () {
    app.modals.ExamineModal = function () {
        var _modalManager,
            _args,
            service = abp.services.app.cartonRule;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
        };
        this.shown = function () {
            var page = {
                $table: $("#remainderTable"),
                datatables: null,
                init: function () {
                    if (this.datatables) {
                        page.datatables.destroy();
                        page.$table.empty();
                    }
                    let _this = this;
                    service.examine({ id: _args.ruleId }).done(function (response) {
                        _this.datatables = _this.$table.WIMIDataTable({
                            serverSide: false,
                            data:response,
                            retrieve: true,
                            responsive: false,
                            ordering: false,
                            order: [],
                            scrollCollapse: true,
                            scrollX: true,
                            columns: page.getColumns()
                        });
                    });
                },
                getColumns: function () {
                    return [
                        {
                            "data": "value",
                            "title": app.localize("Remainder")
                        },
                        {
                            "data": "name",
                            "title": app.localize("RelevantChar")
                        }
                    ];
                }
            };
            page.init();
        };
    };
})();