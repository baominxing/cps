using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.BasicData.MachineSetting
{
    public class AssignCustomerViewModel
    {
        public long? Id { get; set; }

        public List<ComboboxItemDto> TenantList { get; set; }
    }
}
