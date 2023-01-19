namespace Wimi.BtlCore.Visual
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;

    using Wimi.BtlCore.Visual.Dto;

    public interface IVisualAppService : IApplicationService
    {
        Task AddOrUpdateNoticsAsync(GetNoticeInputDto input);

        Task DeleteNotices(EntityDto input);

        Task<GetNoticeOutputDto> GetNoticesForVisual(GetNoticeInputDto input);

        Task<PagedResultDto<GetNoticeOutputDto>> GetNoticesList(GetNoticeInputDto input);

        /// <summary>
        /// 获取设备的实时状态，从Mongo中读取
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<GetRealtimeMachineInfoOutputDto> GetRealtimeMachineInfoFromMongo(
            GetRealtimeMachineInfoInputDto input);

        Task<GetShiftEffDto> GetShiftEffDto(EntityDto<string> input);

        /// <summary>
        /// 获取车间信息
        /// </summary>
        /// <returns></returns>
        Task<List<GetWorkShopsDto>> GetWorkShops();

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ReadFile(ReadFileDto input);

        Task UpdateNoticsActive(EntityDto input);

        /// <summary>
        /// 写配置文件
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool WriteConfigFile(WriteFileDto input);

        Task<IEnumerable<YieldsPerHourDto>> ListYieldsPerHour(EntityDto<string> input);

        Task<IEnumerable<StateRatioDto>> ListStateRatio(EntityDto<string> input);
    }
}