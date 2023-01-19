(function () {
    app.modals.LogModal = function () {

        var _modalManager;

        var $jcropApi;

        var $aspectRatio = $('#AspectRatio');

        var $imageUpload;
        var $cropImage = $('#cropImage');
        var $preImg = $("#preImg");
        var $btnCrop = $('#btnCrop');

        var imageName = null;//data.result.result;
        var cropedImageUrl = null;

        var jcropDefaultOpt = {
            boxWidth: 200,
            boxHeight: 200,
            setSelect: [0, 0, 100, 100],
            keySupport: false
        };

        $aspectRatio.change(function() {
            abp.log.debug($aspectRatio.val());

            var opt = $.extend(jcropDefaultOpt, {
                aspectRatio: $aspectRatio.val()
            });

            $jcropApi.setOptions(opt);

        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var opt = $.extend(jcropDefaultOpt, {
                aspectRatio: $aspectRatio.val()
            });
            var url = abp.appPath + 'UploadIoc/Upload';

            $imageUpload = $('#imageUpload').fileupload({
                autoUpload: true,
                url: url,
                dataType: 'json',
                done: function (e, data) {
                    console.log(data);
                    if ($jcropApi) {
                        $jcropApi.destroy();
                    }
                    var imageUrl = abp.appPath + 'Temp/Uploads/' +
                        data.result.result;
                    imageName = data.result.result;
                    $cropImage.attr("src", imageUrl);
                    $cropImage.Jcrop(jcropDefaultOpt, function () {
                        $jcropApi = this;
                        $jcropApi.setImage(imageUrl);
                        $jcropApi.setSelect([0, 0, 100, 100]);
                       
                    });
                    abp.notify.success("上传完成");
                },
                fail: () => {
                    abp.message.error("仅支持Png,Jpeg图片上传!","上传失败");
                }
            });

        };

      

        $btnCrop.click(function() {

            if (imageName == null) {
                abp.message.error(app.localize("PleaseUploadPicturesFirst"));
                return;
            }

            var resizeParams = {};

            if ($jcropApi) {
                resizeParams = $jcropApi.tellSelect();
            }

            var url = abp.appPath + 'UploadIoc/SaveCropImageToIco';

            abp.ajax({
                url: url,
                dataType: 'json',
                type: 'post',
                contentType: "application/json;charset=UTF-8",
                data: JSON.stringify({

                    ImageName: imageName,
                    CropX: parseInt(resizeParams.x),
                    CropY: parseInt(resizeParams.y),
                    CropWidth: parseInt(resizeParams.w),
                    CropHeight: parseInt(resizeParams.h)
                    
                })
            }).done(function (result) {

                var imageUrl = abp.appPath + 'Content/Images/Icos/' + result;

                abp.log.debug(imageUrl);
                $preImg.attr("src", imageUrl);
                cropedImageUrl = result;
            });


        });

        this.save = function () {

            if (cropedImageUrl == null) {
                abp.message.error(app.localize("PleaseCropThePictureFirst"));
                return;
            }

            _modalManager.close();
            abp.notify.success(app.localize("ModifySystemLOGSuccessfully"));
            abp.event.trigger("app.logModalSaved", cropedImageUrl);

        };
    };
})();

