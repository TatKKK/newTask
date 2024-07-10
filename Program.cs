using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using newTask.Auth;
using newTask.FIlters;
using newTask.Packages;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPKG_COMPANIES, PKG_COMPANIES>();
builder.Services.AddScoped<IPKG_USERS, PKG_USERS>();    
builder.Services.AddScoped<IPKG_TASKS, PKG_TASKS>();
builder.Services.AddScoped<IPKG_ERROR_LOGS, PKG_ERROR_LOGS>();
builder.Services.AddScoped<IJwtManager, JwtManager>();
builder.Services.AddScoped<GlobalExceptionFilter>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddControllers(config =>
{
    config.Filters.AddService(typeof(GlobalExceptionFilter));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(o =>
               {
                   var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:key"]);
                   o.SaveToken = true;
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       //ValidIssuer = builder.Configuration["JWT:Issuer"],
                       //ValidAudience = builder.Configuration["JWT:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(key),
                       RoleClaimType = ClaimTypes.Role,
                       NameClaimType = "UserId"
                   };
               });


builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("RequiredAdminRole", policy => policy.RequireRole("admin"));
    o.AddPolicy("RequiredManagerRole", policy => policy.RequireRole("manager"));
    o.AddPolicy("RequiredOperatorRole", policy => policy.RequireRole("developer"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
