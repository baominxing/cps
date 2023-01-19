using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.DefectiveReasons
{
    [AutoMap(typeof(DefectiveReason))]
    public class DefectiveReasonInputDto : EntityDto
    {
        public string Code { get; set; }

        public string Memo { get; set; }

        public string Name { get; set; }

        public int PartId { get; set; }

        public string PartCode { get; set; }
    }
}
