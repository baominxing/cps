//# sourceURL=dynamic_CreateOrUpdateExtendModal.js
(function () {
    app.modals.CreateOrUpdateExtendModal = function () {

        var service = abp.services.app.customField,
            _modalManager,
            form = null;

        function initPage() {
            var selectedId = $('#DisplayType').val();
            setPageStyle(selectedId);
        };

        function setPageStyle(selectedId) {
            if (selectedId !== "1") {
                $("#backupValue").removeClass("hidden");
                $("#textboxField").addClass("hidden");
            } else {
                $("#backupValue").addClass("hidden");
                $("#textboxField").removeClass("hidden");
                $("input.extend-field").val('');
            }
        }

        this.init = function (modalManager, args) {
            _modalManager = modalManager;

            initPage();

            $("#DisplayType").select2({ minimumResultsForSearch: -1 })
                .on("select2:select",
                    function (e) {
                        var selectedId = e.params.data.id;
                        setPageStyle(selectedId);
                    });

            $(".select2-container").css("width", "100%");

            form = _modalManager.getModal().find('Form[name="CreateOrUpdateForm"]');

            $("#createNewRow").on('click',
                function () {
                    $("#backupValue").append(
                        '<div><div class="col-md-10"><input type="text" class="form-control md-mv-5 extend-field"/></div> <div class="col-md-2 pull-right"> <div class="btn btn-link extend-delete" style="line-height:2">删除</div></div></div>');
                });

            $("#backupValue").on('click',
                '.extend-delete',
                function () {
                    $(this).parent().parent().remove();
                });
        };

        this.save = function () {
            if (!form.valid()) {
                return false;
            }
            var param = form.serializeFormToObject();
            param.IsRequired = $('#IsRequired')[0].checked;
            param.Required = param.IsRequired?"required":"";
            

            var renderHtml = "";
            var data = [];
            _.each($('input.extend-field'),
                function (item) {
                    var value = $(item).val();
                    $(item).attr("value", value);
                    data.push({ value: value });
                });


            var renderParam = {
                item: data,
                param:param
            };

            // 构建渲染Dom
            switch (param.DisplayType) {
                case "1":
                    {
                        var textBoxRender = Handlebars.compile($("#textbox-template").html());
                        renderHtml = textBoxRender(renderParam);
                        break;
                    }
                case "2":
                    {
                        var selectRender = Handlebars.compile($("#select-template").html());
                        renderHtml = selectRender(renderParam);
                        break;
                    }
                case "3":
                    {
                        var checkboxRender = Handlebars.compile($("#checkbox-template").html());
                        renderHtml = checkboxRender(renderParam);
                        break;
                    }
                case "4":
                    {
                        var radioRender = Handlebars.compile($("#radio-template").html());
                        renderHtml = radioRender(renderParam);
                        break;
                    }
            };

            param.HtmlTemplate = $.trim($("#backupValue").html());
            param.RenderHtml = $.trim(renderHtml);
           

            _modalManager.setBusy(true);
            service.createOrUpdate(param).done(function (response) {
                _modalManager.setResult(response);
                abp.event.trigger("app.CreateOrUpdateModalSaved");
                _modalManager.close();

            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);