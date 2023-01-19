using Abp.Configuration;
using Dapper;
using System.Data.SqlClient;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.Cartons
{
    public class CartonRepository : ICartonRepository
    {
        private readonly ISettingManager settingManager;

        private readonly string connectionString;
        public CartonRepository(
            ISettingManager settingManager)
        {
            this.settingManager = settingManager;

            this.connectionString = AppSettings.Database.ConnectionString;
        }


        public bool AppointNumber(string serialNumber)
        {
            bool result = false;
            using (var dbConnection = new SqlConnection(this.connectionString))
            {
                var res = dbConnection.Execute(@"
                       Update CartonSerialNumbers Set Status = 2 Where SerialNumber = @SerialNumber And Status = 1", new { SerialNumber = serialNumber });

                if (res > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        public void InsertCartonSerialNumber(CartonSerialNumber cartonSerialNumber)
        {
            using (var dbConnection = new SqlConnection(this.connectionString))
            {
                var res = dbConnection.Execute(@"INSERT INTO CartonSerialNumbers(SerialNumber, Status, DateKey, CreationTime, CreatorUserId) 
                       VALUES(@SerialNumber, @Status, @DateKey, GETDATE(), 0) ", cartonSerialNumber);

            }
        }
    }
}
