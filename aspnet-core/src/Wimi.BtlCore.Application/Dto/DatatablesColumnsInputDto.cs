namespace Wimi.BtlCore.Dto
{
    public class DatatablesColumnsInputDto
    {
        public string Data { get; set; }

        public string Name { get; set; }

        public bool Orderable { get; set; }

        public DatatablesColumnsSearch Search { get; set; }

        public bool Searchable { get; set; }
    }
}