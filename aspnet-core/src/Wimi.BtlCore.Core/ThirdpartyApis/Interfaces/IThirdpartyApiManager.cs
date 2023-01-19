using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using MongoDB.Bson;
using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.ThirdpartyApis.Interfaces
{
    public interface IThirdpartyApiManager : IDomainService
    {
        void DeleteNotExistApis(IEnumerable<string> apiCodes);

        void Save(ThirdpartyApiDefinition api);

        ApiResponseObject ListPerHourYieldDemoData(string workShopCode);

        ApiResponseObject ListHourlyMachineYiledDemoData(string workShopCode);

        ApiResponseObject ListHourlyMachineYiledByShiftDayDemoData(string workShopCode);

        ApiResponseObject ListRealtimeMachineInfoDemoData(string workShopCode);

        ApiResponseObject ListNoticesDemoData(string workShopCode);

        ApiResponseObject ListToolWarningsDemoData(string workShopCode);

        ApiResponseObject ListMachineAlarmingDemoData(string workShopCode);

        ApiResponseObject ListMachineStateDistributionDemoData(string workShopCode);

        ApiResponseObject ListMachineActivationDemoData(string workShopCode);

        ApiResponseObject ListCurrentCapacityDemoData(string workShopCode);

        ApiResponseObject ListGanttChartDemoData(string workShopCode);

        Task<ApiResponseObject> ListPlanRateDemoData(string workShopCode);

        ApiResponseObject ListConfigRealtimeMachineInfosDemoData(List<BsonDocument> documents, List<Machine> machines);
    }
} 