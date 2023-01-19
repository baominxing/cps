using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.Processes.Dtos
{
    [AutoMap(typeof(Process))]
    public class ProcessDto
    {
        private string code;

        private string memo;

        private string name;

        public string Code
        {
            get
            {
                return this.code;
            }

            set
            {
                this.code = value.Trim();
            }
        }

        public int Id { get; set; }

        public string Memo
        {
            get
            {
                return this.memo;
            }

            set
            {
                this.memo = value.Trim();
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value.Trim();
            }
        }
    }
}
