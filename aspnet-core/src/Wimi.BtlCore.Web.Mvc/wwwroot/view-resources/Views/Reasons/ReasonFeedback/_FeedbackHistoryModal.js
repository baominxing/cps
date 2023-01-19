
(function ($) {
    app.modals.FeedbackHistoryModal = function () {
        var _modalManager;
        var machineId = null;
        var feedbackForm = null;
        var feedbackRecordInfo;
        this.init = function (modalManager,args) {
            _modalManager = modalManager;
            machineId = args.Id;
            feedbackForm = _modalManager.getModal().find("#FeedbackHistoryTable");
            feedbackRecordInfo.tableForm = feedbackForm;
        };
        this.shown= function() {
            feedbackRecordInfo.load();
        }
        feedbackRecordInfo = feedbackRecordInfo = {
            tableForm: null,
            dataTable: null,
            load: function () {
                
                feedbackRecordInfo.dataTable = feedbackRecordInfo.tableForm.WIMIDataTable({
                    "ajax": {
                        url: abp.appAPIPath + "reasonFeedback/listReasonFeedbackHistoryInfo",
                        data: { Id: machineId}
                    },
                    "scrollX": true,
                    "responsive": false,
                    "order": [],
                    "columns": [
                        {
                            data: "stateDisplayName",
                            title: app.localize("Reason"),
                               
                            orderable: false
                        },
                        {
                            data: "startTime",
                            title: app.localize("StartTime"),
                            orderable: false,
                            render: function (data) {
                                return wimi.btl.dateTimeFormat(data);
                            }
                        },
                        {
                            title: app.localize("NewFeedbacks"),
                            searchable: false,
                            orderable: false,
                            data: "feedbackPersonName"
                        },
                        {
                            title: app.localize("EndTime"),
                            searchable: false,
                            orderable: false,
                            data: "endTime",
                            render: function (data) {
                                return moment(data).format("YYYY-MM-DD HH:mm:ss");
                            }
                        },
                        {
                            title: app.localize("EndFeedbackPerson"),
                            searchable: false,
                            orderable: false,
                            data: "endUserName"
                        },
                        {
                            data: "duration",
                            title: app.localize("TimeConsuming") + "(" + app.localize("Minutes") + ")",
                            orderable: false
                        }
                    ]
                });
            }
        };
    };
})()