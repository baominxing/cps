using Wimi.BtlCore.AppSystem.Sessions.Dto;

namespace Wimi.BtlCore.Web.Views.Shared.Components.Footer
{
    public class FooterViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public string GetProductNameWithEdition()
        {
            var productName = "Wimi BtlCore";

            if (this.LoginInformations.Tenant != null && this.LoginInformations.Tenant.EditionDisplayName != null)
            {
                productName += " " + this.LoginInformations.Tenant.EditionDisplayName;
            }

            return productName;
        }
    }
}
