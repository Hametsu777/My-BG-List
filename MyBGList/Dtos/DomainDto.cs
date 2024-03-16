using System.ComponentModel.DataAnnotations;

namespace MyBGList.Dtos
{
    public class DomainDto
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
