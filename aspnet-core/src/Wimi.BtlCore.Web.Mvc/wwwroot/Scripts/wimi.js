var wimi = wimi || {};

wimi.btl = wimi.btl || {};

wimi.btl.dateFormat = function (date) {
    return date == null ? app.localize("NotAssociated") : moment(date).format("YYYY-MM-DD");
};

wimi.btl.dateTimeFormat = function (date) {
    return date == null ? app.localize("NotAssociated") : moment(date).format("YYYY-MM-DD HH:mm:ss");
};

wimi.btl.dateTimeNoSecondFormat = function (date) {
    return date == null ? app.localize("NotAssociated") : moment(date).format("YYYY-MM-DD HH:mm");
};

wimi.btl.addRequiredTag = function ($form) {
    _.each($form.find('[required]'),
        function (item) {
            if ($form.find('td').length === 0) {
                var span = '<span class="text-red pull-left">※</span>';
                $(item).closest("div.form-group").first().prepend(span);
            }
        });
};

wimi.app = wimi.app || {};

wimi.app.name = "";
wimi.app.ico = "";
wimi.app.copyright = "";
