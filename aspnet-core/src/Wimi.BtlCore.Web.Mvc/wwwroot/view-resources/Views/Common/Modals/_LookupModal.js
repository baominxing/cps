//# sourceURL=dynamicLookupModalScript.js

//tonny: add sourceURL for debug use, please refer: http://stackoverflow.com/questions/1705952/is-possible-to-debug-dynamic-loading-javascript-by-some-debugger-like-webkit-fi
//about vm, please refer http://stackoverflow.com/questions/20388563/chrome-console-vm

(function () {
    app.modals.LookupModal = function () {

        var _modalManager;

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
                        url: abp.appAPIPath + _options.serviceMethod,
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

            } else {

                _$datatable.ajax.reload(null);
            }

        };

        $(document).on("click", ".checkbox-all", function () {
            var $self = $(this);
            selectedArray = [];

            if ($self.prop("checked")) {
                var data = $("#lookup-modal-table").DataTable().rows().data();
                _.each(data, function (item) {
                    selectedArray.push(item);
                });
            }
        });

        function selectItem($this, item) {

            if (_options.multiSelect) {
                if ($this.prop("checked")) {
                    if (!_.contains(selectedArray, item)) {
                        selectedArray.push(item);
                    }
                } else {
                    selectedArray = _.reject(selectedArray, function (one) {

                        if (item.hasOwnProperty('value')) {
                            return one.value === item.value
                        } else {
                            return one.code === item.code
                        }
                    });
                }
            } else {
                selectedArray = [item];
            }

        }

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        this.shown = function () {
            _options = $.extend(_options, _modalManager.getOptions().lookupOptions);

            _$filterInput = _modalManager.getModal().find(".lookup-filter-text");
            _$filterInput.val(_options.filterText);

            _$table = _modalManager.getModal().find("#lookup-modal-table");

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
        };

        this.save = function () {
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
        };
    };

    app.modals.LookupModal.create = function (lookupOptions) {
        return new app.ModalManager({
            viewUrl: abp.appPath + "Common/LookupModal",
            scriptUrl: abp.appPath + "view-resources/Views/Common/Modals/_LookupModal.js",
            modalClass: "LookupModal",
            lookupOptions: lookupOptions
        });
    };
})();