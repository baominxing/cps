(function () {
	$(function () {
		var cartonSettingAppService = abp.services.app.cartonSetting;
		var deviceGroupAppService = abp.services.app.deviceGroup;
		var cartonSettingId;
		var vm = new Vue({
			el: '#carton-setting',
			data: {
				printerStyle:'block',
				ruleStyle: 'block',
				traceFlowStyle: 'none',
				isAutoPrintStyle: 'block',
				isUnpackingAfterPrintStyle:'block'
			},
			mounted() {
				$printerName = $("#printerName");
				$cartonRuleId = $("#cartonRuleId");
				$flowIds = $("#flowIds");
				$hasToFlow = $("#hasToFlow");
				$maxPackingCount = $("#maxPackingCount");
				$isGoodOnly = $("#isGoodOnly");
				$forbidHopSequence = $("#forbidHopSequence");
				$forbidRepeatPacking = $("#forbidRepeatPacking");
				$isAutoPrint = $("#isAutoPrint");
				$isUnpackingRedo = $("#isUnpackingRedo");
				$isUnpackingAfterPrint = $("#isUnpackingAfterPrint");
				$isFinalTest = $("#isFinalTest");
				$btnClear = $("#btnClear");
				$btnSave = $("#btnSave");
				$settingForm = $(document).find("form[name=settingForm]");
			}
		});

		deviceGroupTree = {
			$tree: $("#deviceGroupTree"),
			$emptyInfo: $("#deviceGroupTreeEmptyInfo"),

			show: function () {
				deviceGroupTree.$emptyInfo.hide();
				deviceGroupTree.$tree.show();
			},

			hide: function () {
				deviceGroupTree.$emptyInfo.show();
				deviceGroupTree.$tree.hide();
			},
			unitCount: 0,

			setUnitCount: function (unitCount) {
				deviceGroupTree.unitCount = unitCount;
				if (unitCount) {
					deviceGroupTree.show();
				} else {
					deviceGroupTree.hide();
				}
			},

			refreshUnitCount: function () {
				deviceGroupTree.setUnitCount(deviceGroupTree.$tree.jstree("get_json").length);
			},

			selectedOu: {
				id: null,
				code: null,
				name: null,

				set: function (ouInTree) {
					if (!ouInTree) {
						deviceGroupTree.selectedOu.id = null;
						deviceGroupTree.selectedOu.code = null;
						deviceGroupTree.selectedOu.name = null;
					} else {
						deviceGroupTree.selectedOu.id = ouInTree.id;
						deviceGroupTree.selectedOu.code = ouInTree.original.code;
						deviceGroupTree.selectedOu.name = ouInTree.original.name;
					}
					page.getExistSetting(deviceGroupTree.selectedOu.id);
				}
			},

			generateTextOnTree: function (ou) {
				return '<span class="ou-text" data-ou-id="' + ou.id + '">' + ou.displayName + '</span>';
			},

			getTreeDataFromServer: function (callback) {
				deviceGroupAppService.listFirstClassDeviceGroups().done(function (result) {
					var treeData = _.map(result, function (item) {
						return {
							id: item.id,
							parent: "#",
							code: item.code,
							name: item.displayName,
							//memo: item.memo,
							text: deviceGroupTree.generateTextOnTree(item),
							state: {
								opened: true
							}
						};
					});

					callback(treeData);
				});
			},

			init: function () {
				deviceGroupTree.getTreeDataFromServer(function (treeData) {
					deviceGroupTree.setUnitCount(treeData.length);
			
					deviceGroupTree.$tree
						.on("changed.jstree", function (e, data) {
							if (data.selected.length !== 1) {
								deviceGroupTree.selectedOu.set(null);
							} else {
								var selectedNode = data.instance.get_node(data.selected[0]);
								deviceGroupTree.selectedOu.set(selectedNode);
							}
						})
						.on("ready.jstree", function () {
							deviceGroupTree.$tree.jstree(true).select_node('ul > li:first');
						})
						.jstree({
							'core': {
								data: treeData,
								multiple: false
							},
							types: {
								"default": {
									"icon": "fa fa-th tree-item-icon-color icon-lg"
								},
								"file": {
									"icon": "fa fa-th tree-item-icon-color icon-lg"
								}
							},
							plugins: [
								"types",
								"wholerow",
								"sort",
								"dnd"
                            ],
                            sort: function (node1, node2) {
                                if (this.get_node(node2).original.seq <= this.get_node(node1).original.seq) {
                                    return -1;
                                }

                                return 1;
                            }
						});

 

					deviceGroupTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
						e.preventDefault();

						var ouId = $(this).closest(".ou-text").attr("data-ou-id");
						setTimeout(function () {
							deviceGroupTree.$tree.jstree("show_contextmenu", ouId);
						}, 100);
					});
				});
			},

			reload: function () {
				deviceGroupTree.getTreeDataFromServer(function (treeData) {
					deviceGroupTree.setUnitCount(treeData.length);
					deviceGroupTree.$tree.jstree(true).settings.core.data = treeData;
					deviceGroupTree.$tree.jstree("refresh");
				});
			}
		};

		var page = {

			init: function () {
				page.initRuleSelect2();
				page.initPrinterSelect2();
			},

			initPrinterSelect2: function () {
				cartonSettingAppService.listLocalPrinterName().done(function (response) {
					var data = _.map(response,
						function (item) {
							return { id: item.value, text: item.name };
						});

					$printerName.select2({
						data: data,
						multiple: false,
						minimumResultsForSearch: -1,
						placeholder: app.localize("PleaseChoose"),
						language: {
							noResults: function () {
								return app.localize("NoPrinter");
							}
						}
					});
				});
			},

			initRuleSelect2: function () {
				cartonSettingAppService.listCartonRule().done(function (response) {
					var data = _.map(response,
						function (item) {
							return { id: item.value, text: item.name };
						});

					$cartonRuleId.select2({
						data: data,
						multiple: false,
						minimumResultsForSearch: -1,
						placeholder: app.localize("PleaseChoose"),
						language: {
							noResults: function () {
								return app.localize("NoCartonRule");
							}
						}
					});
				});
			},

			intTraceFlowSelect2: function (deviceGroupId) {
				cartonSettingAppService.listTraceFlow(deviceGroupId).done(function (response) {
					var data = _.map(response,
						function (item) {
							return { id: item.value, text: item.name };
						});

					$flowIds.select2({
						data: data,
						multiple: true ,
						minimumResultsForSearch: -1,
						placeholder: app.localize("PleaseChoose"),
						language: {
							noResults: function () {
								return app.localize("NoTraceFlow");
							}
						}
					});
				});

			},

			getAllSetting: function () {

				var isPrint =false ;
				if ($("input[name='isPrint']:checked").val() == "1") {
					isPrint = true;
				}

				var isAutoCartonNo = false;
				var cartonRuleId;
				if ($("input[name='autoCartonNo']:checked").val() == "1") {
					isAutoCartonNo = true;
					cartonRuleId = $cartonRuleId.val();
				}
				else {
					cartonRuleId = 0;
				}

				var isGoodOnly = false;
				if ($isGoodOnly.is(':checked')) {
					isGoodOnly = true;
				} 
					
				var forbidHopSequence = false;
				if ($forbidHopSequence.is(':checked')) {
					forbidHopSequence = true;
				} 

				var forbidRepeatPacking = false;
				if ($forbidRepeatPacking.is(':checked')) {
					forbidRepeatPacking = true;
				} 

				var isAutoPrint = false;
				if ($isAutoPrint.is(':checked')) {
					isAutoPrint = true;
				} 

				var isUnpackingRedo = false;
				if ($isUnpackingRedo.is(':checked')) {
					isUnpackingRedo = true;
				} 

				var isUnpackingAfterPrint = false;
				if ($isUnpackingAfterPrint.is(':checked')) {
					isUnpackingAfterPrint = true;
				} 

				var hasToFlow = false;
				if ($hasToFlow.is(':checked')) {
					hasToFlow = true;
				} 

				var flowIds="";
				if (hasToFlow) {
					if ($flowIds.val() != null) {
						flowIds = $flowIds.val().join(',');
					}
				}

				var isFinalTest = false;
				if ($isFinalTest.is(':checked')) {
					isFinalTest = true;
				}

				var maxPackingCount = 0;
				if ($maxPackingCount.val() != "") {
					maxPackingCount = $maxPackingCount.val();
				}

				var result = {
					Id: cartonSettingId,
					DeviceGroupId: deviceGroupTree.selectedOu.id,
					MaxPackingCount: maxPackingCount,
					IsPrint: isPrint,
					PrinterName: $printerName.find("option:selected").text(),
					AutoCartonNo: isAutoCartonNo,
					CartonRuleId: cartonRuleId,
					IsGoodOnly: isGoodOnly,
					ForbidHopSequence: forbidHopSequence,
					ForbidRepeatPacking: forbidRepeatPacking,
					IsAutoPrint: isAutoPrint,
					IsUnpackingRedo:isUnpackingRedo,
					IsUnpackingAfterPrint: isUnpackingAfterPrint,
					HasToFlow: hasToFlow,
					FlowIds: flowIds,
					IsFinalTest: isFinalTest
				}
				return result;
			},

			getExistSetting: function (deviceGroupId) {
				page.initPrinterSelect2();
				page.initRuleSelect2();
				page.intTraceFlowSelect2(deviceGroupId);

				cartonSettingAppService.getCartonSettingByDeviceGroup(deviceGroupId).done(function (response) {
					cartonSettingId = response.id;

					if (cartonSettingId == 0) {
						page.formReset();
					}
					else {

						$maxPackingCount.val(response.maxPackingCount);

						if (response.isPrint) {
							$("input[name='isPrint'][value=2]").removeAttr("checked");
							$("input[name='isPrint'][value=1]").prop("checked", "checked");
							vm.printerStyle = 'block';
							vm.isAutoPrintStyle = 'block';
							vm.isUnpackingAfterPrintStyle = 'block';
						} else {
							$("input[name='isPrint'][value=1]").removeAttr("checked");
							$("input[name='isPrint'][value=2]").prop("checked", "checked");
							vm.printerStyle = 'none';
							vm.isAutoPrintStyle = 'none';
							vm.isUnpackingAfterPrintStyle = 'none';
						}

						$("#printerName option:contains(" + response.printerName + ")").attr("selected", true).trigger("change");

						if (response.autoCartonNo) {
							$("input[name='autoCartonNo'][value=2]").removeAttr("checked");
							$("input[name='autoCartonNo'][value=1]").prop("checked", "checked");
							vm.ruleStyle = 'block';
							$cartonRuleId.val(response.cartonRuleId).select2({ minimumResultsForSearch: -1, placeholder: app.localize("PleaseChoose") })
						} else {
							$("input[name='autoCartonNo'][value=1]").removeAttr("checked");
							$("input[name='autoCartonNo'][value=2]").prop("checked", "checked");
							vm.ruleStyle = 'none';
						}


						if (response.isGoodOnly) {
							$isGoodOnly.prop("checked", true);
						} else {
							$isGoodOnly.prop("checked", false);
						}

						if (response.forbidHopSequence) {
							$forbidHopSequence.prop("checked", true);
							$hasToFlow.prop('disabled', true);
							$hasToFlow.removeAttr("checked");
							$flowIds.val(null).trigger("change");
							vm.traceFlowStyle = 'none';
						} else {
							$forbidHopSequence.prop("checked", false);
						}

						if (response.forbidRepeatPacking) {
							$forbidRepeatPacking.prop("checked", true);
						} else {
							$forbidRepeatPacking.prop("checked", false);
						}

						if (response.isAutoPrint) {
							$isAutoPrint.prop("checked", true);
						} else {
							$isAutoPrint.prop("checked", false);
						}

						if (response.isUnpackingRedo) {
							$isUnpackingRedo.prop("checked", true);
						} else {
							$isUnpackingRedo.prop("checked", false);
						}

						if (response.isUnpackingAfterPrint) {
							$isUnpackingAfterPrint.prop("checked", true);
						} else {
							$isUnpackingAfterPrint.prop("checked", false);
						}

						if (response.hasToFlow) {
							$hasToFlow.prop("checked", true);

							$forbidHopSequence.prop('disabled', true);
							$forbidHopSequence.removeAttr("checked");
							vm.traceFlowStyle = 'block';
							var flowIds = response.flowIds.split(',');
							$flowIds.val(flowIds).select2({ minimumResultsForSearch: -1 });

						} else {
							$hasToFlow.prop("checked", false);
							vm.traceFlowStyle = 'none';
						}

						if (response.isFinalTest) {
							$isFinalTest.prop("checked", true);
						} else {
							$isFinalTest.prop("checked", false);
						}

						
					}
				});
			},

		    formReset:function () {
 
				$settingForm.resetForm();
				page.clearSelect2();
				$("input[type=radio][value=1]").prop('checked', true);
				vm.printerStyle = 'block';
				vm.ruleStyle = 'block';
				vm.isAutoPrintStyle = 'block';
				vm.isUnpackingAfterPrintStyle = 'block';
			},

			clearSelect2: function () {
				$printerName.val(null).trigger("change").select2({ minimumResultsForSearch: -1, placeholder: app.localize("PleaseChoose") });
				$cartonRuleId.val(null).trigger("change").select2({ minimumResultsForSearch: -1, placeholder: app.localize("PleaseChoose") });
				$flowIds.val(null).trigger("change").select2({ minimumResultsForSearch: -1, placeholder: app.localize("PleaseChoose") });
				$hasToFlow.prop('disabled', false);
				$forbidHopSequence.prop('disabled', false);
			}
		}; 

		$(":radio[name='autoCartonNo']").click(function () {
			if ($(this).val() == '2') {
				vm.ruleStyle = 'none';
			}
			else {
				vm.ruleStyle = 'block';
			}
		});

		$(":radio[name='isPrint']").click(function () {
			if ($(this).val() == '2') {
				vm.printerStyle = 'none';
				vm.isAutoPrintStyle = 'none';
				$isAutoPrint.prop("checked", false);
				vm.isUnpackingAfterPrintStyle = 'none';
				$isUnpackingAfterPrint.prop("checked", false);
			}
			else {
				vm.printerStyle = 'block';
				vm.isAutoPrintStyle = 'block';
				vm.isUnpackingAfterPrintStyle = 'block';

			}
		});

		$hasToFlow.change(function () {
			if ($hasToFlow.is(':checked')) {
				$forbidHopSequence.prop('disabled', true);
				$forbidHopSequence.removeAttr("checked");
				page.intTraceFlowSelect2(deviceGroupTree.selectedOu.id);
				vm.traceFlowStyle = 'block';
			}
			else {
				$forbidHopSequence.prop('disabled', false);
				$flowIds.val(null).trigger("change").select2({ minimumResultsForSearch:-1 });
				vm.traceFlowStyle = 'none';
			}
		});

		$forbidHopSequence.change(function () {

			if ($forbidHopSequence.is(':checked')) {
				$hasToFlow.prop('disabled', true);
				$hasToFlow.removeAttr("checked");
				$flowIds.val(null).trigger("change");
				vm.traceFlowStyle = 'none';
			}
			else {
				$hasToFlow.prop('disabled', false);
			}

		})

		$btnSave.on("click", function () {
			var param = page.getAllSetting();
			cartonSettingAppService.saveCartonSetting(param).done(function (response) {
				page.getExistSetting(deviceGroupTree.selectedOu.id);
				cartonSettingId = response;
				abp.notify.info(app.localize("SavedSuccessfully"));
			})	 
		});

		$btnClear.on("click", function () {
			$settingForm.clearForm();
			page.clearSelect2();
			$maxPackingCount.val(0);
		});

		deviceGroupTree.init();

		page.init();
	});
})();