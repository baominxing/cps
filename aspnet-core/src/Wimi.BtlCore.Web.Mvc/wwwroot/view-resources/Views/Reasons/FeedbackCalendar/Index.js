(function () {
    $(function () {
        var service = abp.services.app.reasonFeedbackCalendar;
        var permissions = { manage: abp.auth.hasPermission("Pages.ReasonFeedback.Calendar.Manage") };
        var page = null;

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "FeedbackCalendar/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Reasons/FeedbackCalendar/CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        var selectMachineModal = new app.ModalManager({
            viewUrl: abp.appPath + "FeedbackCalendar/SelectMachineModal",
            scriptUrl: abp.appPath + "view-resources/Views/Reasons/FeedbackCalendar/_SelectMachineModal.js",
            modalSize:"modal-lg",
            modalClass: "ProcessSelectMachineModal"
        });

        page = {
            $create: $("#create"),
            $search: $("#search"),
            $stateSelect: $("#StateCodeSelect"),
            searchParameters: [],
            $table: $("#table"),
            dataTable: null,
            create: function () {
                createOrUpdateModal.open();
            },
            remove: function (rowData) {
                abp.message.confirm(app.localize("WhetherToDeleteFeedbackCalendar"),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            service.delete({ id: rowData.id })
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

                 page.searchParameters = {
                     Name: $("#Name").val(),
                     StateCode: $("#StateCodeSelect").val()
                };

                page.dataTable = page.$table.WIMIDataTable({
                    "scrollCollapse": true,
                    "scrollX": true,
                    "searching": false,
                    "ajax": {
                        "url": abp.appAPIPath + "reasonFeedbackCalendar/listFeedbackCalendar",
                        "type": "POST",
                        "data": page.searchParameters
                    },
                    "columns": [
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
                                            title: app.localize("DeviceContext"),
                                            clickEvent: function () { selectMachineModal.open(rowData); },
                                            isShow: permissions.manage
                                        },
                                        {
                                            title: app.localize("Delete"),
                                            clickEvent: function () { page.remove(rowData); },
                                            isShow: permissions.manage
                                        }
                                    ]);
                            }
                        },

                        {
                            data: "code",
                            "orderable": false,
                            title: app.localize("Code")
                        },
                        {
                            data: "name",
                            "orderable": false,
                            title: app.localize("RuleName")
                        },
                        {
                            data: "stateName",
                            "orderable": false,
                            title: app.localize("Reason")
                        },
                        {
                            data: "duration",
                            "orderable": false,
                            title: app.localize("Duration") + "(" + app.localize("Minutes")+")"
                        },
                        {
                            data: "cron",
                            "orderable": false,
                            title: app.localize("Cron")
                        },
                        {
                            data: "lastJobState",
                            "orderable": false,
                            title: app.localize("LastJobState"),
                            render: function (data) {
                                if (data === "Scheduled") {
                                    return '<span class="label label-info">' + data + '</span>';
                                }
                                if (data === "Failed") {
                                    return '<span class="label label-danger">' + data + '</span>';
                                }
                                if (!data) {
                                    return "N/A";
                                }
                                return '<span class="label label-success">' + data + '</span>';
                            }
                        },
                        {
                            data: "lastExecution",
                            "orderable": false,
                            title: app.localize("LastExecution"),
                            render: function (data) {
                                return wimi.btl.dateTimeFormat(data);
                            }
                        },
                        {
                            data: "nextExecution",
                            "orderable": false,
                            title: app.localize("NextExecution"),
                            render: function (data) {
                                return wimi.btl.dateTimeFormat(data);
                            }
                        },
                        {
                            data: "error",
                            "orderable": false,
                            title: app.localize("JobException")
                        }
                    ]
                });

                $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();//表头错位
            },
            init: function () {
                page.$create.on("click", function () {
                    page.create();
                });

                page.$search.on("click", function () {
                    page.search();
                });
                this.initStateSelect2();
                page.load();
            },
            initStateSelect2: function() {
                service.listFeedbackState().done(function (response) {
                    var stateCode = [];
                    _.each(response, function (item) {
                        stateCode.push({ id: item.value, text: item.name });
                    });

                   
                    page.$stateSelect.select2({
                        data: stateCode,
                        multiple: false,
                        placeholder: app.localize("PleaseSelect"),
                        minimumResultsForSearch: -1,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainTheFeedbackState");
                            }
                        }
                    });
                });
            }
        };

        page.init();

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
           page.load();
        });
    });

})();