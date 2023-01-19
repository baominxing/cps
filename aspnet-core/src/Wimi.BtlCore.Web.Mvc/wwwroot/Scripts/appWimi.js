//Make sure jQuery has been loaded before wimi.js
if (typeof jQuery === "undefined") {
    throw new Error("WIMI requires jQuery");
}

/* WIMI
 *
 * @type Object
 * @description $.WIMI is the main object for the WIMI web applications.
 *              It's used for implementing functions and options related
 *              to the app. Keeping everything wrapped in an object
 *              prevents conflict with other plugins and is a better
 *              way to organize our code.
 */
$.WIMI = {};

/* --------------------
 * - WIMI Options -
 * --------------------
 */
$.WIMI.options = {
    datatables: {
        defaultOptions: {
            searching: false,
            processing: true,
            serverSide: true,
            retrieve: true,
            destroy: true,
            scrollX: true,
            responsive: false,
            searchDelay: 800,
            order: [],
            language: {
                processing: "处理中...",
                lengthMenu: "每页显示 _MENU_ 项",
                zeroRecords: "没有匹配结果",
                info: "第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项",
                infoEmpty: "显示第 0 至 0 项结果，共 0 项",
                infoFiltered: "",
                infoPostFix: "",
                search: "搜索:",
                searchPlaceholder: "搜索...",
                url: "",
                emptyTable: app.localize("EmptyDataTable"),
                loadingRecords: "载入中...",
                infoThousands: ",",
                paginate: {
                    first: "首页",
                    previous: "上页",
                    next: "下页",
                    last: "末页"
                },
                aria: {
                    paginate: {
                        first: "首页",
                        previous: "上页",
                        next: "下页",
                        last: "末页"
                    },
                    sortAscending: "以升序排列此列",
                    sortDescending: "以降序排列此列"
                },
                decimal: "-",
                thousands: "."
            }
        },
        domWithoutSearch: '<"top">rt<"row"<"col-md-12"<"pull-left dt-page-info"i><"pull-right"p><"pull-left hidden-xs dt-page-lengh"l>>>',
        domWithSearch: '<"top"f>rt<"row"<"col-md-12"<"pull-left dt-page-info"i><"pull-right"p><"pull-left hidden-xs dt-page-lengh"l>>>'
    },
    dateRangePicker: {
        commonLocal: {
            "separator": " - ",
            "applyLabel": "确定",
            "cancelLabel": "取消",
            "fromLabel": "从",
            "toLabel": "至",
            "customRangeLabel": "自定义",
            "weekLabel": "周",
            "daysOfWeek": [
                "日",
                "一",
                "二",
                "三",
                "四",
                "五",
                "六"
            ],
            "monthNames": [
                "一月",
                "二月",
                "三月",
                "四月",
                "五月",
                "六月",
                "七月",
                "八月",
                "九月",
                "十月",
                "十一月",
                "十二月"
            ],
            "firstDay": 1
        },
        dateForamt: "YYYY-MM-DD",
        datetimeForamt: "YYYY-MM-DD HH:mm",
        ranges: {
            "今天": [moment().startOf("day"), moment()],
            '昨天': [moment().subtract(1, "days").startOf("day"), moment().subtract(1, "days").endOf("day")],
            '最近7天': [moment().subtract(6, "days").startOf("day"), moment()],
            '最近30天': [moment().subtract(29, "days").startOf("day"), moment()],
            '上个月': [moment().subtract(1, "month").startOf("month").startOf("day"), moment().subtract(1, "month").endOf("month").endOf("day")]
        }
    },
    echarsDefaultOption: {
        tooltip: {
            trigger: "axis"
        },
        legend: {
            top: "10px"
        },
        grid: {
            left: "20px",
            right: "35px",
            bottom: "30px",
            containLabel: true
        },
        toolbox: {
            feature: {
                dataView: { show: false },
                saveAsImage: {}
            }
        },
        dataZoom: {
            show: true,
            realtime: false,
            height: 15,
            bottom: "10px"
        }
    },
    checkSentinelLdk: {
        switchState: true,
        warningThreshold: 7//day,default 7;
    },
    select2ValidateOption: {
        errorPlacement: function (error, element) {
            error.appendTo(element.parent());
        }
    },
    validate: {
        errorElement: "span", //default input error message container
        errorClass: "help-block", // default input error message class
        focusInvalid: false, // do not focus the last invalid input
        ignore: "",
        highlight: function (element) {
            $(element).closest(".form-group").addClass("has-error");
        },

        success: function (label) {
            label.closest(".form-group").removeClass("has-error");
            label.remove();
        },

        errorPlacement: function (error, element) {
            if (element.closest(".input-icon").size() === 1) {
                error.insertAfter(element.closest(".input-icon"));
            } else {
                error.insertAfter(element);
            }
        },
        //重写showErrors
        showErrors: function (errorMap, errorList) {
            $.each(errorList, function (i, v) {
                layer.tips(v.message, v.element, {
                    time: 2000, tipsMore: true, tips: [3, '#d9534f']
                });
            });
        }
    }
};

