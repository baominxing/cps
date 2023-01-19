using Abp;
using Confluent.Kafka;
using System.Threading.Tasks;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.KafkaProducer
{
    public class KafkaProducerManager : BtlCoreDomainServiceBase, IKafkaProducerManager
    {
        public async Task Produce(NameValue<string> mongoData)
        {
            var config = new ProducerConfig { BootstrapServers = AppSettings.KafkaBootstrapServers };

            using (var p = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("MongoData", new Message<string, string> { Key = mongoData.Name, Value = mongoData.Value });

                    var logMessage = $"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'";

                    Logger.Info(logMessage);
                }
                catch (ProduceException<string, string> e)
                {
                    Logger.Error($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}
