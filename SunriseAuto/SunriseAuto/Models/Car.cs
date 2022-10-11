using SunriseAuto.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseAuto.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName="nvarchar(20)")]
        [DisplayName("Name")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters only.")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("Short description")]
        [Required(ErrorMessage = "This field is required.")]
        public string ShortDesc { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        [DisplayName("Long description")]
        [Required(ErrorMessage = "This field is required.")]
        public string LongDesc { get; set; }
        public string? ImageName { get; set; }
        [NotMapped]
        [DisplayName("UploadFile")]
        [Required(ErrorMessage = "Image is required.")]
        public IFormFile? ImageFile { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public float Mileage { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public float Price { get; set; }

        [DisplayFormat(DataFormatString ="{0:MMM-dd-yy}")]
        public DateTime? Date { get; set; }
        public Status Status { get; set; }
        public int TypeId { get; set; }
        public int BrandId { get; set; }
        public ICollection<UserCar>? Favs { get; set; }
        public Car()
        {
            Favs= new List<UserCar>();
        }
    }
}