$(function () {
    "use strict";
    //Easy access to options

});


/* ----------------------------------
 * return one of "xs"/"sm"/"md"/"lg" 
 * if any exceptions return null
 * ----------------------------------
 */
$.WIMI.getMediaTag = function () {
    var $target = $("#media-width");
    if ($target.length === 0) {
        console("no media-width dom, can get media tag via js");
        return null;
    }
    switch ($target.css("width")) {
        case "480px":
            return "xs";
        case "768px":
            return "sm";
        case "992px":
            return "md";
        default:
            return "lg";
    }
};

/* ----------------------------------
 * get echarts option combined with default setting 
 * ----------------------------------
 */
var _lineChartShowCount = app.consts.chartSetting.lineChartPageSize; //一页显示数
var _lineChartIndex = 0; //当前页数据起始位置
var _lineChartSeriesData = []; //所有折线
var _lineChartLegendData = []; //所有图例
var combinedOpts = null;
var chartInstance = null;


$.WIMI.echartOptionBuilder = function (opt, instance) {

    combinedOpts = $.extend(true, {}, $.WIMI.options.echarsDefaultOption, opt);
    chartInstance = instance;
    var totalBars = 0;

    var xAxisLength = combinedOpts.xAxis.data ? combinedOpts.xAxis.data.length : combinedOpts.xAxis[0].data.length;

    //全局设置柱状图柱条颜色为蓝色
    if (combinedOpts && combinedOpts.series.length > 0 && combinedOpts.series[0].type == 'bar') {
        for (var i = 0; i < combinedOpts.series.length; i++) {
            var serie = {
                itemStyle: {
                    normal: {
                        color: 'rgb(29,137,207)'
                    }
                }
            };
            $.extend(true, combinedOpts.series[i], serie);
        }

        totalBars = xAxisLength * combinedOpts.series.length;
    }

    if ($.WIMI.getMediaTag() !== "lg") {
        combinedOpts.dataZoom.realtime = false;
    }

    //折线图分页
    if (combinedOpts && combinedOpts.series.length > 0 && combinedOpts.series[0].type == 'line') {

        _lineChartIndex = 0;
        _lineChartSeriesData = combinedOpts.series;
        _lineChartLegendData = combinedOpts.legend.data;

        if (_lineChartLegendData.length <= _lineChartShowCount) {
            combinedOpts.legend.data = _lineChartLegendData;
            combinedOpts.series = _lineChartSeriesData;
            $("#legend_page").hide();
        }
        else {

            $("#legend_page").show();
            combinedOpts = updateChart(combinedOpts);
        }

        totalBars = xAxisLength;
    } else {
        $("#legend_page").hide();
    }

    var maxBarCount = app.consts.chartSetting.maxBarCount;

    if (totalBars > maxBarCount) {
        if (combinedOpts.dataZoom[0]) {
            combinedOpts.dataZoom[0].end = maxBarCount * 100 / totalBars;

        } else {
            combinedOpts.dataZoom.end = maxBarCount * 100 / totalBars;
        }

    }

    return combinedOpts;
};

$(function () {
    "use strict";
    //前一页
    $("#prePage").on("click",
        function () {
            if (_lineChartIndex == _lineChartLegendData.length) {
                _lineChartIndex -= _lineChartShowCount + _lineChartLegendData.length % _lineChartShowCount;
            }
            else {
                _lineChartIndex -= _lineChartShowCount * 2;
            }
            _lineChartIndex = _lineChartIndex < 0 ? 0 : _lineChartIndex;
            var option = updateChart(combinedOpts);
            chartInstance.setOption(option, {
                notMerge: true,
                lazyUpdate: false,
                silent: false
            });
        });

    //后一页
    $("#nextPage").on("click",
        function () {
            var option = updateChart(combinedOpts);
            chartInstance.setOption(option, {
                notMerge: true,
                lazyUpdate: false,
                silent: false
            });
        });
});
function updateChart(chartLineOption) {
    chartLineOption.series = [];
    chartLineOption.legend.data = [];
    if (_lineChartIndex == _lineChartLegendData.length) {
        _lineChartIndex = 0;
    }
    for (var n = 0; n < _lineChartShowCount; n++) {
        if (_lineChartIndex + n < _lineChartLegendData.length) {
            chartLineOption.legend.data.push(_lineChartLegendData[_lineChartIndex + n]);
            chartLineOption.series.push(_lineChartSeriesData.find(item => item.name === _lineChartLegendData[_lineChartIndex + n]));
        }
    }
    var totalPages = parseInt(_lineChartLegendData.length / _lineChartShowCount) + (_lineChartLegendData.length % _lineChartShowCount > 0 ? 1 : 0);
    $("#page_text").html(parseInt(_lineChartIndex / _lineChartShowCount) + 1 + "/" + totalPages);

    _lineChartIndex += chartLineOption.series.length;

    return chartLineOption;
    //chartInstance.setOption($.WIMI.echartOptionBuilder(chartLineOption), {
    //    notMerge: true,
    //    lazyUpdate: false,
    //    silent: false
    //});
}

