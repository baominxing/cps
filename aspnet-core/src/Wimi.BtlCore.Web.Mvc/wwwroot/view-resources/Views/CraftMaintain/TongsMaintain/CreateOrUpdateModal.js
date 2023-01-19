//# sourceURL=dynamic_CreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function () {
        var service = abp.services.app.tong,
            _modalManager,
            $createOrUpdateForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=CreateOrUpdateForm]");

            $(".Capacity").change(
                function (data) {
                    console.log("===");
                    var capacity = $(".Capacity").val();
                    capacity < 6 ? $(".no6").attr("hidden", "hidden") : $(".no6").removeAttr("hidden");
                    capacity < 5 ? $(".no5").attr("hidden", "hidden") : $(".no5").removeAttr("hidden");
                    capacity < 4 ? $(".no4").attr("hidden", "hidden") : $(".no4").removeAttr("hidden");
                    capacity < 3 ? $(".no3").attr("hidden", "hidden") : $(".no3").removeAttr("hidden");
                    capacity < 2 ? $(".no2").attr("hidden", "hidden") : $(".no2").removeAttr("hidden");

                    capacity < 6 ? $(".ProgramF").removeAttr("required") : $(".ProgramF").attr("required", "required");
                    capacity < 5 ? $(".ProgramE").removeAttr("required") : $(".ProgramE").attr("required", "required");
                    capacity < 4 ? $(".ProgramD").removeAttr("required") : $(".ProgramD").attr("required", "required");
                    capacity < 3 ? $(".ProgramC").removeAttr("required") : $(".ProgramC").attr("required", "required");
                    capacity < 2 ? $(".ProgramB").removeAttr("required") : $(".ProgramB").attr("required", "required");
                }
            );

            $(".Capacity").empty();
            var data = [
                { id: 1, text: 1 },
                { id: 2, text: 2 },
                { id: 3, text: 3 },
                { id: 4, text: 4 },
                { id: 5, text: 5 },
                { id: 6, text: 6 },
            ];

            var num = $("#num").val();

            $(".Capacity").select2({
                data: data,
                multiple: false,
                minimumResultsForSearch: -1,
                placeholder: app.localize("PleaseChoose"),
                language: {
                    noResults: function () {
                        return app.localize("");
                    }
                }
            }).val(num).trigger('change');
        };

      
        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }

            var parameters = $createOrUpdateForm.serializeFormToObject();

            if (parameters.Id === "0") {
                _modalManager.setBusy(true);
                service.createTong(parameters)
                    .done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _modalManager.setResult(result);
                        abp.event.trigger("app.CreateOrUpdateModalSaved");
                        _modalManager.close();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            } else {
                _modalManager.setBusy(true);
                service.updateTong(parameters)
                    .done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _modalManager.setResult(result);
                        abp.event.trigger("app.CreateOrUpdateModalSaved");
                        _modalManager.close();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            }
        };
    };
})(jQuery);