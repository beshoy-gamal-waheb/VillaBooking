using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaBooking.API.Models
{
    public class VillaAmenities : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        [ForeignKey(nameof(Villa))]
        public int VillaId { get; set; }
        public Villa? Villa { get; set; }
    }
}
