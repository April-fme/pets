using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsAPI.Models
{
    [Table("Pets")]
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Species { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? WeightGoal { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<HealthData> HealthData { get; set; } = new List<HealthData>();
        public ICollection<DailyLog> DailyLogs { get; set; } = new List<DailyLog>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }
}
