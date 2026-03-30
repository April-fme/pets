using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsAPI.Models
{
    [Table("DailyLogs")]
    public class DailyLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int PetID { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal FoodAmount { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal WaterIntake { get; set; }

        [MaxLength(50)]
        public string? StoolStatus { get; set; }

        [MaxLength(50)]
        public string? UrineStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("PetID")]
        public Pet Pet { get; set; } = null!;
    }
}
