(function () {
    app.modals.ShortcutMenuModal = function () {

        var _modalManager;
        var shortcurMenuAppService = abp.services.app.shortcutMenu;

        var $menuTree = null;
        var hasBindMenus = [];

        this.init = function (modalManager) {
            _modalManager = modalManager;
            abp.log.debug(abp.nav.menus.Mpa);

            shortcurMenuAppService.listBindMenu()
                .done(function (result) {

                    hasBindMenus = _.map(result, function (item) {
                        return item.name;
                    });
                    createTree();
                });



        };


        function createTree() {

            function mapMenu(menuItem) {

                if (menuItem.items.length === 0) {
                    menuItem.isValid = true;
                }

                return {
                    id: menuItem.name,
                    text: menuItem.displayName,
                    icon: menuItem.icon,
                    state: {
                        opened: true,
                        selected: _.contains(hasBindMenus, menuItem.name)// false
                    },
                    data: menuItem,
                    children: menuItem.isValid ? [] : _.map(menuItem.items, mapMenu)
                }
            }

            var datas = _.map(abp.nav.menus.Mpa.items, mapMenu);

            $menuTree = $('#menuTree').jstree({
                "plugins": ["wholerow", "checkbox"],
                "core": {
                    "themes": {
                        "variant": "large"
                    },
                    "data": datas
                }, "checkbox": {
                    "keep_selected_style": false
                }
            });
        }

        this.save = function () {

            var selectMenuNodes = $menuTree.jstree().get_selected(true);
            var selectMenus = _.map(_.filter(selectMenuNodes, function (item) {
                return item.data.isValid;
            }), function (item) {
                return item.data;
            });

            shortcurMenuAppService.bindMenus(selectMenus).done(
                function (result) {
                    _modalManager.close();
                    abp.notify.success(app.localize("ConfigurationShortcutMenuSuccess"));
                    abp.event.trigger("app.shortcutMenuModalSaved");
                });

        };
    };
})();