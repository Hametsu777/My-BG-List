using System.ComponentModel.DataAnnotations;

namespace MyBGList.Dtos
{
    public class MechanicDto
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
