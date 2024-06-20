using LoginPage.Web.Data.Context;
using LoginPage.Web.Data.Identity;
using LoginPage.Web.Repositories;
using LoginPage.Web.Services.AccountServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<UserDbContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    var configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giri� yapmadan eri�ilmeye �al���ld���nda y�nlendirilecek sayfa
    });
builder.Services.AddIdentity<AppIdentityUser, AppIdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{

    //options.Password.RequireDigit = true; //�ifre Say�sal karakteri desteklesin mi?
    options.Password.RequiredLength = 6;  //�ifre minumum karakter say�s�
    options.Password.RequireLowercase = true; //�ifre k���k harf olabilir
    options.Password.RequireLowercase = true; //�ifre b�y�k harf olabilir
    options.Password.RequireNonAlphanumeric = false; //Sembol bulunabilir

    options.Lockout.MaxFailedAccessAttempts = 5; //Kullan�c� ka� ba�ar�s�z giri�ten sonra sisteme giri� yapamas�n
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //Ba�ar�s�z giri� i�lemlerinden sonra ne kadar s�re sonra sisteme giri� hakk� tan�ns�n
    options.Lockout.AllowedForNewUsers = true; //Yeni �yeler i�in kilit sistemi ge�erli olsun mu

    options.User.RequireUniqueEmail = true; //Kullan�c� benzersiz e-mail adresine sahip olsun

    options.SignIn.RequireConfirmedEmail = false; //Kay�t i�lemleri i�in email onaylamas� zorunlu olsun mu?
    options.SignIn.RequireConfirmedPhoneNumber = false; //Telefon onay� olsun mu?
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
