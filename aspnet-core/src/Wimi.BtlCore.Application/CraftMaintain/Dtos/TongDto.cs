using System;

namespace Wimi.BtlCore.Tongs.Dto
{
    public class TongDto
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        public string ProgramA { get; set; }

        public string ProgramB { get; set; }

        public string ProgramC { get; set; }

        public string ProgramD { get; set; }

        public string ProgramE { get; set; }

        public string ProgramF { get; set; }

        public string Note { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }

    }
}

