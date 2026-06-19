using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Repository.Models
{
    public class Borrow : BaseEntity
    {
        [Key]
        public int BorrowId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public DateTime BorrowedOn { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public bool IsOverdue { get; set; } = false;

        public bool IsReturned { get; set; } = false;

        public int DaysOverdue { get; set; } = 0;

        public decimal FineAmount { get; set; } = 0;

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }
    }
}
