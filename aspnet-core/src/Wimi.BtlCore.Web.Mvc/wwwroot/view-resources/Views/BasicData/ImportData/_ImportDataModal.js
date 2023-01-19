//# sourceURL=dynamic_ImportDataModal.js
(function ($) {
    app.modals.ImportDataModal = function () {
        var _modalManager;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;

            $("#ImportDataForm input[name=Type]").val(args.Type);

            $("#ImportDataForm input[name=UploadFiles]").on('change', function () {
                $("#ImportDataForm").submit();
                _modalManager.close();
            });

            $("#ImportDataForm").ajaxForm({
                beforeSubmit: function (formData, jqForm, options) {

                    var $fileInput = $("#ImportDataForm input[name=UploadFiles]");
                    var files = $fileInput.get()[0].files;

                    if (!files.length) {
                        return false;
                    }
                    var file = files[0];
                    
                    //File type check
                    var type = "|" + file.name.slice(file.name.lastIndexOf(".") + 1) + "|";
                    if ("|xlsx|xls|".indexOf(type) === -1) {
                        abp.ui.clearBusy();
                        abp.message.warn(app.localize("PleaseChooseExcel"));
                        return false;
                    }

                    //File size check
                    if (file.size > 5242880) //1MB
                    {
                        abp.ui.clearBusy();
                        abp.message.warn(app.localize("FileChooseSizeLimit"));
                        return false;
                    }
                    abp.ui.setBusy();
                    return true;
                },
                success: function (response) {
                    if (response.success) {
                        abp.ui.clearBusy();
                        _modalManager.setResult(response);
                    } else {
                        abp.ui.clearBusy();
                        abp.message.error(response);
                    }
                }
            });
        };
    };
})(jQuery);