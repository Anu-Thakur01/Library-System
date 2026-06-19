    using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Repository.Models
{
    public class Category : BaseEntity
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string? Description { get; set; }
    }

}
