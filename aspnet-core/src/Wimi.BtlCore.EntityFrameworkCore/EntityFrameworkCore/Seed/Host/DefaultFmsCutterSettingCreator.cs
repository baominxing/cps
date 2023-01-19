using Castle.Core.Internal;
using System.Linq.Dynamic.Core;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.EntityFrameworkCore.Seed.Host
{
    public class DefaultFmsCutterSettingCreator
    {
        private readonly BtlCoreDbContext context;

        public DefaultFmsCutterSettingCreator(BtlCoreDbContext context)
        {
            this.context = context;
        }


        public void Create()
        {
            var hasAnyEnity = this.context.FmsCutterSettings.Any();
            if (hasAnyEnity) return;

            var columns = typeof(FmsCutter).GetProperties();

            foreach (var item in columns)
            {
                var orderAttribute = item.GetAttribute<DefaultColumnOrderAttribute>();
                if (orderAttribute == null) continue;

                var name = orderAttribute.DisplayName.IsNullOrEmpty()
                    ? item.Name
                    : orderAttribute.DisplayName;

                this.context.FmsCutterSettings.Add(new FmsCutterSetting(name, orderAttribute.Order));
            }

            this.context.SaveChanges();
        }
    }
}
