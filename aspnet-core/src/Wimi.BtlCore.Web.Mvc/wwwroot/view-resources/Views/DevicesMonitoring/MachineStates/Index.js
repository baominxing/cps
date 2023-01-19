(function () {
    $(function () {
        $(".arrow").css("top", document.documentElement.clientHeight / 2 - 40);
        $(window).resize(function () {
            $(".arrow").css("top", document.documentElement.clientHeight / 2 - 40);
        });
        var paramterService = abp.services.app.paramters;
        var $machineGroupTree = $(".machines-group-tree");
        var source = $("#entry-template").html();
        var rendered = Handlebars.compile(source);

        var source1 = $("#entry-template1").html();
        var rendered1 = Handlebars.compile(source1);

        var source2 = $("#entry-template2").html();
        var rendered2 = Handlebars.compile(source2);

        var source3 = $("#entry-template3").html();
        var rendered3 = Handlebars.compile(source3);

        var source4 = $("#entry-template4").html();
        var rendered4 = Handlebars.compile(source4);

        var mediaTag = $.WIMI.getMediaTag();
        var timer = null;
        var globalRequset = null;
        var $grid = null;
        var pageNum = 1;
        var totalCount = null;

        var groupTree = new MachinesTree();
        groupTree.init($(".machines-tree"), true);
        groupTree.initGroup($machineGroupTree, true);
        groupTree.setSelectAll();
        groupTree.setMachineTreeOpenAll();

        initSelectedGroupTagsData();

        Handlebars.registerHelper('addBorderRight',
            function (index) {
                index = index + 1;
                if (index % 4 === 0) {
                    return new Handlebars.SafeString('class = "col-xs-6 col-sm-3"');
                }

                if ((mediaTag === "sm" || mediaTag === "xs") && index % 2 === 0) {
                    return new Handlebars.SafeString('class = "col-xs-6 col-sm-3"');
                }

                return new Handlebars.SafeString('class = "col-xs-6 col-sm-3 border-right"');
            });
        Handlebars.registerHelper('renderImage',
            function (path) {

                var imagePath;
                if ($.trim(path).length === 0) {
                    imagePath = abp.appPath + 'Content/Images/CNC1-128x128.png';
                } else {
                    imagePath = abp.appPath + path;
                }
                return new Handlebars.SafeString('<img class="img-circle" src="' +
                    imagePath + '" alt="device Avatar" style="height: 70px" />');
                // return new Handlebars.SafeString(imagePath);
            });


        $("#SelectedGroupTags")
            .on("click",
                "span>.fa-times",
                function (e) {
                    var deleteNode = $(e.currentTarget).closest("span");
                    $machineGroupTree.jstree().deselect_node(deleteNode.data('groupid'));
                    deleteNode.remove();
                    initSelectedGroupTagsData();
                });

        $("#OpenTreeLayer")
            .click(function () {
                var layindex = layer.open({
                    type: 1,
                    title: app.localize("PleaseSelectEquipment"),
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
            if (pageNum > 1) {
                if (pageNum === 2) {
                    $("#previous").addClass("showD");
                    $("#next").removeClass("showD");
                } else {
                    $("#previous").removeClass("showD");
                    $("#next").removeClass("showD");
                }
                pageNum--;
                initSelectedGroupTagsData(pageNum);

            }

        });
        $("#next").click(function () {
            if (pageNum < Math.ceil(totalCount / app.consts.machineStates.pageSize)) {
                if (pageNum === (Math.ceil(totalCount / app.consts.machineStates.pageSize) - 1)) {
                    $("#next").addClass("showD");
                    $("#previous").removeClass("showD");
                } else {
                    $("#previous").removeClass("showD");
                    $("#next").removeClass("showD");
                }
                pageNum++;
                initSelectedGroupTagsData(pageNum);
            }

        });
        //初始化并加载选中的数据
        function initSelectedGroupTagsData(x) {
            $('.layer_notice').addClass('hidden');
            if (timer !== null) {
                clearTimeout(timer);
            }
            abp.ui.setBusy();
            if (globalRequset !== null) {
                globalRequset.reject();
            }

            var selectedIds = [];
            if (groupTree.getQueryType() === "0") {
                selectedIds = groupTree.getSelectedMachineIds();
            } else {
                selectedIds = groupTree.getSelectedGroupIds();
            }
            if (selectedIds.length > 0) {
                refreshData(selectedIds);
                timer = setInterval(function () {
                    refreshDataWord(selectedIds);
                },
                    5000);
            } else {
                abp.ui.clearBusy();
                $(".machines-cnt").empty();
                $("#previous").addClass("showD");
                $("#next").addClass("showD");
            }
        }

        var dotdotdotOption = {
            callback: function (isTruncated) {

                if (isTruncated) {
                    var _this = this;

                    $(_this).on("click", function () {

                        layer.open({
                            title: '详细内容'
                            , content: $(_this).attr("value")
                        });

                    });
                }
            },
            ellipsis: "\u2026 查看更多",
            height: 50,
            keep: null,
            tolerance: 0,
            truncate: "word",
            watch: "window",
        }

        //第一次刷新数据
        function refreshData(deviceGroupIds) { 
            var param = getParamter(deviceGroupIds); 
            globalRequset = paramterService.getMachineStatusDetailList(param)
                .done(function (data) {
                    totalCount = data.totalCount;
                    if ((totalCount / app.consts.machineStates.pageSize) <= 1) {
                        $("#previous").addClass("showD");
                        $("#next").addClass("showD");
                    }
                    data = data.items;
                    console.log(data);

                    if (data) {
                        $(".machines-cnt-first").html(rendered(data));

                        $(".ellipsis").dotdotdot();
                        _.each($("div[id*='machine-']"),
                            function (value) {
                                var data = $(value).data('groupid');
                                var groupIds = _.indexOf(data, ',') > -1 ? data.split(',') : [data];
                                _.each(groupIds,
                                    function (id) {
                                        var treeNodes = $machineGroupTree.jstree().get_node(id);
                                        var html = '<i class="fa fa-sitemap"> ' + $.trim(treeNodes.text) + '</i>';
                                        $(value).find('div.group-tags').append(html);
                                    });
                                $(".group-ellipsis").dotdotdot();

                                $(".description-header").dotdotdot(dotdotdotOption);
                            });
                        //注意：destory 
                        if ($grid != null) {
                            $grid.masonry('destroy');
                        }
                        $grid = $('.machines-cnt-first')
                            .masonry({
                                itemSelector: '.col-xs-12 .col-sm-4',
                                columnWidth: 0,
                                isAnimated: true
                            });
                    }
                }).always(function () {
                    abp.ui.clearBusy();
                });
        }

        //刷新数据 局部刷
        function refreshDataWord(deviceGroupIds) {
            var param = getParamter(deviceGroupIds);
          
            globalRequset = paramterService.getMachineStatusDetailList(param)
                .done(function (data) {
                    totalCount = data.totalCount;
                    data = data.items;
                    if (data) {
                        var ee = $(window).scrollTop();

                        var dd = data;
                        $(".ellipsis").dotdotdot();
                        _.each($("div[id*='machine-']"),
                            function (value, index) {

                                $(value).find('.widget-device-far').html(rendered1(dd[index]));
                                $(value).find('.box-footer1').html(rendered2(dd[index]));

                                $(value).find('.box-footer2').html(rendered3(dd[index]));
                                $(value).find('.box-footer3').html(rendered4(dd[index]));

                                var data = $(value).data('groupid');
                                var groupIds = _.indexOf(data, ',') > -1 ? data.split(',') : [data];
                                _.each(groupIds,
                                    function (id) {
                                        var treeNodes = $machineGroupTree.jstree().get_node(id);
                                        var html = '<i class="fa fa-sitemap">  ' + $.trim(treeNodes.text) + '</i>';
                                        $(value).find('div.group-tags').append(html);
                                    });
                                $(".group-ellipsis").dotdotdot();

                                $(".description-header").dotdotdot(dotdotdotOption);
                            });
                        //注意：destory 
                        if ($grid != null) {
                            $grid.masonry('destroy');
                        }
                        $grid = $('.machines-cnt')
                            .masonry({
                                itemSelector: '.col-xs-12 .col-sm-4',
                                columnWidth: 0,
                                isAnimated: true
                            });
                        $(window).scrollTop(ee);
                    }
                }).always(function () {
                    abp.ui.clearBusy();
                });
        }


        function getParamter(selectedIds) {
            var stateCodes = []; 

            $.each($('#StateContainer').find('input:checkbox:checked'), function (i, e) {
                stateCodes.push($(e).val());
            });

            return {
                PageNo: pageNum,
                PageSize: app.consts.machineStates.pageSize,
                DeviceIdGroupIds: selectedIds,
                MachineIds: selectedIds,
                Type: groupTree.getQueryType() * 1.0,
                StateCodes : stateCodes
            };
        }

        function initStateInfo() {
            if (!stateCode) {
                $('#StateContainer').hide();
                return;
            }

            paramterService.listMachineStates().done(function (response) { 
                for (var i = 0; i < response.length; i++) {
                    $('<div class="checkbox-inline"><input type="checkbox" class="flat-red" name="state" value="' + response[i].code + '"><span class="text-white" style="background-color: ' + response[i].hexcode + ' !important;"> ' + response[i].displayName + '</span></div>').appendTo('#StateContainer');
                }

                $('input').on('ifChanged',
                        function() {
                            initSelectedGroupTagsData(1);
                        })
                    .iCheck({
                        checkboxClass: 'icheckbox_minimal-red'
                    })
                    .each(function(i, e) {
                        if ($(e).val() === stateCode) {
                            $(e).iCheck('check');
                        }
                    });

            });
        }

        initStateInfo();
    });
})();