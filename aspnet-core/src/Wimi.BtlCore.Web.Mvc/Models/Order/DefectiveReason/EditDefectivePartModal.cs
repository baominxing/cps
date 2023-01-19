using Abp.AutoMapper;
using Wimi.BtlCore.Order.DefectiveParts;

namespace Wimi.BtlCore.Web.Models.Order.DefectiveReason
{
    [AutoMapFrom(typeof(DefectivePart))]
    public class EditDefectivePartModal
    {
        public string Name { get; set; }

        public long? Id { get; set; }
    }
}
