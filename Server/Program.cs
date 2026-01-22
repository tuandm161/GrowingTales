using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Server.Data;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add SQLite Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=growingtales.db")
    .ConfigureWarnings(warnings => warnings
        .Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// Add HttpClient for external APIs
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<ImageGenerationService>(client =>
{
    var timeoutSeconds = builder.Configuration.GetValue<int?>("WhomeAI:TimeoutSeconds") ?? 60;
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

// Add custom services
builder.Services.AddSingleton<GeminiService>();
builder.Services.AddScoped<StoryStorageService>();
builder.Services.AddSingleton<ImageGenerationService>();
builder.Services.AddSingleton<ImagePromptService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddSingleton<VnPayService>();

var app = builder.Build();

// Auto-run database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Checking database and running migrations...");
        db.Database.Migrate(); // Tự động chạy tất cả migrations
        logger.LogInformation("Database migrations completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database");
        // Fallback to EnsureCreated if migration fails
        db.Database.EnsureCreated();
        logger.LogWarning("Used EnsureCreated as fallback");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
