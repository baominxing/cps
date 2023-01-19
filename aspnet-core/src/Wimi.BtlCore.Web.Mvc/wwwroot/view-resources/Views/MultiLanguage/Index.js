(function () {
    $(function () {

        var countryData = [
          
            { id: "zh-TW", text: "中文(繁体)" },
            { id: "de", text: "德文" },
            { id: "ja", text: "日文" },
            { id: "fr", text: "法文" },
            { id: "ru", text: "俄文" },
            { id: "it", text: "意大利文" },
            { id: "ar", text: "阿拉伯文" },
            { id: "nl", text: "荷兰文" },
            { id: "th", text: "泰文" },
            { id: "ko", text: "韩文" }


        ];
        var service = abp.services.app.multiLanguage;
        var multiLanguage = {
            countrySelect: $("#countryCulture"),
            fileInput: $("#LanguageForm input[name=UploadFiles]"),
            initAddButton: function () {
                $("#AddButton").click(function () {
                    //校验信息的完整性
                    if (!$("#LanguageForm").valid()) {
                        abp.message.error("请将信息填写完整");
                        return;
                    }
                    //校验提交的文件的格式
                    if (!multiLanguage.checkFile()) {
                        multiLanguage.fileInput.val("");
                        return;
                    }
                 
                    var formData = new FormData();
                    formData.append("upfile", multiLanguage.fileInput.get()[0].files[0]);
                    var param = $("#LanguageForm").serializeFormToObject();
                    formData.append("SheetIndex", param.SheetIndex);
                    formData.append("CodeIndex", param.CodeIndex);
                    formData.append("ValueIndex", param.ValueIndex);
                    formData.append("Culture", param.Culture);
                    formData.append("TitleCheck", param.TitleCheck);
                    $.ajax({
                        url: "/MultiLanguage/DealLanguageFile",
                        type: "POST",
                        data: formData,
                        /**
                        *必须false才会自动加上正确的Content-Type
                        */
                        contentType: false,
                        /**
                        * 必须false才会避开jQuery对 formdata 的默认处理
                        * XMLHttpRequest会对 formdata 进行正确的处理
                        */
                        processData: false,
                        success: function (data) {
                            if (data.result== "OK") {
                                abp.message.success("添加语言成功，请重新启动程序")
                            }
                            else {
                                abp.message.error("添加语言失败，具体的错误：" + data);
                            }
                        },
                        error: function () {
                           
                        }
                    });
                });
            },
            checkFile: function () {
                var files = multiLanguage.fileInput.get()[0].files;

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
                return true;
            },
            initCountrySelect: function () {
                multiLanguage.countrySelect.select2({
                    data: countryData,
                    multiple: false,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                }).trigger('change');
            }
        }
        multiLanguage.initAddButton();
        multiLanguage.initCountrySelect();
      


    });
})();