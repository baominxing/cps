//# sourceURL=dynamicLoadOrUnloadModalScript.js
(function () {
    app.modals.UpdateRateModal = function () {
        var _manager,
            _args,
            service = abp.services.app.cutter;
        var rendered = Handlebars.compile($("#loadingCutterTab-template").html());
        //var btnSaved = $("#btnSavePage");

        var blockPanelsource = $("#block-panel").html();
        var blockPanelrendered = Handlebars.compile(blockPanelsource);

        //日期选择tab
        var scrollTabObject = {
            $scrollTab: null,
            init: function (data) {
                if (this.$scrollTab !== null) {
                    this.$scrollTab.destroy();
                }
                this.$scrollTab = $("#loadingCutterTab")
                    .html(rendered(data))
                    .scrollTabs({
                        click_callback: function () {
                            var $this = $(this);
                            scrollTabObject.loadData($(this).text());
                        }
                    });
                this.loadData($.trim($("#loadingCutterTab .tab_selected").text()));
            },
            loadData: function (machineName) {
                service.listMachineCutterDetails({ Id: machineName}).done(function (response) {
                    $("#tvalueDetail").html(blockPanelrendered(response));
                });
            },
            load: function () {
                service.listLoadingMachines().done(function (response) {
                    scrollTabObject.init(response);
                });
            },
            savePage: function () {
                var param = []
                var machineName = $.trim($("#loadingCutterTab .tab_selected").text());
                var rates = [];
                var rate = {};
                $(".cutterRate").each(function () {
                    var tvalueText = $(this).find(".tValue").text()
                    var tValue = tvalueText.substring(app.localize("CutterPosition").length + 1, tvalueText.length); 
                    var tRate = $(this).find(".rate").val();
                    rate = { TValue: tValue, Rate: tRate };
                    rates.push(rate);
                });
                param.push({ MachineName: machineName, CutterRates: rates });
                service.saveMachineCutterRates({ MachineCutterRates: param }).done(function () {
                    scrollTabObject.loadData(machineName);
                    abp.notify.success(app.localize("SavedSuccessfully"));
                });
            }
        }

        this.init = function (manager, args) {
            _manager = manager;
            _args = args;    

            scrollTabObject.load();
        }

        this.shown = function () {
            
            //btnSaved.click(function (e) {
            //    e.preventDefault();
            //    scrollTabObject.savePage();
            //})

            var maxModalHeight = document.documentElement.clienceHeight - 170 + 'px';
            $(".maxModalHeight").css({
                'max-height': 'maxModalHeight',
                'overflow': 'auto'
            }); 
        }

        this.save = function () {
            _manager.setBusy(true);
            scrollTabObject.savePage();
            _manager.setBusy(false);
        }
    }
})();