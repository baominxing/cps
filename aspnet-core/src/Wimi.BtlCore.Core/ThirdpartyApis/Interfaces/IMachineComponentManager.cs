using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using MongoDB.Bson;
using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.ThirdpartyApis.Interfaces
{
    public interface IMachineComponentManager:IDomainService
    {
        Task<ApiResponseObject> ListMachineStateDistribution(string workShopCode);

        Task<ApiResponseObject> ListGanttChart(string workShopCode);

        ApiResponseObject ListMachineAlarming(List<BsonDocument> documents,List<Machine> machines);

        Task<ApiResponseObject> ListRealtimeMachineInfos(List<BsonDocument> documents, List<Machine> machines);

        Task<ApiResponseObject> ListConfigRealtimeMachineInfos(List<BsonDocument> documents, List<Machine> machines);
    }
}