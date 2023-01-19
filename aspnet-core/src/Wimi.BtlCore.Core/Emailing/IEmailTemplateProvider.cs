namespace Wimi.BtlCore.Emailing
{
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate(int? tenantId);

        string GetDefaultTemplate();
    }
}
