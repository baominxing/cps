using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.DefectiveParts;
using Wimi.BtlCore.Order.DefectiveReasons.Dtos;
using WIMI.BTL.Wimi.BtlCore.Order.DefectiveReasons;
using System.Linq.Dynamic.Core;

namespace Wimi.BtlCore.Order.DefectiveReasons
{

    [AbpAuthorize(PermissionNames.Pages_Order_DefectiveReasons)]
    public class DefectiveReasonsAppService : BtlCoreAppServiceBase, IDefectiveReasonsAppService
    {
        private readonly IRepository<DefectiveReason> defectiveReasonRepository;
        private readonly IRepository<User,long> userRepository;
        private readonly IRepository<DefectivePart> defectivePartRepository;
        private readonly DefectiveReasonManager defectiveReasonManager;
        private readonly IRepository<DefectivePartReason> defectivePartReasonRepository;

        public DefectiveReasonsAppService(IRepository<DefectiveReason> defectiveReasonRepository,
            IRepository<User,long> userRepository,
            IRepository<DefectivePart> defectivePartRepository,
            DefectiveReasonManager defectiveReasonManager,
            IRepository<DefectivePartReason> defectivePartReasonRepository)
        {
            this.defectiveReasonRepository = defectiveReasonRepository;
            this.userRepository = userRepository;
            this.defectivePartRepository = defectivePartRepository;
            this.defectiveReasonManager = defectiveReasonManager;
            this.defectivePartReasonRepository = defectivePartReasonRepository;
        }

        /// <summary>
        /// 新建或者修改次品原因
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateDefectiveReason(DefectiveReasonInputDto input)
        {
            await this.defectiveReasonManager.CreateOrUpdateReasonAsync(input);
        }
       
        public async Task<DefectivePartDto> CreateDefectivePart(CreateDefectivePartDto input)
        {
            var defectivePart = new DefectivePart(input.Name,input.ParentId);
            await defectiveReasonManager.CreateAsync(defectivePart);
            // 页面需要保存后的Id，手动保存一次
            await CurrentUnitOfWork.SaveChangesAsync();
         
            return ObjectMapper.Map<DefectivePartDto>(defectivePart);
        }

        public async Task<DefectivePartDto> UpdateDefectivePart(UpdateDefectivePartDto input)
        {
            var defectivePart = await defectivePartRepository.GetAsync(input.Id);
            defectivePart.Name = input.Name;
            await defectiveReasonManager.UpdateAsync(defectivePart);
            var defectivePartDto = ObjectMapper.Map<DefectivePartDto>(defectivePart);
            defectivePartDto.MemberCount = await defectivePartReasonRepository.CountAsync(uou => uou.PartId == defectivePart.Id);
            return defectivePartDto;
        }

        public async Task DeleteDefectivePart(EntityDto input)
        {
            await defectiveReasonManager.DeleteAsync(input.Id);
        }

        public async Task<DefectivePartDto> MoveDefectivePart(MoveDefectivePartDto input)
        {
            await defectiveReasonManager.MoveAsync(input.Id, input.NewParentId);
            var defectivePart = await defectivePartRepository.GetAsync(input.Id);
            var defectivePartDto = ObjectMapper.Map<DefectivePartDto>(defectivePart);
            defectivePartDto.MemberCount = await defectivePartReasonRepository.CountAsync(uou => uou.PartId == defectivePart.Id);
            return defectivePartDto;
        }

        /// <summary>
        /// 删除次品原因
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteDefectiveReason(EntityDto input)
        {
            await this.defectivePartReasonRepository.DeleteAsync(dpr => dpr.ReasonId == input.Id);
            await this.defectiveReasonRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取次品原因
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<DefectiveReasonDto>> ListDefectiveReasons(DefectiveReasonFilterDto input)
        {
            var query = from dr in this.defectiveReasonRepository.GetAll()
                        join u in userRepository.GetAll() on dr.CreatorUserId equals u.Id
                        join dpr in this.defectivePartReasonRepository.GetAll() on dr.Id equals dpr.ReasonId
                        where dpr.PartId==input.PartId
                        select new DefectiveReasonDto()
                        {
                            Id = dr.Id,
                            Name = dr.Name,
                            Code = dr.Code,
                            Memo = dr.Memo,
                            CreateUserName = u.Name,
                            CreationTime = dr.CreationTime
                        };

            var result = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            var resultCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<DefectiveReasonDto>(
                resultCount, 
                ObjectMapper.Map<List<DefectiveReasonDto>>(result),
                resultCount)
            {
                Draw = input.Draw 
            };
        }

        public async Task<IEnumerable<DefectiveReasonDto>> ListDefectiveReasonsByPartId(EntityDto input)
        {
            var query = from dr in this.defectiveReasonRepository.GetAll()                      
                        join dpr in this.defectivePartReasonRepository.GetAll() on dr.Id equals dpr.ReasonId
                        where dpr.PartId == input.Id
                        select new DefectiveReasonDto()
                        {
                            Id = dpr.Id,
                            Name = dr.Name,
                            Code = dr.Code,
                            Memo = dr.Memo
                        };

            return await query.ToListAsync();
        }

        public async Task<ListResultDto<DefectivePartDto>> ListDefectivePart()
        {
            var query = from dp in defectivePartRepository.GetAll()
                        join dpr in defectivePartReasonRepository.GetAll() on dp.Id equals dpr.PartId into g
                        from k in g.DefaultIfEmpty()
                        select new { dp, k };

            var items = (await query.ToListAsync()).GroupBy(x=>x.dp.Id)
                .Select(x => new { x.Key, db = x.First().dp, List = x.ToList(), k = x.First().k });

            return new ListResultDto<DefectivePartDto>(
                        items.Select(
                                item =>
                                {
                                    var dto = ObjectMapper.Map<DefectivePartDto>(item.db);
                                    dto.MemberCount = item.k!=null?item.List.Count:0;
                                    return dto;
                                })
                            .ToList());
        }
    }
}