using Abp.Dependency;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Runtime.Exception
{
    public class InfluxdbExceptionHandler : IEventHandler<AbpHandledExceptionData>, ITransientDependency
    {
        public ILogger Logger { get; set; }
        private readonly InfluxdbManager influxdbManager;

        public InfluxdbExceptionHandler(InfluxdbManager influxdbManager)
        {
            this.influxdbManager = influxdbManager;
        }

        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            this.influxdbManager.Save(eventData);
        }
    }
}
