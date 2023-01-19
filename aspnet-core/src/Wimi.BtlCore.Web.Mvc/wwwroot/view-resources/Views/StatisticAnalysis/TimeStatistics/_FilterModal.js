﻿//# sourceURL=dynamic_FilterModal.js
(function () {
	app.modals.FilterModal = function () {
		var service = abp.services.app.states,
			_$modalManager,
			_$TimeStatisticsFilterForm,
			_$machineTree = new MachinesTree(),
			_$timeRange = $("#reservationtime");

		this.init = function (modalManager) {
			_$modalManager = modalManager;
			_$TimeStatisticsFilterForm = _$modalManager.getModal().find('form[name=TimeStatisticsFilterForm]');

			_$timeRange.WIMIDaterangepicker({
				startDate: moment().subtract(6, "days"),
				endDate: moment()
			});

			_$machineTree.init($("div.machines-tree"), true, machineTreeChangedCallBack);
			_$machineTree.initGroup($("div.machines-group-tree"));
			_$machineTree.setSelectAll();
			_$machineTree.initMoreAndLess();
		}

		this.shown = function () {
			$("#statisticalWay").select2({
				multiple: false,
				minimumResultsForSearch: -1,
				language: {
					noResults: function () {
						return app.localize("NoMatchingData");
					}
				}
			});
		};

		this.save = function () {

			var parameters = getSearchParameters();

			if (parameters.statisticalWay === "ByShift" && $("#checkbox-shift").val() === null) {
				abp.notify.error("请至少选择一个班次");
				return;
			}

			_$modalManager.setResult(parameters);
			_$modalManager.close();
		};

		function getSearchParameters() {
			var startTime = null;
			var endTime = null;
			var queryType = _$machineTree.getQueryType();
			var machineIdList;
			if (queryType * 1 === 0) {
				machineIdList = _$machineTree.getSelectedMachineIds();
			} else {
				machineIdList = _$machineTree.getSelectedGroupIds();
			}

			var statisticalWay = $("#statisticalWay").val();
			var machineShiftSolutionNameList = [];
			var statisticalName = "";

			if (statisticalWay === "ByDay") {
				statisticalName = app.localize("ByDay") + ":";
				startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
				endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
			} else if (statisticalWay==="ByWeek"){
				statisticalName = app.localize("ByWeek") + ":";
				startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
				endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
			} else if (statisticalWay === "ByMonth") {
				statisticalName = app.localize("ByMonth") + ":";
				startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
				endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
			} else if (statisticalWay==="ByYear") {
				statisticalName = app.localize("ByYear") + ":";
				startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
				endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
			}else if (statisticalWay === "ByShift") {
				statisticalName = app.localize("ByShift") + ":";
				startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
				endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
				var selectedDatas = $('#checkbox-shift').select2("data");
				machineShiftSolutionNameList = _.pluck(selectedDatas, "text");
			};

			var parameters = {
				startTime: startTime,
				endTime: endTime,
				statisticalWay: statisticalWay,
				statisticalName: statisticalName,
				machineIdList: machineIdList,
				machineShiftSolutionNameList: machineShiftSolutionNameList,
				queryType: queryType
			};

			return parameters;
		}

		var machineShiftSolutions = [];
		var machines = [];
		function setMachineShiftSolutions() {
			$("#checkbox-shift").select2({
				data: [],
				multiple: true,
				placeholder: app.localize("PleaseSelect"),
				language: {
					noResults: function () {
						return app.localize("NoMatchingData");
					}
				}
			});
			$("#checkbox-shift").select2().empty();
			machineShiftSolutions = [];
			var parameters = getSearchParameters();
			service.getMachineShiftSolutions(parameters).done(function (result) {
				if (result !== null) {

					var data = [];
					machines = result;
					for (var i = 0; i < result.length; i++) {

						(function (n) {
							if ($.grep(machineShiftSolutions, function (v) { return v === result[n].machineShiftSolutionName; }).length === 0) {
								machineShiftSolutions.push(result[n].machineShiftSolutionName);
								data.push({ id: n, text: result[n].machineShiftSolutionName });
							}
						})(i);


					}

					$("#checkbox-shift").select2({
						data: data
					});

					//默认选择全部
					var ids = _.pluck(data, "id");
					$("#checkbox-shift").select2().val(ids).trigger("change");
				}
			});
		}

		function machineTreeChangedCallBack() {
			var statisticalWay = $("#statisticalWay").val();
			if (statisticalWay === "ByShift") {
				setMachineShiftSolutions();
			}
		}

		$("#reservationtime")
			.on("change",
				function () {
					var statisticalWay = $("#statisticalWay").val();
					if (statisticalWay === "ByShift") {
						setMachineShiftSolutions();
					}
				});

		$("#statisticalWay")
			.on("change",
				function () {
					var statisticalWay = $("#statisticalWay").val();
					if (statisticalWay === "ByShift") {
						$(".input-shift").removeAttr("style");
						setMachineShiftSolutions();
					} else {
						$(".input-shift").css('display', 'none');
					}
				});

		$(document).on("mouseover", ".select2-selection__choice", function () {
			$(this).css("cursor", "pointer");
			var machineShiftSolutionName = $(this).attr("title");
			var machineNameString = app.localize("MachineList") + ":\r\n";
			for (var i = 0; i < machines.length; i++) {
				if (machines[i].machineShiftSolutionName === machineShiftSolutionName) {
					machineNameString += machines[i].machineName + ",\r\n";
				}
			}
			layer.tips(machineNameString, this);
		});

		$(document).on("mouseout", ".select2-selection__choice", function () {
			$(this).css("cursor", "default");
			layer.closeAll();
		});
	}
})(jQuery);