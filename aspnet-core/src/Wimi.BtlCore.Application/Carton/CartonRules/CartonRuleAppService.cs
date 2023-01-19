using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Carton.CartonRules.Dtos;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonRules
{
    public class CartonRuleAppService : BtlCoreAppServiceBase, ICartonRuleAppService
    {
        private readonly IRepository<CartonRule> cartonRuleRepository;
        private readonly IRepository<CartonRuleDetail> cartonRuleDetailRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;
        private readonly IRepository<CalibratorCode> calibratorCodeRepository;
        private readonly IRepository<CartonSetting> cartonSettingRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly CartonManager cartonManager;

        public CartonRuleAppService(IRepository<CartonRuleDetail> cartonRuleDetailRepository,
                                    IRepository<CartonRule> cartonRuleRepository,
                                    IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
                                    IRepository<CalibratorCode> calibratorCodeRepository,
                                    IRepository<CartonSetting> cartonSettingRepository,
                                    IRepository<DeviceGroup> deviceGroupRepository,
                                    CartonManager cartonManager)
        {
            this.cartonRuleDetailRepository = cartonRuleDetailRepository;
            this.cartonRuleRepository = cartonRuleRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.calibratorCodeRepository = calibratorCodeRepository;
            this.cartonSettingRepository = cartonSettingRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.cartonManager = cartonManager;
        }

        /// <summary>
        /// 新增或修改箱码规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateCartonRule(CartonRuleInputDto input)
        {
            Validate(input);

            if (input.Id.HasValue && !input.IsActive)
            {
                await this.CheckIsUsing(new EntityDto<int>() { Id = input.Id.Value });
            }

            var entity = ObjectMapper.Map<CartonRule>(input);

            await this.cartonRuleRepository.InsertOrUpdateAsync(entity);
        }

        /// <summary>
        /// 获取左侧规则列表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CartonRuleDto>> ListCartonRules()
        {

            var query = from cr in cartonRuleRepository.GetAll()
                        select new CartonRuleDto
                        {
                            Id = cr.Id,
                            Name = cr.Name,
                            IsActive = cr.IsActive
                        };

            return await query.ToListAsync();
        }

        /// <summary>
        /// 删除或禁用规则前，需校验是否已在使用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CheckIsUsing(EntityDto<int> input)
        {
            var usingSettings = await this.cartonSettingRepository.GetAll().Where(c => c.CartonRuleId == input.Id).ToListAsync();

            if (usingSettings != null && usingSettings.Any())
            {
                throw new UserFriendlyException(this.L("RuleIsInUsing"));
            }
        }

        /// <summary>
        /// 更新箱码规则是否启用状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateRulesStatus(EntityDto<int> input)
        {
            var entity = await this.cartonRuleRepository.GetAsync(input.Id);
            entity.IsActive = !entity.IsActive;

            if (!entity.IsActive)
            {
                await this.CheckIsUsing(input);
            }

            await this.cartonRuleRepository.UpdateAsync(entity);
        }

        private void Validate(CartonRuleInputDto input)
        {
            if (!input.Id.HasValue)
            {
                var nameCheck = this.cartonRuleRepository.FirstOrDefault(c => c.Name.Equals(input.Name));
                if (nameCheck != null) throw new UserFriendlyException(L("DuplicateCartonRuleName"));
            }
            else
            {
                var nameCheck = this.cartonRuleRepository.FirstOrDefault(c => c.Name.Equals(input.Name) && c.Id != input.Id.Value);
                if (nameCheck != null) throw new UserFriendlyException(L("DuplicateCartonRuleName"));
            }
        }

        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteCartonRule(EntityDto<int> input)
        {
            await this.CheckIsUsing(new EntityDto<int>() { Id = input.Id });

            var ruleDetails = await this.cartonRuleDetailRepository.GetAll().Where(c => c.CartonRuleId == input.Id).ToListAsync();

            await this.cartonRuleRepository.DeleteAsync(input.Id);

            foreach (var item in ruleDetails)
            {
                await this.cartonRuleDetailRepository.DeleteAsync(item);
            }
        }

        /// <summary>
        /// 根据所选的班次方案Id，获取班次信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<ShiftItemsDto> GetShiftItemsById(EntityDto<int> input)
        {
            if (input?.Id == null) return new List<ShiftItemsDto>();

            var query = from si in this.shiftSolutionItemRepository.GetAll()
                        where si.ShiftSolutionId == input.Id
                        select new ShiftItemsDto
                        {
                            Id = si.Id,
                            Name = si.Name
                        };

            return query.ToList();
        }

        /// <summary>
        /// 新增规则明细--总体、批量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateRuleDetail(RuleDetailInputDto input)
        {
            bool HaveDuplicates = input.RuleDetailInputItems.GroupBy(x => x.SequenceNo).Any(x => x.Select(y => y.Type).Distinct().Count() > 1);
            if (HaveDuplicates)
            {
                throw new UserFriendlyException(this.L("CartonRulesSequenceNoSame"));
            }
            var dto = input.RuleDetailInputItems.FirstOrDefault(x =>x.Type == EnumRuleType.CalibratorCode);
            if (dto != null)
            {
                if (dto.StartIndex != 0 && dto.EndIndex != 0)
                {
                    if (dto.StartIndex > dto.EndIndex)
                    {
                        throw new UserFriendlyException(this.L("StartIndexGreaterEndIndex"));
                    }
                }

            }

            if (input.RuleDetailInputItems.Any())
            {
                foreach (var item in input.RuleDetailInputItems)
                {
                    if (item.Type == EnumRuleType.Ascii || item.Type == EnumRuleType.FixedString
                        || item.Type == EnumRuleType.Year || item.Type == EnumRuleType.Shift
                        || item.Type == EnumRuleType.Line)
                    {
                        var temp = item.Value;
                        if (!string.IsNullOrEmpty(item.Value) && item.Value != null)
                        {
                            item.Length = temp.Length;
                        }
                    }
                    var entity = ObjectMapper.Map<CartonRuleDetail>(item);
                    await this.cartonRuleDetailRepository.InsertAsync(entity);
                }
            }
        }

        /// <summary>
        /// 获取规则明细--编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RuleDetailInputItem> GetRuleDetailForEdit(EntityDto<int> input)
        {
            var entity = await this.cartonRuleDetailRepository.GetAsync(input.Id);

            var dto = ObjectMapper.Map<RuleDetailInputItem>(entity);

            return dto;
        }

        /// <summary>
        /// 修改单项规则明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateRuleDetail(RuleDetailInputItem input)
        {
            if (input.StartIndex != 0 && input.EndIndex != 0 )
            {
                if (input.StartIndex > input.EndIndex)
                {
                    throw new UserFriendlyException(this.L("StartIndexGreaterEndIndex"));
                }
            }
            var entity = ObjectMapper.Map<CartonRuleDetail>(input);

            if (entity.Type == EnumRuleType.Ascii || entity.Type == EnumRuleType.FixedString
                        || entity.Type == EnumRuleType.Year || entity.Type == EnumRuleType.Shift
                        || entity.Type == EnumRuleType.Line)
            {
                var temp = entity.Value;
                entity.Length = temp.Length;
            }

            if (entity.Type == EnumRuleType.Shift)
            {
                var others = this.cartonRuleDetailRepository.GetAll().Where(c => c.Type == EnumRuleType.Shift && c.Id != input.Id && c.CartonRuleId == input.CartonRuleId).ToList();

                foreach (var item in others)
                {
                    item.SequenceNo = entity.SequenceNo;
                    await this.cartonRuleDetailRepository.UpdateAsync(item);
                }
            }

            if (entity.Type == EnumRuleType.Line)
            {
                var others = this.cartonRuleDetailRepository.GetAll().Where(c => c.Type == EnumRuleType.Line && c.Id != input.Id && c.CartonRuleId == input.CartonRuleId).ToList();

                foreach (var item in others)
                {
                    item.SequenceNo = entity.SequenceNo;
                    await this.cartonRuleDetailRepository.UpdateAsync(item);
                }
            }

            await this.cartonRuleDetailRepository.InsertOrUpdateAsync(entity);
        }

        /// <summary>
        /// 修改时校验输入的顺序号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CheckSequenceNo(CheckSequenceNoInputDto input)
        {
            var existRec = new CartonRuleDetail();
            if (input.RuleDetailId.HasValue)
            {
                var targetRec = await this.cartonRuleDetailRepository.FirstOrDefaultAsync(c => c.Id == input.RuleDetailId.Value);

                if (targetRec.Type == EnumRuleType.Shift || targetRec.Type == EnumRuleType.Line)
                {
                    var shiftOrLineIds = new List<int>();
                    if (targetRec.Type == EnumRuleType.Shift)
                    {
                        shiftOrLineIds = this.cartonRuleDetailRepository.GetAll().Where(c => c.Type == EnumRuleType.Shift).Select(c => c.Id).ToList();
                    }
                    else
                    {
                        shiftOrLineIds = this.cartonRuleDetailRepository.GetAll().Where(c => c.Type == EnumRuleType.Line).Select(c => c.Id).ToList();
                    }
                    existRec = await this.cartonRuleDetailRepository.FirstOrDefaultAsync(c => c.SequenceNo == input.SequenceNo
                                                            && c.CartonRuleId == input.RuleId && !shiftOrLineIds.Contains(c.Id));
                }
                else
                {
                    existRec = await this.cartonRuleDetailRepository.FirstOrDefaultAsync(c => c.SequenceNo == input.SequenceNo
                                                            && c.CartonRuleId == input.RuleId && c.Id != input.RuleDetailId);
                }
            }
            else
            {
                existRec = await this.cartonRuleDetailRepository.FirstOrDefaultAsync(c => c.SequenceNo == input.SequenceNo
                                                        && c.CartonRuleId == input.RuleId);
            }
            if (existRec != null) throw new UserFriendlyException(this.L("DuplicateSequenceNo{0}", input.SequenceNo));
        }

        /// <summary>
        /// 根据规则Id获取规则明细列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RuleDetailDto> GetRuleDetailsByRuleId(EntityDto<int> input)
        {
            var returnVal = new RuleDetailDto()
            {
                AdditionalInfo = new AdditionalInfos()
                {
                    CartonNoLenth = 0
                },
                RuleDetailItems = new List<RuleDetailItem>()
            };

            var ruleDetails = this.cartonRuleDetailRepository.GetAll().Where(c => c.CartonRuleId == input.Id).OrderBy(c => c.SequenceNo).ToList();
            if (ruleDetails.Any())
            {
                returnVal.AdditionalInfo.PreviewCartonNo = await this.cartonManager.GenerateCartonNoById(input.Id);
                returnVal.AdditionalInfo.CartonNoLenth = ruleDetails.Sum(r => r.Length);
                returnVal.RuleDetailItems = ObjectMapper.Map<List<RuleDetailItem>>(ruleDetails);

                var shiftMerge = returnVal.RuleDetailItems.Where(rs => rs.Type == EnumRuleType.Shift).Count();
                var lineMerge = returnVal.RuleDetailItems.Where(rs => rs.Type == EnumRuleType.Line).Count();

                returnVal.RuleDetailItems.ForEach(r =>
                {
                    if (r.Type == EnumRuleType.Shift)
                    {
                        var shift = this.shiftSolutionItemRepository.FirstOrDefault(s => s.Id == r.ExpansionKey);
                        if (shift != null)
                        {
                            string temp = r.Value;
                            r.Value = shift.Name + ": " + temp;
                        }
                        r.Merge = shiftMerge;
                    }

                    if (r.Type == EnumRuleType.Line)
                    {
                        var line = this.deviceGroupRepository.FirstOrDefault(d => d.Id == r.ExpansionKey);
                        if (line != null)
                        {
                            string temp = r.Value;
                            r.Value = line.DisplayName + ": " + temp;
                        }

                        r.Merge = lineMerge;
                    }
                });
            }

            return returnVal;
        }

        public async Task RerSetCalibratorCode(EntityDto<int> input)
        {
            var calibratorCode = await cartonRuleDetailRepository.GetAll().Where(p => p.CartonRuleId == input.Id && p.Type == EnumRuleType.CalibratorCode).FirstOrDefaultAsync();
            calibratorCode.StartIndex = 1;
            calibratorCode.EndIndex = 1;
            await cartonRuleDetailRepository.UpdateAsync(calibratorCode);
            //await cartonRuleDetailRepository.DeleteAsync(calibratorCode);
        }

        /// <summary>
        /// 删除规则明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteRuleDetailById(EntityDto<int> input)
        {
            await this.cartonRuleDetailRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 预览设置好的规则对应的箱码样式
        /// </summary>
        /// <param name="input">规则Id</param>
        /// <returns></returns>
        public async Task<string> Preview(EntityDto<int> input)
        {
            var cartonNo = await cartonManager.GenerateCartonNoById(input.Id);

            return cartonNo;
        }

        /// <summary>
        /// 获取类型下拉框，避免重复
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<NameValueDto<int>>> GetTypeSeletById(EntityDto<int> input)
        {
            var returnVal = new List<NameValueDto<int>>();

            var originalList = this.GetOriginalTypeList();

            var query = await this.cartonRuleDetailRepository.GetAll().Where(c => c.CartonRuleId == input.Id).ToListAsync();

            if (query != null && query.Any())
            {
                var queryList = query.Select(q => (int)q.Type).Distinct().ToList();
                returnVal = originalList.Where(o => !queryList.Contains(o.Value)).ToList();
            }
            else
            {
                returnVal = originalList.ToList();
            }

            return returnVal;
        }

        /// <summary>
        /// 导入余数对照表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> ImportCalibratorCodes(List<ImportDatasDto> input)
        {
            var returnVal = string.Empty;

            if (input.Any())
            {
                try
                {
                    //删除现有
                    var ruleId = input.First().CartonRuleId;
                    var existRecs = await this.calibratorCodeRepository.GetAll().Where(c => c.CartonRuleId == ruleId).ToListAsync();
                    if (existRecs.Any())
                    {
                        foreach (var rec in existRecs)
                        {
                            await this.calibratorCodeRepository.DeleteAsync(rec);
                        }
                    }

                    //新增
                    foreach (var item in input)
                    {
                        var entity = ObjectMapper.Map<CalibratorCode>(item);
                        await this.calibratorCodeRepository.InsertAsync(entity);
                    }
                    returnVal = "Success";
                }
                catch (Exception ex)
                {
                    returnVal = ex.Message;
                }
            }
            return returnVal;
        }

        /// <summary>
        /// 查看导入的余数对照表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<NameValueDto<int>> Examine(EntityDto<int> input)
        {
            var query = (from cc in this.calibratorCodeRepository.GetAll()
                         where cc.CartonRuleId == input.Id
                         select new NameValueDto<int>()
                         {
                             Name = cc.Value,
                             Value = cc.Key
                         }).OrderBy(c => c.Value);
            return query.ToList();

        }

        private IEnumerable<NameValueDto<int>> GetOriginalTypeList()
        {

            var originalList = new List<NameValueDto<int>>();
            FieldInfo[] fields = typeof(EnumRuleType).GetFields().Where(p => p.CustomAttributes.Count() > 0).ToArray();

            for (var i = 0; i < fields.Length; i++)
            {
                var dto = new NameValueDto<int>() { Name = this.L(fields[i].Name), Value = i };
                if (i == (int)EnumRuleType.SerialNumber)
                {
                    dto.Name = this.L("SerialNum");
                }
                originalList.Add(dto);
            }

            return originalList;


        }
    }
}
