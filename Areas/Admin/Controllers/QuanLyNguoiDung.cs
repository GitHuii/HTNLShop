using Azure;
using HTNLShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace HTNLShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class QuanLyNguoiDung : Controller
    {
        private readonly HtlnshopContext db;

        public QuanLyNguoiDung(HtlnshopContext context)
        {
            db = context;
        }

        [Route("Index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var list = db.Users.Include(u => u.Role).OrderBy(p => p.UserId).ToPagedList();

            return View(list);
        }

        public IActionResult AddUser()
        {
            ViewBag.RoleId = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
            return View();
        }

        [HttpPost]
        public IActionResult AddUser(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                return View(model);
            }

            try
            {
                db.Users.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm người dùng: " + ex.Message;

                ViewBag.RoleId = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                return View(model);
            }

            db.Users.Update(model);
            db.SaveChanges();

             return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
