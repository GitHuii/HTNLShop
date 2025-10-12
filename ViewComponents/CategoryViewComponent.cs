using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HTNLShop.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        public CategoryViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            var data = new List<CategoryVM>
            {
                new CategoryVM { CategoryId = 1, CategoryName = "CPU" ,ProductCount = 11},
                new CategoryVM { CategoryId = 2, CategoryName = "Ram" ,ProductCount = 12},
                new CategoryVM { CategoryId = 3, CategoryName = "VGA" ,ProductCount = 13}
            };
            return View(data);
        }
    }
}
