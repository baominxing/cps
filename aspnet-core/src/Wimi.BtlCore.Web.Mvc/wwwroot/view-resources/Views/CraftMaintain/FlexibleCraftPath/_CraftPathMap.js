(function() {
    $(function () {
        var craftMaintainService = abp.services.app.flexibleCraftMaintain;

        var viewCraftPathMap = function (craftId) {
            craftMaintainService.getCraftPathMapData({ id: craftId }).done(function (response) {
                console.log(response);
                try {
                    var $chartDom = $("#craftPathChart").get(0);
                    var chartInstance = window.echarts.getInstanceByDom($chartDom);
                    if (chartInstance) {
                        window.echarts.dispose($chartDom);
                    }
                    chartInstance = window.echarts.init($chartDom);

                    echarts.util.each(response.children, function (datum, index) {
                        index !== 0 && (datum.collapsed = true);
                    });

                    chartInstance.setOption({
                        tooltip: {
                            trigger: 'item',
                            triggerOn: 'mousemove'
                        },
                        series: [
                            {
                                type: 'tree',
                                data: [response],
                                top: '1%',
                                left: '10%',
                                bottom: '1%',
                                right: '20%',
                                symbolSize: 10,
                                initialTreeDepth: 6,
                                label: {
                                    normal: {
                                        position: 'left',
                                        verticalAlign: 'middle',
                                        align: 'right',
                                        fontSize: 12
                                    }
                                },

                                leaves: {
                                    label: {
                                        normal: {
                                            position: 'right',
                                            verticalAlign: 'middle',
                                            align: 'left'
                                        }
                                    }
                                },

                                expandAndCollapse: true,
                                animationDuration: 550,
                                animationDurationUpdate: 750
                            }
                        ]
                    });
                } catch (e) {
                    console.log(e);
                }
            });
        };

        var initPage = function () {
            var productId = $("#productId").val();
            craftMaintainService.getAllCrafts({ ProductId: productId }).done(function (response) {
                if (response.length) {
                    for (var i = 0; i < response.length; i++) {
                        var item = response[i];
                        debugger
                        $('<li"><input type="hidden" value="' + item.id + '"/><a href="#"><i class="fa fa-circle-o text-light-blue"></i> ' + item.name + '</a></li>')
                            .appendTo("#craftList .nav")
                            .on("click", function () {
                                var $craftId = $(this).find("input[type=hidden]");
                                var craftId = $craftId.val();
                                viewCraftPathMap(craftId);
                                $(this).addClass("active")
                                    .siblings()
                                    .removeClass("active");
                            });
                    }

                    $("#craftList .nav li").get(0).click();
                }
                else {
                    $('<p class="text-muted well well-sm no-shadow" style="margin: 10px;">没有数据</p>')
                        .appendTo("#craftList .nav");
                }
            });
        };

        initPage();
    });
})();