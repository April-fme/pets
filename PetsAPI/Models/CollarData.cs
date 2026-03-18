using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsAPI.Models
{
    [Table("CollarData")]
    public class CollarData
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

        [Column(TypeName = "decimal(5, 2)")]
        public decimal SleepQuality { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("PetID")]
        public Pet Pet { get; set; } = null!;
    }
}
