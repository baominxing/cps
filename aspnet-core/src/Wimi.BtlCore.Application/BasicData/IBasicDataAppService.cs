// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBasicDataAppService.cs" company="WimiSoft">
//   WimiSoft Copyright 2017
// </copyright>
// <summary>
//   Defines the IBasicDataAppService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Wimi.BtlCore.BasicData
{
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Wimi.BtlCore.BasicData.Dto;
    using Wimi.BtlCore.Dto;

    public interface IBasicDataAppService : IApplicationService
    {
        #region 设备设定
        Task<PagedResultDto<MachineSettingListDto>> GetMachineSetting(MachineSettingInputDto input);

        Task BatchSave();

        Task AddOrUpdateMachineSetting(MachineSettingListDto input);

        Task<MachineDto> AddOrUpdateMachineInRdbms(MachineSettingListDto input);

        void DeleteMachine(EntityDto id);

        MachineSettingListDto GetMachineSettingById(NullableIdDto input);

        /// <summary>
        /// 测试设备IP是否可以ping通
        /// </summary>
        /// <param name="input"></param>
        void PingTestForMachine(TestPingOrTelnetDto input);

        /// <summary>
        /// 测试设备端口是否可以telnet连通
        /// </summary>
        /// <param name="input"></param>
        void TelnetTestForMachine(TestPingOrTelnetDto input);

        /// <summary>
        /// 测试DMP是否正在启动
        /// </summary>
        void TelnetTesFortDMP();

        // 停用/启用设备
        void EnableOrDisEnableMachine(NullableIdDto input);

        Task<PagedResultDto<MachineTypeDto>> GetMachineTypeList(MachineTypeInputDto input);

        Task<IEnumerable<NameValueDto<int>>> ListMachineTypes();

        IEnumerable<NameValueDto<int>> ListMachineInDeviceGroup(int deviceGroupId);

        Task AddOrUpdateMachineType(MachineTypeDto input);

        Task DeleteMachineType(EntityDto input);

        void CheckImageSize(int size);

        FileDto GetMachinesToExcel();

        Task<FileDto> GetMachinesToXML();

        string GetMachineImagePath(EntityDto<Guid?> input);

        Task<FileDto> ExportMachinesToExcel();
        #endregion

        #region 设备采集配置

        // 设定设备参数
        Task<MachineGatherParamDto> GetMachineGatherParamForEdit(NullableIdDto input);

        Task<MachineGatherParamDto> UpdateGatherParams(UpdateGatherParamsInputDto input);

        Task CopyParameterToMachines(GatherParameterCopyInputDto input);

        Task DeleteMachineGatherParam(EntityDto input);

        Task UpdateMachineGatherParamPriorShow(EntityDto input);

        Task UpdateMachineGatherParamShowState(EntityDto input);

        Task UpdateMachineGatherParamShowParam(EntityDto input);

        #endregion

        #region 状态维护

        Task<PagedResultDto<CreateOrUpdateStateInfoDto>> GetStateInfoList(GetStateInfoListDto input);

        Task CreatOrUpdateStateInfo(CreateOrUpdateStateInfoDto input);

        Task DeleteStateInfo(NullableIdDto input);

        Task<IEnumerable<StateInfoOutputDto>> GetStateForVisual();

         #endregion

        #region 数据导入

        List<ImportDataTablesColumnsDto> GetImportDataTablesColumns(ImportDataInputDto input);

        Task<ImportDataOutputDto> ValidateImportData(ImportDataOutputDto input);
        Task<IEnumerable<StateInfoOutputDto>> GetStateAndReasonForVisual();
        #endregion
    }
}