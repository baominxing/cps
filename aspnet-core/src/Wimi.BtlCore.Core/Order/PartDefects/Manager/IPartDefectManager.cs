using Abp.Application.Services.Dto;
using Abp.Domain.Services;
using System.Collections.Generic;

namespace Wimi.BtlCore.Order.PartDefects.Manager
{
    public interface IPartDefectManager : IDomainService
    {
        IEnumerable<NameValueDto> ListDefectiveParts();

        IEnumerable<NameValueDto> ListDefectiveReasonsByPartId(EntityDto input);

        IEnumerable<NameValueDto> ListMachinesByDeviceGroupId(EntityDto input);

        IEnumerable<NameValueDto> ListShift();

        void CheckPartDefectiveInfo(PartDefect input);
    }
}
