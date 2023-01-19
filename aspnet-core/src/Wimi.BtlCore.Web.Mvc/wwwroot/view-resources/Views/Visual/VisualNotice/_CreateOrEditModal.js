(function() {
    app.modals.CreateOrUpdateModal = function () {
        var _modalManager,
            $noticesInfoForm=null,
            _noticesService = abp.services.app.visual;

        this.init = function(modalManager) {
            _modalManager = modalManager;
            $noticesInfoForm = _modalManager.getModal().find('form[name=noticesInfoForm]');
        };

        this.shown = function () {
            $("#WorkShopCode").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });

            $("#IsActive").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });
        }

        this.save = function () {
            $noticesInfoForm.validate({
                rules: {
                    RootDeviceGroupCode : { required: true }
                },
                messages: {
                    RootDeviceGroupCode : { required: "请选择车间,如果没有数据，请联系管理员维护!" }
                }
            });

            //检查车间是否已维护
            if (!$noticesInfoForm.valid()) {
                return;
            }

            var notices = $noticesInfoForm.serializeFormToObject();
            _noticesService.addOrUpdateNotics(notices)
                .done(function(result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    abp.event.trigger("app.CreateOrUpdateModalSaved");
                    _modalManager.close();
                })
                .always(function() {
                    _modalManager.setBusy(false);
                });
        };
    }
})();