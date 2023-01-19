var $dateTabRender = Handlebars.compile($("#tab-date-template").html());
var detailModal = new app.ModalManager({
    viewUrl: abp.appPath + "StatusDistributionMap/ShowStateDetailModal",
    scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/StatusDistributionMap/_ShowStateDetailModal.js",
    modalClass: "ShowStateDetailModal"
});

(function () {
    $(function () {
        var yieldService = abp.services.app.yield,
            service = abp.services.app.statusDistributionMap,
            $sel = undefined,
            $deviceGroupSel = undefined,
            macIdList = null,
            deviceGroupId = 0,
            lsc
        // $dateTabRender = Handlebars.compile($("#tab-date-template").html());

        // var detailModal = new app.ModalManager({
        //     viewUrl: abp.appPath + "StatusDistributionMap/ShowStateDetailModal",
        //     scriptUrl: abp.appPath + "Views/StatusDistributionMap/_ShowStateDetailModal.js",
        //     modalClass: "ShowStateDetailModal"
        // });
        $(".content-wrapper").css("min-height", $(window).height() - 70);

        var defaultSelectCount = $(".DefaultSearchMachineCount").val();

        const parameters = {
            startTime: null,
            endTime: null
        };

        if (app.consts.fixedCalendar.enabled) {
            parameters.startTime = app.consts.fixedCalendar.startTime;
            parameters.endTime = app.consts.fixedCalendar.endTime;
        }



        //页面初始化时，自动加载数据
        yieldService.getFirstQueryParam(parameters).done(function (response) {
            scrollTabObject.init(response.dataList, response.machineIdList);
        });
        abp.services.app.basicData.getStateAndReasonForVisual().done(function (response) {
            //  console.log(response);
            for (var i = 0; i < response.length; i++) {
                $("#statusDetail").append('<li><label style="background-color:' + response[i].hexcode + '; width: 30px; height: 15px;"></label><span style="font-size: 12px; position: relative; top: -8px;">' + response[i].displayName + '</span></li>')
            }
            // 
        });

        function initDeviceGroupSelector() {
            service.listDeviceGroups().done(function (response) {
                var data = _.map(response, function (item) {
                    return {
                        id: item.value,
                        text: item.name
                    }
                })

                data.unshift({
                    text: app.localize("CheckAll"),
                    id: 0
                })

                $deviceGroupSel = $("#sel_menu1").select2({
                    data: data
                });

                $deviceGroupSel.val(0).trigger("change");
            })
        }

        function initMachineSelector(selectId) {
            service.listMachines({ Id: selectId }).done(function (response) {
                var data = _.map(response, function (item) {
                    return {
                        id: item.value,
                        text: item.name
                    }
                })

                macIdList = _.map(response, function (item) {
                    return item.value
                })

                data.unshift({
                    text: app.localize("Default"),
                    id: "default"
                })

                data.unshift({
                    text: app.localize("CheckAll"),
                    id: "all"
                })

                $("#sel_menu2").empty();

                $sel = $("#sel_menu2").select2({
                    data: data
                });

                //$sel.val("all").trigger("change");
                $sel.val("default").trigger("change");


                $("#sel_menu2").on("change", function (e) {
                    var v = $("#sel_menu2").select2("val")
                    if (v != null) {
                        if (v.length > 1) {
                            v.forEach(function (item, index) {
                                if (item == 'all') {
                                    v.splice(index, 1);
                                    $sel.val(v).trigger("change");
                                }
                                if (item == 'default') {
                                    v.splice(index, 1);
                                    $sel.val(v).trigger("change");
                                }
                            })
                        }
                    } else {
                        $sel.val("all").trigger("change");
                    }
                })
                $("#sel_menu2").on("select2:select", function (e) {
                    if (e != null && e.params.data.id == 'all') {
                        $sel.val("all").trigger("change");
                    }
                    if (e != null && e.params.data.id == 'default') {
                        $sel.val("default").trigger("change");
                    }
                })
            })
        }

        $("#sel_menu1").on("change", function (e) {
            deviceGroupId = $("#sel_menu1").val();
            initMachineSelector(deviceGroupId);
        });

        initDeviceGroupSelector();

        $(".searchFn").on("click", function () {
            var startTime = $datepicker.data("daterangepicker").startDate.format("YYYYMMDD");
            var endTime = $datepicker.data("daterangepicker").endDate.format("YYYYMMDD");
            var idlist;
            var type;
            if ($('input[name="optionsRadiosinline"]:checked').val() == "1") {//日期
                type = 1;
            } else {
                type = 0;
            }

            if ($("#sel_menu2").val() == 'all') {
                idlist = macIdList
            } else if ($("#sel_menu2").val() == 'default') {
                if (macIdList.length <= defaultSelectCount) {
                    idlist = macIdList
                }
                else {
                    idlist = macIdList.slice(0, defaultSelectCount);
                }
            } else {
                idlist = $("#sel_menu2").val()
            }

            service.listSummaryConditions({ startTime: startTime, endTime: endTime, machineIdList: idlist, mode: type }).done(function (response) {
                lsc = response
                var re = _.map(response, function (item) {
                    return item.name
                })
                scrollTabObject.init(re, idlist);
            })

        });
        var $datepicker = $("#daterange-btn")
        $datepicker.WIMIDaterangepicker({
            startDate: moment().subtract(6, "days"),
            endDate: moment()
        });


        //日期选择tab
        var scrollTabObject = {
            $scrollTab: null,
            init: function (dateList, machineIdList) {
                if (this.$scrollTab !== null) {
                    this.$scrollTab.destroy();
                }
                this.$scrollTab = $("#tabsDate")
                    .html($dateTabRender({ dates: dateList }))
                    .scrollTabs({
                        click_callback: function () {
                            var $this = $(this);
                            scrollTabObject.loadData($this.text(), machineIdList);
                        }
                    });
                this.loadData($.trim($("#tabsDate .tab_selected").text()), machineIdList);
            },
            loadData: function (summaryDate, machineIdList) {
                var startTime = $datepicker.data("daterangepicker").startDate.format("YYYY-MM-DD");
                var endTime = $datepicker.data("daterangepicker").endDate.format("YYYY-MM-DD");
                yieldTableObject.loadData(summaryDate, machineIdList);
            }
        }
        //表格
        var yieldTableObject = {
            //加载表格数据
            loadData: function (summaryDate, machineIdList) {
                $("#table_status").empty();
                abp.ui.setBusy();
                if ($("#sel_menu2").val() != null) {
                    if ($("#sel_menu2").val() == 'all') {
                        machineIdList = macIdList
                    } else if ($("#sel_menu2").val() == 'default') {
                        if (macIdList.length <= defaultSelectCount) {
                            machineIdList = macIdList
                        }
                        else {
                            machineIdList = macIdList.slice(0, defaultSelectCount);
                        }
                    } else {
                        machineIdList = $("#sel_menu2").val()
                    }

                }
                //0设备 1日期
                if ($('input[name="optionsRadiosinline"]:checked').val() == "1") {
                    var arr = summaryDate.split("-")
                    var parm = {
                        startTime: arr.join(''),
                        endTime: arr.join(''),
                        feedback: true,
                        machineIdList: machineIdList,
                        mode: 1
                    };
                    service.listStatusDistribution(parm)
                        .done(function (response) {
                            abp.ui.clearBusy();
                            // console.log(response);
                            if (response.length != 0) {
                                //格式整理
                                var data = {
                                    Status: 0,
                                    Message: app.localize("SuccessfulOperation"),
                                    Data: [{
                                        Date: "/Date(" + Date.parse(response[0].shiftDay) + ")/",
                                        StatusData: [

                                        ]
                                    }]
                                }
                                response.forEach(function (items, indexs) {
                                    data.Data[0].StatusData.push({
                                        NAME: items.machineName,
                                        Data: [

                                        ],
                                        MyRateStatusList: items.statusSummaryRate,
                                        MacNo: items.machineId
                                    })
                                    items.data.forEach(function (item, index) {
                                        data.Data[0].StatusData[indexs].Data.push({
                                            STATUSID: item.stateId,
                                            NAME: item.stateName,
                                            COLOR: item.hexcode,
                                            START: "/Date(" + Date.parse(item.startTime) + ")/",
                                            END: "/Date(" + Date.parse(item.endTime) + ")/"
                                        })
                                    });

                                })
                                // console.log(data);
                                //*****
                                abp.ui.clearBusy();
                                for (var i = 0; i < data.Data.length; i++) {
                                    for (var j = 0; j < data.Data[i].StatusData.length; j++) {
                                        $("#table_status").append('<tr id="' + i + "_" + j + '"><td width="100" style="display:none"></td><td width="100"></td><td width="20"><i style="font-size:20px;cursor:pointer;" value="0" macno="' + data.Data[i].StatusData[j].MacNo + '" date="' + data.Data[i].Date + '" id="shift_' + i + "_" + j + '" class="fa fa-caret-right"></i></td><td><div id="statusList_' + i + "_" + j + '"></div></td><td width="20"><i class="fa fa-search" style="cursor:pointer;" macno="' + data.Data[i].StatusData[j].MacNo + '" date="' + data.Data[i].Date + '" onclick="showStatusDetail(this)"></i></td><td class="statusRate_' + i + '" id="statusRate_' + i + "_" + j + '" width="2"></td></tr>');
                                        //状态比例
                                        // var shtml = '<div style="display:none;position: absolute; right: 5px;border: 1px solid #000000; background-color: #FFFFFF;"><table class="statustable table-bordered">';
                                        // var tr1 = "<tr>"; var tr2 = "<tr>";
                                        // if (data.Data[i].StatusData[j].MyRateStatusList != null) {
                                        //     for (var k = 0; k < data.Data[i].StatusData[j].MyRateStatusList.length; k++) {
                                        //         tr1 = tr1 + '<th colspan="2" style="background-color:' + data.Data[i].StatusData[j].MyRateStatusList[k].hexcode + '">' + data.Data[i].StatusData[j].MyRateStatusList[k].stateName + '</th>';
                                        //         tr2 = tr2 + '<td align="center">' + data.Data[i].StatusData[j].MyRateStatusList[k].hour + 'h</td><td align="center">' + (data.Data[i].StatusData[j].MyRateStatusList[k].percent).toFixed(1) + '%</td>';
                                        //     }
                                        //     shtml = shtml + tr1 + tr2 + "</table></div>";
                                        //     console.log(shtml);
                                        //     console.log("#statusRate_" + i + "_" + j);
                                        //     $("#statusRate_" + i + "_" + j).append('');
                                        // }
                                        var shtml = '<div class="m-rate" style="text-align: center;border: 0px solid #000;overflow: hidden;height: 40px;position: absolute;display:none;background-color:#fff;width:' + 80 * data.Data[i].StatusData[j].MyRateStatusList.length + 'px;right: 15px;">';
                                        if (data.Data[i].StatusData[j].MyRateStatusList != null) {
                                            for (var k = 0; k < data.Data[i].StatusData[j].MyRateStatusList.length; k++) {
                                                //shtml=shtml+'<div style="width: 80px;overflow: hidden;float: left;"><div style="height: 20px;line-height: 20px;background-color:'+data.Data[i].StatusData[j].MyRateStatusList[k].hexcode+'">'+data.Data[i].StatusData[j].MyRateStatusList[k].stateName+'</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">'+data.Data[i].StatusData[j].MyRateStatusList[k].hour+'h</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">'+(data.Data[i].StatusData[j].MyRateStatusList[k].percent).toFixed(1)+'%</div></div>'
                                                shtml = shtml + '<div style="width: 80px;overflow: hidden;float: left;"><div style="height: 20px;line-height: 20px;background-color:' + data.Data[i].StatusData[j].MyRateStatusList[k].hexcode + '">' + data.Data[i].StatusData[j].MyRateStatusList[k].stateName + '</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">' + data.Data[i].StatusData[j].MyRateStatusList[k].hour + 'h</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">' + (data.Data[i].StatusData[j].MyRateStatusList[k].percent).toFixed(1) + '%</div></div>'
                                            }
                                            shtml = shtml + "</div>";
                                            $("#statusRate_" + i + "_" + j).append(shtml);
                                        }

                                        if (j == 0) {//样式处理:设备名称显示在第一格
                                            $("#" + i + "_" + j + " td").eq(0).append('<span id="allshowStatusRate_' + i + "_" + j + '" class="badge badge-inverse" style="cursor:pointer;">' + moment(data.Data[i].Date).format("YYYY-MM-DD") + '</span>');
                                        }
                                        $("#" + i + "_" + j + " td").eq(1).append('<span id="showStatusRate_' + i + "_" + j + '" mac=' + data.Data[i].StatusData[j].MacNo + ' date=' + data.Data[i].Date + ' class="badge badge-important" style="cursor:pointer;">' + data.Data[i].StatusData[j].NAME + '</span>');
                                        //显示状态比例
                                        $("#showStatusRate_" + i + "_" + j).on("click", { i: i, j: j }, function (event) {
                                            //跟新位置

                                            var top = $("#statusRate_" + event.data.i + "_" + event.data.j).parent().offset().top;
                                            $("#statusRate_" + event.data.i + "_" + event.data.j).children().css("top", top + "px");
                                            if ($("#statusRate_" + event.data.i + "_" + event.data.j).children().is(":hidden")) {

                                                $("#statusRate_" + event.data.i + "_" + event.data.j).children().show('slide', { direction: 'right' }, 500);
                                            }
                                            else {

                                                // console.log($("#statusRate_" + event.data.i + "_" + event.data.j));
                                                $("#statusRate_" + event.data.i + "_" + event.data.j).children().hide('slide', { direction: 'right' }, 500);
                                            }

                                            //更新饼图
                                            // var data = {
                                            //     ObjectIDs: parseInt($(this).attr('mac')),
                                            //     StartTime: moment($(this).attr('date')).format('YYYY/MM/DD'),
                                            //     EndTime: moment($(this).attr('date')).format('YYYY/MM/DD'),
                                            //     ShowDetails: $('input[name="detailshow"]').prop("checked")
                                            // };
                                            //GetMachineStatusRatio(data.StartTime, data.StartTime, data.ObjectIDs, 'chartRato2', moment($(this).attr('date')).format('YYYY/MM/DD') + "(" + groupOrMachine.dataAarry[data.ObjectIDs] + ")设备状态比例图", data.ShowDetails);
                                        });
                                        $("#allshowStatusRate_" + i + "_" + j).on("click", { i: i, j: j }, function (event) {
                                            //跟新位置
                                            var obj = $(".statusRate_" + event.data.i);
                                            var flag = $($(".statusRate_" + event.data.i)[0]).children().is(":hidden");
                                            for (var m = 0; m < obj.length; m++) {
                                                var tt = $($(".statusRate_" + event.data.i)[m]);
                                                var top = tt.parent().offset().top;
                                                tt.children().css("top", top + "px");
                                                if (flag) {
                                                    tt.children().show('slide', { direction: 'right' }, 500);
                                                }
                                                else {
                                                    tt.children().hide('slide', { direction: 'right' }, 500);
                                                }
                                            }
                                        });

                                        //状态条形图
                                        $("#statusList_" + i + "_" + j).viewChart({
                                            width: $("#statusList_" + i + "_" + j).width() <= 720 ? 720 : $("#statusList_" + i + "_" + j).width(),
                                            url: "/MachineStatus/StatusDistributionMap/GetMachineStatusDetails",//改逻辑
                                            startdate: data.Data[i].Date,
                                            mac_no: data.Data[i].StatusData[j].MacNo, //设备编号
                                            filter: $('input[name="filter"]').prop("checked"),
                                            logic: '',
                                            logicValue: $("#filterValue").val(),
                                            select: function (data) {
                                                var dd = {
                                                    ObjectIDs: [data.mac_nbr],
                                                    StartTime: data.startdate.format("YYYY-MM-DD HH:mm:ss"),
                                                    EndTime: data.enddate.format("YYYY-MM-DD HH:mm:ss"),
                                                    ShowDetails: $('input[name="detailshow"]').prop("checked")
                                                }


                                                $.x5window(app.localize("StateAmplification"), kendo.template($("#popup-showStatus").html()));
                                                $.post("/MachineStatus/StatusDistributionMap/GetMachineStatusListByDate", JSON.stringify(dd), function (data) {

                                                });
                                            },
                                            three: false//$('input[name="3dshow"]').prop("checked")
                                        }).data("BZ-viewChart");
                                        $("#statusList_" + i + "_" + j).data("BZ-viewChart").saveData(data.Data[i].StatusData[j].Data);
                                        $("#statusList_" + i + "_" + j).data("BZ-viewChart").drawStatus(data.Data[i].StatusData[j].Data);

                                        //班次显示
                                        $('#shift_' + i + "_" + j).on("click", { i: i, j: j }, function (event) {
                                            $(".m-rate").hide('slide', { direction: 'right' }, 500);
                                            var self = this;
                                            var ii = event.data.i;
                                            var jj = event.data.j;
                                            var macno = parseInt($(this).attr("macno"))
                                            if ($(this).attr("value") == 0) {//显示班次
                                                var pr = {
                                                    machineId: parseInt($(this).attr("macno")),
                                                    shiftDay: moment($(this).attr("date")).format("YYYY-MM-DD")
                                                }

                                                abp.ui.setBusy();

                                                service.listShiftStatusDistribution(pr).done(function (res) {
                                                    // console.log(res);
                                                    var data = {
                                                        Status: 0,
                                                        Message: app.localize("SuccessfulOperation"),
                                                        Data: []
                                                    }
                                                    res.forEach(function (items, indexs) {
                                                        // console.log(items.startTime);
                                                        // console.log(items.endTime);
                                                        var myRateStatusList = [];
                                                        items.statusSummaryRate.forEach(function (ite, ind) {
                                                            myRateStatusList.push({
                                                                RATE: ite.percent,
                                                                TIME: ite.hour,
                                                                STATUS_NAME: ite.stateName,
                                                                COLOR: ite.hexcode,
                                                            })

                                                        })

                                                        data.Data.push({
                                                            SHIFT: items.shiftName,
                                                            MyRateStatusList: myRateStatusList,
                                                            START: "/Date(" + Date.parse(items.startTime) + ")/",
                                                            END: "/Date(" + Date.parse(items.endTime) + ")/",
                                                            StatusData: [],
                                                            MacNo: macno,
                                                            machineShiftDetailId: items.machineShiftDetailId
                                                        })
                                                        items.data.forEach(function (item, index) {
                                                            data.Data[indexs].StatusData.push({
                                                                STATUSID: item.stateId,
                                                                NAME: item.stateName,
                                                                COLOR: item.hexcode,
                                                                memo: item.memo,
                                                                START: "/Date(" + Date.parse(item.startTime) + ")/",
                                                                END: "/Date(" + Date.parse(item.endTime) + ")/"
                                                            })
                                                        })
                                                    })
                                                    // console.log('###############');
                                                    // console.log(data);/////
                                                    // $.post("/MachineStatus/StatusDistributionMap/GetMachineStatusListByShift", JSON.stringify(data), function (data) {
                                                    if (data.Status == 0) {
                                                        $(".shift_" + ii + "_" + jj).remove();
                                                        for (var mm = data.Data.length - 1; mm >= 0; mm--) {
                                                            //插入tr
                                                            $('<tr class="shift_' + ii + "_" + jj + '" id="shift_' + ii + "_" + jj + '"><td width="100" style="display:none;"></td><td width="100"></td><td width="20"></td><td><div id="shiftstatusList_' + ii + "_" + jj + '"></div></td><td width="20"><i class="fa fa-search" style="cursor:pointer;" machineShiftDetailId="' + data.Data[mm].machineShiftDetailId + '"macno="' + data.Data[ii].MacNo + '" date="' + data.Data[ii].Date + '" onclick="showShiftStatusDetail(this)"></i></td><td class="statusRate_' + ii + '" id="shiftstatusRate_' + ii + "_" + jj + "_" + mm + '" width="2"></td></tr>').insertAfter($(self).parent().parent());
                                                            //状态比例
                                                            var shtml = '<div style="display:none;position: absolute; right: 5px;border: 1px solid #000000; background-color: #FFFFFF;"><table class="statustable table-bordered">';
                                                            var tr1 = "<tr>"; var tr2 = "<tr>";
                                                            if (data.Data[mm].MyRateStatusList != null) {
                                                                for (var k = 0; k < data.Data[mm].MyRateStatusList.length; k++) {
                                                                    tr1 = tr1 + '<th align="center" colspan="2" style="text-align:center;background-color:' + data.Data[mm].MyRateStatusList[k].COLOR + '">' + data.Data[mm].MyRateStatusList[k].STATUS_NAME + '</th>';
                                                                    tr2 = tr2 + '<td align="center">&nbsp;' + data.Data[mm].MyRateStatusList[k].TIME + 'h&nbsp;</td><td align="center">&nbsp;' + (data.Data[mm].MyRateStatusList[k].RATE).toFixed(1) + '%&nbsp;</td>';
                                                                }
                                                                shtml = shtml + tr1 + tr2 + "</table></div>";
                                                                $("#shiftstatusRate_" + ii + "_" + jj + "_" + mm).append(shtml);
                                                            }
                                                            //状态条形图
                                                            $("#shift_" + ii + "_" + jj + " td").eq(1).append('<span id="shiftshowStatusRate_' + ii + "_" + jj + "_" + mm + '" class="badge badge-warning" style="cursor:pointer;">' + data.Data[mm].SHIFT + '  <i class="fa fa-hand-o-up" aria-hidden="true"></i></span>');
                                                            $("#shiftstatusList_" + ii + "_" + jj).viewChart({
                                                                width: $("#shiftstatusList_" + ii + "_" + jj).width() <= 720 ? 720 : $("#shiftstatusList_" + ii + "_" + jj).width(),
                                                                type: "shift",
                                                                url: "/MachineStatus/StatusDistributionMap/GetMachineStatusDetails",//改逻辑
                                                                shiftstartdate: data.Data[mm].START,
                                                                filter: $('input[name="filter"]').prop("checked"),
                                                                logic: '',
                                                                logicValue: $("#filterValue").val(),
                                                                three: false,//$('input[name="3dshow"]').prop("checked"),
                                                                timedepth: moment(data.Data[mm].END).diff(moment(data.Data[mm].START), "seconds")
                                                            }).data("BZ-viewChart").drawStatus(data.Data[mm].StatusData);
                                                            //显示状态比例
                                                            $("#shiftshowStatusRate_" + ii + "_" + jj + "_" + mm).on("click", { i: ii, j: jj, m: mm }, function (event) {
                                                                //跟新位置
                                                                var top = $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).parent().offset().top;
                                                                $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().css("top", top + "px");
                                                                if ($("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().is(":hidden")) {
                                                                    $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().show('slide', { direction: 'right' }, 500);
                                                                }
                                                                else {
                                                                    $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().hide('slide', { direction: 'right' }, 500);
                                                                }
                                                            });
                                                        }
                                                        //更新图标
                                                        $(self).attr("value", 1).addClass("fa-sort-desc").removeClass("fa-caret-right");
                                                    }
                                                    else {
                                                        BzAlert(data.Message);
                                                    }
                                                }).always(function () {
                                                    abp.ui.clearBusy();
                                                });
                                            }
                                            else {
                                                $(".shift_" + ii + "_" + jj).remove();
                                                //更新图标
                                                $(self).attr("value", 0).addClass("fa-caret-right").removeClass("fa-sort-desc");
                                            }
                                        });
                                    }
                                }
                                //结束
                            }

                        }).always(function () {
                            abp.ui.clearBusy();
                        });
                } else {
                    machineIdList = []
                    lsc.forEach(function (item, index) {
                        if (item.name == summaryDate) {
                            machineIdList.push(item.value)
                        }
                    })
                    var parm = {
                        startTime: $datepicker.data("daterangepicker").startDate.format("YYYYMMDD"),
                        endTime: $datepicker.data("daterangepicker").endDate.format("YYYYMMDD"),
                        feedback: true,
                        machineIdList: machineIdList,
                        mode: 0
                    };
                    //   console.log(parm);
                    service.listStatusDistribution(parm)
                        .done(function (response) {
                            //  console.log(response);
                            abp.ui.clearBusy();
                            if (response.length != 0) {
                                //格式整理
                                var data = {
                                    Status: 0,
                                    Message: app.localize("SuccessfulOperation"),
                                    Data: [{
                                        NAME: response[0].machineName,
                                        StatusData: [

                                        ],
                                        MacNo: response[0].machineId
                                    }]
                                }
                                response.forEach(function (items, indexs) {
                                    data.Data[0].StatusData.push({
                                        Date: "/Date(" + Date.parse(items.shiftDay) + ")/",
                                        Data: [

                                        ],
                                        MyRateStatusList: items.statusSummaryRate
                                    })
                                    items.data.forEach(function (item, index) {
                                        data.Data[0].StatusData[indexs].Data.push({
                                            STATUSID: item.stateId,
                                            NAME: item.stateName,
                                            COLOR: item.hexcode,
                                            START: "/Date(" + Date.parse(item.startTime) + ")/",
                                            END: "/Date(" + Date.parse(item.endTime) + ")/"
                                        })
                                    });

                                })
                                //   console.log(data);
                                //*****
                                $("#table_status").empty();
                                for (var i = 0; i < data.Data.length; i++) {
                                    for (var j = 0; j < data.Data[i].StatusData.length; j++) {
                                        $("#table_status").append('<tr id="' + i + "_" + j + '"><td width="100" style="display:none"></td><td width="100"></td><td width="20"><i style="font-size:20px;cursor:pointer;" value="0" macno="' + data.Data[i].MacNo + '" date="' + moment(data.Data[i].StatusData[j].Date).format("YYYY-MM-DD") + '" id="shift_' + i + "_" + j + '" class="fa fa-caret-right"></i></td><td><div id="statusList_' + i + "_" + j + '"></div></td><td width="20"><i class="fa fa-search" style="cursor:pointer;" macno="' + data.Data[i].MacNo + '" date="' + moment(data.Data[i].StatusData[j].Date).format("YYYY-MM-DD") + '" onclick="showStatusDetail(this)"></i></td><td class="statusRate_' + i + '" id="statusRate_' + i + "_" + j + '" width="2"></td></tr>');
                                        //状态比例
                                        // var shtml = '<div style="display:none;position: absolute; right: 5px;border: 1px solid #000000; background-color: #FFFFFF;"><table class="statustable table-bordered">';
                                        // var tr1 = "<tr>"; var tr2 = "<tr>";
                                        // if (data.Data[i].StatusData[j].MyRateStatusList != null) {
                                        //     for (var k = 0; k < data.Data[i].StatusData[j].MyRateStatusList.length; k++) {
                                        //         tr1 = tr1 + '<th colspan="2" style="background-color:' + data.Data[i].StatusData[j].MyRateStatusList[k].COLOR + '">' + data.Data[i].StatusData[j].MyRateStatusList[k].STATUS_NAME + '</th>';
                                        //         tr2 = tr2 + '<td align="center">' + data.Data[i].StatusData[j].MyRateStatusList[k].TIME + 'h</td><td align="center">' + (data.Data[i].StatusData[j].MyRateStatusList[k].RATE).toFixed(1) + '%</td>';
                                        //     }
                                        //     shtml = shtml + tr1 + tr2 + "</table></div>";
                                        //     $("#statusRate_" + i + "_" + j).append(shtml);
                                        // }

                                        var shtml = '<div class="m-rate" style="text-align: center;border: 0px solid #000;overflow: hidden;height: 40px;position: absolute;display:none;background-color:#fff;width:' + 80 * data.Data[i].StatusData[j].MyRateStatusList.length + 'px;right: 15px;">';
                                        if (data.Data[i].StatusData[j].MyRateStatusList != null) {
                                            for (var k = 0; k < data.Data[i].StatusData[j].MyRateStatusList.length; k++) {
                                                shtml = shtml + '<div style="width: 80px;overflow: hidden;float: left;"><div style="height: 20px;line-height: 20px;background-color:' + data.Data[i].StatusData[j].MyRateStatusList[k].hexcode + '">' + data.Data[i].StatusData[j].MyRateStatusList[k].stateName + '</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">' + data.Data[i].StatusData[j].MyRateStatusList[k].hour + 'h</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">' + (data.Data[i].StatusData[j].MyRateStatusList[k].percent).toFixed(1) + '%</div></div>'
                                                // shtml=shtml+'<div style="width: 80px;overflow: hidden;float: left;"><div style="height: 20px;line-height: 20px;background-color:'+data.Data[i].StatusData[j].MyRateStatusList[k].hexcode+'">'+data.Data[i].StatusData[j].MyRateStatusList[k].stateName+'</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">'+data.Data[i].StatusData[j].MyRateStatusList[k].hour+'h</div><div style="width: 40px;height: 20px;line-height: 20px;overflow: hidden;float: left;border: 1px solid #eee">'+(data.Data[i].StatusData[j].MyRateStatusList[k].percent).toFixed(1)+'%</div></div>'
                                            }
                                            shtml = shtml + "</div>";
                                            $("#statusRate_" + i + "_" + j).append(shtml);
                                        }
                                        if (j == 0) {//样式处理:设备名称显示在第一格
                                            $("#" + i + "_" + j + " td").eq(0).append('<span id="allshowStatusRate_' + i + "_" + j + '" class="badge badge-inverse" style="cursor:pointer;">' + data.Data[i].NAME + '</span>');
                                        }
                                        $("#" + i + "_" + j + " td").eq(1).append('<span id="showStatusRate_' + i + "_" + j + '" mac="' + data.Data[i].MacNo + '" date="' + moment(data.Data[i].StatusData[j].Date).format("YYYY-MM-DD") + '" class="badge badge-important" style="cursor:pointer;">' + moment(data.Data[i].StatusData[j].Date).format("YYYY-MM-DD") + '</span>');
                                        //显示状态比例
                                        $("#showStatusRate_" + i + "_" + j).on("click", { i: i, j: j }, function (event) {

                                            //跟新位置
                                            var top = $("#statusRate_" + event.data.i + "_" + event.data.j).parent().offset().top;
                                            $("#statusRate_" + event.data.i + "_" + event.data.j).children().css("top", top + "px");
                                            if ($("#statusRate_" + event.data.i + "_" + event.data.j).children().is(":hidden")) {
                                                $("#statusRate_" + event.data.i + "_" + event.data.j).children().show('slide', { direction: 'right' }, 500);
                                            }
                                            else {
                                                $("#statusRate_" + event.data.i + "_" + event.data.j).children().hide('slide', { direction: 'right' }, 500);
                                            }
                                            //更新饼图
                                            // var data = {
                                            //     ObjectIDs: parseInt($(this).attr('mac')),
                                            //     StartTime: moment($(this).attr('date')).format('YYYY/MM/DD'),
                                            //     EndTime: moment($(this).attr('date')).format('YYYY/MM/DD'),
                                            //     ShowDetails: $('input[name="detailshow"]').prop("checked")
                                            // };
                                            // GetMachineStatusRatio(data.StartTime, data.StartTime, data.ObjectIDs, 'chartRato2', moment($(this).attr('date')).format('YYYY/MM/DD') + "(" + groupOrMachine.dataAarry[data.ObjectIDs] + ")设备状态比例图", data.ShowDetails);

                                        });
                                        $("#allshowStatusRate_" + i + "_" + j).on("click", { i: i, j: j }, function (event) {
                                            //跟新位置
                                            var obj = $(".statusRate_" + event.data.i);
                                            var flag = $($(".statusRate_" + event.data.i)[0]).children().is(":hidden");
                                            for (var m = 0; m < obj.length; m++) {
                                                var tt = $($(".statusRate_" + event.data.i)[m]);
                                                var top = tt.parent().offset().top;
                                                tt.children().css("top", top + "px");
                                                if (flag) {
                                                    tt.children().show('slide', { direction: 'right' }, 500);
                                                }
                                                else {
                                                    tt.children().hide('slide', { direction: 'right' }, 500);
                                                }
                                            }
                                        });
                                        //状态条形图
                                        $("#statusList_" + i + "_" + j).viewChart({
                                            width: $("#statusList_" + i + "_" + j).width() <= 720 ? 720 : $("#statusList_" + i + "_" + j).width(),
                                            url: "/MachineStatus/StatusDistributionMap/GetMachineStatusDetails",
                                            startdate: data.Data[i].StatusData[j].Date,
                                            filter: $('input[name="filter"]').prop("checked"),
                                            logic: '',
                                            logicValue: $("#filterValue").val(),
                                            three: false//$('input[name="3dshow"]').prop("checked")
                                        }).data("BZ-viewChart");
                                        $("#statusList_" + i + "_" + j).data("BZ-viewChart").saveData(data.Data[i].StatusData[j].Data);
                                        $("#statusList_" + i + "_" + j).data("BZ-viewChart").drawStatus(data.Data[i].StatusData[j].Data);
                                        //班次显示
                                        $('#shift_' + i + "_" + j).on("click", { i: i, j: j }, function (event) {
                                            $(".m-rate").hide('slide', { direction: 'right' }, 500);
                                            var self = this;
                                            var ii = event.data.i;
                                            var jj = event.data.j;
                                            var macno = parseInt($(this).attr("macno"))
                                            if ($(this).attr("value") == 0) {//显示班次
                                                var pr = {
                                                    // ObjectID: parseInt($(this).attr("macno")),
                                                    // Date: $(this).attr("date"),
                                                    // ShowDetails: $('input[name="detailshow"]').prop("checked")
                                                    machineId: parseInt($(this).attr("macno")),
                                                    shiftDay: moment($(this).attr("date")).format("YYYY-MM-DD")
                                                }

                                                //   console.log(data);
                                                abp.ui.setBusy();
                                                service.listShiftStatusDistribution(pr).done(function (res) {
                                                    //console.log(res);
                                                    var data = {
                                                        Status: 0,
                                                        Message: app.localize("SuccessfulOperation"),
                                                        Data: []
                                                    }
                                                    res.forEach(function (items, indexs) {
                                                        //   console.log(items.startTime);
                                                        // console.log(items.endTime);
                                                        var myRateStatusList = [];
                                                        items.statusSummaryRate.forEach(function (ite, ind) {
                                                            myRateStatusList.push({
                                                                RATE: ite.percent,
                                                                TIME: ite.hour,
                                                                STATUS_NAME: ite.stateName,
                                                                COLOR: ite.hexcode,
                                                            })

                                                        })

                                                        data.Data.push({
                                                            SHIFT: items.shiftName,
                                                            MyRateStatusList: myRateStatusList,
                                                            START: "/Date(" + Date.parse(items.startTime) + ")/",
                                                            END: "/Date(" + Date.parse(items.endTime) + ")/",
                                                            StatusData: [],
                                                            MacNo: macno,
                                                            machineShiftDetailId: items.machineShiftDetailId
                                                        })
                                                        items.data.forEach(function (item, index) {
                                                            data.Data[indexs].StatusData.push({
                                                                STATUSID: item.stateId,
                                                                NAME: item.stateName,
                                                                COLOR: item.hexcode,
                                                                memo: item.memo,
                                                                START: "/Date(" + Date.parse(item.startTime) + ")/",
                                                                END: "/Date(" + Date.parse(item.endTime) + ")/"
                                                            })
                                                        })
                                                    })
                                                    // console.log('###############');
                                                    //console.log(data);
                                                    //$.post("/MachineStatus/StatusDistributionMap/GetMachineStatusListByShift", JSON.stringify(data), function (data) {
                                                    if (data.Status == 0) {
                                                        $(".shift_" + ii + "_" + jj).remove();
                                                        for (var mm = data.Data.length - 1; mm >= 0; mm--) {
                                                            //插入tr
                                                            //console.log((data.Data.length - 1)-mm);
                                                            //console.log(data.Data[((data.Data.length - 1)-mm)].machineShiftDetailId);
                                                            $('<tr class="shift_' + ii + "_" + jj + '" id="shift_' + ii + "_" + jj + '"><td width="100" style="display:none;"></td><td width="100"></td><td width="20"></td><td><div id="shiftstatusList_' + ii + "_" + jj + '"></div></td><td width="20"><i class="fa fa-search" style="cursor:pointer;" machineShiftDetailId="' + data.Data[mm].machineShiftDetailId + '"macno="' + data.Data[ii].MacNo + '" date="' + data.Data[ii].Date + '" onclick="showShiftStatusDetail(this)"></i></td><td class="statusRate_' + ii + '" id="shiftstatusRate_' + ii + "_" + jj + "_" + mm + '" width="2"></td></tr>').insertAfter($(self).parent().parent());
                                                            //状态比例
                                                            var shtml = '<div style="display:none;position: absolute; right: 5px;border: 1px solid #000000; background-color: #FFFFFF;"><table class="statustable table-bordered">';
                                                            var tr1 = "<tr>"; var tr2 = "<tr>";
                                                            if (data.Data[mm].MyRateStatusList != null) {
                                                                for (var k = 0; k < data.Data[mm].MyRateStatusList.length; k++) {
                                                                    tr1 = tr1 + '<th align="center" colspan="2" style="text-align:center;background-color:' + data.Data[mm].MyRateStatusList[k].COLOR + '">' + data.Data[mm].MyRateStatusList[k].STATUS_NAME + '</th>';
                                                                    tr2 = tr2 + '<td align="center">&nbsp;' + data.Data[mm].MyRateStatusList[k].TIME + 'h&nbsp;</td><td align="center">&nbsp;' + (data.Data[mm].MyRateStatusList[k].RATE).toFixed(1) + '%&nbsp;</td>';
                                                                }
                                                                shtml = shtml + tr1 + tr2 + "</table></div>";
                                                                $("#shiftstatusRate_" + ii + "_" + jj + "_" + mm).append(shtml);
                                                            }
                                                            //状态条形图
                                                            $("#shift_" + ii + "_" + jj + " td").eq(1).append('<span id="shiftshowStatusRate_' + ii + "_" + jj + "_" + mm + '" class="badge badge-warning" style="cursor:pointer;">' + data.Data[mm].SHIFT + '  <i class="fa fa-hand-o-up" aria-hidden="true"></i></span>');
                                                            $("#shiftstatusList_" + ii + "_" + jj).viewChart({
                                                                width: $("#shiftstatusList_" + ii + "_" + jj).width() <= 720 ? 720 : $("#shiftstatusList_" + ii + "_" + jj).width(),
                                                                type: "shift",
                                                                url: "/MachineStatus/StatusDistributionMap/GetMachineStatusDetails",
                                                                shiftstartdate: data.Data[mm].START,
                                                                filter: $('input[name="filter"]').prop("checked"),
                                                                logic: '',
                                                                logicValue: $("#filterValue").val(),
                                                                three: false,//$('input[name="3dshow"]').prop("checked")
                                                                timedepth: moment(data.Data[mm].END).diff(moment(data.Data[mm].START), "seconds")
                                                            }).data("BZ-viewChart").drawStatus(data.Data[mm].StatusData);
                                                            //显示状态比例
                                                            $("#shiftshowStatusRate_" + ii + "_" + jj + "_" + mm).on("click", { i: ii, j: jj, m: mm }, function (event) {
                                                                //跟新位置
                                                                var top = $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).parent().offset().top;
                                                                $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().css("top", top + "px");
                                                                if ($("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().is(":hidden")) {
                                                                    $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().show('slide', { direction: 'right' }, 500);
                                                                }
                                                                else {
                                                                    $("#shiftstatusRate_" + event.data.i + "_" + event.data.j + "_" + event.data.m).children().hide('slide', { direction: 'right' }, 500);
                                                                }
                                                            });
                                                        }
                                                        //更新图标
                                                        $(self).attr("value", 1).addClass("fa-sort-desc").removeClass("fa-caret-right");
                                                    }
                                                    else {
                                                        BzAlert(data.Message);
                                                    }
                                                }).always(function () {
                                                    abp.ui.clearBusy();
                                                });
                                            }
                                            else {
                                                $(".shift_" + ii + "_" + jj).remove();
                                                //更新图标
                                                $(self).attr("value", 0).addClass("fa-caret-right").removeClass("fa-sort-desc");
                                            }
                                        });
                                    }
                                }
                            }
                        })
                }


            }
        }










    });

})();

