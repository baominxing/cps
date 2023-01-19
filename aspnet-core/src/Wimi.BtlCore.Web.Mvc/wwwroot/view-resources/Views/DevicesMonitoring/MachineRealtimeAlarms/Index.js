(function () {
    $(function () {
        $(".arrow").css("top",document.documentElement.clientHeight/2-40)
        $(window).resize(function() {
          $(".arrow").css("top",document.documentElement.clientHeight/2-40)
        });
        var $machineGroupTree = $(".machines-group-tree");
        var paramtersService = abp.services.app.paramters;
        var source = $("#alarm-template").html();
        var rendered = Handlebars.compile(source);
        var globalRequset = null;
        var selectTreeNode;
        var pageSize = 5;
        var pageNo = 1;
        var pageNum = 1;
        var totalCount = null;
        var groupTree = new MachinesTree();
        groupTree.initGroup($machineGroupTree,true);
        groupTree.setSelectGroupsTree();
        initSelectedGroupTagsData();

        Handlebars.registerHelper('renderImage',
            function (path) {

                var imagePath;
                if ($.trim(path).length === 0) {
                    imagePath = abp.appPath + 'Content/Images/CNC1-128x128.png';
                } else {
                    imagePath = abp.appPath + path;
                }

                return new Handlebars.SafeString('<img class="img-circle" src="' +imagePath +'" alt="device Avatar" style="height: 60px" >');
            });

        Handlebars.registerHelper('renderLoadMore',
            function (alarmItems) {
                if (alarmItems.length >= pageSize) {
                    return new Handlebars.SafeString(' <button class="btn btn-default btn-xs col-md-12 col-xs-12 loadMore">' + app.localize("LoadMore")+'</button>');
                }
            });

        //选择设备组
        $("#OpenTreeLayer").click(function () {
            var layindex = layer.open({
                type: 1,
                title: app.localize("ChooseDeviceGroup"),
                closeBtn: 1,
                shade: 0.6,
                shadeClose: false,
                area: '400px',
                btn: [app.localize("Confirm"), app.localize("Cancel")],
                yes: function () {
                    pageNum = 1;
                    $("#previous").addClass("showD");
                    $("#next").removeClass("showD");
                    initSelectedGroupTagsData();
                    layer.close(layindex);
                },
                content: $('.layer_notice').removeClass('hidden'), //捕获的元素
                end: function () {
                    $('.layer_notice').addClass('hidden');
                }
            });
        });

        $("#previous").click(function () {
                if(pageNum>1){
                    if(pageNum===2){
                        $("#previous").addClass("showD");
                        $("#next").removeClass("showD");
                    }else{
                        $("#previous").removeClass("showD");
                        $("#next").removeClass("showD");
                    }
                    pageNum--;
                    initSelectedGroupTagsData(pageNum);

                }
                
            });
            $("#next").click(function () {
                if (pageNum < Math.ceil(totalCount / app.consts.mchineRealtimeAlarms.pageSize)) {
                    if (pageNum === (Math.ceil(totalCount / app.consts.mchineRealtimeAlarms.pageSize) - 1)) {
                        $("#next").addClass("showD");
                        $("#previous").removeClass("showD");
                    }else{
                        $("#previous").removeClass("showD");
                        $("#next").removeClass("showD");
                    }
                    pageNum++;
                    initSelectedGroupTagsData(pageNum);
                }
                
            });

        //删除标签
        $("#SelectedGroupTags")
            .on("click",
                "span>.fa-times",
                function(e) {
                    var deleteNode = $(e.currentTarget).closest("span");
                    $machineGroupTree.jstree().deselect_node(deleteNode.data('groupid'));
                    deleteNode.remove();
                    initSelectedGroupTagsData();
                });

        //加载更多按钮
        $("#machineAlarmPanel")
            .on('click',
                '.loadMore',
                function(e) {
                    var targetbtn = $(e.currentTarget).closest("div.col-xs-12");
                    paramtersService.getLodingMoreAlarm({ PageSize: pageSize, PageNo: ++pageNo, MachineCode: targetbtn.data('code') })
                        .done(function(data) {
                            if (data.length > 0) {
                                var template = Handlebars.compile($("#alarmCommit").html());
                                $(e.currentTarget).before(template(data));
                            } else {
                                $(e.currentTarget).addClass('hidden');
                            }
                        });
                });

        //手动刷新数据
        $("#machineAlarmPanel")
            .on('click',
                '.refresh',
                function(e) {
                    pageNo = 1;
                    var targetbtn = $(e.currentTarget).closest("div.col-xs-12");
                    refreshData(targetbtn.data('code'));
                });

        //加载数据
        function initSelectedGroupTagsData() {
            selectTreeNode = groupTree.getSelectedGroupNode();
            var html = [];
            _.each(selectTreeNode,
                function (value) {
                    html.push('<span class="label label-warning xs-mr-5" data-groupId="' + value.id + '">' + $.trim(value.text) + ' <i class="fa fa-times" style="cursor: pointer"></i></span>');
                });
            $("#SelectedGroupTags").empty().append(html);
            $('.layer_notice').addClass('hidden');

            abp.ui.setBusy();
            if (globalRequset !== null) {
                globalRequset.reject();
            }
            if (selectTreeNode.length > 0) {
                refreshData();

            } else {
                abp.ui.clearBusy();
                $("#machineAlarmPanel").empty();
                $("#previous").addClass("showD");
                $("#next").addClass("showD");
            }
        }

        function getParamter(machineCode) {
            return {
                CurrentPageNo: pageNum,
                CurrentPageSize: app.consts.mchineRealtimeAlarms.pageSize,
                PageSize: pageSize, 
                PageNo: pageNo, 
                MachineCode: machineCode,
                GroupIdList: _.pluck(selectTreeNode, "id")
               
            };
        }

        function refreshData(machineCode) {
            var param = getParamter(machineCode);
            globalRequset= paramtersService
                .getRealTimeAlarmList(param)
                .done(function(data) {
                    totalCount=data.totalCount;
                    if((totalCount / app.consts.mchineRealtimeAlarms.pageSize)<=1){
                        $("#previous").addClass("showD");
                        $("#next").addClass("showD");
                    }
                    data=data.items
                    if (data) {
                        if (machineCode) {
                            $("#machine-" + machineCode+"").empty().append($(rendered(data)).find('div.widget-user-2')[0].outerHTML);
                        } else {
                            $("#machineAlarmPanel").html(rendered(data));
                        }

                        $(".widget-user-username").dotdotdot();                       
                        if (machineCode==undefined) {
                            machineCode = '';
                        }
                        _.each($("div[id*='machine-" + machineCode + "']"),
                            function (value) {
                                $(value).find('div.widget-user-desc').find('span.group').remove();
                                var data = $(value).data('groupid');
                                var groupIds = _.indexOf(data, ',') > -1 ? data.split(',') : [data];
                                _.each(groupIds,
                                    function(id) {
                                        var treeNodes = $machineGroupTree.jstree().get_node(id);
                                        var html =
                                            '<span class="group label bg-black" style="opacity:0.8">' +
                                                $.trim(treeNodes.text) +
                                                '</span>';

                                        $(value).find('div.widget-user-desc').append(html);
                                        $(".widget-user-desc").dotdotdot();
                                    });

                            });
                    }
                }).always(function () {
                    abp.ui.clearBusy();
                });
        }
    });
})();