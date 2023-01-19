using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.MultiTenancy;
using System.Collections.Generic;

namespace Wimi.BtlCore
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class BtlCoreAppServiceBase : ApplicationService
    {
        protected const string PlcAlarmNokey = "STD::PLCALARM+A";

        protected const string AlarmNokey = "STD::AlarmNo";

        protected const string AlarmTextKey = "STD::AlarmText";

        protected const string CapacityKey = "STD::YieldCounter";

        protected const string ProgramKey = "STD::Program";

        protected const string StateKey = "STD::Status";

        protected readonly List<string> MachineParamKeys;


        protected BtlCoreAppServiceBase()
        {
            LocalizationSourceName = BtlCoreConsts.LocalizationSourceName;
            MachineParamKeys = new List<string>() {
                AlarmNokey,
                AlarmTextKey,
                ProgramKey,
                StateKey
            };
        }

        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected virtual Tenant GetCurrentTenant()
        {
            using (this.CurrentUnitOfWork.SetTenantId(null))
            {
                return this.TenantManager.GetById(this.AbpSession.GetTenantId());
            }
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual User GetCurrentUser()
        {
            var user = this.UserManager.GetUserById(this.AbpSession.GetUserId());

            if (user == null)
            {
                throw new ApplicationException(this.L("ThereIsNoCurrentUser"));
            }

            return user;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }
    }
}
