//# sourceURL=dynamicCreateDeviceGroupShiftSolutionModal.js

(function () {
    app.modals.CreateDeviceGroupShiftSolutionModal = function () {

        var _modalManager;
        var _$machineTree = new MachinesTree();
        var _shiftAppService = abp.services.app.shift;
        var _$form = $('#DeviceGroupShiftSolutionForm');
        $('#StartTime').daterangepicker({
            "singleDatePicker": true,
            locale: {
                format: 'YYYY-MM-DD',
                monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                    '七月', '八月', '九月', '十月', '十一月', '十二月'],
                daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
            },
            minDate: moment()
        });

        $('#EndTime').daterangepicker({
            "singleDatePicker": true,
            locale: {
                format: 'YYYY-MM-DD',
                monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                    '七月', '八月', '九月', '十月', '十一月', '十二月'],
                daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
            },

            minDate: moment()
        });

        //function getDeviceGroups() {
        //    _shiftAppService.getDeviceGroups().done(function (data) {
        //        for (var i = 0; i < data.items.length; i++) {
        //            $("#DeviceGroups").append($("<option></option>").val(data.items[i].id).html(data.items[i].name));
        //        }
        //    });
        //}

        function getShiftSolutions2() {
            _shiftAppService.getShiftSolutions().done(function (data) {
                for (var i = 0; i < data.items.length; i++) {
                    $("#ShiftSolutions2").append($("<option></option>").val(data.items[i].id).html(data.items[i].name));
                }
            });
        }

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _$form = _modalManager.getModal().find("form[name=DeviceGroupShiftSolutionForm]");

            _$form.find("input:not([type=hidden]):first").focus();
        };

        this.shown = function () {
            var $tree = _modalManager.getModal().find("div.machines-tree");
            console.log($tree);
            
            _$machineTree.init($tree, true);

            var body = _modalManager.getModal().find(".modal-body");
            var footer = _modalManager.getModal().find(".modal-footer");

            $('.modal-dialog').css("height", body.height() + footer.height());

            $("#ShiftSolutions2").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });
        }

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var machineIdList = _$machineTree.getSelectedMachineIds();

            if (machineIdList.length === 0) {
                abp.message.warn(app.localize("PleaseSelectEquipment"));
                return;
            }

            var startTime = $('#StartTime').val();
            var endTime = $('#EndTime').val();

            if (startTime.replace("-", "") > endTime.replace("-", "")) {
                abp.message.warn(app.localize("StartAndEndTimeValidate"));
                return;
            }

            var params = {
                machineIdList: machineIdList,
                shiftSolutionId: $("#ShiftSolutions2").val(),
                startTime: startTime,
                endTime: endTime
            }

            _modalManager.setBusy(true);
            _shiftAppService.createMultiMachineShift(params).done(function (data) {
                if (data === null) {
                    abp.notify.success(app.localize("OperationSuccess"));
                }
                else {
                    abp.message.warn(data, app.localize("Warning"));
                }

                _modalManager.close();
                $("#btnQuery").click();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };

        //getDeviceGroups();
        getShiftSolutions2();
    };
})();