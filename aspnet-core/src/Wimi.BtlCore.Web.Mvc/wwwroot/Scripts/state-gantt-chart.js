; (function ($, window, document, d3, _, undefined) {

    $.fn.StateGanttChart = function (options) {

        var defaultSettings = {
            'stateHeight': 14,
            'reasonHeight': 6,
            'tooltipHeight': 14,
            'isDateOnlyFormat': false,
            'showTopXAxis': false,
            'showTitle': false,
            'titleWidth': 100,
            'xScaleHeight': 20,
            'data': []
        };

        var hPadding = 4;
        var settings = $.extend({}, defaultSettings, options);
        var dataset = settings.data;

        function _getMessages(obj) {
            return Object.values(obj).join(' ');
        }

        function _containsReason() {
            return this.reason !== undefined;
        }

        function _getSVGHeight(targetData) {
            var height = 0;

            var showXAxis = targetData.showXAxis
            var innerIntervals = targetData.intervals;

            for (var j = 0; j < innerIntervals.length; j++) {

                if (showXAxis) {
                    height += settings.xScaleHeight;
                }

                height += settings.stateHeight + hPadding;

                if (_containsReason.call(innerIntervals[j])) {
                    height += settings.reasonHeight;
                }
            }
            return height;
        }

        // d3-tooltip
        var div = d3.select('body')
            .append('div')
            .style('opacity', 0)
            .attr('id', 'd3_tooltip');;


        return this.each(function () {
            var $this = $(this), isDateOnlyFormat = settings.isDateOnlyFormat;
            var targetData, startDate, endDate, showXAxis;
            var svgWidth = $this.width();
            //if show xAxis for each intervals
            var showTopXAxis;
            //if show title for each machine
            var showTitle = settings.showTitle ? true : false;
            //calculate the value of transform y distance
            this.totalTransformDistance = 0;
            //machine id
            var thisKey = $this.data('key');
            if (thisKey === undefined) {
                /** html element should contains data-key attribute
                 *  we use the key to filter data from dataset
                */
                throw "element should has data-key which bind with machine-id";
            }

            /** get data by machine id via html element data-key attribute */
            targetData = _.find(settings.data, function (item) {
                return item.id == thisKey
            });

            if (targetData === undefined) {
                console.log("no corresponding data exist,id:" + thisKey);
                return;
            }

            if (!targetData.intervals) { throw "should contains at least one interval"; return; }

            showTopXAxis = targetData.showXAxis ? true : false;

            this.gDataId = 'g_data_' + thisKey;
            this.svg = d3.select(this)
                .append('svg')
                .attr('width', svgWidth + 'px')
                .attr('height', _getSVGHeight(targetData))
                .append('g');

            var g = this.svg.append('g').attr('id', this.gDataId);

            //show title, need transform and give title space to fill title text
            if (showTitle) {

                //g.attr('transform', function (d, i) {
                //    return 'translate(' + settings.titleWidth + ',0)';
                //});

                this.svg.selectAll('text')
                    .data([targetData])
                    .enter()
                    .append('text')
                    .attr('x', 0)
                    .attr('y', function () {
                        var defaultHeight = settings.stateHeight + settings.reasonHeight + hPadding
                        return (showTopXAxis ? (settings.xScaleHeight + defaultHeight)
                            : defaultHeight) / 2;
                    })
                    .attr('transform', function (d, i) {
                        return 'translate(0,0)';
                    })
                    .attr('class', 'ytitle')
                    .text(function (d) {
                        return d.name;
                    });
            }


            _.each(targetData.intervals, function (thisData, index, list) {

                //thisData = intervals[index]
                var containsReason = _containsReason.call(thisData);
                var height = hPadding + (containsReason ? settings.stateHeight + settings.reasonHeight : settings.stateHeight);
                var width = $this.width();
                var parseDate = d3.time.format('%Y-%m-%d');
                var parseDateTime = d3.time.format('%Y-%m-%d %H:%M:%S');
                var parseDateRegEx = new RegExp(/^\d{4}-\d{2}-\d{2}$/);
                var parseDateTimeRegEx = new RegExp(/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/);
                var totalTransformDistance = this.totalTransformDistance;

                startDate = moment(thisData.datetime[0]);
                endDate = moment(thisData.datetime[1]);

                // define Xscales
                var startPoint = (settings.showTitle ? settings.titleWidth : 0);

                var xScale = d3.time.scale()
                    .domain([startDate, endDate])
                    .range([startPoint, width])
                    .clamp(true);

                var oneDataArray = [thisData];
                //check dataset
                oneDataArray.forEach(function (d) {
                    d.state.forEach(function (d1) {
                        //检查时间格式
                        if (!(d1.startDatetime instanceof Date)) {
                            if (parseDateRegEx.test(d1.startDatetime)) {
                                //不包含时间格式
                                d1.startDatetime = parseDate.parse(d1.startDatetime);
                            } else if (parseDateTimeRegEx.test(d1.startDatetime)) {
                                //含有时间格式
                                d1.startDatetime = parseDateTime.parse(d1.startDatetime);
                                isDateOnlyFormat = false;
                            } else {
                                //时间格式错误
                                throw new Error('时间格式错误. 请用 \'YYYY-MM-DD\' 或 ' +
                                    '\'YYYY-MM-DD HH:MM:SS\' 的格式.');
                            }

                            //获取时间段结束时间
                            if (parseDateRegEx.test(d1.endDatetime)) {
                                //不包含时间格式
                                d1.endDatetime = parseDate.parse(d1.endDatetime);
                            } else if (parseDateTimeRegEx.test(d1.endDatetime)) {
                                //含有时间格式
                                d1.endDatetime = parseDateTime.parse(d1.endDatetime);
                            } else {
                                throw new Error('时间格式错误. 请用 \'YYYY-MM-DD\' 或 ' +
                                    '\'YYYY-MM-DD HH:MM:SS\' 的格式.');
                            }
                        }
                    });
                    if (containsReason) {
                        d.reason.forEach(function (d1) {
                            //检查时间格式
                            if (!(d1.startDatetime instanceof Date)) {
                                if (parseDateRegEx.test(d1.startDatetime)) {
                                    //不包含时间格式
                                    d1.startDatetime = parseDate.parse(d1.startDatetime);
                                } else if (parseDateTimeRegEx.test(d1.startDatetime)) {
                                    //含有时间格式
                                    d1.startDatetime = parseDateTime.parse(d1.startDatetime);
                                    isDateOnlyFormat = false;
                                } else {
                                    //时间格式错误
                                    throw new Error('时间格式错误. 请用 \'YYYY-MM-DD\' 或 ' +
                                        '\'YYYY-MM-DD HH:MM:SS\' 的格式.');
                                }

                                //获取时间段结束时间
                                if (parseDateRegEx.test(d1.endDatetime)) {
                                    //不包含时间格式
                                    d1.endDatetime = parseDate.parse(d1.endDatetime);
                                } else if (parseDateTimeRegEx.test(d1.endDatetime)) {
                                    //含有时间格式
                                    d1.endDatetime = parseDateTime.parse(d1.endDatetime);
                                } else {
                                    // throw new Error('时间格式错误. 请用 \'YYYY-MM-DD\' 或 ' +
                                    // '\'YYYY-MM-DD HH:MM:SS\' 的格式.');
                                }
                            }
                        });
                    }
                });

                //draw xAxis
                if (showTopXAxis) {

                    var xAxis = d3.svg.axis()
                        .scale(xScale);
                    var xaxisId = this.gDataId + '_g_axis_' + index;
                    this.svg.append('g').attr('id', xaxisId);

                    this.svg.select('#' + xaxisId).append('g')
                        .attr('class', 'axis')
                        .attr('transform', function (d, i) {
                            return 'translate(20,' + (totalTransformDistance) + ')';
                        })
                        .call(xAxis.orient('button'));
                }

                //add trans distance
                this.totalTransformDistance += showTopXAxis ? settings.xScaleHeight : 0;
                totalTransformDistance = this.totalTransformDistance;

                var s = this.svg.select('#' + this.gDataId).selectAll('.g_data_' + index)
                    .data(oneDataArray)
                    .enter()
                    .append('g')
                    .attr('transform', function (d, i) {
                        return 'translate(0,' + (totalTransformDistance) + ')';
                    })
                    .attr('class', 'state');

                //add trans distance
                this.totalTransformDistance += settings.stateHeight + hPadding / 2;
                totalTransformDistance = this.totalTransformDistance;

                // add data series
                s.selectAll('rect')
                    .data(function (d) {
                        return d.state;
                    })
                    .enter()
                    .append('rect')
                    .attr('x', function (d) {
                        return xScale(d.startDatetime);
                    })
                    .attr('y', 1)
                    .attr('width', function (d) {
                        return (xScale(d.endDatetime) - xScale(d.startDatetime));
                    })
                    .attr('height', settings.stateHeight)
                    .attr('fill', function (d) {
                        return d.color;
                    })
                    .on('mouseover', function (d, i) {
                        var matrix = this.getScreenCTM().translate(+this.getAttribute('x'), +this.getAttribute('y'));
                        var overflow = matrix.e > (document.body.clientWidth - 300);
                        var tooltipClass = overflow ? "d3-tooltip right" : "d3-tooltip left";
                        div.transition()
                            .duration(200)
                            .attr('class', tooltipClass)
                            .style("border-color", d.color)
                            .style('opacity', 0.8);

                        div.html(function () {
                            var output = '<i class="glyphicon glyphicon-stop" style="color:' + d.color + '"></i><strong> ' + d.displayName + ' </strong>';

                            if (isDateOnlyFormat) {
                                if (d.endDatetime > d3.time.second.offset(d.startDatetime, 86400)) {
                                    output += moment(parseDate(d.startDatetime)).format('l')
                                        + ' - ' + moment(parseDate(d.endDatetime)).format('l');
                                }
                                output += moment(parseDate(d.startDatetime)).format('l');
                            } else {
                                if (d.endDatetime > d3.time.second.offset(d.startDatetime, 86400)) {
                                    output += moment(d.startDatetime).format('hh:mm:ss') + ' '
                                        + moment(d.startDatetime).format('hh:mm:ss') + ' - '
                                        + moment(d.endDatetime).format('hh:mm:ss') + ' '
                                        + moment(d.endDatetime).format('hh:mm:ss');
                                }
                                output += moment(d.startDatetime).format('hh:mm:ss') + ' - '
                                    + moment(d.endDatetime).format('hh:mm:ss');
                            }

                            return output += ' ' + _getMessages(d.message);
                        })
                            .style('left', function () {
                                var left = window.pageXOffset + matrix.e;
                                if (overflow) {
                                    left -= 200;
                                }
                                return left + 'px';
                            })
                            .style('top', function () {
                                return window.pageYOffset + matrix.f - settings.tooltipHeight + 'px';
                            })
                            .style('height', settings.tooltipHeight + 'px');
                    })
                    .on('mouseout', function () {
                        div.transition()
                            .duration(500)
                            .style('opacity', 0);
                    });

                if (containsReason) {
                    var r = this.svg.select('#' + this.gDataId).selectAll('.g_reason')
                        .data(oneDataArray)
                        .enter()
                        .append('g')
                        .attr('transform', function (d, i) {
                            return 'translate(0,' + totalTransformDistance + ')';
                        })
                        .attr('class', 'reason');

                    this.totalTransformDistance += settings.reasonHeight + hPadding / 2;

                    r.selectAll('rect')
                        .data(function (d) {
                            return d.reason;
                        })
                        .enter()
                        .append('rect')
                        .attr('x', function (d) {
                            return xScale(d.startDatetime);
                        })
                        .attr('y', 0)
                        .attr('width', function (d) {
                            return xScale(d.endDatetime) - xScale(d.startDatetime);
                        })
                        .attr('height', settings.reasonHeight)
                        .attr('fill', function (d) {
                            return d.color;
                        })
                        .on('mouseover', function (d, i) {
                            var matrix = this.getScreenCTM().translate(+this.getAttribute('x'), +this.getAttribute('y'));
                            var overflow = matrix.e > (document.body.clientWidth - 300);
                            var tooltipClass = overflow ? "d3-tooltip right" : "d3-tooltip left";

                            div.transition()
                                .duration(200)
                                .attr('class', tooltipClass)
                                .style('border-color', d.color)
                                .style('opacity', 0.8);

                            div.html(function () {
                                var output = '<i class="glyphicon glyphicon-stop" style="color:' + d.color + '"></i><strong> ' + d.displayName + ' </strong>';

                                if (isDateOnlyFormat) {
                                    if (d.endDatetime > d3.time.second.offset(d.startDatetime, 86400)) {
                                        output += moment(d.startDatetime).format('l')
                                            + ' - ' + moment(d.endDatetime).format('l');
                                    }
                                    output += moment(d.startDatetime).format('l');
                                } else {
                                    if (d.endDatetime > d3.time.second.offset(d.startDatetime, 86400)) {
                                        output += moment(d.startDatetime).format('hh:mm:ss') + ' '
                                            + moment(d.startDatetime).format('hh:mm:ss') + ' - '
                                            + moment(d.endDatetime).format('hh:mm:ss') + ' '
                                            + moment(d.endDatetime).format('hh:mm:ss');
                                    }
                                    output += moment(d.startDatetime).format('hh:mm:ss') + ' - '
                                        + moment(d.endDatetime).format('hh:mm:ss');
                                }

                                return output += ' ' + _getMessages(d.message);
                            })
                                .style('left', function () {
                                    var left = window.pageXOffset + matrix.e;
                                    if (overflow) {
                                        left -= 200;
                                    }
                                    return left + 'px';
                                })
                                .style('top', function () {
                                    return window.pageYOffset + matrix.f + 6 + 'px';
                                })
                                .style('height', settings.tooltipHeight + 'px');
                        })
                        .on('mouseout', function () {
                            div.transition()
                                .duration(500)
                                .style('opacity', 0.0);
                        });
                }
            }, this);
        });
    };

})(jQuery, window, document, d3, _);