using Wimi.BtlCore.ThirdpartyApis.Interfaces;

namespace Wimi.BtlCore.ThirdpartyApis
{
    using System;
    using System.Linq;
    using Abp.Domain.Uow;
    using Abp.Logging;

    public class ThirdpartyApiStore: IThirdpartyApiStore
    {
        private readonly ThirdpartyApiProvider thirdpartyApiProvider;
        private readonly IThirdpartyApiManager thirdpartyApiManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public ThirdpartyApiStore(ThirdpartyApiProvider thirdpartyApiProvider, IThirdpartyApiManager thirdpartyApiManager, IUnitOfWorkManager unitOfWorkManager)
        {
            this.thirdpartyApiProvider = thirdpartyApiProvider;
            this.thirdpartyApiManager = thirdpartyApiManager;
            this._unitOfWorkManager = unitOfWorkManager;
        }

 
        public void Initialize()
        {
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var apis = this.thirdpartyApiProvider.ListApiDefinitions();
                this.thirdpartyApiManager.DeleteNotExistApis(apis.Select(t => t.Code));
                apis.ForEach(
                    a =>
                    {
                        this.thirdpartyApiManager.Save(a);
                    });

                unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("开放接口Api初始化失败,原因：", ex);
            }
        }
    }
}