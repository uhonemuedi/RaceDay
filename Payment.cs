using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    public class Payment
    {

        [Key]
        public int PaymentID { get; set; }

        [Required]
        [ForeignKey("Enrolment")]
        public int EnrolmentID { get; set; }
        public Enrolment? Enrolment { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(30)]
        public string PaymentMethod { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Paid, Pending, Failed

        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
