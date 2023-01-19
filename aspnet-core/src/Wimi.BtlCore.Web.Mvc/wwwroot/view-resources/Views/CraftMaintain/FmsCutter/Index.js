(function () {
    $(function () {
        var service = abp.services.app.fmsCutter;
        var settingService = abp.services.app.fmsCutterSetting;

        var permissions = { manage: abp.auth.hasPermission("Pages.CraftMaintain.FmsCutter.Manage") };
        var page = null;

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "FmsCutter/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/CraftMaintain/FmsCutter/CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
            //modalSize: "modal-lg"
        });

        page = {
            $create: $("#create"),
            $search: $("#search"),
            searchParameters: function() {
                return {
                    CutterNo: $("#CutterNo").val(),
                    CutterCase: $("#CutterCase").val(),
                    Type: $("#Type").val()
                };
            } ,
            $table: $("#table"),
            dataTable: null,
            create: function () {
                createOrUpdateModal.open();
            },

            remove: function (rowData) {
                abp.message.confirm("删除该笔记录吗",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            service.deleteFmsCutter({ id: rowData.id })
                                .done(function () {
                                    abp.notify.success(app.localize("SuccessfullyDeleted"));
                                    page.dataTable.ajax.reload(null);
                                });
                        }
                    });
            },

            search: function () {
                page.load();
            },
            load: function () {
                if (page.dataTable) {
                    page.dataTable.destroy();
                    page.$table.empty();
                }

                settingService.listColumns({}).done(function (response) {
                    page.dataTable = page.$table.WIMIDataTable({
                        "scrollCollapse": true,
                        "scrollX": true,
                        "searching": false,
                        "ajax": {
                            "url": abp.appAPIPath + "fmscutter/listFmsCutter",
                            "type": "POST",
                            "data": function(d) {
                               $.extend(d, page.searchParameters());
                            },
                            "dataSrc": function (json) {

                                _.each(json.data,
                                    function (item, index) {
                                        var names = _.pluck(_.values(item.customFileds), 'code');
                                        var values = _.pluck(_.values(item.customFileds), 'fieldValue');
                                      
                                        var differenceCodes = _.difference(_.pluck(response, 'code'), names);
                                        _.each(differenceCodes,
                                            function (key) {
                                                names.push(key);
                                                values.push("");
                                            });

                                        var objs = _.object(names, values);
                                        _.extend(json.data[index], objs);
                                    });



                                return json.data;
                            }
                        },
                        "columns": page.getColumns(response)
                    });
                });

                $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();//表头错位
            },
            getColumns: function (data) {
                var param = [{
                    "data": null,
                    "title": app.localize("SerialNumber"),
                    "width": "30px",
                    "orderable": false,
                    "render": function (data, type, row, meta) {
                        return meta.row +
                            1 +
                            meta.settings._iDisplayStart;
                    }
                },
                {
                    "defaultContent": "",
                    "title": app.localize("Actions"),
                    "orderable": false,
                    "width": "30px",
                    "className": "action",
                    "createdCell": function (td, cellData, rowData, row, col) {
                        $(td)
                            .buildActionButtons([
                                {
                                    title: app.localize("Editor"),
                                    clickEvent: function () {
                                        createOrUpdateModal.open({ Id: rowData.id });
                                    },
                                    isShow: permissions.manage
                                },
                                {
                                    title: app.localize("Delete"),
                                    clickEvent: function () { page.remove(rowData); },
                                    isShow: permissions.manage
                                }
                            ]);
                    }
                }];

                _.each(data,
                    function (item, index) {
                        // 基础字段
                        if (item.type === 0) {
                            var names = ['UseType', 'CountType', 'State'];

                            if (_.contains(names, item.code)) {

                                if (item.code == 'State') {
                                    param.push({
                                        data: "state",
                                        title: app.localize(item.code),
                                        render: function (data) {
                                            if (data == 0) { return app.localize("Unactivated"); }
                                            else if (data == 1) { return app.localize("Normal"); }
                                            else if (data == 2) { return app.localize("Disabled"); }
                                            else { return app.localize("Unknown") }
                                        }
                                    });
                                }
                                else if (item.code == 'UseType') {
                                    param.push({
                                        data: "useType",
                                        title: app.localize(item.code),
                                        render: function (data) {
                                            if (data == 0) { return app.localize("Unused"); }
                                            else if (data == 1) { return app.localize("Normal"); }
                                            else if (data == 2) { return app.localize("Warning"); }
                                            else { return app.localize("Unknown") }
                                        }
                                    });
                                }
                                else if (item.code == 'CountType') {
                                    param.push({
                                        data: "countType",
                                        title: app.localize(item.code),
                                        render: function (data) {
                                            if (data == 0) { return app.localize("Degree"); }
                                            else if (data == 1) { return app.localize("Time"); }
                                            else if (data == 2) { return app.localize("UseLength"); }
                                            else { return app.localize("Unknown") }
                                        }
                                    });
                                }

                                //param.push({
                                //    data: item.code.replace(item.code[0], item.code[0].toLowerCase())+'Name',
                                //    title: app.localize(item.code),
                                //    render: function(data) {
                                //        return app.localize(data);
                                //    }
                                //});
                            } else {
                                param.push({
                                    orderable: item.code === "MachineName" ? false : true,
                                    data: item.code.replace(item.code[0], item.code[0].toLowerCase()),
                                    title: app.localize(item.code)
                                });
                            }
                          
                        } else {  //拓展字段
                            param.push({
                                orderable: false,
                                data: item.code,
                                title: item.name
                            });
                        }
                    });

                return param;
            },
            init: function () {

                page.$create.on("click", function () {
                    page.create();
                });

                page.$search.on("click", function () {
                    page.search();
                });

                page.load();
            }
        };

        page.init();

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            page.load();
        });
    });

})();

