namespace Wimi.BtlCore.Visual.Dto
{
    using System.Collections.Generic;

    using Castle.Components.DictionaryAdapter;

    public class GetRealtimeMachineInfoInputDto
    {
        public GetRealtimeMachineInfoInputDto()
        {
            this.MacNoList = new EditableList<string>();
        }

        public List<string> MacNoList { get; set; }
    }
}