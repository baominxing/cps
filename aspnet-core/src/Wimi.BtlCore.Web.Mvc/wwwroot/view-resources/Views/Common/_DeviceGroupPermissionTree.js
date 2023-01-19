var DeviceGroupPermissionTree = (function ($) {
    return function () {
        var $groupTree = null;
        var pluginsOption = ["types", "wholerow", "changed", "checkbox", "sort"];

        function initGroup($groupTreeContainer, checkboxOption) {
            $groupTree = $groupTreeContainer;
            if (checkboxOption === undefined) {
                checkboxOption = {
                    keep_selected_style: true,
                    three_state: false,
                    cascade: "down",
                    whole_node: true
                };
            }

            $groupTree.jstree({
                "core": {
                    expand_selected_onload: true
                },
                "types": {
                    "default": {
                        "icon": "fa fa-folder tree-item-icon-color icon-lg"
                    },
                    "file": {
                        "icon": "fa fa-file tree-item-icon-color icon-lg"
                    }
                },
                'checkbox': checkboxOption,
                sort: function (node1, node2) {
                    if (this.get_node(node2).seq <= this.get_node(node1).seq) {
                        return -1;
                    }
                    return 1;
                },
                plugins: pluginsOption
            }),
                $groupTree.on("changed.jstree", function (e, data) {
                    if (!data.node) {
                        return;
                    }
                });
        };

        function selectNodeAndAllParents(node) {
            $groupTree.jstree("select_node", node, true);
            var parent = $groupTree.jstree("get_parent", node);
            if (parent) {
                selectNodeAndAllParents(parent);
            }
        };
        function openAll() {
            $groupTree.jstree("open_all");
        }
        function getSelectedPermissionNames() {
            var permissionNames = [];

            var selectedPermissions = $groupTree.jstree("get_selected", true);
            for (var i = 0; i < selectedPermissions.length; i++) {
                permissionNames.push(selectedPermissions[i].id);
            }

            return permissionNames;
        };
        function getSelectedPermissionNames2() {
            var permissionNames = [];

            var selectedPermissions = $groupTree.jstree("get_selected", true);
            for (var i = 0; i < selectedPermissions.length; i++) {
                permissionNames.push(selectedPermissions[i].id);
            }


            $.each($groupTree.jstree("get_undetermined").find(".jstree-undetermined"), function (index, item) {
                permissionNames.push(item.closest(".jstree-node").id);
            });

            return permissionNames;
        }

        return {
            initGroup: initGroup,
            getSelectedPermissionNames: getSelectedPermissionNames,
            getSelectedPermissionNames2: getSelectedPermissionNames2,
            openAll: openAll
        };
    };
})(jQuery);