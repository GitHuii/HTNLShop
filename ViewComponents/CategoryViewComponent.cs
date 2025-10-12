using HTNLShop.Data;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HTNLShop.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly HtlnshopContext _context;
        public CategoryViewComponent(HtlnshopContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var data = _context.Categories
                .Select(c => new CategoryVM
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.CategoryId)
                })
                .ToList();
            return View(data);
        }
    }
}
