(function () {
    app.modals.CreateProcesseModal = function () {
        var _modalManager;
        var _$CreateProcesseForm = null;
        var _$service = abp.services.app.flexibleCraftMaintain;
        var _$tongService = abp.services.app.tong;

        var _options = {
            serviceMethod: null, //Required
            title: app.localize("SelectAnItem"),
            multiSelect: true,
            loadOnStartup: true,
            showFilter: true,
            filterText: "",
            columnsDefinitions: [
                {
                    "data": "name",
                    "title": app.localize("Name")
                }
            ],
            canSelect: function (item) {
                /* This method can return boolean or a promise which returns boolean.
                 * A false value is used to prevent selection.
                 */
                return true;
            }
        };

        var _$table;
        var _$datatable;
        var _$filterInput;
        var selectedArray = [];

        function refreshTable() {
            if (!$.fn.DataTable.isDataTable(_$table)) {
                var columnsDefinition = (function getColumnsDefinition() {
                    var columns = [
                        {
                            "defaultContent": "",
                            "title": _options.multiSelect ? '<input type="checkbox" class="checkbox-all">' : "",
                            "orderable": false,
                            "width": "30px",
                            "className": "text-center",
                            "createdCell": function (td, cellData, rowData, row, col) {
                                var $controler = _options.multiSelect ? $('<input type="checkbox" class="checkbox-item" name="selected">')
                                    : $('<input type="radio" class="checkbox-item" name="selected">');

                                $controler.appendTo($(td))
                                    .click(function () {
                                        selectItem($(this), rowData);
                                    });
                            }
                        }
                    ];
                    return columns.concat(_options.columnsDefinitions);
                })();

                _$datatable = _$table.WIMIDataTable({
                    "ajax": {
                        url: abp.appAPIPath + "flexibleCraftMaintain/GetCraftProcesses",
                        data: function (d) {
                            var args = _modalManager.getArgs();

                            //todo: need support regex search?
                            d.search = {
                                value: _$filterInput.val(),
                                regex: false
                            };
                            $.extend(d, args);
                        }
                    },
                    "order": [],
                    "autoWidth": false,
                    "columns": columnsDefinition,
                    "serverSide": false
                });
                 
                var checkAll = _modalManager.getModal().find(".checkbox-all");

                checkAll.on("click", function () {
                    var $self = $(this);
                    selectedArray = [];

                    if ($self.prop("checked")) {
                        var data = _$datatable.rows().data();
                        _.each(data, function (item) {
                            selectedArray.push(item);
                        });
                    }
                });
            } else {
                _$datatable.ajax.reload(null);
            }
        };

        function selectItem($this, item) {

            if (_options.multiSelect) {
                if ($this.prop("checked")) {
                    if (!_.contains(selectedArray, item)) {
                        selectedArray.push(item);
                    }
                } else {
                    selectedArray = _.reject(selectedArray, function (one) { return one.value === item.value });
                }
            } else {
                selectedArray = [item];
            }

        }

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _options = $.extend(_options, _modalManager.getOptions().lookupOptions);

            _$filterInput = _modalManager.getModal().find(".lookup-filter-text");
            _$filterInput.val(_options.filterText);

            _$table = _modalManager.getModal().find("#find-process-modal-table");

            _modalManager.getModal()
                .find(".lookup-filter-button")
                .click(function (e) {
                    e.preventDefault();
                    refreshTable();
                });

            _modalManager.getModal()
                .find(".modal-body")
                .keydown(function (e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        refreshTable();
                    }
                });

            if (_options.loadOnStartup) { 
                refreshTable(); 
            }

            _$CreateProcesseForm = _modalManager.getModal().find("form[name=CreateProcesseForm]");
            _$CreateProcesseForm.validate();

            _$tongService.getTongsForSelect().done(function (response) {
                var data = _.map(response,
                    function (item) {
                        return {
                            id: item.value,
                            text: item.name
                        };
                    });

                _modalManager.getModal().find("#TongSelect").select2({
                    data: data,
                    multiple: false,
                    placeholder: app.localize("PleaseSelect"),
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                });
            });
        };

        this.save = function () {
            if ($("#SelectProcesseTab").hasClass("active")) {
                if (selectedArray.length > 0) {

                    var boolOrPromise = _options.canSelect(selectedArray);
                    if (!boolOrPromise) {
                        return;
                    }
                    if (boolOrPromise === true) {
                        _modalManager.setResult(selectedArray);
                        _modalManager.close();
                        return;
                    }

                    //assume as promise
                    boolOrPromise.then(function (result) {
                        if (result) {
                            _modalManager.setResult(selectedArray);
                            _modalManager.close();
                        }
                    });

                } else {
                    abp.message.info(app.localize("PleaseSelectData"));
                }
            }
            else {
                if (!_$CreateProcesseForm.valid()) {
                    return;
                }

                var model = _$CreateProcesseForm.serializeFormToObject();

                model.TongId = _modalManager.getModal().find("#TongSelect option:selected").val();
                _modalManager.setBusy(true);
                _$service.createOrEditCraftProcesse(model).done(function (response) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.close();
                    abp.event.trigger("app.createOrEditCraftProcesseFinished", {
                        id: response.id,
                        name: response.name,
                        sequence: response.sequence,
                        tongName: response.tongName,
                        programes: response.programes
                    });
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
        };

        this.shown = function () {
            $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
        };
    };

    app.modals.CreateProcesseModal.create = function (options) {
        return new app.ModalManager({
            viewUrl: abp.appPath + "FlexibleCraftPath/CreateProcesseModal",
            scriptUrl: abp.appPath + "view-resources/Views/CraftMaintain/FlexibleCraftPath/_CreateProcesseModal.js",
            modalClass: "CreateProcesseModal",
            lookupOptions: $.extend(
                {
                    columnsDefinitions: [
                        {
                            "data": "name",
                            "title": app.localize("Name")
                        },
                        {
                            "data": "tongName",
                            "title": app.localize("TongName")
                        },
                        {
                            "data": "programes",
                            "title": app.localize("Program")
                        }
                    ]
                }, options)
        });
    };
})();