(function () {
    app.modals.FeedbackDefectiveReason = function () {
        var _modalManager,
            _args,
            totalYield,
            $form,
            templateRender = Handlebars.compile($("#defectiveReasontemplate").html()),
            service = abp.services.app.machineReport;
        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            $form = _modalManager.getModal().find("form[name='DefectiveReasonForm']");
            totalYield = args.row.yield;
            //请求不良原因及数据
            var param = {
                machineId: _args.row.machineId,
                shiftSolutionItemId: _args.row.shiftSolutionItemId,
                creationTime: _args.row.creationTime,
                productId: _args.row.productId
            };

            service.listMachineDefectiveRecords(param).done(function (response) {
                $form.empty();
                $form.append(templateRender(response));
            }).fail(function () {
                abp.message.error(app.localize("FailureToObtainDefectiveProducts")+"！");
                return false;
            });
        }

        this.save = function () {
            if (!$form.valid()) {
                return false;
            }

            var reasonArray = [];
            var param = $form.serializeFormToObject();
            var countError = false;
            var totalCount = 0;
            _.each($("input.defective-Reasons"),
                function (item) {
                    var defectiveCount = 0;
                    if (!_.isNaN($(item).val() * 1)) {
                        defectiveCount = $(item).val() * 1;
                    }
                    if (defectiveCount > totalYield) {
                        countError = true;
                    }
                    totalCount += defectiveCount;
                    reasonArray.push({ Name: $(item).data().defectivereasonsid, Value: defectiveCount });
                });
            if (countError === true) {
                abp.message.error(app.localize("DefectiveProductsShouldNotExceedTheOutput"));
                return false;
            }
            if (totalCount > totalYield) {
                abp.message.error(app.localize("TotalDefectiveProductsShouldNotExceedTheOutput"));
                return false;
            }
            var values = _.filter(reasonArray,
                function (item) {
                    return item.Value > 0;
                });

            if (values.length === 0) {
                abp.message.error(app.localize("AtLeastOneAdverseEventGreaterThan0") + "!", app.localize("PleaseConfirm"));
                return false;
            }
            param.Reasons = reasonArray;
            param.MachineId = _args.row.machineId;
            param.ProductId = _args.row.productId;
            param.ShiftSolutionItemId = _args.row.shiftSolutionItemId;
            param.Date = _args.row.creationTime;

            console.log(param);
            _modalManager.setBusy(true);
            service.feedbackDefectiveReason(param).done(function (response) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.close();
                _modalManager.setResult(response);
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }
    }
})(jQuery);