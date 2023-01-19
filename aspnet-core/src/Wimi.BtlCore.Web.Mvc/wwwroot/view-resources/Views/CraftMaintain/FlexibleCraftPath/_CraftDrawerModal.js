(function () {
    app.modals.craftDrawer = function () {
        var _$drawer;
        var craftMaintainService = abp.services.app.flexibleCraftMaintain;

        var _cutterLookupModal = app.modals.LookupModal.create({
            title: app.localize("PleaseSelectEquipment"),
            serviceMethod: "commonLookup/findFmsCutters"
        });

        var _findProcesseModal = app.modals.CreateProcesseModal.create();

        this.init = function ($drawer) {
            _$drawer = $drawer;
            var sourceCreateCutter = _$drawer.find("#create-cutter-template").html();
            var render_step2_CreateCraftCutter = Handlebars.compile(sourceCreateCutter);

            var craftCreator = {
                craftProcessTable: null,
                $table: null,
                loadCraftProcessTable: function (processes) {

                    if (!craftCreator.$table) {
                        craftCreator.$table = _$drawer.find("#CraftProcessTable");
                    }

                    if (craftCreator.craftProcessTable) {
                        craftCreator.craftProcessTable.destroy();
                        craftCreator.$table.empty();
                    }

                    craftCreator.craftProcessTable = craftCreator.$table.WIMIDataTable({
                        paging: false,
                        serverSide: false,
                        data: processes,
                        order: [],
                        columns: [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "30px",
                                "className": "text-center not-mobile",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $('<button type="button" class="btn btn-default"><i class="fa fa-trash"></i></button>')
                                        .appendTo($(td))
                                        .click(function () {
                                            abp.message.confirm("将删除该项",
                                                function (isConfirmed) {
                                                    if (isConfirmed) {
                                                        craftCreator.removeCraftProcessTableRow(rowData);
                                                    }
                                                });
                                        });
                                }
                            },
                            {
                                "data": null,
                                "title": app.localize("SerialNumber"),
                                "width": "30px",
                                "orderable": false,
                                "render": function (data, type, row, meta) {
                                    return meta.row + 1 +
                                        meta.settings._iDisplayStart;
                                }
                            },
                            {
                                data: "name",
                                title: app.localize("Name")
                            },
                            {
                                data: "sequence",
                                title: app.localize("Sequence")
                            },
                            {
                                data: "tongName",
                                title: app.localize("TongName")
                            },
                            {
                                data: "programes",
                                title: app.localize("Programes")
                            }
                        ]
                    });
                    $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();//表头错位 
                },
                removeCraftProcessTableRow: function (rowData) {
                    for (var i = 0; i < craftCreator.craft.CraftProcesses.length; i++) {
                        var item = craftCreator.craft.CraftProcesses;
                        if (item[i].id === rowData.id) {
                            craftCreator.craft.CraftProcesses.splice(i, 1);
                        }
                    }
                    craftCreator.loadCraftProcessTable(craftCreator.craft.CraftProcesses);
                },
                craft: {
                    Id: _$drawer.find("#CraftId").val(),
                    ProductId: _$drawer.find("#ProductId").val(),
                    Name: "",
                    Version: "",
                    CraftProcesses: [],
                    CraftProcesseIds: null,
                    CraftProcedureCutters: []
                },
                init: function () {
                    _$drawer.find("#createCraftProcess").on("click", function () {
                        _findProcesseModal.open({}, function (selectedItems) {
                            for (var i = 0; i < selectedItems.length; i++) {
                                debugger
                                var index = craftCreator.craft.CraftProcesses.findIndex(x => x.id === selectedItems[i].id);
                                if (index === -1) {
                                    craftCreator.craft.CraftProcesses.push(selectedItems[i]);
                                    _$drawer.find("#hintSelectProcesse").hide();
                                }
                            }
                            craftCreator.loadCraftProcessTable(craftCreator.craft.CraftProcesses);
                        });
                    });
                    craftCreator.stepOne(craftCreator.craft.Id);
                    _$drawer.drawer('show');


                    abp.event.on("app.createOrEditCraftProcesseFinished", function (data) {
                        craftCreator.craft.CraftProcesses.push(data);
                        _$drawer.find("#hintSelectProcesse").hide();
                        craftCreator.loadCraftProcessTable(craftCreator.craft.CraftProcesses);
                    });
                },
                stepOne: function (craftId) {
                    craftCreator.loadCraftProcessTable([]);

                    if (!craftId || craftId === 0)
                        return;

                    craftMaintainService.stepOne({ Id: craftId }).done(function (response) {
                        _$drawer.find("#CraftId").val(response.id);
                        _$drawer.find("#CraftName").val(response.name);
                        _$drawer.find("#CraftVersion").val(response.version);
                        craftCreator.craft.Id = response.id;
                        craftCreator.craft.Name = response.name;
                        craftCreator.craft.Version = response.version;

                        for (var i = 0; i < response.processes.length; i++) {
                            craftCreator.craft.CraftProcesses.push(response.processes[i]);
                        }

                        craftCreator.loadCraftProcessTable(response.processes);
                    });

                },
                stepTwo: function (input) {
                    craftMaintainService.stepTwo(input).done(function (response) {
                        if (response.length) {
                            craftCreator.cutterTab.load(response);
                        }
                    });
                },
                cutterTab: {
                    load: function (data) {
                        var step2_CreateCraftCutter = _$drawer.find("#step2_CreateCraftCutter");
                        step2_CreateCraftCutter.html(render_step2_CreateCraftCutter(data));
                        step2_CreateCraftCutter.find(".nav li").eq(0).children("a").click();

                        step2_CreateCraftCutter.find(".btn-delete-cutter").on("click", function () {
                            that = this;
                            abp.message.confirm("将删除该项",
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        $(that).parents("tr").remove();
                                    }
                                });
                        });

                        step2_CreateCraftCutter.find(".btn-create").on("click", function (e) {
                            var $tbody = $(this).parents(".tab-pane").find("tbody");
                            var cutterIds = [];
                            $tbody.find("input[type=hidden]").each(function () {
                                cutterIds.push($(this).val());
                            });

                            _cutterLookupModal.open({ }, function (selectedItems) {
                                debugger
                                for (var i = 0; i < selectedItems.length; i++) {
                                    var index = cutterIds.findIndex(x => x === selectedItems[i].value.toString());
                                    if (index === -1) {
                                        var $tr = $('<tr role="row"><td><input type="hidden" value="' + selectedItems[i].value + '" /><button type="button" class="btn btn-default btn-delete-cutter"><i class="fa fa-trash"></i></button></td><td>' + selectedItems[i].name + '</td><td>' + selectedItems[i].value+'</td></tr>');
                                        $tr.appendTo($tbody).find(".btn-delete-cutter").on("click", function () {
                                            that = this;
                                            abp.message.confirm("将删除该项",
                                                function (isConfirmed) {
                                                    if (isConfirmed) {
                                                        $(that).parents("tr").remove();
                                                    }
                                                });
                                        });
                                    }
                                }
                            });
                        });
                    },
                    getCraftProcedureCutters: function () {
                        var step2_CreateCraftCutter = _$drawer.find("#step2_CreateCraftCutter");
                        $.each(step2_CreateCraftCutter.find(".box"), function () {
                            var craftProcesseId = $(this).find(".CraftProcesseId").val();
                            var procedureNumber = $(this).find(".ProcedureNumber").val();

                            $.each($(this).find("tbody tr"), function () {
                                var item = {
                                    CraftProcesseId: craftProcesseId,
                                    ProcedureNumber: procedureNumber,
                                    CutterId: $(this).find("input[type=hidden]").val()
                                };
                                craftCreator.craft.CraftProcedureCutters.push(item);
                            });
                        });
                    }
                },
                dispose: function () {
                    _$drawer.drawer('hide').remove();
                }
            };

            _$drawer.find("#example-basic").steps({
                headerTag: "h3",
                bodyTag: "section",
                transitionEffect: "slideLeft",
                autoFocus: true,
                enableCancelButton: true,
                enableFinishButton: true,
                onStepChanging: function (event, currentIndex, newIndex) {
                    var craft = craftCreator.craft;
                    if (currentIndex === 0) {
                        _$craftCreateForm = _$drawer.find("form[name=craftCreateForm]");
                        if (!_$craftCreateForm.valid()) {
                            return false;
                        }
                        if (!craft.CraftProcesses.length) {
                            _$drawer.find("#hintSelectProcesse").show();
                            return false;
                        }

                        craftCreator.stepTwo({
                            craftId: craft.Id, craftProcesseIds: _.map(craft.CraftProcesses, function (i) {
                                return i.id;
                            })
                        });

                        return true;
                    }
                    if (currentIndex === 1) {

                        return true;
                    }
                },
                onFinishing: function (event, currentIndex) {
                    craftCreator.craft.id = _$drawer.find("#CraftId").val();
                    craftCreator.craft.Name = _$drawer.find("#CraftName").val();
                    craftCreator.craft.Version = _$drawer.find("#CraftVersion").val();
                    craftCreator.craft.CraftProcesseIds = _.pluck(craftCreator.craft.CraftProcesses, "id");
                    craftCreator.cutterTab.getCraftProcedureCutters();
                    craftMaintainService.creatOrEditCraft(craftCreator.craft).done(function (response) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _$drawer.find("#createCraftModel").drawer('hide');
                        abp.event.trigger("app.CraftDrawerFinished");
                        craftCreator.dispose();
                    });

                    return true;
                },
                onCanceled: function (event) {
                    _$drawer.drawer('hide');
                },
                labels: {
                    cancel: "取消",
                    finish: "完成",
                    next: "下一步",
                    previous: "上一步",
                    current: "步骤"
                }
            });

            craftCreator.init();
        };
    };
})();