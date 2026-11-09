using RaceDay.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC
builder.Services.AddControllersWithViews();

// 2. Add HttpContextAccessor (Required for ApiService to read Session)
builder.Services.AddHttpContextAccessor();

// 3. Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Connect to the API URL from appsettings.json
// This registers ApiService so you can inject it into your Controllers!
builder.Services.AddHttpClient<IApiService, ApiService>(client => {
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
});

var app = builder.Build();

// 5. Middleware Pipeline (The order here is CRITICAL)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Allows CSS, JS, and Images to load

app.UseRouting();

app.UseSession(); // MUST be before Authorization

app.UseAuthorization();

// 6. Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 7. Start the app (ALWAYS the very last line)
app.Run();