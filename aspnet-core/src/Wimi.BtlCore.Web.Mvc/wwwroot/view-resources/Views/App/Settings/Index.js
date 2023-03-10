(function(wimi) {
    $(function () {

        var _tenantSettingsService = abp.services.app.tenantSettings;
        var _initialTimeZone = $("#GeneralSettingsForm [name=Timezone]").val();
        var _usingDefaultTimeZone = $("#GeneralSettingsForm [name=TimezoneForComparison]").val() === abp.setting.values["Abp.Timing.TimeZone"];

        var _logModal = new app.ModalManager({
            viewUrl: abp.appPath + "Settings/LogModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Settings/_LogModal.js",
            modalClass: "LogModal"
        });

        var $btn_update_log = $('#btn_update_log');
        $btn_update_log.click(function (event) {
            _logModal.open();

        });

        //Toggle form based registration options
        var _$selfRegistrationOptions = $("#FormBasedRegistrationSettingsForm")
            .find("input[name=IsNewRegisteredUserActiveByDefault], input[name=UseCaptchaOnRegistration]")
            .closest(".md-checkbox");

        function toggleSelfRegistrationOptions() {
            if ($("#Setting_AllowSelfRegistration").is(":checked")) {
                _$selfRegistrationOptions.slideDown("fast");
            } else {
                _$selfRegistrationOptions.slideUp("fast");
            }
        }

        $("#Setting_AllowSelfRegistration").change(function() {
            toggleSelfRegistrationOptions();
        });

        toggleSelfRegistrationOptions();

        //Toggle SMTP credentials
        var _$smtpCredentialFormGroups = $("#EmailSmtpSettingsForm")
            .find("input[name=SmtpDomain], input[name=SmtpUserName], input[name=SmtpPassword]")
            .closest(".form-group");

        function toggleSmtpCredentialFormGroups() {
            if ($("#Settings_SmtpUseDefaultCredentials").is(":checked")) {
                _$smtpCredentialFormGroups.slideUp("fast");
            } else {
                _$smtpCredentialFormGroups.slideDown("fast");
            }
        }

        $("#Settings_SmtpUseDefaultCredentials").change(function() {
            toggleSmtpCredentialFormGroups();
        });

        toggleSmtpCredentialFormGroups();

        //Toggle LDAP credentials
        var _$ldapCredentialFormGroups = $("#LdapSettingsForm")
            .find("input[name=Domain], input[name=UserName], input[name=Password]")
            .closest(".form-group");

        function toggleLdapCredentialFormGroups() {
            if ($("#Setting_LdapIsEnabled").is(":checked")) {
                _$ldapCredentialFormGroups.slideDown("fast");
            } else {
                _$ldapCredentialFormGroups.slideUp("fast");
            }
        }

        toggleLdapCredentialFormGroups();

        $("#Setting_LdapIsEnabled").change(function() {
            toggleLdapCredentialFormGroups();
        });

        //Save settings
        $("#SaveAllSettingsButton").click(function() {
            _tenantSettingsService.updateAllSettings({
                general: $("#GeneralSettingsForm").serializeFormToObject(),
                userManagement: $.extend($("#FormBasedRegistrationSettingsForm").serializeFormToObject(), $("#OtherSettingsForm").serializeFormToObject()),
                email: $("#EmailSmtpSettingsForm").serializeFormToObject(),
                version: $("#VersionSettingsForm").serializeFormToObject(),
                ldap: $("#LdapSettingsForm").serializeFormToObject(),
                cutter: $("#SettingsCutterForm").serializeFormToObject(),
            }).done(function() {
                abp.notify.info(app.localize("SavedSuccessfully"));

                var newTimezone = $("#GeneralSettingsForm [name=Timezone]").val();
                if (abp.clock.provider.supportsMultipleTimezone &&
                    _usingDefaultTimeZone &&
                    _initialTimeZone !== newTimezone) {
                    abp.message.info(app.localize("TimeZoneSettingChangedRefreshPageNotification")).done(function() {
                        window.location.reload();
                    });
                }

            });
        });

        abp.event.on("app.logModalSaved", function (e) {
            
            var imageUrl = abp.appPath + 'Content/Images/Icos/' + e;
            
            $("#icoImg").attr("src", imageUrl);

            $('input[name="Ico"]').val(e);
        });

    });
})(wimi);