using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users.Dto;

namespace Wimi.BtlCore.Authorization.Users
{
    [AbpAuthorize]
    public class UserLoginAppService : BtlCoreAppServiceBase, IUserLoginAppService
    {
        private readonly IRepository<UserLoginAttempt, long> userLoginAttemptRepository;

        public UserLoginAppService(IRepository<UserLoginAttempt, long> userLoginAttemptRepository)
        {
            this.userLoginAttemptRepository = userLoginAttemptRepository;
        }

        [DisableAuditing]
        public async Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts()
        {
            var userId = this.AbpSession.GetUserId();

            var loginAttempts =
                await
                this.userLoginAttemptRepository.GetAll()
                    .Where(la => la.UserId == userId)
                    .OrderByDescending(la => la.CreationTime)
                    .Take(10)
                    .ToListAsync();

            var result = new List<UserLoginAttemptDto>();
            ObjectMapper.Map(loginAttempts,result);
            return new ListResultDto<UserLoginAttemptDto>(result);
        }
    }
}
