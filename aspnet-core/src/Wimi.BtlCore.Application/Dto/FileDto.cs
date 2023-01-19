using System;
using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Dto
{
    public class FileDto
    {
        public FileDto()
        {
        }

        public FileDto(string fileName, string fileType)
        {
            this.FileName = fileName;
            this.FileType = fileType;
            this.FileToken = Guid.NewGuid().ToString("N");
        }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileToken { get; set; }

        [Required]
        public string FileType { get; set; }
    }
}
