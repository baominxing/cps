(function () {
	$(function () {
		var timeStatisticsSevice = abp.services.app.states;
		var _$datepicker = $("#daterange-btn");
		var _$TimeStatisticsTable = $("#timeStatisticsTable");
		var _$TimeStatisticsDataTable = null;
		var statisticalName = "";
		var percentOfChartDataZoom = 0;
		var _$showName = $("#showName");
		var _$showValue = $("#showValue");
		var statisticalWay = "ByDay";
		var lastQueryParams = null;
		var shiftSolutionNames = null;

		var exportParam = {};

		var filterModal = new app.ModalManager({
			viewUrl: abp.appPath + "TimeStatistics/FilterModal",
			scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/TimeStatistics/_FilterModal.js",
			modalClass: "FilterModal"
		});



		_$datepicker.WIMIDaterangepicker({
			startDate: moment().subtract(6, "days"),
			endDate: moment()
		});

		//构建以班次方案为分割的Tab
		function constructShiftTabs(parameters) {
			$("#TimeStatisticChart").empty();

			var tabData = [];
			for (var i = 0; i < parameters.machineShiftSolutionNameList.length; i++) {
				var className = "";
				if (i === 0) {
					className = "active";
				}

				tabData.push({
					className: className,
					machineShiftSolutionName: parameters.machineShiftSolutionNameList[i]
				});
			}

			var source = $("#chart-template").html();
			var rendered = Handlebars.compile(source);
			$("#TimeStatisticChart").html(rendered());
			var tabDataSource = $("#tab-data-template").html();
			var tabDataRender = Handlebars.compile(tabDataSource);

			$("#tabData").html(tabDataRender({ datas: tabData }));
			$("#tabData").scrollTabs(
				{
					click_callback: function () {

						parameters.machineShiftSolutionNameList = [$(this).text()];

						loadTableAndChart(parameters, "TimeStatisticShiftChart");
					}
				});

			//默认加载一个tab的班次数据
			parameters.machineShiftSolutionNameList = [tabData[0].machineShiftSolutionName];
			timeStatisticsSevice.getMachineStateRateByMac(parameters).done(function (result) {
				if (result !== null) {
					var filter = parameters.statisticalName + parameters.startTime + "~" + parameters.endTime;
					setSearchFilter(filter);
					loadChart(result, "TimeStatisticShiftChart");
					loadTable(result, parameters.statisticalWay);
				}
			});
		}

		function loadTableAndChart(parameters, elementId) {
			var param = parameters;
			timeStatisticsSevice.getMachineStateRateByMac(param).done(function (result) {
				var filter = parameters.statisticalName + parameters.startTime + "~" + parameters.endTime;
				setSearchFilter(filter);
				loadChart(result, elementId);
				loadTable(result, param.statisticalWay)
			});
		}

		function loadChart(result, elementId) {

			var isShowName = _$showName.is(":checked");
			var isShowValue = _$showValue.is(":checked");

			var chartDom = document.getElementById(elementId);

			var chartInstance = echarts.getInstanceByDom(chartDom);
			if (chartInstance) {
				echarts.dispose(chartDom);
			}
			chartInstance = echarts.init(chartDom);

			var legendsData = [app.localize("Stop"), app.localize("Run"), app.localize("Free"), app.localize("Offline"), app.localize("Debug")];
			var xAxisData = [];
			var seriesData = [];

			//#region   echarts控件初始化
			var chartByDateOption = {
				color: ["#d43a36", "#4cae4c", "#f2a332", "#c4c4c4", "#1d89cf"],
				legend: {
					data: legendsData
				},
				tooltip: {
					trigger: "item",
					formatter: function (params) {
						var machineName = params.seriesName.split(",")[0];
						var programName = params.seriesName.split(",")[1];
						var capacity = params.value;
						var res = app.localize("MachineName") + ":" + machineName + "<br/>";
						res += app.localize("DeviceStatus") + ":" + programName + "<br/>";
						res += app.localize("ProportionOfTimeOccupied") + ":" + capacity + "%<br/>";
						return res;
					}
				},
				grid: {
					left: "3%",
					right: "4%",
					bottom: "15%",
					containLabel: true
				},
				dataZoom: [
					{
						xAxisIndex: 0,
						show: true,
						start: 0,
						end: 100,
						height: 20,
						showDataShadow: false
					}
				],
				xAxis: [
					{
						name: app.localize("Date"),
						type: "category",
						boundaryGap: true,
						axisLabel: {
							interval: 0
						},
						data: xAxisData
					}

				],
				yAxis: [
					{
						name: app.localize("Percentage") + "(%)           ",
						type: "value",
						axisLabel: {
							show: true,
							interval: "auto",
							formatter: "{value}"
						},
						min: 0,
						max: 100
					}
				],
				series: seriesData
			};

			legendsData.length = 0;
			xAxisData.length = 0;
			seriesData.length = 0;
			for (var i = 0; i < result.length; i++) {

				var xData = result[i].summaryDate;

				if (jQuery.inArray(xData, xAxisData) === -1) {
					xAxisData.push(xData);
				}

				if (i === result.length - 1 || (result[i].summaryDate + result[i].summaryId) !== (result[i + 1].summaryDate + result[i + 1].summaryId)) {

					(function (n) {
						if ($.grep(seriesData, function (v) { return v.stack === result[n].summaryId; }).length === 0) {
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Stop"),
									type: "bar",
									barGap: '20%',
									barCategoryGap: '10%',
									stack: result[n].summaryId,
									data: [(result[n].stopDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Run"),
									type: "bar",
									stack: result[n].summaryId,
									data: [(result[n].runDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Free"),
									type: "bar",
									stack: result[n].summaryId,
									data: [(result[n].freeDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Offline"),
									type: "bar",
									stack: result[n].summaryId,
									data: [(result[n].offlineDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Debug"),
									type: "bar",
									stack: result[n].summaryId,
									itemStyle: {
										normal: {
											label: {
												textStyle: {
													fontSize: 12,
													color: "black"
												},
												show: true,
												position: 'top',
												align: 'left',
												verticalAlign: 'middle',
												formatter: function (item) {
													//console.log(item);
													if (isShowName && isShowValue) {
														return item.seriesName.split(',')[0] + "-" + '100%';
													} else if (isShowName) {
														return item.seriesName.split(',')[0];
													} else if (isShowValue) {
														return "100";
													} else {
														return "";
													}
												}
											}
										}
									},
									data: [(result[n].debugDurationRate * 100).toFixed(2)]
								});
						} else {
							for (var j = 0; j < seriesData.length; j++) {
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Stop")) {
									seriesData[j].data.push((result[n].stopDurationRate * 100).toFixed(2));
									continue;
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Run")) {
									seriesData[j].data.push((result[n].runDurationRate * 100).toFixed(2));
									continue;
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Free")) {
									seriesData[j].data.push((result[n].freeDurationRate * 100).toFixed(2));
									continue;
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Offline")) {
									seriesData[j].data.push((result[n].offlineDurationRate * 100).toFixed(2));
									continue;
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Debug")) {
									seriesData[j].data.push((result[n].debugDurationRate * 100).toFixed(2));
									seriesData[j].itemStyle = {
										normal: {
											label: {
												textStyle: {
													fontSize: 12,
													color: "black"
												},
												show: true,
												position: 'top',
												align: 'left',
												verticalAlign: 'middle',
												rotate: 90,
												formatter: function (item) {
													//console.log(item);
													if (isShowName && isShowValue) {
														return item.seriesName.split(',')[0] + "\r\n" + '100';
													} else if (isShowName) {
														return item.seriesName.split(',')[0];
													} else if (isShowValue) {
														return "100";
													} else {
														return "";
													}

												}
											}
										}
									};
									continue;
								}
							}
						}
					})(i);

				} else {

					(function (n) {
						if ($.grep(seriesData, function (v) { return v.stack === result[n].summaryId; }).length === 0) {
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Stop"),
									type: "bar",
									barGap: '20%',
									barCategoryGap: '10%',
									stack: result[n].summaryId,
									data: [(result[n].stopDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Run"),
									type: "bar",
									stack: result[n].summaryId,
									data: [(result[n].runDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Free"),
									type: "bar",
									stack: result[n].summaryId,
									data: [(result[n].freeDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Offline"),
									type: "bar",
									stack: result[n].summaryId,
									data: [(result[n].offlineDurationRate * 100).toFixed(2)]
								});
							seriesData.push(
								{
									name: result[n].summaryName + "," + app.localize("Debug"),
									type: "bar",
									stack: result[n].summaryId,
									data: [(result[n].debugDurationRate * 100).toFixed(2)]
								});
						} else {
							for (var j = 0; j < seriesData.length; j++) {
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("UnplannedOutage")) {
									seriesData[j].data.push((result[n].stopDurationRate * 100).toFixed(2));
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Run")) {
									seriesData[j].data.push((result[n].runDurationRate * 100).toFixed(2));
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Free")) {
									seriesData[j].data.push((result[n].freeDurationRate * 100).toFixed(2));
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Offline")) {
									seriesData[j].data.push((result[n].offlineDurationRate * 100).toFixed(2));
								}
								if (seriesData[j].stack === result[n].summaryId && seriesData[j].name === result[n].summaryName + "," + app.localize("Debug")) {
									seriesData[j].data.push((result[n].debugDurationRate * 100).toFixed(2));
								}
							}
						}
					})(i);
				}
			}

			var maxBarCount = app.consts.chartSetting.maxBarCount;

			if (result.length > maxBarCount) {
				chartByDateOption.dataZoom[0].end = maxBarCount * 100 / result.length;
			}
			else {
				chartByDateOption.dataZoom[0].end = 100;
			}

			chartInstance.setOption(chartByDateOption);
		}

		function loadTable(result, statisticalWay) {

			if (_$TimeStatisticsDataTable !== null) {
				_$TimeStatisticsDataTable.destroy();
				_$TimeStatisticsDataTable = null;
				_$TimeStatisticsTable.empty();
			}

			_$TimeStatisticsDataTable = _$TimeStatisticsTable.WIMIDataTable({
				serverSide: false,
				data: result,
				columns: [
					{
						title: app.localize("StatisticalMethod") + "(" + app.localize(statisticalWay) + ")",
						data: "summaryDate"
					},
					{
						title: app.localize("Machines"),
						data: "summaryName"
					},
					{
						title: app.localize("Stop"),
						data: "stopDurationRate",
						render: function (data) {
							var $div = $('<div class="text-center">' + (data * 100).toFixed(2) + "%</div>");
							return $div.html();
						}
					},
					{
						title: app.localize("Run"),
						data: "runDurationRate",
						render: function (data) {
							var $div = $('<div class="text-center">' + (data * 100).toFixed(2) + "%</div>");
							return $div.html();
						}
					},
					{
						title: app.localize("Free"),
						data: "freeDurationRate",
						render: function (data) {
							var $div = $('<div class="text-center">' + (data * 100).toFixed(2) + "%</div>");
							return $div.html();
						}
					},
					{
						title: app.localize("Offline"),
						data: "offlineDurationRate",
						render: function (data) {
							var $div = $('<div class="text-center">' + (data * 100).toFixed(2) + "%</div>");
							return $div.html();
						}
					},
					{
						title: app.localize("Debug"),
						data: "debugDurationRate",
						render: function (data) {
							var $div = $('<div class="text-center">' + (data * 100).toFixed(2) + "%</div>");
							return $div.html();
						}
					}
				]
			})
		}

		//设置查询参数显示
		function setSearchFilter(filter) {
			$("#searchFilter").text(filter);
		}

		//设置查询参数回调函数
		function setSearchParametersAndLoadBarEChart(parameters) {
			shiftSolutionNames = parameters.machineShiftSolutionNameList;
			lastQueryParams = parameters;
			exportParam = parameters;
			$("#TimeStatisticDetailHeadMessage").text('');
			statisticalWay = parameters.statisticalWay;

			if (statisticalWay === "ByShift") {
				constructShiftTabs(parameters);
			}
			else {
				loadTableAndChart(parameters, "TimeStatisticChart");
			}

		}

		function firstOpen() {

			var parameters = {
				startTime: moment().add(-6, "days").format('YYYY-MM-DD'),//moment().format('YYYY-MM-DD'),
				endTime: moment().format('YYYY-MM-DD'),
				statisticalWay: "ByDay",
				statisticalName: "按天",
				queryType: 0,
				machineIdList: null
			};

			if (app.consts.fixedCalendar.enabled) {
				parameters.startTime = app.consts.fixedCalendar.startTime;
				parameters.endTime = app.consts.fixedCalendar.endTime;
			}
			exportParam = parameters;
			setSearchParametersAndLoadBarEChart(parameters);
		}

		//查询
		$("#btnQuery").on("click", function (e) {

			filterModal.open({}, setSearchParametersAndLoadBarEChart);
		});

		_$showName.on('click', function () {

			lastQueryParams.machineShiftSolutionNameList = shiftSolutionNames;
			setSearchParametersAndLoadBarEChart(lastQueryParams);
		});
		_$showValue.on('click', function () {
			lastQueryParams.machineShiftSolutionNameList = shiftSolutionNames;
			setSearchParametersAndLoadBarEChart(lastQueryParams);
		});

		firstOpen();

		$("#btnExport").on("click",
			function () {
				var paramter = exportParam;

				timeStatisticsSevice.export(paramter).done(function (result) {
					app.downloadTempFile(result);
				});
			});

	});
})();