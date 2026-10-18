using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [Required]
        [ForeignKey("Participant")]
        public int ParticipantID { get; set; }
        public User? Participant { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public Category? Category { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        // Navigation properties
        public Result? Result { get; set; }
        public Payment? Payment { get; set; }
    }
}