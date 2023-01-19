using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class BtlCoreDbContextFactory : IDesignTimeDbContextFactory<BtlCoreDbContext>
    {
        public BtlCoreDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BtlCoreDbContext>();

            try
            {
                BtlCoreDbContextConfigurer.Configure(builder, AppSettings.Database.ConnectionString);

                return new BtlCoreDbContext(builder.Options);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException($"");
            }
        }
    }
}
