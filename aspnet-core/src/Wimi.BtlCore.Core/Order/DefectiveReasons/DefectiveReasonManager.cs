using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.Order.DefectiveParts;
using Wimi.BtlCore.Order.DefectiveReasons;

namespace WIMI.BTL.Wimi.BtlCore.Order.DefectiveReasons
{
    public class DefectiveReasonManager: BtlCoreDomainServiceBase
    {
        private readonly IRepository<DefectivePart> defectivePartRepository;
        private readonly IRepository<DefectiveReason> defectiveReasonRepository;
        private readonly IRepository<DefectivePartReason> defectivePartReasonRepository;

        public DefectiveReasonManager(IRepository<DefectivePart> defectivePartRepository, IRepository<DefectiveReason> defectiveReasonRepository, IRepository<DefectivePartReason> defectivePartReasonRepository)
        {
            this.defectivePartRepository = defectivePartRepository;
            this.defectiveReasonRepository = defectiveReasonRepository;
            this.defectivePartReasonRepository = defectivePartReasonRepository;
        }

        public async Task CreateOrUpdateReasonAsync(DefectiveReasonInputDto input)
        {
            if (input.Id == 0)
            {
                // Create
                await this.CheckCode(input.Code, input.PartId);
                await this.CheckName(input.Name, input.PartId);
              
              
                var createReason = ObjectMapper.Map<DefectiveReason>(input);
                var createPartReasonId = await this.defectiveReasonRepository.InsertAndGetIdAsync(createReason);

                var createPartReason = new DefectivePartReason();
                createPartReason.PartId = input.PartId;
                createPartReason.PartCode = input.PartCode;
                createPartReason.ReasonId = createPartReasonId;
                await this.defectivePartReasonRepository.InsertAsync(createPartReason);
                return;
            }
            // update
            var entity = await this.defectiveReasonRepository.GetAsync(input.Id);
            if (entity.Name != input.Name)
            {
                await this.CheckName(input.Name, input.PartId);
            }
       
            ObjectMapper.Map(input,entity);
            await this.defectiveReasonRepository.UpdateAsync(entity);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var children = await FindChildrenAsync(id, true);
            foreach (var child in children)
            {
                await defectivePartRepository.DeleteAsync(child);
            }
            await defectivePartRepository.DeleteAsync(id);
            var defectivePartIds = children.Select(n => n.Id).ToList();
            defectivePartIds.Add(id);
            await defectivePartReasonRepository.DeleteAsync(t => defectivePartIds.Contains(t.PartId));
        }

        public async Task<List<DefectivePart>> FindChildrenAsync(int? parentId, bool recursive = false)
        {
            if (recursive)
            {
                if (!parentId.HasValue)
                {
                    return await defectivePartRepository.GetAllListAsync();
                }
                var code = await GetCodeAsync(parentId.Value);
                return await defectivePartRepository.GetAllListAsync(ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value);
            }
            return await defectivePartRepository.GetAllListAsync(ou => ou.ParentId == parentId);
        }
        public virtual async Task<string> GetCodeAsync(int id)
        {
            return (await defectivePartRepository.GetAsync(id)).Code;
        }

        public virtual async Task CreateAsync(DefectivePart defectivePart)
        {
            defectivePart.Code = await GetNextChildCodeAsync(defectivePart.ParentId);
            await ValidateDefectivePartAsync(defectivePart);
            await defectivePartRepository.InsertAsync(defectivePart);
        }

        public virtual async Task<string> GetNextChildCodeAsync(int? parentId)
        {
            var lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return DefectivePart.AppendCode(parentCode, DefectivePart.CreateCode(1));
            }
            return DefectivePart.CalculateNextCode(lastChild.Code);
        }

        public virtual async Task<DefectivePart> GetLastChildOrNullAsync(int? parentId)
        {
            var children = await defectivePartRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            return children.OrderBy(c => c.Code).LastOrDefault();
        }

        public virtual async Task UpdateAsync(DefectivePart defectivePart)
        {
            await ValidateDefectivePartAsync(defectivePart);
            await defectivePartRepository.UpdateAsync(defectivePart);
        }

        public virtual async Task MoveAsync(int id, int? parentId)
        {
            var defectivePart = await defectivePartRepository.GetAsync(id);
            if (defectivePart.ParentId == parentId)
            {
                return;
            }
            var children = await FindChildrenAsync(id, true);
            var oldCode = defectivePart.Code;
            defectivePart.Code = await GetNextChildCodeAsync(parentId);
            defectivePart.ParentId = parentId;
            await ValidateDefectivePartAsync(defectivePart);
            var groupRefs = defectivePartReasonRepository.GetAll().Where(q => q.PartId == id);

            foreach (DefectivePartReason partReason in groupRefs)
            {
                partReason.PartCode = defectivePart.Code;
            }

            foreach (var child in children)
            {
                child.Code = DeviceGroup.AppendCode(defectivePart.Code, DeviceGroup.GetRelativeCode(child.Code, oldCode));
                var childGroupRefs = defectivePartReasonRepository.GetAll().Where(q => q.PartId == child.Id);
                foreach (DefectivePartReason defectivePartReason in childGroupRefs)
                {
                    defectivePartReason.PartCode = child.Code;
                }
            }
        }

        protected virtual async Task ValidateDefectivePartAsync(DefectivePart defectivePart)
        {
            var siblings =(await FindChildrenAsync(defectivePart.ParentId)).Where(ou => ou.Id != defectivePart.Id).ToList();
            if (siblings.Any(ou => ou.Name == defectivePart.Name))
            {
                throw new UserFriendlyException(this.L("NgCodeAlreadyExist"));
            }
        }    
       
        private async Task CheckCode(string code, int partId)
        {
            var ckeckCode = from dr in defectiveReasonRepository.GetAll()
                join dpr in defectivePartReasonRepository.GetAll() on dr.Id equals dpr.ReasonId
                where dpr.PartId == partId
                select dr;
            if (await ckeckCode.AnyAsync(m => m.Code == code))
            {
                throw new UserFriendlyException(this.L("NgReasonCodeMustBeUnique"));
            }
        }

        private async Task CheckName(string name, int partId)
        {
            var checkName = from dr in defectiveReasonRepository.GetAll()
                join dpr in defectivePartReasonRepository.GetAll() on dr.Id equals dpr.ReasonId
                where dpr.PartId == partId
                select dr;
            if (await checkName.AnyAsync(m => m.Name == name))
            {
                throw new UserFriendlyException(this.L("NgReasonNameMustBeUnique"));
            }
        }
    }
}
