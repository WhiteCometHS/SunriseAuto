namespace SunriseAuto.Models
{
    public class VehicleViewModel
    {
        public Car Car { get; set; }
        public List<Models.Type>? Types { get; set; }
        public List<Brand>? Brands { get; set; }

    }
}
