using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        [ForeignKey("Enrolment")]
        public int EnrolmentID { get; set; }
        public Enrolment? Enrolment { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Finished"; // Finished, DNF, DQ
    }
}
