using CorporateSite.Web.Infrastructure;
using CorporateSite.Web.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(o =>
{
    o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

// 切換 Mock / 真實 Api：appsettings.Development.json 設 "UseMockData": true 即可不需 DB
if (builder.Configuration.GetValue<bool>("UseMockData"))
{
    builder.Services.AddSingleton<INewsService, MockNewsService>();
    builder.Services.AddSingleton<IArticleService, MockArticleService>();
}
else
{
    builder.Services.AddHttpClient<NewsApiClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001");
    });
    builder.Services.AddScoped<INewsService>(sp => sp.GetRequiredService<NewsApiClient>());

    builder.Services.AddHttpClient<ArticleApiClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001");
    });
    builder.Services.AddScoped<IArticleService>(sp => sp.GetRequiredService<ArticleApiClient>());
}

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(o =>
    {
        o.MaxAge = TimeSpan.FromDays(365);
        o.IncludeSubDomains = true;
        o.Preload = true;
    });
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute("areas", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
