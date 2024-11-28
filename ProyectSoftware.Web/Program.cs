using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web;
using ProyectSoftware.Web.Data;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();// Agrega servicios necesarios para admitir controladores y vistas MVC.


builder.AddCustomBuilderConfiguration();

WebApplication app = builder.Build();// Construye la aplicaci�n.


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

app.UseAuthorization();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Dashboard}/{id?}");

app.AddCustomConfiguration();

app.Run();
