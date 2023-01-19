using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Runtime.Security;

namespace Wimi.BtlCore.Emailing
{
    public class SmtpEmailSenderConfiguration : Abp.Net.Mail.Smtp.SmtpEmailSenderConfiguration
    {
        public SmtpEmailSenderConfiguration(ISettingManager settingManager) : base(settingManager)
        {

        }

        public override string Password => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));
    }
}