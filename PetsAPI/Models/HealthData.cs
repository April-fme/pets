using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsAPI.Models
{
    [Table("HealthData")]
    public class HealthData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int PetID { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal Temperature { get; set; }

        public int HeartRate { get; set; }

        public int ActivityLevel { get; set; }

        public int SleepQuality { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("PetID")]
        public Pet Pet { get; set; } = null!;
    }
}
