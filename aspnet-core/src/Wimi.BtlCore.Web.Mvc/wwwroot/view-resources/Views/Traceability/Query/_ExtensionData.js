//# sourceURL=extensionDataModal.js
(function () {
    app.modals.extensionDataModal = function () {
        var _modalManager,
            _args,
            service = abp.services.app.trace;

        var language = {
            Measure: app.localize("Measure"),
            MeasureResult: app.localize("MeasurementResult"),
            MeasureResult_PartNo: app.localize("WorkpieceNumber"),
            MeasureResult_Time: app.localize("DetectionTime"),
            MeasureResult_Qualified: app.localize("Result"),
            MeasureResult_ProgId: app.localize("InternalId"),
            MeasureResult_ProgName: app.localize("TestFile"),
            MeasureResult_MeasureParams: app.localize("MeasurementParameters"),
            MeasureResult_MeasureParams_Name: app.localize("Name"),
            MeasureResult_MeasureParams_Value: app.localize("MeasuredValue"),
            MeasureResult_MeasureParams_NominalValue: app.localize("NormalValue"),
            MeasureResult_MeasureParams_Qualified: app.localize("MeasurementResult"),
            MeasureResult_MeasureParams_Code: app.localize("Code"),
            MeasureResult_MeasureParams_IsValid: app.localize("IsQualified"),
            MeasureResult_MeasureParams_LowerTolerance: app.localize("LowerLimitOfDeviation"),
            MeasureResult_MeasureParams_UpperTolerance: app.localize("UpperLimitOfDeviation"),
            QaResult: app.localize("QualityTesting")
        };
        this.L = function (n) {
            return language[n] || n;
        };

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;

            var msg = abp.utils.formatString(app.localize("ProcessParametersOf{0}Processing{1}"), _args.machineName, _args.partNo);
            _modalManager.getModal().find(".modal-title span").text(msg);
        };
        this.shown = function () {
            let _this = this;
            var page = {
                $table: $("#parametersTable"),
                init: function () {
                    service.getTraceFlowExtensionData({ Id: _args.recordId }).done(function (result) {

                        var jsonObj = JSON.parse(result);
                        //{ "MeasureResult:{"PartNo":"P00987","Qualified":true, MeasureParams:[{"Name":"外径35#02","Value":"32","Qualified":true,"NominalValue":35},{"Name":"外径40#03","Value":"2","Qualified":true,"NominalValue":4}]}"}
                        str = '';
                        for (var key in jsonObj) {
                            var obj = jsonObj[key];     //2层

                            if (!obj) continue;

                            str += `<dl class="info-dl">`;

                            if (typeof obj !== 'object') {

                                str += `<dt>${_this.L(key)}: ${obj}</dt>`;
                            } else {

                                str += `<dt>${_this.L(key)}</dt>`;
                                str += '<dd>';
                                for (var k in obj) {           //3
                                    var sVlaue = obj[k];
                                    if (typeof sVlaue !== 'object') {
                                        str += `<p><span class="key">${_this.L(key + '_' + k)}:</span>${sVlaue}</p>`;
                                    } else if ($.isArray(sVlaue) && sVlaue.length) {
                                        str += `<p>${_this.L(key + "_" + k)}</p>`;
                                        str += '<table class="table table-bordered"><tr>';
                                        for (var arrK in sVlaue[0]) {
                                            str += `<th>${_this.L(key + "_" + k + "_" + arrK)}</th>`;
                                        }
                                        str += '</tr>';
                                        sVlaue.forEach(function (item) {
                                            str += '<tr>';
                                            for (var itemK in item) {
                                                str += `<td>${item[itemK]}</td>`;
                                            }
                                            str += '</tr>';
                                        })
                                        str += '</table>';
                                    } else if (sVlaue && typeof sVlaue === 'object' && !$.isArray(sVlaue)) {
                                        str += `<p>${_this.L(key + "_" + k)}</p>`;
                                        for (var innerK in sVlaue) {
                                            str += `<p><span class="key">${_this.L(key + '_' + k + "_" + innerK)}:</span>${sVlaue[innerK]}</p>`;
                                        }
                                    }
                                }
                                str += '</dd>';
                            }

                            str += '</dl>';
                        }

                        _modalManager.getModal().find(".modal-body").html(str);

                    });
                }
            };

            page.init();
        };
    };
})();