function showStatusDetail(e) {

    var startDate = moment($(e).attr("date")).format('YYYY-MM-DD');
    // var endDate = moment(startDate).add('day', 1).format('YYYY/MM/DD');
    var machines = [parseInt($(e).attr("macno"))];
    //console.log(machines);
    detailModal.open(
        {
            shiftDay: startDate,
            machineId: machines[0]
        }
    )
}

function showShiftStatusDetail(e) {
    //console.log(e);

    var machines = $(e).attr("macno")
    var machineShiftDetailId = $(e).attr("machineShiftDetailId")
    //console.log(machines);
    detailModal.open(
        {
            machineShiftDetailId: machineShiftDetailId,
            machineId: machines,
            shif: true
        }
    )
}

function GetMachineStatusRatio(startDate, endDate, machines, ele, title, Isdetail) {
    var data = {
        MAC_NBR_LIST: machines,
        beginDate: startDate,
        endDate: endDate,
        Isdetail: Isdetail
    }
    $.post("/MachineStatus/StatusDistributionMap/GetSunStatusRate", JSON.stringify(data), function (data) {
        if (data.Status == 0) {
            //处理数据
            var td = [];
            var total = 0;
            for (var i = 0; i < data.Data[0].SUB_DURATION_LSIT.length; i++) {
                total = total + parseFloat((data.Data[0].SUB_DURATION_LSIT[i].Single_STATUS_RATE * 100).toFixed(1));
                td.push({
                    name: data.Data[0].SUB_DURATION_LSIT[i].STATUS_NAME,
                    color: data.Data[0].SUB_DURATION_LSIT[i].Color,
                    y: parseFloat((data.Data[0].SUB_DURATION_LSIT[i].Single_STATUS_RATE * 100).toFixed(1))
                });
            }
            if (total < 100) {
                td.push({
                    name: "null",
                    color: '#ffffff',
                    y: 100 - total
                });
            }
            drawRatio1(ele, title, td);
        }
        else {

        }
    });
}


