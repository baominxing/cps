using Abp.AutoMapper;
using Wimi.BtlCore.Order.Crafts.Dtos;

namespace Wimi.BtlCore.Web.Models.Order.Craft
{

    [AutoMap(typeof(CraftDto))]
    public class CraftModel : CraftDto
    {
        public CraftModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
