using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UretimTakip.Business.services;
using UretimTakip.DataAccess.Context;
using UretimTakip.DataAccess.Interceptors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(new AuditInterceptor()));

// Business Services (İş Mantığı Katmanı Servisleri)
builder.Services.AddScoped<IStokService, StokService>();
builder.Services.AddScoped<IUrunService, UrunService>();
builder.Services.AddScoped<IDepoService, DepoService>();
builder.Services.AddScoped<ICariService, CariService>();
builder.Services.AddScoped<ISiparisService, SiparisService>();
builder.Services.AddScoped<IHedefService, HedefService>();
builder.Services.AddScoped<IBildirimService, BildirimService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
