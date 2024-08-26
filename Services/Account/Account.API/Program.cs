
using Account.Application.AutoMapper;
using Account.Application.Services;
using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Cache;
using Account.Infrastructure.Context;
using Account.Infrastructure.Repositories;
using Account.Presentation.Endpoints;
using Common.Cache;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Thêm cấu hình Bearer Token cho Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter JWT Bearer token in the format **Bearer [token]**",
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
// Thêm cấu hình Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
//Rate limiting middlewares
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));



// Add Identity services
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
// Register application services
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<UserRedisCache>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
// Add DbContext configuration
builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Account"), b => b.MigrationsAssembly("Account.PostgresMigrations")));

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<AccountDbContext>();
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
    options.ExcludedHosts.Add("example.com");
    options.ExcludedHosts.Add("www.example.com");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();

}
//Minimals APIs

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

//Map group Endpoints
UserEndpoints.Map(app);
IdentityEndpoints.MapCustomIdentityApi<User>(app);

//Identity call map group
//app.MapGroup("/Account").MapIdentityApi<User>();

// Run the application
app.Run();
