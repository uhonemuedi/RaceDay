using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [Required]
        [MaxLength(150)]
        public string VenueName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string Province { get; set; }

        // Navigation property
        public ICollection<Event>? Events { get; set; }
    }
}