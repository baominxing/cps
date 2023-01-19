using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Extensions;
using Abp.ObjectMapping;
using System.Collections.Generic;
using Wimi.BtlCore.Authorization.Users.Profile.Dto;

namespace Wimi.BtlCore.Web.Models.App.Profile
{
    [AutoMapFrom(typeof(CurrentUserProfileEditDto))]
    public class MySettingsViewModel : CurrentUserProfileEditDto
    {
        public List<ComboboxItemDto> TimezoneItems { get; set; }

        public bool SmsVerificationEnabled { get; set; }

        public bool CanChangeUserName => UserName != AbpUserBase.AdminUserName;

        public string Code { get; set; }

        public MySettingsViewModel(CurrentUserProfileEditDto currentUserProfileEditDto)
        {
            this.Name = currentUserProfileEditDto.Name;
            this.Surname = currentUserProfileEditDto.Surname;
            this.UserName = currentUserProfileEditDto.UserName;
            this.EmailAddress = currentUserProfileEditDto.EmailAddress;
            this.PhoneNumber = currentUserProfileEditDto.PhoneNumber;
            this.IsPhoneNumberConfirmed = currentUserProfileEditDto.IsPhoneNumberConfirmed;
            this.Timezone = currentUserProfileEditDto.Timezone;
            this.QrCodeSetupImageUrl = currentUserProfileEditDto.QrCodeSetupImageUrl;
            this.IsGoogleAuthenticatorEnabled = currentUserProfileEditDto.IsGoogleAuthenticatorEnabled;

        }

        public bool CanVerifyPhoneNumber()
        {
            return SmsVerificationEnabled && !PhoneNumber.IsNullOrEmpty() && !PhoneNumber.Trim().IsNullOrEmpty();
        }
    }
}
