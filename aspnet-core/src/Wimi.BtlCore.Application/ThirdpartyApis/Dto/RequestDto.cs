namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    using System.Collections.Generic;

    public class RequestDto
    {
        public RequestDto()
        {
            this.MachineCodes = new List<string>();
            this.Demo = false;
        }

        public bool Demo { get; set; }

        public string WorkShopCode { get; set; }

        public IEnumerable<string> MachineCodes { get; set; }
    }
}