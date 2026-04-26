using System.Text;
using AdminPanelBack;
using AdminPanelBack.DB;
using AdminPanelBack.Middleware;
using AdminPanelBack.Models;
using AdminPanelBack.Profiles;
using AdminPanelBack.Repository;
using AdminPanelBack.Services.Auth;
using AdminPanelBack.Services.Broadcast;
using AdminPanelBack.Services.Feedback;
using AdminPanelBack.Services.Login;
using AdminPanelBack.Services.Statistic;
using AdminPanelBack.Services.Token;
using AdminPanelBack.Services.User;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using Serilog;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["DefaultConnection"];

if (string.IsNullOrWhiteSpace(connectionString) && builder.Environment.IsDevelopment())
{
    connectionString = "Host=db;Port=5432;Database=feedbackdb;Username=postgres;Password=postgres";
}

if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("DefaultConnection not set");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<Admin, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBroadcastMessageService, BroadcastMessageService>();
builder.Services.AddScoped<IBroadcastMessageRepository, BroadcastMessageRepository>();
builder.Services.AddAutoMapper(typeof(FeedbackProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(builder.Configuration["CORS_ORIGIN"] ?? "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); 
});

var secretKey = builder.Configuration["JWT_SECRET_KEY"];
var issuer = builder.Configuration["JWT_ISSUER"];
var audience = builder.Configuration["JWT_AUDIENCE"];

if (builder.Environment.IsDevelopment())
{
    secretKey ??= "dev-dev-dev-dev-dev-dev-dev-dev-dev-dev-32chars";
    issuer ??= "adminpanel";
    audience ??= "frontadminpanel";
}

if (string.IsNullOrWhiteSpace(secretKey)) throw new Exception("JWT_SECRET_KEY not set");
if (string.IsNullOrWhiteSpace(issuer)) throw new Exception("JWT_ISSUER not set");
if (string.IsNullOrWhiteSpace(audience)) throw new Exception("JWT_AUDIENCE not set");
int.TryParse(builder.Configuration["JWT_EXPIRES_IN_MINUTES"], out var expiration);

var jwtSettings = new JwtSettings
{
    SecretKey = secretKey,
    Issuer = issuer,
    Audience = audience,
    ExpiresInMinutes = expiration > 0 ? expiration : 60
};

builder.Services.Configure<JwtSettings>(options =>
{
    options.SecretKey = jwtSettings.SecretKey;
    options.Issuer = jwtSettings.Issuer;
    options.Audience = jwtSettings.Audience;
    options.ExpiresInMinutes = jwtSettings.ExpiresInMinutes;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["SEQ_URL"] ?? "http://localhost:5341")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

var migrateOnStartup = app.Environment.IsDevelopment() ||
                       builder.Configuration.GetValue<bool>("MIGRATE_ON_STARTUP");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<Admin>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var env = services.GetRequiredService<IHostEnvironment>();

        if (migrateOnStartup)
        {
            dbContext.Database.Migrate();
            Log.Information("Database migrations applied on startup.");
        }
        else
        {
            Log.Information("Skipping database migrations on startup. Set MIGRATE_ON_STARTUP=true to enable.");
        }

        var seeded = await SeedAdmin.SeedAdminAsync(userManager, roleManager, builder.Configuration, env);
        Log.Information("Admin seed finished. Database: {DbName}. Admin seeded: {AdminStatus}",
            dbContext.Database.GetDbConnection().Database, seeded);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Startup DB initialization error");
        throw;
    }
}

app.UseMiddleware<ErrorHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapPrometheusScrapingEndpoint("/metrics");

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
