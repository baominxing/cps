//# sourceURL=dynamicIndexFeedbackModalModal.js
(function () {
    //初始化Machine模板
    function initMachineTemplate(data) {
        var source = $("#machineFeedback-template").html();
        var template = Handlebars.compile(source);
        var html = template(data);
        $('#machineContainer').html(html);
    }
    var node = null;
    //获取到到node的id和text
    function selectNodeCallback(data) {
        node = data.node;
        var machineContainer = $("#machineContainer");
        machineContainer.empty();
        var machineService = abp.services.app.reasonFeedback;
        machineService.listReasonFeedbackInfo({ Id: node.id }).done(function (response) {
            $.each(response, function (index, item) {
                item.startTime = moment(item.startTime).format("YYYY-MM-DD HH:mm:ss");
            });
            initMachineTemplate(response);
        });
    }
    //刷新设备
    function refreshNodeMachine(nodeId) {
        var machineContainer = $("#machineContainer");
        machineContainer.empty();
        var machineService = abp.services.app.reasonFeedback;
        machineService.listReasonFeedbackInfo({ Id: nodeId }).done(function (response) {
            $.each(response, function (index, item) {
                item.startTime = moment(item.startTime).format("YYYY-MM-DD HH:mm:ss");
            });
            initMachineTemplate(response);
        });
    }
    $(function () {
        //初始化设备组树
        var machineTree = new MachinesTree();
        machineTree.initWithoutCheckBoxGroup($("div.machines-group-tree"), selectNodeCallback);
        //获取设备图片
        Handlebars.registerHelper('renderImage',
            function (path) {
                var imagePath;
                if ($.trim(path).length === 0) {
                    imagePath = abp.appPath + 'Content/Images/CNC1-128x128.png';
                } else {
                    imagePath = abp.appPath + path;
                }
                return new Handlebars.SafeString('<img class="img-circle" src="' +
                    imagePath +
                    '" alt="device Avatar" style="height: 60px" >');
            });
        var feedbackModal = new app.ModalManager({
            viewUrl: abp.appPath + "ReasonFeedback/FeedbackModal",
            scriptUrl: abp.appPath + "view-resources/Views/Reasons/ReasonFeedback/_FeedbackModal.js",
            modalClass: "FeedbackModal",
            modalSize: 'modal-lg'
        });
        var finishFeedbackModal = new app.ModalManager({
            viewUrl: abp.appPath + "ReasonFeedback/FinishFeedbackModal",
            scriptUrl: abp.appPath + "view-resources/Views/Reasons/ReasonFeedback/_FinishFeedbackModal.js",
            modalClass: "FinishFeedbackModal"
        });
        var feedbackHistoryModal = new app.ModalManager({
            viewUrl: abp.appPath + "ReasonFeedback/FeedbackHistoryModal",
            scriptUrl: abp.appPath + "view-resources/Views/Reasons/ReasonFeedback/_FeedbackHistoryModal.js",
            modalClass: "FeedbackHistoryModal",
            modalSize: 'modal-lg'
        });
        //为模板元素附加事件
        $("#machineContainer").on('click', '.finishFeedback',
            function () {
                var machineId = $(this).attr("machineId");
                finishFeedbackModal.open({ Id: machineId }, function () {
                    refreshNodeMachine(node.id);
                });
            });
        $("#machineContainer").on('click',
            '.feedback',
            function () {
                var machineId = $(this).attr("machineId");
                feedbackModal.open({ Id: machineId },
                    function () {
                        refreshNodeMachine(node.id);
                    });
            });
        $("#machineContainer").on('click',
            '#btnHistory',
            function () {
               var machineId = $(this).attr("machineId");
                feedbackHistoryModal.open({ Id: machineId });
            });
    });
})();