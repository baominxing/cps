using Abp;
using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Wimi.BtlCore.KafkaProducer
{
    public interface IKafkaProducerManager: IDomainService
    {
        Task Produce(NameValue<string> mongoData);
    }
}
