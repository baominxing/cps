//# sourceURL=dynamic_ShowStateDetailModal.js
(function () {

    var _args,theUrl;
    app.modals.ShowStateDetailModal = function() {     
 
        var page = {
            $table: $('#StatusDistributionMapTables'),
            datatables: null,
            init: function() {
                this.datatables = page.$table.WIMIDataTable({
                    order: [],
                    "ajax": {
                        "url": abp.appAPIPath + theUrl,
                        "type": "POST",
                        "data": function(d) {
                            $.extend(d, _args);
                        }
                    },
                    'columns': [
                        {
                            "orderable": false,
                            "data": "startTime",
                            "title": app.localize("StartTime"),
                            render: function(data) {
                                return wimi.btl.dateTimeFormat(data);
                            }
                        },
                        {
                            "orderable": false,
                            "data": "endTime",
                            "title": app.localize("EndTime"),
                            render: function (data) {
                                return wimi.btl.dateTimeFormat(data);
                            }
                        }
                        ,
                        {
                            "orderable": false,
                            "data": "stateName",
                            "title": app.localize("StateName"),
                            render: function (data) {
                                return app.localize(data);
                            }
                        }
                    ]

                });

            }
        };

        this.init = function (modalManager, args) {
            _args = args;
            if(_args.shif) {
                theUrl = "statusDistributionMap/listStatusByShiftDetail";
            }else {
                theUrl = "statusDistributionMap/listStatusByMachine";
            }
           
        };

        this.shown = function() {
            page.init();
        }
    };
})(jQuery);