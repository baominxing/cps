(function () {
    $(function () {

        var productAppService = abp.services.app.product;

        var productGroupTree = null,
            product = null;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Order.Product.Manage")
        };

        var createOrUpdateProductGroup = new app.ModalManager({
            viewUrl: abp.appPath + "Product/CreateOrUpdateProductGroup",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/Product/_CreateOrUpdateProductGroup.js",
            modalClass: "CreateOrUpdateProductGroup"
        });

        var createOrUpdateProduct = new app.ModalManager({
            viewUrl: abp.appPath + "Product/CreateOrUpdateProduct",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/Product/_CreateOrUpdateProduct.js",
            modalClass: "CreateOrUpdateProduct"
        });

        var createOrUpdateProductCraft = new app.ModalManager({
            viewUrl: abp.appPath + "Product/CreateOrUpdateProductCraft",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/Product/_CreateOrUpdateProductCraft.js",
            modalClass: "CreateOrUpdateProductCraft"
        });

        productGroupTree = {
            $tree: $("#productGroupTree"),
            $btnCreateRootCutterType: $("#btnCreateProductGroup"),
            $emptyInfo: $("#productGroupTreeEmptyInfo"),

            show: function () {
                productGroupTree.$emptyInfo.hide();
                productGroupTree.$tree.show();
            },

            hide: function () {
                productGroupTree.$emptyInfo.show();
                productGroupTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                productGroupTree.unitCount = unitCount;
                if (unitCount) {
                    productGroupTree.show();
                } else {
                    productGroupTree.hide();
                }
            },

            refreshUnitCount: function () {
                productGroupTree.setUnitCount(productGroupTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                code: null,
                name: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        productGroupTree.selectedOu.id = null;
                        productGroupTree.selectedOu.code = null;
                        productGroupTree.selectedOu.name = null;
                    } else {
                        productGroupTree.selectedOu.id = ouInTree.id;
                        productGroupTree.selectedOu.code = ouInTree.original.code;
                        productGroupTree.selectedOu.name = ouInTree.original.name;
                    }

                    product.load();
                }
            },

            contextMenu: function (node) {

                var items = {
                    edit: {
                        label: app.localize("Edit"),
                        _disabled: !permissions.manage,
                        action: function () {
                            createOrUpdateProductGroup.open({
                                id: node.id
                            }, function () {
                                productGroupTree.refreshUnitCount();
                                productGroupTree.reload();
                            });
                        }
                    },

                    delete: {
                        label: app.localize("Delete"),
                        _disabled: !permissions.manage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                app.localize("AreYouSureToDeleteProductGroup") +
                                " : " + node.original.name + "？" + app.localize("TheirProcessesWillBeDeleted")+"!",
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        productAppService.deleteProductGroup({
                                            id: node.id
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                                            instance.delete_node(node);
                                            productGroupTree.refreshUnitCount();
                                            productGroupTree.reload();
                                        }).fail(function (err) {
                                            setTimeout(function () { abp.message.error(err.message); }, 500);
                                        });;
                                    }
                                }
                                );
                        }
                    }
                };
                return items;
            },

            addUnit: function () {
                createOrUpdateProductGroup.open({}, function () {
                    productGroupTree.refreshUnitCount();
                    productGroupTree.reload();
                });
            },

            generateTextOnTree: function (ou) {
                return '<span class="ou-text" data-ou-id="' + ou.id + '">' + ou.name + '</span>';
            },

            getTreeDataFromServer: function (callback) {
                productAppService.getProductGroups().done(function (result) {
                    var treeData = _.map(result, function (item) {
                        return {
                            id: item.id,
                            parent: "#",
                            code: item.code,
                            name: item.name,
                            memo: item.memo,
                            text: productGroupTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            init: function () {
                productGroupTree.getTreeDataFromServer(function (treeData) {
                    productGroupTree.setUnitCount(treeData.length);

                    productGroupTree.$tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                productGroupTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                productGroupTree.selectedOu.set(selectedNode);
                            }
                        })
                        .on("ready.jstree", function () {
                            productGroupTree.$tree.jstree(true).select_node('ul > li:first');
                        })
                        .jstree({
                            'core': {
                                data: treeData,
                                multiple: false
                            },
                            types: {
                                "default": {
                                    "icon": "fa fa-th tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-th tree-item-icon-color icon-lg"
                                }
                            },
                            contextmenu: {
                                items: productGroupTree.contextMenu
                            },
                            sort: function (node1, node2) {
                                if (this.get_node(node2).original.name < this.get_node(node1).original.name) {
                                    return 1;
                                }

                                return -1;
                            },
                            plugins: [
                                "types",
                                "contextmenu",
                                "wholerow",
                                "sort",
                                "dnd"
                            ]
                        });

                    productGroupTree.$btnCreateRootCutterType.click(function (e) {
                        e.preventDefault();
                        productGroupTree.addUnit();
                    });

                    productGroupTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            productGroupTree.$tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });
                });
            },

            reload: function () {
                productGroupTree.getTreeDataFromServer(function (treeData) {
                    productGroupTree.setUnitCount(treeData.length);
                    productGroupTree.$tree.jstree(true).settings.core.data = treeData;
                    productGroupTree.$tree.jstree("refresh");
                });
            }
        };

        product = {
            $table: $("#table"),
            $dataTable: null,
            $emptyInfo: $("#productEmptyInfo"),
            $btnCreateProduct: $('#btnCreateProduct'),
            $selectedProductRightTitle: $("#SelectedProductRightTitle"),

            showTable: function () {
                product.$emptyInfo.hide();
                product.$table.show();
                product.$btnCreateProduct.show();
                product.$selectedProductRightTitle.text("[" + productGroupTree.selectedOu.name + "]").show();
            },

            hideTable: function () {
                product.$selectedProductRightTitle.hide();
                product.$btnCreateProduct.hide();
                if (product.$dataTable != null) {
                    product.$dataTable.destroy();
                    product.$dataTable = null;
                    product.$table.empty();
                }
                product.$table.hide();
                product.$emptyInfo.show();
            },

            load: function () {
                if (!productGroupTree.selectedOu.id) {
                    product.hideTable();
                    return;
                }

                product.showTable();

                if (product.$dataTable != null) {
                    product.$dataTable.destroy();
                    product.$dataTable = null;
                    product.$table.empty();
                }

                productAppService.getProducts({ productGroupId: productGroupTree.selectedOu.id }).done(function (result) {
                    var cols = [{
                        title: app.localize("Actions"),
                        data: null,
                        width: "10%",
                        className: "text-center",
                        orderable: false,
                        render: function () {
                            return "";
                        },
                        createdCell: function (td, cellData, rowData, row, col) {
                            $(td).buildActionButtons([
                                {
                                    title: app.localize("Editor"),
                                    clickEvent: function () {
                                        product.edit(rowData);
                                    },
                                    isShow: permissions.manage
                                },
                                {
                                    title: app.localize("Delete"),
                                    clickEvent: function () {
                                        product.remove(rowData);
                                    },
                                    isShow: permissions.manage,
                                    className: "btn-danger"
                                }
                            ]);
                        }
                    },
                    {
                        data: "code",
                        orderable: true,
                        title: app.localize("ProductCode")
                    },
                    {
                        data: "name",
                        orderable: false,
                        title: app.localize("ProductName")
                    },
                    {
                        data: "spec",
                        orderable: false,
                        title: app.localize("ProductSpecification")
                    },
                    {
                        data: "drawingNumber",
                        orderable: false,
                        title: app.localize("DrawingNumber")
                    },
                    {
                        data: "partType",
                        orderable: false,
                        title: app.localize("PartType")
                    },
                    {
                        data: "memo",
                        orderable: false,
                        title: app.localize("Note")
                    },
                    {
                        data: "status",
                        visible: false
                    }
                    ];

                    product.$dataTable = product.$table.WIMIDataTable({
                        serverSide: false,
                        responsive: false,
                        retrieve: true,
                        paging: true,
                        order: [],
                        data: result,
                        columns: cols
                    });
                });
            },

            add: function () {
                var productGroupId = productGroupTree.selectedOu.id;
                if (!productGroupId) {
                    return;
                }

                createOrUpdateProduct.open({ id: null, productGroupId: productGroupId }, function () {
                    productGroupTree.reload();
                    product.load();
                });
            },

            edit: function (rowdata) {
                var productGroupId = productGroupTree.selectedOu.id;
                if (!productGroupId) {
                    return;
                }

                createOrUpdateProduct.open({ id: rowdata.id, productGroupId: productGroupId }, function () {
                    productGroupTree.reload();
                    product.load();
                });
            },

            remove: function (rowdata) {
                productAppService.productIsInProgress({ id: rowdata.id }).done(function(status) {
                    if (status == true) {
                        abp.message.confirm(
                            app.localize("Product")+
                            "[" + rowdata.name + "]" + app.localize("ConfirmDeletionInProgress")+"?",
                            function(isConfirmed) {
                                if (isConfirmed) {
                                    productAppService.deleteProduct({
                                        id: rowdata.id
                                    }).done(function() {
                                        abp.notify.success(app.localize("SuccessfullyRemoved"));
                                        productGroupTree.reload();
                                        product.load();
                                    });
                                }
                            }
                        );
                    } else {
                        abp.message.confirm(
                            app.localize("AreYouSureToDeleteProduct")+
                            "[" + rowdata.name + "]?",
                            function(isConfirmed) {
                                if (isConfirmed) {
                                    productAppService.deleteProduct({
                                        id: rowdata.id
                                    }).done(function() {
                                        abp.notify.success(app.localize("SuccessfullyRemoved"));
                                        productGroupTree.reload();
                                        product.load();
                                    });
                                }
                            }
                        );
                    }
                });
            },

            manageProductCraft: function (rowdata) {
                createOrUpdateProductCraft.open({ productId: productId }, function () {
                    productGroupTree.reload();
                    product.load();
                });
            },

            init: function () {

                product.$btnCreateProduct.click(function (e) {
                    e.preventDefault();
                    product.add();
                });

                product.hideTable();


            }
        };

        productGroupTree.init();
        product.init();
    });
})();