(function() {
    $(function() {
        var performanceService = abp.services.app.performanceDevice;
        var onlineOrOfflineRecordService = abp.services.app.onlineOrOfflineRecord;

        var selectShiftDetailModal = new app.ModalManager({
            viewUrl: abp.appPath + "StaffPerformance/OnlineShiftDetailModal",
            scriptUrl: abp.appPath + "view-resources/Views/StaffPerformance/_OnlineShiftDetailModal.js",
            modalClass: "SelectShiftDetailModal"
        });

        var selectAllModal = new app.ModalManager({
            viewUrl: abp.appPath + "StaffPerformance/OnlineAllModal",
            scriptUrl: abp.appPath + "view-resources/Views/StaffPerformance/_OnlineAllModal.js",
            modalClass: "SelectAllModal"
        });

        var selectDeviceGroupModal = new app.ModalManager({
            viewUrl: abp.appPath + "StaffPerformance/OfflineDeviceGroupModal",
            scriptUrl: abp.appPath + "view-resources/Views/StaffPerformance/_OfflineDeviceGroupModal.js",
            modalClass: "SelectDeviceGroupModal"
        });

        Handlebars.registerHelper('renderImage',
            function(path) {
                var imagePath;
                if ($.trim(path).length === 0) {
                    imagePath = abp.appPath + 'Content/Images/CNC1-128x128.png';
                } else {
                    imagePath = abp.appPath + path;
                }

                return new Handlebars.SafeString('<img class="img-circle" src="' + imagePath + '" alt="device Avatar"  style="height: 70px"> ');
            });

        Handlebars.registerHelper('renderButton',
            function(personnelUserId) {
                return new Handlebars.SafeString('<button type="button" class="btn btn-default btn-sm btn-block" data-isOnline="true">' + app.localize("Offline")+'</button>');
            });

        var page = {
            $btnQuery: $("#btnQuery"),
            $machineTree: new MachinesTree(),
            $statusfilter: $("#filter-status"),
            template: $("#machine-template").html(),
            init: function() {
                this.$machineTree.init($("div.machines-tree"), true);
                this.$machineTree.initGroup($("div.machines-group-tree"));
                this.$machineTree.setSelectAll();
                this.bindingEvent();
                this.load();
            },
            load: function(useState) {
                var queryType = this.$machineTree.getQueryType();
                var param = this.getParams(queryType);
                if (!_.isUndefined(useState)) {
                    param.UseState = useState;
                }
                performanceService.getDevices(param)
                    .done(function(response) {
                        var render = Handlebars.compile(page.template);
                        $("#show_machines").empty().append(render(response.items));
                        $(".ellipsis").dotdotdot();
                    });
            },
            getParams: function (queryType) {
                var machineIdList = [], groupIdList = [];
                if (queryType === "0") {
                    machineIdList = page.$machineTree.getSelectedMachineIds();
                } else {
                    groupIdList = page.$machineTree.getSelectedGroupIds();
                }
                return {
                    UseState: page.$statusfilter.find('.active input').data('target'),
                    DeviceIds: machineIdList,
                    DeviceGroupIds: groupIdList
                };
            },
            bindingEvent: function() {
                //查询按钮
                this.$btnQuery.click(function() {
                    page.load();
                });
                //查询状态按钮
                $("div.center-block")
                    .on('click',
                        '.btn',
                        function(e) {
                            var useState = $(e.currentTarget).find('input').data('target');
                            page.load(useState);
                        });

                //上下线按钮
                $("#show_machines")
                    .on('click',
                        '.btn',
                        function(e) {
                            var target = $(e.currentTarget).closest('div.widget-device');
                            var isOnline = $(e.currentTarget).data('isonline');
                            if (isOnline) {
                                page.doOfflineAction(target.data('machineid'));
                            } else {
                                page.doOnlineAction(target.data('machineid'));
                            }
                    });
                $("#onlineAll").click(function () {
                    $("#shiftDetailId").empty();
                    page.allOnlineAction();
                });

                $("#offlineAll").click(function () {
                    
                    page.allOfflineAction();
                });
            },
            doOnlineAction: function (machineId) {
                selectShiftDetailModal.open({ machineId: machineId },
                    function(data) {
                        var param = { MachineId: machineId, UserId: document.getElementById("userId").value, ShiftId: data };
                        performanceService.online(param)
                            .done(function() {
                                page.load();
                            });
                    });

            },
            doOfflineAction: function(machineId) {
                performanceService.offline({ MachineId: machineId })
                    .done(function() {
                        page.load();
                    });
            },
            allOnlineAction: function () {
                selectAllModal.open({},
                    function (data) {
                        var deviceGroupId = document.getElementById("machineGroup").value;
                        var userId = document.getElementById("userId").value;
                        var param = { DeviceGroupId: deviceGroupId, UserId: userId, ShiftId: data};
                                    performanceService.onlineAll(param)
                                        .done(function (data) {
                                            page.load();
                                            if (data != null) {
                                                abp.message.error(data,app.localize("EquipmentHasNoSchedule"));   
                                            }
                                        });
                                
                    });
            },
            allOfflineAction: function () {
                selectDeviceGroupModal.open({},
                    function () {
                        var deviceGroupId = document.getElementById("offlineMachineGroup").value;
                        var userId = document.getElementById("offlineUser").value;
                        var param = { DeviceGroupId: deviceGroupId, UserId: userId};
                        performanceService.offlineBatch(param)
                            .done(function (data) {
                                page.load();
                                if (data != null) {
                                    abp.message.error(data);
                                }
                            });
                    }
                )
            }

        }


        page.init();

    });
})(jQuery);