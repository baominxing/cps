using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.EntityFrameworkCore
{
    public static class BtlCoreDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<BtlCoreDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<BtlCoreDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
