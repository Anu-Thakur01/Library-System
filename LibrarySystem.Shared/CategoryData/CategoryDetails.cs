using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Shared.CategoryData
{
    public class CategoryDetails
    {
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string? Status { get; set; }
        public string? User { get; set; }

    }
}
