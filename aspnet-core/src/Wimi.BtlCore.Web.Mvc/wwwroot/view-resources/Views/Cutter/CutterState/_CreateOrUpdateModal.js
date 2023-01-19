//# sourceURL=dynamicCreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function() {
        var _manager,
            _args,
            _form = null,
            cutterAppService = abp.services.app.cutter;
        var $cutterType = $("#cutterType_Create"),
            $cutterModel = $("#cutterModel_Create"),
            $hiddenTypeId = $("#CutterTypeId"),
            $hiddenModelId = $("#CutterModelId");


        this.init = function(manager, args) {
            _manager = manager;
            _args = args;
            _form = _manager.getModal().find("form[name='createOrUpdateForm']");
        }

        this.shown = function () {

            $(".enumDropDown").select2({ minimumResultsForSearch: -1 });

            var option = {
                $typeSelector: $cutterType,
                istypeMultiple: false,
                istypeDisabled: _args.hasOwnProperty('Id'),
                typeSelectedValue: $hiddenTypeId.val(),
                $modalselector: $cutterModel,
                ismodalMultiple: false,
                ismodalDisabled: _args.hasOwnProperty('Id'),
                modalSelectedValue: $hiddenModelId.val()
            };
            //初始化
            _args.callback(option);
            //赋值
            if (_args.hasOwnProperty('Id')) {
                cutterAppService.findCutterTypeById({ Id: $hiddenTypeId.val() })
                    .done(function(response) {
                        $cutterType.append(new Option(response.name, response.id, false, true));
                    });
            } else {
                $("#CutterUsedStatus").prop("disabled", true);
            }

            $cutterModel.select2({ minimumResultsForSearch: -1})
                .on("select2:select",
                    function(e) {
                        cutterAppService.getCutterModelDefaultValue({ id: e.params.data.id })
                            .done(function(response) {
                                $("input[name='cutterNo']").val(response.cutterNo);
                                $("#CountingMethod").select2({ minimumResultsForSearch: -1 }).val(response.countingMethod).trigger("change");
                                $("input[name='originalLife']").val(response.originalLife);
                                $("input[name='restLife']").val(response.originalLife);
                                $("input[name='warningLife']").val(response.warningLife);

                                $('input[name*="parameter"]')
                                    .each(function(index, key) {
                                        var param = $(key).attr("name");
                                        $("input[name=" + param + "]").val(response[param]);
                                    });

                            });
                    });
        }

        this.save = function() {

            //验证规则
            _form.validate({
                rules: {
                    originalLife: { required: true, number: true, min: 0.1 },
                    restLife: { required: true, number: true, min: 0.1 },
                    warningLife: { required: true, number: true, min: 0.1 }
                },
                messages: {
                    originalLife: { required: app.localize("EnterTheInitialLife"), number: app.localize("EnterNumber"), min: app.localize("EnterNumberBigThanZero") },
                    restLife: { required: app.localize("EnterUsableLife"), number: app.localize("EnterNumber"), min: app.localize("EnterNumberBigThanZero") },
                    warningLife: { required: app.localize("EnterEarlyWarningLife"), number: app.localize("EnterNumber"), min: app.localize("EnterNumberBigThanZero") }
                }
            });

            if (!_form.valid()) {
                return false;
            }
          
            var cutterStateObject = _form.serializeFormToObject();
            var intOriginalLife = parseInt(cutterStateObject.originalLife);
            var intRestLife = parseInt(cutterStateObject.restLife);
            var intWarningLife = parseInt(cutterStateObject.warningLife);
            if (intOriginalLife < intRestLife) {
                abp.message.error(app.localize("OriginalLifeCanNotLessThanRestLife"));
                return false;
            } else if (intOriginalLife < intWarningLife) {
                abp.message.error(app.localize("OriginalLifeCanNotLessThanWarningLife"));
                return false;
            }
            var d = $cutterType.select2('data');
            if (d.length === 0) {
                abp.message.error(app.localize("PleaseChooseCutterType"));
                return false;
            }

            cutterStateObject.cutterModelId = cutterStateObject.cutterModel;
            cutterStateObject.cutterTypeId = d[0].id;
          
            _manager.setBusy(true);
            cutterAppService.createOrUpdateCutterStates(cutterStateObject)
                .done(function(response) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _manager.close();
                    _manager.setResult(response);
                })
                .always(function() {
                    _manager.setBusy(false);
                });
        };
    }
})();