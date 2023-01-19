(function ($) {
    app.modals.ShowSelectQueryConditionsModal = function () {
        var _modalManger;
        var _datepicker = $("#daterange-btn");
        var _machineTree = new MachinesTree();
        var commonService = abp.services.app.commonLookup;

        this.init = function (modalManger) {
            _modalManger = modalManger;

            _machineTree.init($("div.machines-tree"), true);
            _machineTree.selectMachineTreeByDefault();
            _machineTree.openFirstMachineTreeNode();

            _datepicker.WIMIDaterangepicker({
                startDate: moment().subtract(6, "days"),
                endDate: moment()
            });
        }

        this.save = function () {
            var machineIdList = _machineTree.getSelectedMachineIds();
            var param = {
                MachineIdList: machineIdList,
                StartTime: _datepicker.data("daterangepicker").startDate.format("YYYY-MM-DD"),
                EndTime: _datepicker.data("daterangepicker").endDate.format("YYYY-MM-DD")
            };
            checkQueryDataCount(param);
        }

        function checkQueryDataCount(param) {
            if (param.MachineIdList.length > app.consts.maximumNumberofQueries.machineYield) {
                layer.open({
                    type: 1,
                    title: false //不显示标题栏
                    ,
                    closeBtn: false,
                    area: '300px;',
                    shade: 0.8,
                    id: 'LAY_layuipro' //设定一个id，防止重复弹出
                    ,
                    resize: false,
                    btn: [app.localize("Confirm"), app.localize("Cancel")],
                    btnAlign: 'c',
                    moveType: 1 //拖拽模式，0或者1
                    ,
                    content:
                        '<div style="padding: 30px; line-height: 22px; background-color: #393D49; color: #fff; font-weight: 300;">' + app.localize("QueryResultsContainTooMuchData") + '？</div>',
                    success: function (layero) {
                        var btn = layero.find(".layui-layer-btn0");
                        btn.click(function () {
                            doQueryActiion(param);
                        });
                    }
                });
            } else {
                doQueryActiion(param);
                return true;
            }
        }

        function doQueryActiion(param) {
            _modalManger.setResult(param);
            _modalManger.close();
        }
    }
})(jQuery);