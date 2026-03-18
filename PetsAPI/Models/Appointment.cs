using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsAPI.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int PetID { get; set; }

        [Required]
        [MaxLength(200)]
        public string ClinicName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? VetName { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "預約中";

        // Navigation property
        [ForeignKey("PetID")]
        public Pet Pet { get; set; } = null!;
    }
}
