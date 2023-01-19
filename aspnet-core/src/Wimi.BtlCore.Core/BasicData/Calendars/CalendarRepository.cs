using Abp.Application.Services.Dto;
using Abp.Configuration;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.BasicData.Calendars
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly ISettingManager settingManager;
        private readonly string connectionString;

        public CalendarRepository(ISettingManager settingManager)
        {
            this.settingManager = settingManager;
            connectionString = AppSettings.Database.ConnectionString;
        }

        public async Task<Calendar> GetCalendarsByKey(EntityDto input)
        {
            var sql = @"SELECT * FROM [Calendars]
                        WHERE DateKey = @DateKey
                        ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = await conn.QueryFirstOrDefaultAsync<Calendar>(
                                 sql,
                                 new { DateKey = input.Id });
                return result;
            }
        }
    }
}
