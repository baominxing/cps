(function () {
    $(function () {
        var productService = abp.services.app.product;
        var craftMaintainService = abp.services.app.flexibleCraftMaintain;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Order.Product.Manage")
        };

        var sourceCraft = $("#list-craft-template").html();
        var render = Handlebars.compile(sourceCraft);

        var sourceCutter = $("#list-cutter-template").html();
        var render_Cutter = Handlebars.compile(sourceCutter); 

        var productTree = {
            $tree: $("#productGroupTree"),

            $emptyInfo: $("#productGroupTreeEmptyInfo"),

            show: function () {
                productTree.$emptyInfo.hide();
                productTree.$tree.show();
            },

            hide: function () {
                productTree.$emptyInfo.show();
                productTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                productTree.unitCount = unitCount;
                if (unitCount) {
                    productTree.show();
                } else {
                    productTree.hide();
                }
            },

            refreshUnitCount: function () {
                productTree.setUnitCount(productTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                displayName: null,
                code: null,

                set: function (ouInTree) {
                    if (!ouInTree || ouInTree.parent === "#") {
                        return;
                    }

                    productTree.selectedOu.id = ouInTree.id.replace('_', '');
                    productTree.selectedOu.displayName = ouInTree.original.displayName;
                    productTree.selectedOu.code = ouInTree.original.code;

                    $("#createCraft").show()
                        .off()
                        .on("click", function () {
                            craftList.createCraft(productTree.selectedOu.id);
                        });

                    $("#viewCraftPathMap").show()
                        .off()
                        .on("click", function () { 
                           
                            layer.open({
                                title: ['工艺路线图','font-size:18px'],
                                type: 2,
                                anim: 2,
                                area: [window.innerWidth+'px', window.innerHeight/1.5+'px'],
                                offset: 'b',
                                fixed: true, 
                                maxmin: true,
                                content: '/FlexibleCraftPath/CraftPathMap?id=' + productTree.selectedOu.id
                            });
                        });

                    craftList.loadCraft(productTree.selectedOu.id);
                }
            },

            generateTextOnTree: function (ou) {
                var itemClass = ou.memberCount > 0 ? " ou-text-has-members" : " ou-text-no-members";
                return '<span title="' + ou.code + '" class="ou-text' + itemClass + '" data-ou-id="' + ou.id + '">' + ou.displayName + ' (<span class="ou-text-member-count">' + ou.memberCount + '</span>) <i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = productTree.$tree.jstree("get_node", ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                productTree.$tree.jstree("rename_node", treeNode, productTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                $('.product-loadding').show();
                productService.getProductsTree().done(function (result) {
                    var treeData = _.map(result.items, function (item) {
                        return {
                            'id': item.groupName,
                            'text': item.groupName,
                            'state': {
                                'opened': true
                            },
                            'children': _.map(item.children, function (p) {
                                return { 'id': "_"+p.id, 'text': p.name };
                            })
                        };
                    });

                    callback(treeData);
                }).always(function () {
                    $('.product-loadding').hide();
                });
            },

            init: function () {
                productTree.getTreeDataFromServer(function (treeData) {
                    productTree.setUnitCount(treeData.length);

                    productTree.$tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                productTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                productTree.selectedOu.set(selectedNode);
                            }
                        })
                        .jstree({
                            'core': {
                                data: treeData,
                                multiple: false,
                                check_callback: function (operation, node, node_parent, node_position, more) {
                                    return true;
                                }
                            },
                            types: {
                                "default": {
                                    "icon": "fa fa-folder tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-file tree-item-icon-color icon-lg"
                                }
                            },
                            sort: function (node1, node2) {
                                if (this.get_node(node2).original.displayName < this.get_node(node1).original.displayName) {
                                    return 1;
                                }

                                return -1;
                            },
                            search: {

                            },
                            plugins: [
                                "types",
                                "wholerow",
                                "sort",
                                "dnd",
                                "search"
                            ]
                        });

                    $('#searchProductTree').keyup(function () {
                        productTree.$tree.jstree(true).search($(this).val());
                    });

                });
            },

            reload: function () {
                productTree.getTreeDataFromServer(function (treeData) {
                    productTree.setUnitCount(treeData.length);
                    productTree.$tree.jstree(true).settings.core.data = treeData;
                    productTree.$tree.jstree("refresh");
                });
            }
        };

        var craftList = {
            loadCraft: function (productId) {
                $('.craft-loadding').show();
                craftMaintainService.getCrafts({ ProductId: productId }).done(function (response) { 
                    if (response.length) {
                        $("#accordion_craft").html(render(response));

                        $('#accordion_craft').on('shown.bs.collapse', function (e) {
                            $(e.target).parent().find(".btn-deleteCraft").show()
                                .on("click", function () {
                                    var craft = $(this).siblings("input");
                                    craftList.deleteCraft(craft.val(), craft.attr("data-name"));
                                });

                            $(e.target).parent().find(".btn-editCraft").show()
                                .off()
                                .on("click", function () {
                                    var craftId = $(this).siblings("input").val();
                                    craftList.editCraft(0, craftId);
                                });
                        })
                            .on('hidden.bs.collapse', function () {
                                $(this).find(".btn-deleteCraft").hide();
                                $(this).find(".btn-editCraft").hide();
                            });

                        $("#accordion_craft .box-title a").on("click", function () {
                            var craftId = $(this).attr("data-id");
                            craftList.loadCutter(craftId);
                        });

                        $("#accordion_craft .box-title a").eq(0).click();

                        $("#craftPathMapDrawer ul").html();
                        for (var i = 0; i < response.length; i++) { 
                            var item = response[i];
                            $('<li><input type="hidden" value="' + item.id + '" /><a href="#"><i class="fa fa-filter"></i>' + item.name + '</a></li>')
                                .appendTo("#craftPathMapDrawer ul");
                        }
                        $("#craftPathMapDrawer ul li").on("click", function () {
                            var id = $(this).find("input[type=hidden]").val();
                            craftList.viewCraftPathMap(id);
                        });
                    }
                    else {
                        $("#accordion_craft").html('<p class="text-muted well well-sm no-shadow" style="margin-top: 10px;">没有数据</p>');
                        $("#accordion_cutter").html('<p class="text-muted well well-sm no-shadow" style="margin-top: 10px;">没有数据</p>');
                    }
                })
                    .always(function () {
                        $('.craft-loadding').hide();
                    });
            },
            loadCutter: function (craftId) {
                $('.cutter-loadding').show();
                craftMaintainService.getCraftCutters({ CraftId: craftId }).done(function (response) {
                    if (response.craftPathCutters.length) {
                        $("#accordion_cutter").html(render_Cutter(response.craftPathCutters));
                    }
                    else {
                        $("#accordion_cutter").html('<p class="text-muted well well-sm no-shadow" style="margin-top: 10px;">没有数据</p>');
                    }
                })
                    .always(function () {
                        $('.cutter-loadding').hide();
                    });
            },
            createCraft: function (productId) {
                craftDrawer.open({
                    ProductId: productId,
                    CraftId: null
                });
            },
            editCraft: function (productId, craftId) {
                craftDrawer.open({
                    ProductId: productId,
                    CraftId: craftId
                });
            },
            deleteCraft: function (craftId, craftName) {
                abp.message.confirm("此操作将删除工艺：[" + craftName + "]及所有工序信息",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            craftMaintainService.deleteCraft({ id: craftId }).done(function (response) {
                                abp.notify.info(app.localize("SavedSuccessfully"));
                                craftList.loadCraft(productTree.selectedOu.id);
                            });
                        }
                    });
            }
        };

        var craftDrawer = {
            open: function (args) {
                var viewUrl = abp.appPath + "FlexibleCraftPath/CraftDrawerModal";
                var scriptUrl = abp.appPath + "view-resources/Views/CraftMaintain/FlexibleCraftPath/_CraftDrawerModal.js";
                var id = "Drawer_Modal";
                var _containerSelector = "#" + id;

                var $container = $(_containerSelector);
                if ($container.length) {
                    $container.remove();
                }

                $('<div id="' + id + '" class="drawer dw-xs-6 dw-sm-12 dw-md-5 fold drawer-right" data-toggle="drawer" aria-labelledby="createCraftModel"></div>').appendTo("body")
                    .load(viewUrl, args, function (response, status, xhr) {
                        if (status === "error") {
                            abp.message.warn(abp.localization.abpWeb("InternalServerError"));
                            return;
                        }

                        var $drawer = $("#" + id);

                        $.getScript(scriptUrl)
                            .done(function (script, textStatus) {
                                var craftDrawer = app.modals["craftDrawer"];
                                if (craftDrawer) {
                                    _craftDrawerObject = new craftDrawer();
                                    if (_craftDrawerObject.init) {
                                        _craftDrawerObject.init($drawer);
                                    }
                                }
                            })
                            .fail(function (jqxhr, settings, exception) {
                                abp.message.warn(abp.localization.abpWeb("InternalServerError"));
                            });
                    });
            }
        };

        productTree.init();

        abp.event.on("app.CraftDrawerFinished", function () {
            craftList.loadCraft(productTree.selectedOu.id);
        });
    });
})();