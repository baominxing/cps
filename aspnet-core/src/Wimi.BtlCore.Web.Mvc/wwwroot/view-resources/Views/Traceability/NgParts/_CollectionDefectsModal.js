(function () {
    app.modals.CollectionDefectsModal = function () {
        var _modalManager,
            _args,
            service = abp.services.app.trace;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            $collectionForm = _modalManager.getModal().find("form[name=CollectionDefectsModalForm]");
            var msg = abp.utils.formatString(app.localize("NGCause"), _args.PartNo);
            _modalManager.getModal().find(".modal-title span").text(msg);
            initSelect();
            bindingSaveContinueEvent();
        };
        this.shown = function () {

        };
        this.save = function () {
            if (!$collectionForm.valid()) {
                return;
            }

            _modalManager.setBusy(true);
            service.saveCollectionDefects(getParam(_args))
                .done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    abp.event.trigger("app.CollectionDefectsModalSaved");
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };

        var getParam = function (_args) {
            var param = {
                DefectivePartId: $("#CollectionDefectivePartId").val(),
                DefectiveReasonsId: $("#CollectionDefectiveReasonId").val(),
                PartNo: _args.PartNo,
                DefectiveMachineId: _args.DefectiveMachineId
            };
            return param
        }

        var bindingSaveContinueEvent=function () {
            $("#SaveAndContinue").click(function () {
                if (!$collectionForm.valid()) {
                    return;
                }
                _modalManager.setBusy(true);
                service.saveCollectionDefects(getParam(_args))
                    .done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _modalManager.setResult(result);
                        initSelect();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            });
        }

        var initSelect = function () {
            $("#CollectionDefectivePartId").empty();
            $("#CollectionDefectiveReasonId").empty();
            service.listDefectiveParts()
                .done(function (response) {
                    $("#CollectionDefectivePartId").on("change",
                        function () {

                            service.listDefectiveReasonsByPartId({ Id: $("#CollectionDefectivePartId").val() })
                                .done(function (response) {
                                    $("#CollectionDefectiveReasonId").empty();
                                    var data = _.map(response,
                                        function (item) {
                                            return { id: item.value, text: item.name };
                                        });

                                    $("#CollectionDefectiveReasonId").select2({
                                        data: data,
                                        multiple: true,
                                        minimumResultsForSearch: -1,
                                        placeholder: app.localize("PleaseChoose"),
                                        language: {
                                            noResults: function () {
                                                return app.localize("NoMatchingData");
                                            }
                                        }
                                    });
                                });
                        });
                    var data = _.map(response,
                        function (item) {
                            return { id: item.value, text: item.name };
                        });

                    $("#CollectionDefectivePartId").select2({
                        data: data,
                        multiple: false,
                        minimumResultsForSearch: -1,
                        placeholder: app.localize("PleaseChoose"),
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });

                    if (!$("#CollectionDefectivePartId").val()) {
                        return false;
                    }

                    service.listDefectiveReasonsByPartId({ Id: $("#CollectionDefectivePartId").val() })
                        .done(function (response) {
                            $("#CollectionDefectiveReasonId").empty();
                            var data = _.map(response,
                                function (item) {
                                    return { id: item.value, text: item.name };
                                });

                            $("#CollectionDefectiveReasonId").select2({
                                data: data,
                                multiple: true,
                                minimumResultsForSearch: -1,
                                placeholder: app.localize("PleaseChoose"),
                                language: {
                                    noResults: function () {
                                        return app.localize("NoMatchingData");
                                    }
                                }
                            });
                        });

                });

        };
    };
})();