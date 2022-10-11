using Microsoft.AspNetCore.Mvc.Rendering;
using SunriseAuto.Models;

namespace SunriseAuto.Extensions
{
    public static class ConvertExtensions
    {
        public static List<SelectListItem> GetStatusSelectList()
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem(Status.Available.ToString(), ((int)Status.Available).ToString()),
                new SelectListItem(Status.NotAccepted.ToString(), ((int)Status.NotAccepted).ToString()),
                new SelectListItem(Status.Ordered.ToString(), ((int)Status.Ordered).ToString()),
                new SelectListItem(Status.InUse.ToString(), ((int)Status.InUse).ToString())
            };
            return list;
        }
    }
}
