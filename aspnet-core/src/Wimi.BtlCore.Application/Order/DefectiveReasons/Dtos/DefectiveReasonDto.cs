using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace Wimi.BtlCore.Order.DefectiveReasons.Dtos
{
    [AutoMap(typeof(DefectiveReason))]
    public class DefectiveReasonDto : EntityDto<int>
    {
        public string Code { get; set; }

        public string Memo { get; set; }

        public string Name { get; set; }

        public string CreateUserName { get; set; }

        public DateTime? CreationTime { get; set; }

        public int PartId { get; set; }

        public string PartCode { get; set; }
    }
}
