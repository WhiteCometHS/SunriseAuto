using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseAuto.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(40)")]
        public string Name { get; set; }

        public string? ImageName { get; set; }
        [NotMapped]
        [DisplayName("Upload File")]
        [Required(ErrorMessage = "Image is required.")]
        public IFormFile? ImageFile { get; set; }
        [ForeignKey("BrandId")]
        public ICollection<Car>? Cars { get; set; }
    }
}