//自定义字段
(function () {
    $(function () {

        var service = abp.services.app.fmsCutterSetting;
        var cuStomFieldService = abp.services.app.customField;

        var createOrUpdateExtendModal = new app.ModalManager({
            viewUrl: abp.appPath + "FmsCutter/CreateExtendFieldModal",
            scriptUrl: abp.appPath + "view-resources/Views/CraftMaintain/FmsCutter/_CreateExtendFieldModal.js",
            modalClass: "CreateOrUpdateExtendModal"
        });


        new Vue({
            created: function () {
                this.loadData();
            },
            data: {
                fields: {
                    basic: [],
                    extend: []
                },
                selected: [],
                saveFields: []
            },
            mounted: function () {
            
            },
            methods: {
                basicFiedlClick: function (item, index) {
                    let _this = this;
                    _this.fields.basic[index].isShow = item.isShow;
                    if (!item.isShow) {
                        _this.addSelected(item);
                    } else {
                        _this.removeSelected(item.code);
                    }
                },
                extendFiedlClick: function (item, index) {
                    let _this = this;
                    _this.fields.extend[index].isShow = item.isShow;

                    if (!item.isShow) {
                        _this.addSelected(item);
                    } else {
                        _this.removeSelected(item.code);
                    }
                },
                addSelected: function (item) {
                    let _this = this;
                    _this.selected.push(item);
                    //_this.selected = _.sortBy(_this.selected, 'seq');
                },
                removeSelected: function (code) {
                    let _this = this;

                    _.each(_this.selected,
                        function (item, index) {
                            if (item && item.code === code) {
                                _this.selected.splice(index, 1);
                            }
                        });
                    _this.selected = _.sortBy(_this.selected, 'seq');
                },
                deleteSelected: function (item) {
                    let _this = this;

                    this.removeSelected(item.code);
                    this.setFieldnotChecked(_this.fields.basic, item.code);
                    this.setFieldnotChecked(_this.fields.extend, item.code);
                },
                setFieldnotChecked: function (array, code) {
                    _.each(array,
                        function (item, index) {
                            if (item.code === code) {
                                item.isShow = false;
                            }
                        });
                },
                save: function () {
                    let _this = this;
                    _this.saveFields = [];

                    // 获取页面数据
                    _.each($(".handle"),
                        function (item, index) {
                            var code = $(item).data('code');
                            _this.saveFields.push({ name: code, value: index });
                        });

                    if (_this.saveFields.length === 0) {
                        abp.message.error("至少保留一个字段!");
                        return false;
                    }

                    service.update(_this.saveFields).done(function (response) {
                        abp.notify.success("保存成功!");
                        $("#drawerClose").click();
                        abp.event.trigger("app.CreateOrUpdateModalSaved");
                    });
                },
                createExtendField: function () {
                    let _this = this;
                    createOrUpdateExtendModal.open({}, function () {
                        _this.loadData();
                    });
                },
                updateExtendField: function (code) {
                    let _this = this;
                    createOrUpdateExtendModal.open({ code: code }, function () {
                        _this.loadData();
                    });
                },
                deleteExtendField: function () {
                    let _this = this;

                    var param = [];
                    _.each($("input.extend"),
                        function(item) {
                            if ($(item)[0].checked) {
                                var id = $(item).data('id');
                                param.push(id);
                            }
                        });

                    if (param.length === 0) {
                        abp.message.error("至少选择一项要删除的字段!");
                        return false;
                    }

                    abp.message.confirm(
                        app.localize("是否删除", ""),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                cuStomFieldService.delete({ id: param }).done(function () {
                                    abp.notify.success(app.localize("SuccessfullyDeleted"));
                                    _this.loadData();
                                });
                            }
                        }
                    );
                },
                loadData: function () {
                    let _this = this;

                    _this.selected = [];
                    _this.fields.basic = [];
                    _this.fields.extend = [];

                    service.getSettingDto({}).done(function (response) {
                        _this.fields.basic = response.basicFields;
                        _this.fields.extend = response.extendFields;
                    });

                    service.listColumns({}).done(function (response) {
                        _this.selected = response;
                    });
                }
            }
        }).$mount('#drawer');



        $(".custom-field").css("min-height", window.innerHeight - 120);

        var soret = new Sortable(document.getElementById('example5'),
            {
                handle: '.handle', // handle's class
                animation: 150
            });
    });
})();
