using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [ForeignKey("Organiser")]
        public int OrganiserID { get; set; }
        public User? Organiser { get; set; }

        [Required]
        [ForeignKey("Venue")]
        public int VenueID { get; set; }
        public Venue? Venue { get; set; }

        [Required]
        [MaxLength(150)]
        public string EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string EventType { get; set; } // "Run", "Walk", "Cycle"

        // Navigation property
        public ICollection<Category>? Categories { get; set; }
    }
}