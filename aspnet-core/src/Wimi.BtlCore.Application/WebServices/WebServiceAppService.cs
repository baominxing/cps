using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.MachineTypes;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Cutter;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.Utilitis;

namespace Wimi.BtlCore.WebServices
{
    public class WebServiceAppService : BtlCoreAppServiceBase, IWebServiceAppService
    {
        private readonly IRepository<CutterStates> cutterStatesRepository;

        private readonly IRepository<StateInfo> siRepository;

        private readonly IRepository<MachineType> machineTypeRepository;

        private readonly MongoMachineManager mongoMachineManager;

        public WebServiceAppService(
            IRepository<CutterStates> cutterStatesRepository,
            IRepository<StateInfo> siRepository,
            IRepository<MachineType> machineTypeRepository,
            MongoMachineManager mongoMachineManager)
        {
            this.cutterStatesRepository = cutterStatesRepository;
            this.siRepository = siRepository;
            this.machineTypeRepository = machineTypeRepository;
            this.mongoMachineManager = mongoMachineManager;
        }

		public string Ping(string s)
		{
			Console.WriteLine("Exec ping method");
			return s;
		}

		public ComplexModelResponse PingComplexModel(ComplexModelInput inputModel)
		{
			Console.WriteLine("Input data. IntProperty: {0}, StringProperty: {1}", inputModel.IntProperty, inputModel.StringProperty);

			return new ComplexModelResponse
			{
				FloatProperty = float.MaxValue / 2,
				StringProperty = inputModel.StringProperty,
				ListProperty = inputModel.ListProperty,
				DateTimeOffsetProperty = inputModel.DateTimeOffsetProperty
			};
		}

		public void VoidMethod(out string s)
		{
			s = "Value from server";
		}

		public Task<int> AsyncMethod()
		{
			return Task.Run(() => 42);
		}

		public int? NullableMethod(bool? arg)
		{
			return null;
		}

		public void XmlMethod(XElement xml)
		{
			Console.WriteLine(xml.ToString());
		}
	}
}
