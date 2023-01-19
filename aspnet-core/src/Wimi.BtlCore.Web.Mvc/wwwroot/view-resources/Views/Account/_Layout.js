var AccountLayout = function () {

    var handleLayout = function () {
        var setting = abp.setting;
        var $appNameContainer = $("#app-name");

        if ($appNameContainer) {
            $appNameContainer.text(setting.get("App.Configuration.VersionSetting.AppName"));
        }
    };

    return {
        init: function () {

            handleLayout();
        }
    };
}();