var app = app || {};
(function () {

    var appLocalizationSource = abp.localization.getSource("BtlCore");
    app.localize = function () {
        return appLocalizationSource.apply(this, arguments);
    };

    app.downloadTempFile = function (file) {
        location.href = abp.appPath + "File/DownloadTempFile?fileType=" + file.fileType + "&fileToken=" + file.fileToken + "&fileName=" + file.fileName;
    };

    app.createDateRangePickerOptions = function () {
        var todayAsString = moment().format("YYYY-MM-DD");
        var options = {
            locale: {
                format: "YYYY-MM-DD",
                applyLabel: app.localize("Apply"),
                cancelLabel: app.localize("Cancel"),
                customRangeLabel: app.localize("CustomRange")
            },
            min: "2015-05-01",
            minDate: "2015-05-01",
            max: todayAsString,
            maxDate: todayAsString,
            ranges: {}
        };

        options.ranges[app.localize("Today")] = [moment().startOf("day"), moment().endOf("day")];
        options.ranges[app.localize("Yesterday")] = [moment().subtract(1, "days").startOf("day"), moment().subtract(1, "days").endOf("day")];
        options.ranges[app.localize("Last7Days")] = [moment().subtract(6, "days").startOf("day"), moment().endOf("day")];
        options.ranges[app.localize("Last30Days")] = [moment().subtract(29, "days").startOf("day"), moment().endOf("day")];
        options.ranges[app.localize("ThisMonth")] = [moment().startOf("month"), moment().endOf("month")];
        options.ranges[app.localize("LastMonth")] = [moment().subtract(1, "month").startOf("month"), moment().subtract(1, "month").endOf("month")];

        return options;
    };

    app.getUserProfilePicturePath = function (profilePictureId) {
        return profilePictureId ?
            (abp.appPath + "Profile/GetProfilePictureById?id=" + profilePictureId) :
            (abp.appPath + "Content/Images/SampleProfilePics/default-profile.png");
    };
    app.getUserProfilePicturePath = function () {
        return abp.appPath + "Profile/GetProfilePicture?v=" + new Date().valueOf();
    };
    app.getShownLinkedUserName = function (linkedUser) {
        if (!abp.multiTenancy.isEnabled) {
            return linkedUser.userName;
        } else {
            if (linkedUser.tenancyName) {
                return linkedUser.tenancyName + "\\" + linkedUser.username;
            } else {
                return ".\\" + linkedUser.username;
            }
        }
    };
    app.notification = app.notification || {};

    app.notification.getUiIconBySeverity = function (severity) {
        switch (severity) {
            case abp.notifications.severity.SUCCESS:
                return "fa fa-check";
            case abp.notifications.severity.WARN:
                return "fa fa-warning";
            case abp.notifications.severity.ERROR:
                return "fa fa-bolt";
            case abp.notifications.severity.FATAL:
                return "fa fa-bomb";
            case abp.notifications.severity.INFO:
            default:
                return "fa fa-info";
        }
    };

    app.dateScope = function (startDate, endDate) {

        var getDate = function (str) {
            var tempDate = new Date();
            var list = str.split("-");
            tempDate.setFullYear(list[0]);
            tempDate.setMonth(list[1] - 1);
            tempDate.setDate(list[2]);
            return tempDate;
        }

        var dateMin = getDate(startDate);
        var dateMax = getDate(endDate);

        if (dateMin == dateMax) {
            return [dateMin.getFullYear() + "-" + (dateMin.getMonth() + 1) + "-" + dateMin.getDate()];
        }

        if (dateMin > dateMax) {
            var tempDate = dateMin;
            dateMin = dateMax;
            dateMax = tempDate;
        }

        var dateArr = [];

        //var dayStr = dateMin.getDate().toString();
        //if (dayStr.length == 1) {
        //    dayStr = "0" + dayStr;
        //}
        //dateArr[0] = dateMin.getFullYear() + "-" + (dateMin.getMonth() + 1) + "-" + dayStr;

        var i = 1;
        while (!(dateMin.getFullYear() == dateMax.getFullYear()
            && dateMin.getMonth() == dateMax.getMonth() && dateMin.getDate() == dateMax
                .getDate())) {

            var dayStr = dateMin.getDate().toString();
            if (dayStr.length == 1) {
                dayStr = "0" + dayStr;
            }

            dateArr[i] = dateMin.getFullYear() + "-" + (dateMin.getMonth() + 1) + "-" + dayStr;
            i++;

            dateMin.setDate(dateMin.getDate() + 1);
        }

        dateArr[i] = dateMax.getFullYear() + "-" + (dateMax.getMonth() + 1) + "-" + dateMax.getDate();

        return dateArr;
    }
})();