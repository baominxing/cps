using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Wimi.BtlCore.BasicData.Machines;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Wimi.BtlCore.WebServices
{

	[ServiceContract]
	public interface IWebServiceAppService
	{
		[OperationContract]
		string Ping(string s);

		[OperationContract]
		ComplexModelResponse PingComplexModel(ComplexModelInput inputModel);

		[OperationContract]
		void VoidMethod(out string s);

		[OperationContract]
		Task<int> AsyncMethod();

		[OperationContract]
		int? NullableMethod(bool? arg);

		[OperationContract]
		void XmlMethod(System.Xml.Linq.XElement xml);
	}
}
