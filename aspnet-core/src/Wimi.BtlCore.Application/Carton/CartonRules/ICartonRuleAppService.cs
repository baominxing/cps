using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Carton.CartonRules.Dtos;

namespace Wimi.BtlCore.Carton.CartonRules
{
    public interface ICartonRuleAppService : IApplicationService
    {

        /// <summary>
        /// 新增或修改箱码规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateOrUpdateCartonRule(CartonRuleInputDto input);

        /// <summary>
        /// 获取左侧规则列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CartonRuleDto>> ListCartonRules();

        /// <summary>
        /// 删除或禁用规则前，需校验是否已在使用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CheckIsUsing(EntityDto<int> input);

        /// <summary>
        /// 更新箱码规则是否启用状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateRulesStatus(EntityDto<int> input);

        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteCartonRule(EntityDto<int> input);

        /// <summary>
        /// 根据所选的班次方案Id，获取班次信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<ShiftItemsDto> GetShiftItemsById(EntityDto<int> input);

        /// <summary>
        /// 新增规则明细--总体、批量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateRuleDetail(RuleDetailInputDto input);

        /// <summary>
        /// 获取规则明细--编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<RuleDetailInputItem> GetRuleDetailForEdit(EntityDto<int> input);

        /// <summary>
        /// 修改单项规则明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateRuleDetail(RuleDetailInputItem input);

        /// <summary>
        /// 修改时校验输入的顺序号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CheckSequenceNo(CheckSequenceNoInputDto input);

        /// <summary>
        /// 根据规则Id获取规则明细列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<RuleDetailDto> GetRuleDetailsByRuleId(EntityDto<int> input);

        /// <summary>
        /// 删除规则明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteRuleDetailById(EntityDto<int> input);

        /// <summary>
        /// 预览设置好的规则对应的箱码样式
        /// </summary>
        /// <param name="input">规则Id</param>
        /// <returns></returns>
        Task<string> Preview(EntityDto<int> input);

        /// <summary>
        /// 获取类型下拉框，避免重复
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<NameValueDto<int>>> GetTypeSeletById(EntityDto<int> input);

        //校验码查看导入文件接口--待定

        /// <summary>
        /// 导入余数对照表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> ImportCalibratorCodes(List<ImportDatasDto> input);

        /// <summary>
        /// 查看导入的余数对照表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<NameValueDto<int>> Examine(EntityDto<int> input);

        Task RerSetCalibratorCode(EntityDto<int> input);
    }
}
