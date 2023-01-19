//# sourceURL=dynamicSelectShiftDetailModal.js
(function () {
    app.modals.SelectDeviceGroupModal = function () {
        var _args, _manager;
        var performanceService = abp.services.app.performanceDevice;
        var onlineOrOfflineRecordService = abp.services.app.onlineOrOfflineRecord;
        this.init = function (manager, args) {
            _manager = manager;
            _args = args;
            $offlineMachineGroup = manager.getModal().find("#offlineMachineGroup");
            $offlineUser = manager.getModal().find("#offlineUser");


            onlineOrOfflineRecordService.listDeviceGroups().done(function (data) {

                var listdata = _.map(data.items,
                    function (item) {
                        return { id: item.value, text: item.name };
                    });

                $offlineMachineGroup.select2({
                    data: listdata,
                    multiple: false,
                    minimumResultsForSearch: -1,
                    placeholder: app.localize("PleaseChoose"),
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                });
            });          
            onlineOrOfflineRecordService.listUsers({})
                .done(function (response) {

                    var data = _.map(response.items,
                        function (item) {
                            return { id: item.value, text: item.name };
                        });

                    $offlineUser.select2({
                        data: data,
                        multiple: false,
                        minimumResultsForSearch: -1,
                        placeholder: app.localize("PleaseChoose"),
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });
                });

        };


        this.save = function () {
            _manager.setResult();
            _manager.close();
        };

    };
})(jQuery);