(function (abp) {
	$(function () {
		var machineSelect = $('#machineId');
		var cutterLoadAndUnloadService = abp.services.app.cutter;
		var commonService = abp.services.app.commonLookup;
		var variable = {
			$TextFilter: $("#TextFilter")
		};


		var page = {
			$timeRange: $("#reservationtime"),
			$btnQuery: $("#btnQuery"),
			$btnExport: $("#btnExport"),
			$datatable: null,
			$table: $("#table"),
			$select: $("#CutterModelsSelect"),
			dynamicArray: [],

			getQueryParameters: function () {
				var queryParameters = {};
				var self = this;

				queryParameters.dateTimeFrom = self.$timeRange.data("daterangepicker")
					.startDate.format("YYYY-MM-DD HH:mm");
				queryParameters.dateTimeEnd = self.$timeRange
					.data("daterangepicker")
					.endDate.format("YYYY-MM-DD HH:mm");
				queryParameters.cutterNo = $.trim(variable.$TextFilter.val());

				var machineIdList = machineSelect.val();
				queryParameters.machineIdList = machineIdList;
				queryParameters.cutterModelId = page.$select.val();

				return queryParameters;
			},

			getDynamicColumns: function () {

				cutterLoadAndUnloadService.getDynamicColumns({})
					.done(
						function (result) {
							if (result != null) {
								for (var i = 0; i < result.items.length; i++) {
									var increment = i + 1;
									if (result.items.length <= 9) {
										page.dynamicArray.push(
											{
												"data": "parameter" + increment,
												"title": result.items[i].name
											}
										);
									}
								}
								page.load();
							}
						});
			},

            initSelect: function () {
				var self = this;
				cutterLoadAndUnloadService.getCutterModels({})
					.done(function (result) {
						if (result == null) {
							page.$select.append("<option value='0'>" + app.localize("CutterTypeNotFound") + "</option>");
							return;
						} else {
							self.$select.append("<option value='0'>" + app.localize("PleaseChooseCutterType") + "</option>");
							var temp;
							for (var i = 1; i <= result.items.length; i++) {
								temp = "<option value=" +
									result.items[i - 1].id +
									">" +
									result.items[i - 1].name +
									"</option>";
								self.$select.append(temp);
							}
                        }
					});

				$("#CutterModelsSelect").select2({
					multiple: false,
					minimumResultsForSearch: -1,
					language: {
						noResults: function () {
							return app.localize("NoMatchingData");
						}
					}
				});
			},

			initMachinesSelect2: function () {
				commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
					var machines = _.chain(response.machines)
						.filter(
							function (item) {
								return _.contains(response.grantedGroupIds, item.deviceGroupId);
							})
						.map(function (item) {
							var machine = { id: item.id, text: item.name };
							return machine;
						});

					//var machinesData = [];
					var machinesData = [{ id: 0, text: app.localize("All") }];
					//_.each(_.groupBy(machines._wrapped, "id"),
					//	function (item) {
					//		machinesData.push(item[0]);
     //                   });
                    for (var n = 0; n < machines._wrapped.length; n++) {
                        machinesData.push(machines._wrapped[n]);
                    }
					machineSelect.select2({
						data: machinesData,
						multiple: true,
						language: {
							noResults: function () {
								return "请维护设备";
							}
						}
					}).val(0).trigger('change');
				});

			},

			init: function () {

				this.$timeRange.WIMIDaterangepicker();

				this.initSelect();

				this.getDynamicColumns();

				this.initMachinesSelect2();

				page.$btnQuery
					.on("click",
						function (e) {
							e.preventDefault();

							if (page.$datatable != null) {
								page.$datatable.destroy();
								page.$datatable = null;
								page.$table.empty();
							}
							page.getDynamicColumns();

						});

				page.$btnExport
					.on("click",
						function (e) {
							e.preventDefault();

							page.export();
						});
			},

			load: function () {

				var columns = (function () {

					var defalutColumns = [
						{
							"data":
								"cutterNo",
							"title":
								app.localize("CutterNo")
						},
						{
							"data":
								"cutterModelName",
							"title":
								app.localize("CutterType")
						},
						{
							"data":
								"machineName",
							"title":
								app.localize("MachineName")
						},
						{
							"data":
								"cutterTValue",
							"title":
								app.localize("EquipmentCutterLocation")
						},
						{
							"data":
								"countingMethodName",
							"title":
								app.localize("LifeCountingTypes")
						},
						{
							"data":
								"originalLife",
							"title":
								app.localize("RawLife")
						},
						{
							"data":
								"usedLife",
							"title":
								app.localize("UsedLife")
						},
						{
							"data":
								"restLife",
							"title":
								app.localize("RestLife")
						},
						{
							"data":
								"operationTypeName",
							"title":
								app.localize("OperationType")
						}
					];
					var lastColumns = [
						{
							"data":
								"creatorUserName",
							"title":
								app.localize("Creator")
						},
						{
							"data":
								"creationTime",
							"title":
								app.localize("CreationTime"),
							"width": "80px",
							"render": function (data) {
								return moment(data).format("YYYY-MM-DD HH:mm");
							}
						},
						{
							"data":
								"operator",
							"title":
								app.localize("Operator")
						},
						{
							"data":
								"operatorTime",
							"title":
								app.localize("OperationTime"),
							"width": "80x",
							"render": function (data) {
								return moment(data).format("YYYY-MM-DD HH:mm");
							}
						}
					];
					for (var i = 0; i < page.dynamicArray.length; i++) {
						defalutColumns.push(page.dynamicArray[i]);
					}

					for (var j = 0; j < lastColumns.length; j++) {
						defalutColumns.push(lastColumns[j]);
					}

					return defalutColumns;
				})();
				var p = page.getQueryParameters();
				page.$datatable = page.$table.WIMIDataTable(
					{
						"ordering": false,
						"scrollX": true,
						"retrieve": true,
						"responsive": false,
						"ajax": {
							url: abp.appAPIPath + "cutter/queryCutterLoadAndUnloadRecords",
							data: function (d) {
								d.cutterNo = p.cutterNo;
								d.cutterModelId = p.cutterModelId;
								d.startTime = p.dateTimeFrom;
								d.endTime = p.dateTimeEnd;
								d.machineIdList = p.machineIdList;
							}
						},
						"columns": columns
					}
				);
				page.dynamicArray.length = 0;
			},
			export: function () {
				var params = page.getQueryParameters();
				params.startTime = params.dateTimeFrom;
				params.endTime = params.dateTimeEnd;
				cutterLoadAndUnloadService.exportCutterLoadAndUnloadRecords(params).done(function (result) {
					app.downloadTempFile(result);
				});
			}
		};
		page.init();
	});
})(abp);