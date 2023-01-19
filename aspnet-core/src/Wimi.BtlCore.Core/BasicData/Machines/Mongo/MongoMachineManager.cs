using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BTLMongoDB.Repositories;

namespace Wimi.BtlCore.Machines.Mongo
{
    public class MongoMachineManager: BtlCoreDomainServiceBase
    {
        public readonly MongoDbRepositoryBase<MongoMachine> MongoMachineRepository;

        public MongoMachineManager(MongoDbRepositoryBase<MongoMachine> mongoMachineRepository)
        {
            this.MongoMachineRepository = mongoMachineRepository;
        }

        public void Delete(Machine machine)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == machine.Id);
            this.MongoMachineRepository.DeleteOne(filter);
        }

        public void InsertOrUpdateMongoMachine(Machine machine)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == machine.Id);
            var targetMachine = MongoMachineRepository.Get(filter);

            if (targetMachine == null)
            {
                var mongoMachine = new MongoMachine()
                {
                    MachineCode = machine.Code,
                    MachineId = machine.Id,
                    Name = machine.Name,
                    IsActive = true,
                    DateKey = Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd"))
                };

                MongoMachineRepository.InsertOne(mongoMachine);
            }
            else
            {
                var update = Builders<MongoMachine>.Update.Set(s => s.Name, machine.Name).Set(s => s.IsActive, machine.IsActive);
                MongoMachineRepository.UpdateOne(filter, update);
            }
        }

        public void UpdateMongoMachineUser(int machineId, long userId, int userShiftDeatilId,string shiftItemName)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == machineId);
            var update = Builders<MongoMachine>.Update.Set(s => s.UserId, userId)
                .Set(s => s.UserShiftDetailId, userShiftDeatilId)
                .Set(s => s.ShiftExtras.StaffShiftItemName, shiftItemName);

            MongoMachineRepository.UpdateOne(filter, update);
        }

        public void UpdateMongoMachineState(int machineId, MongoMachine.MachineState state)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == machineId);
            var update = Builders<MongoMachine>.Update.Set(s => s.State, state);
            MongoMachineRepository.UpdateOne(filter, update);
        }

        public void BulkWriteMongoMachineShiftDetailId(List<WriteModel<MongoMachine>> input)
        {
            MongoMachineRepository.Collection.BulkWrite(input);
        }

        public void UpdateDateKey(int machineId)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == machineId);
            var update = Builders<MongoMachine>.Update.Set(s => s.DateKey, Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd")));
            MongoMachineRepository.UpdateOne(filter, update);
        }

        public IEnumerable<MongoMachine> ListMongoMachine()
        {
            return MongoMachineRepository.GetAll();
        }

        public MongoMachine GetMongoMachineByCode(string code)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineCode == code);
            return MongoMachineRepository.Get(filter);
        }

        public MongoMachine GetMongoMachineById(int machineId)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == machineId);
            return MongoMachineRepository.Get(filter);
        }

        public void UpdateMongoMachineShiftDetailId(int machineId, int machineShifitDetailId, MongoMachine.MachineShiftExtras shiftExtras)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == machineId);
            var update = Builders<MongoMachine>.Update.Set(s => s.MachinesShiftDetailId, machineShifitDetailId)
                .Set(s=>s.ShiftExtras.ShiftSolutionName,shiftExtras.ShiftSolutionName)
                .Set(s=>s.ShiftExtras.ShiftDay, shiftExtras.ShiftDay)
                .Set(s=>s.ShiftExtras.MachineShiftItemName,shiftExtras.MachineShiftItemName)
                .Set(s => s.LastModificationTime, DateTime.Now.ToString("yyyyMMdd HH:mm:ss"))
                .Set(s=>s.DateKey,Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd")));

            MongoMachineRepository.UpdateOne(filter, update);
        }

        public  MongoMachine GetMachineInfoFromMongo(string machineCode)
        {
            if (string.IsNullOrEmpty(machineCode)) return new MongoMachine();
            var filter = Builders<MongoMachine>.Filter.Where(s=>s.MachineCode== machineCode);
            return MongoMachineRepository.Get(filter);
        }

        public IEnumerable<MongoMachine> OriginalMongoMachineList(IEnumerable<int> machineIds)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s=>machineIds.Contains(s.MachineId));

            return MongoMachineRepository.GetMany(filter);
        }

        public IEnumerable<MongoMachine> ListOriginalMongoMachines(IEnumerable<string> machineCodeList)
        {
            var filter = Builders<MongoMachine>.Filter;
            var doFilter = filter.Where(s=>machineCodeList.Contains(s.MachineCode)) & filter.Where(s=>s.IsActive== true);
            var result = MongoMachineRepository.GetMany(doFilter);
            return result;
        }

        public List<BsonDocument> ListOriginalMongoMachinesBsonDocument(IEnumerable<string> machineCodeList)
        {
            var filter = Builders<MongoMachine>.Filter;
            var doFilter = filter.Where(s => machineCodeList.Contains(s.MachineCode)) & filter.Where(s => s.IsActive == true);
            var mongoData = MongoMachineRepository.GetMany(doFilter);

            var result = new List<BsonDocument>();
            foreach (var data in mongoData)
            {
                var mongDataBson = BsonSerializer.Deserialize<BsonDocument>(JsonConvert.SerializeObject(data));

                result.Add(mongDataBson);
            }
            return result;
        }

        public  void SaveProductIdIntoMongo(int machineId, int productId, int processId)
        {
            try
            {
                var filter = Builders<MongoMachine>.Filter.Where (s=>s.MachineId== machineId);
                var result = MongoMachineRepository.Get(filter);

                if (result != null)
                {
                    var update = Builders<MongoMachine>.Update.Set(s=>s.ProductId, productId).Set(s=>s.ProcessId, processId);
                    MongoMachineRepository.UpdateOne(filter, update);
                }
            }
            catch (Exception e)
            {
                this.Logger.Fatal("设备换产,操作Mongo失败：", e);
            }

        }
    }
}
