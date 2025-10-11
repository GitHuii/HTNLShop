using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HTNLShop.ViewComponents
{
    public class DanhMucViewComponent : ViewComponent
    {
        public DanhMucViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            var data = new List<DanhMucVM>
            {
                new DanhMucVM { MaDanhMuc = 1, TenDanhMuc = "CPU" },
                new DanhMucVM { MaDanhMuc = 2, TenDanhMuc = "Ram" },
                new DanhMucVM { MaDanhMuc = 3, TenDanhMuc = "VGA" }
            };
            return View(data);
        }
    }
}