$(function () {
    "use strict";
    //Easy access to options
    var o = $.WIMI.options;

    $("#fullscreen").click(function () {
        $(document).toggleFullScreen();
        $(this).find("i").toggleClass("fa-compress");
    });

    //Setting Datatables option
    //$.extend($.fn.dataTable.defaults, o.datatables.defaultOptions);

    $(document).on("click", ".checkbox-all", function () {

        var $self = $(this);
        $self.closest(".dataTables_wrapper").find(".checkbox-item").prop("checked", $self.prop("checked"));
    });
});

/* 
* Wapper datatables 
* ----------------------- 
* @usage $table.WIMIDataTable( options ); 
* entirely datatables options 
*/
(function ($) {

    "use strict";
    /**
     * Wimi DataTable
     * @param {
     *  order:[[2, "asc"]],默认按第2列排序，如果传空 [],需要再dto中继承IShouldNormalize并重新制定默认排序字段
     * 
     * } opts 
     * @returns {} 
     */
    $.fn.WIMIDataTable = function (opts) {

        var options = $.extend({}, $.WIMI.options.datatables.defaultOptions, opts);

        if (opts.aLengthMenu == null) {
            var pageSizeOptionStr = abp.setting.values["App.Page.PageSizeOptions"].split(",");
            var pageSizeOptions = _.map(pageSizeOptionStr,
                function (item) {
                    return parseInt(item);
                });

            if (opts.pageLength == null) {
                options.pageLength = pageSizeOptions[0];
            }

            pageSizeOptions = pageSizeOptions.sort(function (a, b) { return a - b });
            options.aLengthMenu = [pageSizeOptions, pageSizeOptions];
        }

        if (options.ajax != null) {

            options.ajax = options.ajax || {};
            options.ajax.type = options.ajax.type || "POST";
            options.ajax.dataType = options.ajax.dataType || "json";
            options.ajax.contentType = options.ajax.contentType || "application/json";

            if (!options.ajax.dataFilter) {

                options.ajax.dataFilter = function (data) {
                    var json = $.parseJSON(data);
                    if (json.result === null) {
                        return JSON.stringify({ "recordsTotal": 0, "recordsFiltered": 0, "draw": 0, "items": [] });
                    }

                    json.result.data = json.result.items;
                    return JSON.stringify(json.result);
                };
            }
        }

        if (!options.dom) {

            if (options.searching) {
                options.dom = $.WIMI.options.datatables.domWithSearch;
            } else {
                options.dom = $.WIMI.options.datatables.domWithoutSearch;
            }
        }

        if (options.columns && options.responsive) {
            //add responsive controler
            options.columns.unshift({
                className: "control",
                orderable: false,
                defaultContent: ""
            });
        }

        return this.DataTable(options);
    };
})(jQuery);

/*
 * DT DROPDOWN ACTION BUTTONS CUSTOM PLUGIN
 * -----------------------
 * This plugin depends on Datatables/bootstrap/[Bootstrap Hover Dropdown] plugins
 *
 * @type plugin
 * @usage $td.buildDTDropdownActionButtons( buttonList );
 * buttonList : [ {title, clickEvent, isShow}]
 */
