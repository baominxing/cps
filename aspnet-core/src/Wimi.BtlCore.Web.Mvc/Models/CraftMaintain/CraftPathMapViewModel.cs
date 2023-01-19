using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Wimi.BtlCore.Web.Models.CraftMaintain
{
    public class CraftPathMapViewModel
    {
        public List<NameValueDto> Crafts { get; set; }
        = new List<NameValueDto>();
    }
}