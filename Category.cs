using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventID { get; set; }
        public Event? Event { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DistanceKM { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal EntryFee { get; set; }

        // Navigation property
        public ICollection<Enrolment>? Enrolments { get; set; }
    }
}
