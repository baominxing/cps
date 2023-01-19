using Abp;
using Abp.Application.Services.Dto;

using AutoMapper;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Authorization.Users.Dto;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Order.ProductionPlans;
using Wimi.BtlCore.Order.ProductionPlans.Dtos;

namespace Wimi.BtlCore.Dto
{
    internal static class CustomDtoMapper
    {
        private static readonly object SyncObj = new object();

        private static volatile bool mappedBefore;

        public static void CreateMappings(IMapperConfigurationExpression mapper)
        {
            lock (SyncObj)
            {
                if (mappedBefore)
                {
                    return;
                }

                CreateMappingsInternal(mapper);
                CreateProductionPlanMapper(mapper);

                mappedBefore = true;
            }
        }

        private static void CreateMappingsInternal(IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<User, UserEditDto>().ForMember(dto => dto.Password, options => options.Ignore()).ReverseMap().ForMember(user => user.Password, options => options.Ignore());

            mapper.CreateMap<MachineSettingListDto, Machine>();
            mapper.CreateMap<CreateOrUpdateStateInfoDto, StateInfo>();
            mapper.CreateMap<ImportUsersOutputDto, UserEditDto>();
            mapper.CreateMap<ImportMachinesOutputDto, Machine>().ForMember(dest => dest.Desc, opts => opts.MapFrom(src => src.Description));

            mapper.CreateMap<NameValue, NameValueDto>();
            mapper.CreateMap<NameValue<long>, NameValueDto<long>>();
            mapper.CreateMap<NameValue<int>, NameValueDto<int>>();

            mapper.CreateMap<MachineSettingListDto, Machine>().ForMember(dest => dest.CreationTime, opts => opts.Ignore()).ForMember(dest => dest.CreatorUserId, opts => opts.Ignore());
        }

        private static void CreateProductionPlanMapper(IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<ProductionPlan, ProductionPlanListDto>().ForMember(dto => dto.CanDelete, options => options.MapFrom(s => s.IsPrepared()));
        }
    }
}
