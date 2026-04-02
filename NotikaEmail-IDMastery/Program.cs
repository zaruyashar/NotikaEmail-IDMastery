using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;
using NotikaEmail_IDMastery.Models.IdentityModels;
using NotikaEmail_IDMastery.Models.JWTModels;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EmailContext>();
builder.Services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<EmailContext>()
        .AddErrorDescriber<CustomIdentityValidator>()
        .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);


builder.Services.Configure<JwtSettingsModel>(builder.Configuration.GetSection("JwtSettings"));


builder.Services.AddAuthentication()
    .AddJwtBearer(opt =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsModel>();

        if (string.IsNullOrEmpty(jwtSettings?.Key))
        {
            throw new InvalidOperationException("JWT Secret Key bulunamadı. Lütfen appsettings veya User Secrets yapılandırmasını kontrol edin.");
        }

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    })
    // Google Authentication Config:
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

        var clientId = googleAuthNSection["ClientId"];
        var clientSecret = googleAuthNSection["ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Google ClientId veya ClientSecret yapılandırması eksik.");
        }

        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.CallbackPath = "/signin-google"; // this is already the default - just added for clarity.
    });


builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Error/{0}");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
