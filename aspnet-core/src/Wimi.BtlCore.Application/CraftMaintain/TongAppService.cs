using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Tongs.Dto;
using Abp.Collections.Extensions;
using Abp.UI;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.CraftMaintain;
using Wimi.BtlCore.Dto;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.Tongs
{
    public class TongAppService : BtlCoreAppServiceBase, ITongAppService
    {
        private readonly IRepository<Tong> tongRepository;

        public TongAppService(IRepository<Tong> tongRepository)
        {
            this.tongRepository = tongRepository;
        }

        public async Task<DatatablesPagedResultOutput<TongDto>> ListTong(TongInputDto input)
        {
            var query = from tong in this.tongRepository.GetAll()
                        select new TongDto
                        {
                            Id = tong.Id,
                            Code = tong.Code,
                            Name = tong.Name,
                            Capacity = tong.Capacity,
                            ProgramA = tong.ProgramA,
                            ProgramB = tong.ProgramB,
                            ProgramC = tong.ProgramC,
                            ProgramD = tong.ProgramD,
                            ProgramE = tong.ProgramE,
                            ProgramF = tong.ProgramF,
                            Note = tong.Note,
                            IsDeleted = tong.IsDeleted,
                            DeleterUserId = tong.DeleterUserId,
                            DeletionTime = tong.DeletionTime,
                            LastModificationTime = tong.LastModificationTime,
                            LastModifierUserId = tong.LastModifierUserId,
                            CreationTime = tong.CreationTime,
                            CreatorUserId = tong.CreatorUserId,
                        };

            if (string.IsNullOrEmpty(input.Code))
            {
                query = query.WhereIf(!string.IsNullOrEmpty(input.Name), p => p.Name.Contains(input.Name))
                    .WhereIf(!string.IsNullOrEmpty(input.ProgramName), p => p.ProgramA.Contains(input.ProgramName) || p.ProgramB.Contains(input.ProgramName) 
                    || p.ProgramC.Contains(input.ProgramName) || p.ProgramD.Contains(input.ProgramName) || p.ProgramE.Contains(input.ProgramName) || p.ProgramF.Contains(input.ProgramName));
                var capacityList = input.CapacityList == null ? new List<int>(): input.CapacityList;
                query = query.WhereIf(capacityList.Count > 0, p => capacityList.Contains(p.Capacity));
            }

            query = query.WhereIf(!string.IsNullOrEmpty(input.Code), p => p.Code.Contains(input.Code));

            var entitiyList = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var entitiyListCount = await query.CountAsync();

            return new DatatablesPagedResultOutput<TongDto>(
                       entitiyListCount,
                       ObjectMapper.Map<List<TongDto>>(entitiyList),
                       entitiyListCount)
            {
                Draw = input.Draw
            };
        }

        public async Task CreateTong(TongInputDto input)
        {
            await HasTongCode(input);
            await HasTongName(input);
            var entity = new Tong();

            entity.Code = input.Code;
            entity.Name = input.Name;
            entity.Capacity = input.Capacity;
            entity.ProgramA = input.ProgramA;
            entity.ProgramB = input.Capacity >= 2 ? input.ProgramB : null;
            entity.ProgramC = input.Capacity >= 3 ? input.ProgramC : null;
            entity.ProgramD = input.Capacity >= 4 ? input.ProgramD : null;
            entity.ProgramE = input.Capacity >= 5 ? input.ProgramE : null;
            entity.ProgramF = input.Capacity >= 6 ? input.ProgramF : null;
            entity.Note = input.Note;

            await this.tongRepository.InsertAsync(entity);
        }

        public async Task UpdateTong(TongInputDto input)
        {
            await HasTongName(input);
            var entity = this.tongRepository.FirstOrDefault(s => s.Id == input.Id);

            entity.Code = input.Code;
            entity.Name = input.Name;
            entity.Capacity = input.Capacity;
            entity.ProgramA = input.ProgramA;
            entity.ProgramB = input.Capacity >= 2 ? input.ProgramB : null;
            entity.ProgramC = input.Capacity >= 3 ? input.ProgramC : null;
            entity.ProgramD = input.Capacity >= 4 ? input.ProgramD : null;
            entity.ProgramE = input.Capacity >= 5 ? input.ProgramE : null;
            entity.ProgramF = input.Capacity >= 6 ? input.ProgramF : null;
            entity.Note = input.Note;

            await this.tongRepository.UpdateAsync(entity);
        }

        public async Task DeleteTong(EntityDto input)
        {
            var entity = this.tongRepository.FirstOrDefault(s => s.Id == input.Id);
            if (entity!=null)
            {
                await this.tongRepository.DeleteAsync(entity);
            }
        }

        [HttpPost]
        public async Task<TongDto> GetTongForEdit(TongInputDto input)
        {
            var query = from tong in this.tongRepository.GetAll()
                        where tong.Id == input.Id
                        select new TongDto
                        {
                            Id = tong.Id,
                            Code = tong.Code,
                            Name = tong.Name,
                            Capacity = tong.Capacity,
                            ProgramA = tong.ProgramA,
                            ProgramB = tong.ProgramB,
                            ProgramC = tong.ProgramC,
                            ProgramD = tong.ProgramD,
                            ProgramE = tong.ProgramE,
                            ProgramF = tong.ProgramF,
                            Note = tong.Note,
                            IsDeleted = tong.IsDeleted,
                            DeleterUserId = tong.DeleterUserId,
                            DeletionTime = tong.DeletionTime,
                            LastModificationTime = tong.LastModificationTime,
                            LastModifierUserId = tong.LastModifierUserId,
                            CreationTime = tong.CreationTime,
                            CreatorUserId = tong.CreatorUserId,
                        };

            return await query.FirstOrDefaultAsync();
        }
        private async Task HasTongName(TongInputDto input)
        {
            if (await tongRepository.IsExistAsync(p => p.Name == input.Name&&input.Id!=p.Id))
            {
                throw new UserFriendlyException(this.L("TongNameMustBeUnique"));
            }
        }
        private async Task HasTongCode(TongInputDto input)
        {
            if (await tongRepository.IsExistAsync(p => p.Code == input.Code))
            {
                throw new UserFriendlyException(this.L("TongCodeMustBeUnique"));
            }
        }

        [HttpPost]
        public async Task<List<NameValueDto>> GetTongsForSelect()
        {
            var list = await tongRepository.GetAllListAsync();

            return list.Select(s => new NameValueDto(s.Name, s.Id.ToString())).ToList();
        }
    }
}


