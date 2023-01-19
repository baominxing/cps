//# sourceURL=dynamic_CreateOrUpdateModalForType.js
(function () {
    app.modals.CreateOrUpdateModalForType = function () {

        var _modalManager;
        var cutterAppService = abp.services.app.cutter;
        var _$createOrUpdateForm = null;
        var $isCutterNoPrefixCanEdit = $(".isCutterNoPrefixCanEdit");

        this.init = function (modalManager) {
            _modalManager = modalManager;

            if ($("#SelectedIsCutterNoPrefixCanEdit").val() != null) {
                $isCutterNoPrefixCanEdit.val($("#SelectedIsCutterNoPrefixCanEdit").val());
            }

            _$createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");
        };

        this.shown = function () {

            $("#IsCutterNoPrefixCanEdit").select2({
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
            if (!_$createOrUpdateForm.valid()) {
                return;
            }

            var parameters = {
                id: $("#Id").val(),
                name: $(".name").val(),
                pId: $("#PId").val(),
                cutterNoPrefix: $(".cutterNoPrefix").val(),
                isCutterNoPrefixCanEdit: $(".isCutterNoPrefixCanEdit").val(),
                creationTime: $("#CreationTime").val(),
                creatorUserId: $("#CreatorUserId").val()
            }

            _modalManager.setBusy(true);
            cutterAppService.createOrUpdateCutterType(
                parameters
            ).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();