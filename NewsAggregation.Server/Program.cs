using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Configuration;
using NewsAggregation.Server.Services.Interfaces;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Repository;
using NewsAggregation.Server.Services;
using System.Text.Json;
using NewsAggregation.Server.Services.ExternalClients;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;


var appSettings = new AppSettings();
configuration.Bind(appSettings);
services.Configure<AppSettings>(configuration);


services.AddDbContext<NewsAggregationContext>(options =>
    options.UseSqlServer(appSettings.ConnectionStrings.DefaultConnection));


var jwtSettings = appSettings.JwtSettings;
var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

services.AddAuthorization();


services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<INewsRepository, NewsRepository>();
services.AddScoped<IExternalServerRepository, ExternalServerRepository>();
services.AddScoped<INotificationRepository, NotificationRepository>();
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<IReportRepository, ReportRepository>();
services.AddScoped<IFilteredKeywordRepository, FilteredKeywordRepository>();
services.AddScoped<IUserArticleLikeRepository, UserArticleLikeRepository>();
services.AddScoped<IUserArticleReadRepository, UserArticleReadRepository>();


services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<INewsService, NewsService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<IExternalNewsService, ExternalNewsService>();
services.AddScoped<ITheNewsApiClient, TheNewsApiClient>();
services.AddScoped<IExternalServerService, ExternalServerService>();
services.AddScoped<ICategoryService, CategoryService>();
services.AddScoped<INotificationService, NotificationService>();
services.AddScoped<ExternalNewsService, ExternalNewsService>();
builder.Services.AddScoped<IExternalNewsService, ExternalNewsService>();
builder.Services.AddScoped<ExternalNewsService>();


services.AddHttpClient();


services.AddHostedService<NewsAggregationService>();


services.AddControllers()
    .AddJsonOptions(options =>
    {
     
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

       
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

     
        options.JsonSerializerOptions.WriteIndented = true;
    });

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "News Aggregation API",
        Version = "v1",
        Description = "API for News Aggregation System"
    });

    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "News Aggregation API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NewsAggregationContext>();

    context.Database.EnsureCreated();
}

Console.WriteLine("News Aggregation Server is starting...");
Console.WriteLine($"Swagger UI available at: https://localhost:7000");
Console.WriteLine($"API Base URL: https://localhost:7000/api");

app.Run();