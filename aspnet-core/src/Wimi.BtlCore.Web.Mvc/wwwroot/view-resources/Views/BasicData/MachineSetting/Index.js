(function () {
    $(function () {
        var _$machineSettingTable = $("#MachineSettingTable");
        var _$machineSettingDataTables = null;
        var _machineSettingService = abp.services.app.basicData;
        var $time = $("#Time");
        var _machineImageForm = $("#machineImageForm");
        var _$machineSettingInformationForm = $(document).find("form[name=MachineSettingForm]");
        var imageId = $("input[name=ImageId]").val();
        var $create = $("#Create");
        var $saveMachine = $("#SaveMachine");
        var $code = $("#Code");
        var $id = $("#Id");
        var $name = $("#Name");
        var $desc = $("#Desc");
        var $createTime = $("#CreateTime");
        var $lastModifiedTime = $("#LastModifiedTime");
        var $IsActive = $("#IsActive");
        var tr;
        var validator;
        var $sortSeq = $("#SortSeq");

        wimi.btl.addRequiredTag(_$machineSettingInformationForm);

        var _permissions = {
            create: abp.auth.hasPermission("Pages.BasicData.MachineSetting.Create"),
            edit: abp.auth.hasPermission("Pages.BasicData.MachineSetting.Edit"),
            'delete': abp.auth.hasPermission("Pages.BasicData.MachineSetting.Delete")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineType/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/MachineType/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        var pingTestModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineSetting/PingTest",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/MachineSetting/_PingTest.js",
            modalClass: "PingTestModal"
        });

        var telnetTestModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineSetting/TelnetTest",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/MachineSetting/_TelnetTest.js",
            modalClass: "TelnetTestModal"
        });

        _$machineSettingDataTables = _$machineSettingTable.WIMIDataTable({
            //"aLengthMenu": [[10, 20, 50, 100], [10, 20, 50, 100]],//设置每页显示数据条数的下拉选项
            "searching": true,
            "initComplete": function () {
                selectFirstRow();
            },
            order: [],
            "ajax": {
                "url": abp.appAPIPath + "basicData/getMachineSetting",
                "type": "POST"
            },
            "columns": [
                {
                    "defaultContent": "",
                    "title": app.localize("Actions"),
                    "orderable": true,
                    "width": "80px",
                    "className": "action",
                    "createdCell": function (td, cellData, rowData, row, col) {
                        if (!rowData.isActive) {
                            $(td).closest("tr").css("color", "#999");
                        }
                        $(td).buildActionButtons([
                            {
                                title: rowData.isActive ? app.localize('MachineSetting_Stop') : app.localize('MachineSetting_Start'),
                                clickEvent: function () { stopMachineSetting(rowData) },
                                isShow: _permissions.edit
                            },
                            {
                                title: app.localize('MachineSetting_Delete'),
                                clickEvent: function () { deleteMachineSetting(rowData) },
                                isShow: _permissions.delete && rowData.state !== 1
                            }
                        ]);
                    }
                },
                {
                    "data": "code",
                    "className": "not-mobile",
                    "title": app.localize("MachineCode"),
                    "render": function (data) {
                        var ellipsis = data.length > 15 ? "..." : "";
                        return '<div style="cursor:pointer">' + data.substring(0, 15) + ellipsis + "</div>";
                    }
                },
                {
                    "data": "name",
                    "title": app.localize("MachineName")
                },
                {
                    "data": "sortSeq",
                    "title": app.localize("SortSeq")
                }
            ]
        });

        function selectFirstRow() {
            var trObj = _$machineSettingDataTables.row(0).node();
            if (trObj) {
                settingForSelectedRow($(trObj));
            }
        }

        function cleanUpValidErrorInfo() {
            if (validator) {
                validator.resetForm();
                validator = null;
            }
        }
        //绑定表格每行的click事件
        $("#MachineSettingTable tbody").on("click", "tr",
            function () {
                cleanUpValidErrorInfo();

                settingForSelectedRow($(this));

                $(document).find('.form-group').removeClass("has-error").removeClass("help-block");
            });
        //选中行的相关设定
        function settingForSelectedRow($currentRow) {
            tr = _$machineSettingDataTables.row($currentRow).node();
            $currentRow = $(tr);
            if ($currentRow.hasClass("selected")) {
                return;
            } else {
                var uploadinput = document.getElementById("UploadedFile");
                uploadinput.value = null;
                _$machineSettingDataTables.$("tr.selected")
                    .removeClass("selected");
                $currentRow.addClass("selected");
                getMachine($currentRow);
            }

        }
        //获取机器
        function getMachine($currentRow) {
            if (_$machineSettingDataTables.row($currentRow).data()) {
                var rowdata = _$machineSettingDataTables.row($currentRow).data();
                getMachineSettingById(rowdata.id);
                imageHandle(rowdata.imageId);
                //console.log(rowdata.imageId);
                //$("input[name=ImageId]").val(rowdata.imageId);
                $time.show();
            }
        }
        //新建设备
        $("#CreateNewMachineButton").on("click",
            function () {
                resetMahcineInfoList();
                cleanUpValidErrorInfo();
            }
        );

        function resetMahcineInfoList() {
            $(".img-circle").attr("src", abp.appPath + "Content/Images/CNC1-128x128.png");
            $("input:not([type=submit])").val(null);
            $("select:not([name=MachineSettingTable_length])").val(null);
            $time.hide();
            $code.removeAttr("readonly");
            $IsActive.find("option:first").prop("selected", "selected");
            $("#MachineType").val(0).trigger("change");
        }

        function format(time) {
            if (time !== null) {
                return moment(time).format("YYYY-MM-DD HH:mm");
            } else {
                return "N/A";
            }
        }
        //编辑显示的内容
        function getMachineSettingById(id) {
            _machineSettingService.getMachineSettingById({ id: id }).done(function (data) {
                $code.val(data.code).attr("readonly", true);
                $name.val(data.name);
                $desc.val(data.desc);
                $createTime.val(moment(data.creationTime).format("YYYY-MM-DD HH:mm"));
                $("#CreatorUserId").val(data.creatorUserId);
                $lastModifiedTime.val(format(data.lastModificationTime));
                $id.val(id);
                $("#MachineType").val(data.machineTypeId).trigger("change");
                $sortSeq.val(data.sortSeq);
                $("#IpAddress").val(data.ipAddress);

                $("#TcpPort").val(data.tcpPort);

                if (data.isActive) {
                    $IsActive.val("Yes").trigger("change");
                } else {
                    $IsActive.val("No").trigger("change");
                }

                $("#PingTest").on('click', function () {
                    pingTestModal.open({ ipAddress: data.ipAddress, machineName: data.name, machineId: data.id });
                });

                $("#TelnetTest").on('click', function () {
                    telnetTestModal.open({ ipAddress: data.ipAddress, tcpPort: data.tcpPort, machineName: data.name, machineId: data.id });
                });

                $("#TelnetTest").on('click', function () {

                });
            });
        }

        $("#DMPTelnetTest").on('click', function () {
            abp.ui.setBusy();
            _machineSettingService.telnetTesFortDMP().done(function () {
                abp.ui.clearBusy();
                abp.notify.success(app.localize("TestSuccessfully"));
            }).always(function () {
                abp.ui.clearBusy();
            });
        });

        //载入图片
        function imageHandle(imageId) {
            _machineSettingService.getMachineImagePath({ id: imageId }).done(function (response) {
                if (response.length > 0) {
                    $(".img-circle").attr("src", abp.appPath + response);
                    $(".img-circle").css("height", 128);
                    //console.log(response);
                } else {
                    $(".img-circle").attr("src", abp.appPath + "Content/Images/CNC1-128x128.png");
                }
            });
        }

        //删除
        function deleteMachineSetting(rowData) {
            abp.message.confirm(app.localize("MachineSettingDeleteWarningMessage", rowData.name),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _machineSettingService.deleteMachine({ id: rowData.id })
                            .done(function () {
                                reloadMachinesTable();
                                resetMahcineInfoList();
                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                            });
                    }
                });
        }

        //停用 or 启用
        function stopMachineSetting(rowData) {
            _machineSettingService.enableOrDisEnableMachine({ id: rowData.id })
                .done(function () {
                    reloadTableAndGetMachineSetting();
                });
        }

        //load表格数据
        function reloadMachinesTable() {
            _$machineSettingDataTables.ajax.reload(function () {
                settingForSelectedRow();
            });
        }

        function reloadTableAndGetMachineSetting() {
            _$machineSettingDataTables.ajax.reload(function () {
                settingForSelectedRow($(tr));
            });
        }
        //获取已有设备类型并填充select
        function getMachineType() {
            $("#MachineType").empty();
            _machineSettingService.listMachineTypes({}).done(function (data) {
                for (var i = 0; i < data.length; i++) {
                    $("#MachineType").append($("<option></option>").val(data[i].value).html(data[i].name));

                    $("#MachineType").select2({
                        multiple: false,
                        minimumResultsForSearch: -1,
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });

                }
            });
        }

        //导出设备
        $("#ExportMachines").click(function (e) {
            e.preventDefault();
            _machineSettingService.exportMachinesToExcel()
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        $create.on("click", function () {
            createOrUpdateModal.open({},
                function () {

                    getMachineType();
                });
        });
        $saveMachine.on("click",
            function () {

                _$machineSettingInformationForm.validate($.WIMI.options.validate);

                if (!_$machineSettingInformationForm.valid()) {
                    validator = _$machineSettingInformationForm.validate($.WIMI.options.validate);
                    return;
                }
                var machineSetting = _$machineSettingInformationForm.serializeFormToObject();
                //machineSetting.SortSeq = 1;
                if (machineSetting.IsActive === "Yes") {
                    machineSetting.IsActive = true;
                } else {
                    machineSetting.IsActive = false;
                }

                machineSetting.MachineTypeId = $("#MachineType").val();
                _machineSettingService.addOrUpdateMachineSetting(machineSetting)
                    .done(function () {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        if ($id.val() === "") {
                            //新建机器刷新Table
                            //_$machineSettingDataTables.order([4, "desc"]);
                            reloadMachinesTable();
                            resetMahcineInfoList();
                        } else {
                            $("input[name=ImageId]").val("");
                            //编辑机器
                            reloadTableAndGetMachineSetting();
                        }
                    });
            });

        function init() {
            $("#machineImageForm input[type=file]").on("change",
                function () {
                    _machineImageForm.submit();
                });
            //上传文件
            _machineImageForm.submit(function (e) {
                var formData = new FormData($(this)[0]);
                e.preventDefault();
                $.ajax({
                    type: "POST",
                    url: "/MachineSetting/UploadMachineImage?imageId=" + imageId + "&v=" + new Date().valueOf(),
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        var $fileInput = $("#machineImageForm input[type=file]");
                        var files = $fileInput.get()[0].files;

                        if (!files.length) {
                            return false;
                        }

                        var file = files[0];

                        //File type check
                        var type = "|" + file.type.slice(file.type.lastIndexOf("/") + 1) + "|";
                        if ("|jpg|jpeg|png|".indexOf(type) === -1) {
                            abp.message.warn(app.localize("ProfilePicture_Warn_FileType"));
                            return false;
                        }

                        //File size check
                        if (file.size > 1048576) //1MB
                        {
                            abp.message.warn(app.localize("ProfilePicture_Warn_SizeLimit"));
                            return false;
                        }

                        return true;
                    },
                    success: function (data) {
                        if (data.success) {
                            var $profilePictureResize = $("#machinePictureResize");

                            var newCanvasHeight = data.result.height * $profilePictureResize.width() / data.result.width;
                            $profilePictureResize.height(newCanvasHeight + "px");

                            var profileFilePath = abp.appPath + "Temp/Downloads/" + data.result.fileName + "?v=" + new Date().valueOf();
                            var uploadedFileName = data.result.fileName;
                            $("input[name=ImageId]").val(uploadedFileName.split(".")[0]);
                            $profilePictureResize.attr("src", profileFilePath);
                            $(".img-circle").css("height", 128);
                            //$("input[name=ImageId]").val(data.result.fileName);
                        } else {
                            abp.message.error(data.error.message);
                        }
                    },
                    error: function (event, xmlHttpRequest, ajaxOptions) {
                        abp.message.error(app.localize("CheckFileIsValid{0}", ajaxOptions), app.localize("UploadFailure"));
                    }
                });
            })
            $("#MachineType").select2();
            getMachineType();

            //$("#BatchSave").on("click", function () {
            //    _machineSettingService.batchSave()
            //        .done(function () {
            //            abp.notify.info(app.localize("SavedSuccessfully"));
            //        });
            //});
        }

        $("#IsActive").select2({
            multiple: false,
            minimumResultsForSearch: -1,
            language: {
                noResults: function () {
                    return app.localize("NoMatchingData");
                }
            }
        });


        init();
    });
})();