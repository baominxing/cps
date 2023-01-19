using Abp.Dependency;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.ExceptionHandler
{
    /// <summary>
    /// 捕获全局未处理异常
    /// </summary>
    public class LogExceptionHandler : BtlCoreAppServiceBase,IEventHandler<AbpHandledExceptionData>, ITransientDependency
    {
        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            //TODO: Check eventData.Exception!
            //logger.Error($"{eventData.Exception.Message}||{eventData.Exception}");
            //TODO: Check eventData.Exception!
            Logger.Error($"{eventData.Exception.Message},{eventData.Exception.InnerException},{eventData.Exception.StackTrace}");
        }
    }
}