(function ($) {

    "use strict";

    $.fn.buildDTDropdownActionButtons = function (buttonList) {

        return this.each(function () {

            var $this = $(this), needShowButtonCnt = false;

            //check data
            if (!buttonList || !(buttonList instanceof Array)) {
                throw new Error("You should pass array of button list; $(td).buildDTDropdownActionButtons(buttonList)");
            }

            var $popoverContent = $("<div>");

            //fill popover content via pass button list
            $.each(buttonList,
                function (index, button) {
                    button.title = button.title || "Not Defined";
                    button.className = button.className || "btn-default";

                    if (!button.clickEvent || typeof button.clickEvent !== "function") {
                        button.clickEvent = function () { alert("Please Define Click Event") };
                    }

                    if (button.isShow) {
                        $('<button class="btn ' + button.className + ' btn-xs">' + button.title + '</button>')
                            .appendTo($popoverContent)
                            .click(button.clickEvent);

                        needShowButtonCnt = true;
                    }
                });

            //if no button need to show, return without trigger popover
            if (!needShowButtonCnt) {
                return;
            }

            var $buttonGroupCnt = $(
                '<div class="btn-group">' +
                '<button type="button" class="btn btn-default btn-xs" role="button" data-trigger="focus">' +
                '<i class="fa fa-lg fa-chevron-circle-down"></i>' +
                "</button>" +
                "</div>");

            $buttonGroupCnt.popover({
                container: 'body',
                trigger: 'focus',
                placement: 'bottom',
                html: true,
                template: '<div class="popover table-actions" role="tooltip"><div class="arrow"></div><div class="popover-content"></div></div>',
                content: function () {
                    return $popoverContent;
                }
            });

            $buttonGroupCnt.on('click', function () {
                $this.popover('show');
            });

            $buttonGroupCnt.appendTo($this);
        });
    };

    $.fn.buildActionButtons = function (buttonList) {

        return this.each(function () {

            var $this = $(this), needShowButtonCnt = false;

            //check data
            if (!buttonList || !(buttonList instanceof Array)) {
                throw new Error("You should pass array of button list; $(td).buildDTDropdownActionButtons(buttonList)");
            }

            var $actionContent = $("<div class='action-content'>");

            //fill popover content via pass button list
            $.each(buttonList,
                function (index, button) {
                    button.title = button.title || "Not Defined";
                    button.className = button.className || "btn-default";

                    if (!button.clickEvent || typeof button.clickEvent !== "function") {
                        button.clickEvent = function () { alert("Please Define Click Event") };
                    }

                    if (button.title === "删除") {
                        button.className = "btn-danger btn-xs";
                    }

                    if (button.isShow) {
                        $('<button class="btn ' + button.className + ' btn-xs">' + button.title + '</button>')
                            .appendTo($actionContent)
                            .click(button.clickEvent);

                        $actionContent.append("&nbsp;");
                        needShowButtonCnt = true;
                    }
                });

            //if no button need to show, return without trigger popover
            if (!needShowButtonCnt) {
                return;
            }

            $actionContent.appendTo($this);
        });
    };
})(jQuery);

/* 
* Wapper daterangepicker 
* ----------------------- 
* @usage $input.WIMIDaterangepicker( options ); 
* entirely daterangepicker options 
*/
(function ($) {

    "use strict";

    $.fn.WIMIDaterangepicker = function (opt) {
        var defaults = {
            "maxDate": moment(),
            "minDate": moment("2016-01-01"),
            "autoApply": true,
            "locale": $.WIMI.options.dateRangePicker.commonLocal,
            "ranges": $.WIMI.options.dateRangePicker.ranges,
            "linkedCalendars": false,
            "showWeekNumbers": true
        };
        var options = $.extend({}, defaults, opt);

        if (options.timePicker && !options.locale.format) {
            options.locale.format = $.WIMI.options.dateRangePicker.datetimeForamt;
        } else if (!options.timePicker && !options.locale.format) {
            options.locale.format = $.WIMI.options.dateRangePicker.dateForamt;
        }

        if (app.consts.fixedCalendar.enabled) {
            if (options.endTimeOnly) {
                options.startDate = app.consts.fixedCalendar.endTime;
            } else {
                options.startDate = app.consts.fixedCalendar.startTime;
                options.endDate = app.consts.fixedCalendar.endTime;
            }
        }

        return this.daterangepicker(options);
    };
})(jQuery);

/*
 * @param(grid高度/宽度，结果集中一个组的数据量，结果集的总数据量，柱条最小宽度)
 * 若无分组则oneSeriessLength为1；柱条最小宽度为字体大小，默认为12
 * -------------------------------------------------------------------------------
 * example
 *     see in index.js of TimeStatistics
 */
function InitdataZoom(gridHeightorWidth, oneSeriesLength, totalNum, minBarWidth) {
    minBarWidth = minBarWidth | 12;
    var totalSeriesNum = totalNum / oneSeriesLength;
    var percent;
    var maxSeries = 9 * gridHeightorWidth / (12 * oneSeriesLength * minBarWidth - 2 * minBarWidth);
    var maxSeriesNum = parseInt(maxSeries);
    if (maxSeries >= totalSeriesNum) {
        return 100;
    } else {
        var percent = (2 * maxSeriesNum - 1) / (2 * totalSeriesNum - 2) * 100 - 0.00001;
        return percent;
    }
} (jQuery);

$.validator.addMethod("checkSpace", function (value, element) {
    //验证输用户入不能为空格
    if (value.trim() === "") {
        return false;
    } else {
        return true;
    }
}, "不能为纯空格");

$.validator.addMethod("checkNum", function (value, element) {
    var exj = new RegExp("^[0-9]*[1-9][0-9]*$");
    if (exj.test(value)) {
        return true;
    } else {
        return false;
    }

}, "请输入大于0的整数");