//# sourceURL=dynamicSelectShiftDetailModal.js
(function () {
    app.modals.SelectShiftDetailModal = function () {
        var _args, _manager;
        var $shiftDetail;
        var performanceService = abp.services.app.performanceDevice;
        var onlineOrOfflineRecordService = abp.services.app.onlineOrOfflineRecord;
        this.init = function(manager, args) {
            _manager = manager;
            _args = args;
            $shiftDetail = _manager.getModal().find("#shiftDetailId");
            $userId = _manager.getModal().find("#userId");
        }


        this.shown=function() {
            performanceService.getMachineShiftDetail({ Id: _args.machineId })
                .done(function (response) {

                    var data = _.map(response.items,
                        function(item) {
                            return { id: item.value, text: item.name };
                        });

                    $shiftDetail.select2({
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
            onlineOrOfflineRecordService.listUsers({ Id: _args.machineId })
                .done(function (response) {

                    var data = _.map(response.items,
                        function (item) {
                            return { id: item.value, text: item.name };
                        });

                    $userId.select2({
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
        }

        this.save = function () {
            var selected = _.first(_.pluck($shiftDetail.select2('data'), 'id'));
            if (!selected) {
                abp.message.error(app.localize("PleaseSelectTheShift"));
            } else {
                _manager.setResult(selected);
                _manager.close();
            }
        }
    }
})(jQuery);