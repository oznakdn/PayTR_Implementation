using Microsoft.Extensions.Options;
using PayTR.WebMVC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.Configure<PayTrConfiguration>(builder.Configuration.GetSection(nameof(PayTrConfiguration)));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<PayTrConfiguration>>().Value);


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
