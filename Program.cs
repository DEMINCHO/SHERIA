using NLog;
using OfficeOpenXml;
using SHERIA.Helpers;
using SHERIA.Models;
using SHERIA.Utility;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMvc().AddNewtonsoftJson();
// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<DBHandler>(d => new DBHandler(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(10); //You can set Time   
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<ILoggerManager, LoggerManager>();
builder.Services.AddHttpClient();

// Set EPPlus License Context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
SD.mpesa = builder.Configuration["ServiceUrls:mpesaAPI"];

var app = builder.Build();

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/NLog.config"));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AppAuth}/{action=AdminLogin}/{id?}");

app.Run();
