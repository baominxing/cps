(function ($) {
    $(function () {

        //Back to my account
        $("#UserProfileBackToMyAccountButton").click(function (e) {
            e.preventDefault();
            abp.ajax({
                url: abp.appPath + "Account/BackToImpersonator"
            });
        });

        //My settings

        var mySettingsModal = new app.ModalManager({
            viewUrl: abp.appPath + "Profile/MySettingsModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Profile/_MySettingsModal.js",
            modalClass: "MySettingsModal"
        });

        $("#UserProfileMySettingsLink").click(function (e) {
            e.preventDefault();
            mySettingsModal.open();
        });

        //Change password

        var changePasswordModal = new app.ModalManager({
            viewUrl: abp.appPath + "Profile/ChangePasswordModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Profile/_ChangePasswordModal.js",
            modalClass: "ChangePasswordModal"
        });

        $("#UserProfileChangePasswordLink").click(function (e) {
            e.preventDefault();
            changePasswordModal.open();
        });

        //Change profile picture

        var changeProfilePictureModal = new app.ModalManager({
            viewUrl: abp.appPath + "Profile/ChangePictureModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Profile/_ChangePictureModal.js",
            modalClass: "ChangeProfilePictureModal"
        });

        //Login attemtps
        var userLoginAttemptsModal = new app.ModalManager({
            viewUrl: abp.appPath + "Users/LoginAttemptsModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Users/_LoginAttemptsModal.js",
            modalClass: "LoginAttemptsModal"
        });

        $("#ShowLoginAttemptsLink").click(function (e) {
            e.preventDefault();
            userLoginAttemptsModal.open();
        });

        $("#UserProfileChangePictureLink").click(function (e) {
            e.preventDefault();
            changeProfilePictureModal.open();
        });



        /*
        $("#ManageLinkedAccountsLink").click(function (e) {
            abp.log.debug('manageLinkedAccountsModal');
            e.preventDefault();
            manageLinkedAccountsModal.open();
        });
        */

        //Manage linked accounts
        var _userLinkService = abp.services.app.userLink;

        var manageLinkedAccountsModal = new app.ModalManager({
            viewUrl: abp.appPath + "Profile/LinkedAccountsModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Profile/_LinkedAccountsModal.js",
            modalClass: "LinkedAccountsModal"
        });

        var getRecentlyLinkedUsers = function () {
            _userLinkService.getRecentlyUsedLinkedUsers()
                .done(function (result) {
                    if (result != null) {
                        loadRecentlyUsedLinkedUsers(result);


                        var _$btnManageLinkedAccountsLink = $("#ManageLinkedAccountsLink");
                        _$btnManageLinkedAccountsLink.click(function (event) {
                            event.preventDefault();
                            abp.log.debug('manageLinkedAccountsModal');
                            openManaverLinkedAccountsModal();
                            return false;
                        });

                        function openManaverLinkedAccountsModal() {
                            manageLinkedAccountsModal.open();
                        }

                        $(".recently-linked-user").click(function (e) {
                            e.preventDefault();
                            var userId = $(this).attr("data-user-id");
                            var tenantId = $(this).attr("data-tenant-id");
                            if (userId) {
                                switchToUser(userId, tenantId);
                            }
                        });
                    }
                });
        };

        function loadRecentlyUsedLinkedUsers(result) {

            var $ul = $("ul#RecentlyUsedLinkedUsers");

            $.each(result.items, function (index, linkedUser) {

                // linkedUser
                var shownUserName = getShownUserName(linkedUser);
                linkedUser.shownUserName = shownUserName;// app.getShownLinkedUserName(linkedUser);
            });

            result.hasLinkedUsers = function () {
                return this.items.length > 0;
            };

            var template = $("#linkedAccountsSubMenuTemplate").html();
            var rendered = Handlebars.compile(template);
            $ul.html(rendered(result));
        }

        function switchToUser(linkedUserId, linkedTenantId) {

            var targetUrl = window.location.pathname + window.location.hash;

            abp.ajax({
                url: abp.appPath + "Account/SwitchToLinkedAccount",
                data: JSON.stringify({
                    targetUserId: linkedUserId,
                    targetTenantId: linkedTenantId,
                    targetUrl: targetUrl
                })
            });
        };

        manageLinkedAccountsModal.onClose(function () {
            getRecentlyLinkedUsers();
        });

        //Notifications
        var _appUserNotificationHelper = new app.UserNotificationHelper();
        var _notificationService = abp.services.app.notification;

        function bindNotificationEvents() {
            $('#setAllNotificationsAsReadLink').click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                _appUserNotificationHelper.setAllAsRead(function () {
                    loadNotifications();
                });
            });

            $('.set-notification-as-read').click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                var notificationId = $(this).attr("data-notification-id");
                if (notificationId) {
                    _appUserNotificationHelper.setAsRead(notificationId, function () {
                        loadNotifications();
                    });
                }
            });

            $('#openNotificationSettingsModalLink').click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                _appUserNotificationHelper.openSettingsModal();
            });
        }

        function loadNotifications() {
            _notificationService.getUserNotifications({
                maxResultCount: 3
            }).done(function (result) {
                result.notifications = [];
                $.each(result.items, function (index, item) {
                    result.notifications.push(_appUserNotificationHelper.format(item));
                });

                var $li = $('#header_notification_bar');

                var template = $('#headerNotificationBarTemplate').html();
                var rendered = Handlebars.compile(template);
                $li.html(rendered(result));

                bindNotificationEvents();
            });
        }

        abp.event.on('abp.notifications.received', function (userNotification) {
            _appUserNotificationHelper.show(userNotification);
            loadNotifications();
        });

        abp.event.on('app.notifications.refresh', function () {
            loadNotifications();
        });

        abp.event.on('app.notifications.read', function (userNotificationId) {
            loadNotifications();
        });


        function getShownUserName(linkedUser) {
            if (!abp.multiTenancy.isEnabled) {
                return linkedUser.username;
            } else {
                if (linkedUser.tenancyName) {
                    return linkedUser.tenancyName + '\\' + linkedUser.username;
                } else {
                    return '.\\' + linkedUser.username;
                }
            }
        };

        function getSentinelLdkFeatures() {

            $.get(abp.appPath + "layout/GetSentinelLDKFeatures", function (data) {
                var sentinelLdkFeatures = [],
                    message = "", 
                    alertClass = "alert-danger", 
                    alertIconClass = "fa-ban";

                //illegal json, thus use eval to convert to javascript object
                eval("sentinelLdkFeatures=[" + data + "]");

                if (!sentinelLdkFeatures
                    || !sentinelLdkFeatures.length
                    || sentinelLdkFeatures.length < 2) return;

                //current active feature is before the last object
                var currentLdkIndex = sentinelLdkFeatures.length - 2;
                var currentLdk = sentinelLdkFeatures[currentLdkIndex];

                    if (currentLdk.typ === "HASP Master") {
                        //master ignore
                        return;
                    }

                    if (currentLdk.lic === "Perpetual") {
                    return;
                }

                if (currentLdk.lic.indexOf("Disabled in VM") > -1) {
                    message = currentLdk.lic.replace(/Disabled in VM/, app.localize("ProhibitProductAuthorization"));
                }

                if (message.length === 0) {
                    if (parseInt(currentLdk.dis)) {
                        message = app.localize("AuthorizationIsUnable");
                    }
                }
                if (message.length === 0) {
                    if (parseInt(currentLdk.ex)) {
                        message = app.localize("AuthorizationExpired");
                    }
                }
                if (message.length === 0) {
                    if (parseInt(currentLdk.unusable)) {
                        message = app.localize("AuthorizationCannotUse");
                    }
                }
                if (message.length === 0) {
                    if (currentLdk.lic.length > 2) {

                        var $nobrs = $(currentLdk.lic).filter('nobr');

                        if (currentLdk.lic.indexOf('Not&nbsp;started') > -1) {
                            //Not active yet
                            return;
                        }

                        if ($nobrs.length === 3 && $nobrs[2].innerHTML.indexOf('&nbsp;End:') >= 0) {

                            var dateString = $nobrs[2].innerHTML.replace(/&nbsp;End:/g, '');
                            var diffDays = moment(dateString).diff(moment(), 'days');

                            //check appwimi setting
                            if (!$.isNumeric($.WIMI.options.checkSentinelLdk.warningThreshold)
                                || $.WIMI.options.checkSentinelLdk.warningThreshold * 1 < 0) {
                                $.WIMI.options.checkSentinelLdk.warningThreshold = 7;
                            }

                            var warningThreshold = $.WIMI.options.checkSentinelLdk.warningThreshold;

                            if (diffDays <= warningThreshold && diffDays >= 0) {

                                alertClass = 'alert-warning';
                                alertIconClass = 'fa-warning';

                                message += app.localize("AuthorizationWillExpireTo{0}", moment(dateString).format('YYYY/MM/DD'));
                            }

                            if (diffDays < 0) {
                                message = app.localize("AuthorizationExpired");
                            }
                        }
                    }
                }

                if (message.length === 0) {
                    return;
                }

                var infoCnt = '<div style="padding:10px;padding-bottom:0;"></div>';
                var errorAlertCnt = '<div class="alert ' + alertClass + ' alert-dismissible" style="margin:0;padding:10px;padding-right:30px;">' +
                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>' +
                 '<h4><i class="icon fa ' + alertIconClass + '"></i> ' + message + '</h4>' +
                 '</div>';

                $(".content-wrapper").prepend($(infoCnt).append($(errorAlertCnt)));

            }, "text")
             .error(function () {
                 abp.log.error("ajax error, get sentinel LDK features failed");
             });
        }

        function init() {
            loadNotifications();
            getRecentlyLinkedUsers();

            if ($.WIMI.options.checkSentinelLdk.switchState) {
                getSentinelLdkFeatures();
            }
        }

        init();
    });
})(jQuery);