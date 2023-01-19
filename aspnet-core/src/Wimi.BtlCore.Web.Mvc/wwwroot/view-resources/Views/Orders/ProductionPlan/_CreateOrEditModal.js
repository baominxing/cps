(function () {

    Vue.component('wimiSelect2', {
        template: '<select class="form-control drop-downlist" style="height:31px" ></select>',
        props: ['datas', 'value','enable'],
        mounted: function () {
            var vm = this;
            
            $(vm.$el).select2({
                placeholder: app.localize("PleaseSelect"),
                data: this.datas
            })
           .on("select2:select", (e) => {

               var data = $(vm.$el).select2("data");
               if (data != null) {
                   vm.$emit('change', data[0]);
               }   

           });
        },
        watch: {
            value: function () {
                if (this.value == null) {
                    return;
                }
                $(this.$el).select2('val', this.value);
                $(this.$el).select2().trigger('select2:select');

            },
            datas: function () {
                var vm = this;

                $(vm.$el).select2().empty();
                $(vm.$el).select2({
                    data: this.datas
                });

            },
            enable: function () {
                var vm = this;
                $(vm.$el).prop("disabled", !vm.enable);
            }
        },
        destroyed: function () {
            $(this.$el).off().select2('destroy');
        }
    });

    Vue.component('wimiDaterangepick', {
        template: '<input type="text" class="form-control input-sm" id="daterange-btn" />',
        props: ['start', 'end'],
        mounted: function() {
            var vm = this;

            $(vm.$el).WIMIDaterangepicker({
                maxDate: null,
                startDate: moment(),
                endDate: moment()
            }).on('change', e => {
                
                var start = $(vm.$el).data("daterangepicker").startDate.format("YYYY-MM-DD");
                var end = $(vm.$el).data("daterangepicker").endDate.format("YYYY-MM-DD");

                vm.$emit('change', {
                    start: start,
                    end: end
                });
            });
        },
        watch: {
            start: function () {

                var vm = this;
                
                if (vm.start == null) {
                    return;
                }

                $(vm.$el).data("daterangepicker").setStartDate(vm.start);
            },
            end: function() {
                var vm = this;
                
                if (vm.end == null) {
                    return;
                }
                $(vm.$el).data("daterangepicker").setEndDate(vm.end);

            }
        },
        destroyed: function () {
            
        }
    });

})();

(function ($, abp, app) {

    app.modals.CreateOrEditProductionPlanModal = function() {

        var _planService = abp.services.app.productionPlan;

        var _modalManager;

        var vmData = {
            vm: {},
            productListVm: {
                datas: [],
                selected: [],
                enable:true,
                selectChanged: function (e) {

                    if (e == null) {
                        return;
                    }
                    vmData.vm.productId = e.id;

                }
            },
            carftListVm: {
                datas: [],
                selected: [],
                enable: true,
                selectChanged: function (e) {
                    if (e == null) {
                        return;
                    }
                    vmData.vm.craftId = e.id;
                }
            },
            dateRangeVm: {
                start: null,
                end: null,
                dateRangeChange: function (e) {

                    vmData.vm.startDate = e.start;
                    vmData.vm.endDate = e.end;
                }
            }
        };

// ReSharper disable once UnusedLocals
        var vue = new Vue({
            el: '#app-productionPlanForm',
            data: vmData
        });

        this.init = function(modalManager) {
            _modalManager = modalManager;

         

            var id = _modalManager.getArgs().id;

            _planService.getProductionPlanForEdit({ id: id })
                .done(function(res) {
                    vmData.vm = res.productionPlan;

                    vmData.dateRangeVm.start =
                        res.productionPlan.startDate;
                    vmData.dateRangeVm.end =
                        res.productionPlan.endDate;
                    
                    var productDatas = _.map(res.productList, function(item) {
                        return {
                            id: item.value,
                            text: item.name,
                            data:item
                        }
                    });

                    vmData.productListVm.datas = productDatas;
                    setTimeout(function () {
                        vmData.productListVm.selected = [vmData.vm.productId];
                    }, 100);


                    changeCarftList(res.carftList);

                });

        };

        function changeCarftList(datas) {

            var carftDatas = _.map(datas, function (item) {
                return {
                    id: item.value,
                    text: item.name,
                    data: item
                }
            });
            
            vmData.carftListVm.datas = carftDatas;
            setTimeout(function () {
                vmData.carftListVm.selected = [vmData.vm.craftId];
            }, 100);
            
        }

        this.save = function () {

            if (!$('#app-productionPlanForm').valid()) {
                return;
            }
            
            _modalManager.setBusy(true);
            _planService.createOrUpdate(vmData.vm)
                .done(function() {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.close();
                    abp.event.trigger("app.CreateOrEditProductionPlanModalSaved", _modalManager.getArgs().id);
                }).always(function () {
                    _modalManager.setBusy(false);
                });

        };
    };
})(jQuery,abp,app);