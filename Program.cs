using HTNLShop.Controllers;
using HTNLShop.Data;
using HTNLShop.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CartController>();

builder.Services.AddDbContext<HtlnshopContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("HTLNShop"));
});


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Customer/Login";
        options.LogoutPath = "/Customer/Logout";
        options.AccessDeniedPath = "/Customer/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var userPrincipal = context.Principal;
                var roleClaim = userPrincipal?.FindFirst(ClaimTypes.Role);

                if (roleClaim == null ||
                    (roleClaim.Value != "Admin" && roleClaim.Value != "Customer"))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        };
    });

var app = builder.Build();

app.UseStaticFiles();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication(); 
app.UseAuthorization();
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    if (path == "/" || path == "/trangchu" || path == "/trangchu/index")
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var roleClaim = context.User.FindFirst(ClaimTypes.Role);

            if (roleClaim?.Value == "Admin")
            {
                context.Response.Redirect("/admin");
                return;
            }
        }
    }

    await next();
});
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TrangChu}/{action=Index}/{id?}");

app.Run();