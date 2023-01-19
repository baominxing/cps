//# sourceURL=dynamic_MachinesTree.js
var MachinesTree = (function ($) {
    return function () {
        var $tree = null;
        var $groupTree = null;
        var pluginsOption = ["types", "wholerow", "changed", "sort", "massload"];
        var defaultSelectedCount = 10;//默认查询设备数

        function init($treeContainer, ischeckbox, changedCallBack) {

            var setCount = $(".DefaultSearchMachineCount").val();

            if (setCount != null && setCount > 0) {
                defaultSelectedCount = setCount;
            }

            $tree = $treeContainer;
            if (ischeckbox) {
                pluginsOption.push("checkbox");
            }
            $tree.jstree({
                "core": {
                    expand_selected_onload: false
                },
                "types": {
                    "default": {
                        "icon": "fa fa-circle-o"
                    }
                },
                'checkbox': {
                    keep_selected_style: true,
                    three_state: true,
                    whole_node: true
                },
                sort: function (node1, node2) {
                    //if ($.trim(this.get_node(node2).text) < $.trim(this.get_node(node1).text)) {
                    //    return 1;
                    //}
                    //return -1;
                },
                plugins: pluginsOption
            }).on("changed.jstree", function (e, data) {
                if (changedCallBack) {
                    changedCallBack(data);
                }
            }).on("ready.jstree", function () {
                $treeContainer.removeClass("hidden");
            }).on("after_open.jstree", function () {
                initMoreAndLess();
            }); ;
        };

        function initMoreAndLess() {
            $(".moreAndless").children().remove();
            $(".moreAndless").append('<a class="jstree-anchor" href="#" tabindex="-1"><i class="jstree-icon jstree-themeicon glyphicon glyphicon-menu-right jstree-themeicon-custom" role="presentation"></i>查看更多</a>');
        };

        function initGroup($groupTreeContainer) {
            $groupTree = $groupTreeContainer;
            if (!_.contains(pluginsOption, "checkbox")) {
                pluginsOption.push("checkbox");
            }
            $groupTree.jstree({
                "core": {
                    expand_selected_onload: false
                },
                'checkbox': {
                    keep_selected_style: true,
                    three_state: false,
                    cascade: "down",
                    whole_node: true
                },
                sort: function (node1, node2) {
                    if (this.get_node(node2).original.seq <= this.get_node(node1).original.seq) {
                        return -1;
                    }

                    return 1;
                },
                plugins: pluginsOption
            });

            $groupTree.on("changed.jstree",
                function (e, data) {
                    if (!data.node) {
                        return;
                    }
                }).on("ready.jstree",
                function () {
                    $groupTreeContainer.removeClass("hidden");
                });
        };
        function initWithoutCheckBoxGroup($groupTreeContainer,callback) {
            $groupTree = $groupTreeContainer;
            $groupTree.jstree({
                "core": {
                    expand_selected_onload:true
                },
                'checkbox': {
                    keep_selected_style: true,
                    three_state: false,
                    cascade: "down",
                    whole_node: false
                },
                sort: function (node1, node2) {
                    if (this.get_node(node2).original.seq <= this.get_node(node1).original.seq) {
                        return -1;
                    }

                    return 1;
                },
                plugins: pluginsOption
            });

            $groupTree.on("changed.jstree",
                function(e, data) {
                    if (!data.node) {
                        return;
                    }
                    if (callback)
                        callback(data);
                }).on("ready.jstree",
                function(e, data) {
                    $groupTreeContainer.removeClass("hidden");
                    setGroupTreeFirstNode();
                    $groupTree.jstree('open_all');
                });
        };
        // jstree 如何使全选，jstree 会返回一个空集合
        function allMachinIsSelectedWithJsonTree() {

            function machinData(item) {
                return item.data;
            }

            function mapMachin(item) {
                return item.isMachine != null && item.isMachine;
            }

            var selectDatas = _.map($groupTree.jstree()
                    .get_selected(true), machinData);

            var machines = _.filter(selectDatas, mapMachin);

            var machineIds = _.uniq(_.map(machines, "id"));
            return machineIds;

        }

        function selectMachineTreeByDefault() {
            var nodes = $tree.jstree().get_node($(".machines-tree"));
            var count = 0;
            _.each(nodes.children_d, function (d) {

                if (d.indexOf(_.first(nodes.children) + "#") === 0 && $tree.jstree().get_node(d).data.hasOwnProperty("machineid") && count < defaultSelectedCount) {
                    $tree.jstree().select_node(d);
                    count++;
                }
            });

        }

        function initGroupWithJson($treeContainer, jsonData, ischeckbox, loadAllCallback) {
            $treeContainer.jstree(':jstree').jstree('destroy');
            $groupTree = $treeContainer;
            if (ischeckbox) {
                pluginsOption.push("checkbox");
            }
            $groupTree.jstree({
                "core": {
                    expand_selected_onload: false,
                    data: jsonData
                },
                'checkbox': {
                    keep_selected_style: true,
                    three_state: true,
                    whole_node: true
                },
                sort: function (node1, node2) {
                    if (this.get_node(node2).original.seq <= this.get_node(node1).original.seq) {
                        return -1;
                    }

                    return 1;
                },
                plugins: pluginsOption
            }).on("changed.jstree", function (e, data) {

            }).on("loaded.jstree", function (e, data) {
                if (loadAllCallback) {
                    loadAllCallback();
                }
                }); 
        };

        function getSelectedNode() {
            return $tree.jstree("get_selected", true);
        }

        function getSelectedGroupNode() {
            return $groupTree.jstree().get_selected(true);
        }

        function selectAll() {
            //selectMachineTree();
            selectMachineTreeByDefault();
            selectGroupTree();
        }

        function selectMachineTree() {
            $tree.jstree("select_all", true);
        }

        //根据默认查询设备数，选中设备
        function selectMachineTreeByDefault() {
            var nodes = $tree.jstree().get_node($(".machines-tree"));
            var count = 0;
            _.each(nodes.children_d, function (d) {

                if (d.indexOf(_.first(nodes.children) + "#") === 0 && $tree.jstree().get_node(d).data.hasOwnProperty("machineid") && count < defaultSelectedCount) {
                    $tree.jstree().select_node(d);
                    count++;
                }
            });

        }

        function openFirstMachineTreeNode() {
            $tree.jstree("open_node", "li:first");
        }

        function selectGroupTree() {
            $groupTree.jstree("select_all", true);
        }

        function getQueryType() {
            return $("a[href='#tab_10-10']").closest("li").hasClass("active") ? "1" : "0";
        }

        function getAllMachineNode() {
            return $tree.jstree().get_node(true);
        }

        function selectFirst() {
            var nodes = $tree.jstree().get_node($(".machines-tree"));
            var id = _.find(nodes.children_d, function (d) {
                return d.indexOf(_.first(nodes.children)) === 0 && $tree.jstree().get_node(d).data.hasOwnProperty("machineid");
            });
            if (id) {
                $tree.jstree().select_node(id);
            }
        }

        function getAllMachineGroupNode() {
            return $groupTree.jstree().get_node(true);
        }

        function getSelectedMachineIds() {
            var selectedNodeIdList = [];
            _.each(getSelectedNode(),
                function (key) {
                    if (key.data.hasOwnProperty('machineid'))
                        selectedNodeIdList.push(key.data.machineid);
                });
            return _.uniq(selectedNodeIdList);
        }

        function getSelectedGroupIds() {
            return _.uniq(_.pluck(getSelectedGroupNode(), "id"));
        }

        function setGroupTreeFirstNode() {
            return $groupTree.jstree().select_node('ul > li:first');
        }

        function selectMachineFirstNode() {
            return $tree.jstree().select_node('ul > li:first');
        }

        function selectGroupTreeNode(id) {
            return $groupTree.jstree().select_node(id);
        }

        function setMachineTreeOpenAll() {
            $tree.jstree('open_all');
        }

        function setPluginsOption(option) {
            pluginsOption = option;
        }

        return {
            init: init,
            initGroup: initGroup,
            initWithoutCheckBoxGroup: initWithoutCheckBoxGroup,
            initGroupWithJson: initGroupWithJson,
            getSelectedNode: getSelectedNode,
            getSelectedMachineIds: getSelectedMachineIds,
            getSelectedGroupNode: getSelectedGroupNode,
            getSelectedGroupIds: getSelectedGroupIds,
            getAllNode: getAllMachineNode,
            getAllGroupNode: getAllMachineGroupNode,
            setSelectAll: selectAll,
            setSelectMachinesTree: selectMachineTree,
            setSelectGroupsTree: selectGroupTree,
            getQueryType: getQueryType,
            allMachinIsSelectedWithJsonTree: allMachinIsSelectedWithJsonTree,
            selectFirst: selectFirst,
            setGroupTreeFirstNode: setGroupTreeFirstNode,
            setMachineTreeOpenAll: setMachineTreeOpenAll,
            selectAll,
            selectMachineTree,
            selectGroupTree,
            selectGroupTreeNode: selectGroupTreeNode,
            selectMachineFirstNode: selectMachineFirstNode,
            openFirstMachineTreeNode,
            setPluginsOption: setPluginsOption,
            initMoreAndLess: initMoreAndLess,
            selectMachineTreeByDefault: selectMachineTreeByDefault
        };
    };
})(jQuery);