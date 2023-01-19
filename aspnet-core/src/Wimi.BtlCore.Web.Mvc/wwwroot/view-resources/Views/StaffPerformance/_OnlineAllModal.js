//# sourceURL=dynamicSelectShiftDetailModal.js
(function () {
    app.modals.SelectAllModal = function () {
        var _args, _manager;
        var $shiftDetail;
        var performanceService = abp.services.app.performanceDevice;
        var onlineOrOfflineRecordService = abp.services.app.onlineOrOfflineRecord;
        this.init = function (manager, args) {
            _manager = manager;
            _args = args;
            $shiftDetail = _manager.getModal().find("#shiftDetailId");
            $userId = _manager.getModal().find("#userId");
            $machineGroup = manager.getModal().find("#machineGroup");


            onlineOrOfflineRecordService.listDeviceGroups().done(function (data) {

                var listdata = _.map(data.items,
                    function (item) {
                        return { id: item.value, text: item.name };
                    });

                $machineGroup.select2({
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

                performanceService.listShiftDetailByDeviceGroupId({ Id: data.items[0].value })
                    .done(function (response) {
                        $("#machineGroup").on("change",
                            function() {
                                $("#shiftDetailId").empty();
                                performanceService.listShiftDetailByDeviceGroupId({ Id: $("#machineGroup").val() })
                                    .done(function(response) {

                                        var data = _.map(response,
                                            function(item) {
                                                return { id: item.value, text: item.name };
                                            });

                                        $shiftDetail.select2({
                                            data: data,
                                            multiple: false,
                                            minimumResultsForSearch: -1,
                                            placeholder: app.localize("PleaseChoose"),
                                            language: {
                                                noResults: function() {
                                                    return app.localize("NoMatchingData");
                                                }
                                            }
                                        });
                                    });
                            });
                        var data = _.map(response,
                            function (item) {
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
           
        };


        this.save = function () {
            var selected = _.first(_.pluck($shiftDetail.select2('data'), 'id'));  

            if (!selected) {
                abp.message.error(app.localize("PleaseSelectTheShift"));
            } else {
                _manager.setResult(selected);
               
                _manager.close();
            }
        };

    };
})(jQuery);