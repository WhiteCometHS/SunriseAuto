using System.ComponentModel.DataAnnotations;

namespace SunriseAuto.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; } = DateTime.Now;
        public int CarId { get; set; }
        public string UserId { get; set; }
    }
}
