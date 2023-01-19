
var StateStatisticModule = (function ($) {
    return function () {
        var stateService = abp.services.app.states;
        var $run = $("#total-in-run-state");
        var $stop = $("#total-in-stop-state");
        var $free = $("#total-in-free-state");
        var $offline = $("#total-in-offline-state");
        var $debug = $("#total-in-debug-state");

        var $mongodbAlert = $("#mongodbAlert");//.alert();
        var $mongodbAlertContent = $('#mongodbAlertContent');
        var $syncDataState = $("#syncDataState");
        var $syncDataStateContent = $("#syncDataStateContent");
        function run() {
            stateService.getRealtimeMachineStateSummary().done(function (summary) {
                //清空数据
                var $totalCnt = $("#total-summary-info");
                $totalCnt.find(".amount").text(0);
                // $totalCnt.find(".title").text('');
                $totalCnt.find(".rate").text('0%');
                //if (summary.syncDataState == "NO") {
                //    // $mongodbAlert.show();
                //    $syncDataState.removeClass('hide');// .show();
                //    $syncDataStateContent.html("存在同步异常数据，请到【系统管理】下的【维护】页面清理");
                //}
                //else {
                //    $syncDataState.addClass('hide');// .show();
                //}
                if (summary.isError) {
                    // $mongodbAlert.show();
                    $mongodbAlert.removeClass('hide');// .show();
                    $mongodbAlertContent.html(summary.message);
                    return;
                }

                _.each(summary.stateCollection, function (item) {
                    switch (item.code) {
                        case "Run":
                            $run.find(".amount").text(item.amount);
                            $run.find(".title").text(item.name);
                            $run.find(".rate").text(item.rate + "%");
                            $run.find(".amount").css("background-color", item.hexcode);
                            $run.find("a").attr("href", abp.appPath + "MachineStates?stateCode=" + item.code);
                            break;
                        case "Stop":
                            $stop.find(".amount").text(item.amount);
                            $stop.find(".title").text(item.name);
                            $stop.find(".rate").text(item.rate + "%");
                            $stop.find(".amount").css("background-color", item.hexcode);
                            $stop.find("a").attr("href", abp.appPath + "MachineStates?stateCode=" + item.code);
                            break;
                        case "Free":
                            $free.find(".amount").text(item.amount);
                            $free.find(".title").text(item.name);
                            $free.find(".rate").text(item.rate + "%");
                            $free.find(".amount").css("background-color", item.hexcode);
                            $free.find("a").attr("href", abp.appPath + "MachineStates?stateCode=" + item.code);
                            break;
                        case "Offline":
                            $offline.find(".amount").text(item.amount);
                            $offline.find(".title").text(item.name);
                            $offline.find(".rate").text(item.rate + "%");
                            $offline.find(".amount").css("background-color", item.hexcode);
                            $offline.find("a").attr("href", abp.appPath + "MachineStates?stateCode=" + item.code);
                            break;
                        case "Debug":
                            $debug.find(".amount").text(item.amount);
                            $debug.find(".title").text(item.name);
                            $debug.find(".rate").text(item.rate + "%");
                            $debug.find(".amount").css("background-color", item.hexcode);
                            $debug.find("a").attr("href", abp.appPath + "MachineStates?stateCode=" + item.code);
                            break;
                        default:
                    }
                });
            });
        }

        return { run: run };
    };
})(jQuery);