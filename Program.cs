using ImageCommentApp.Data;
using ImageCommentApp.Models;
using ImageCommentApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ImageCommentApp.Endpoints;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SixLabors.ImageSharp;
using ImageCommentApp.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ��������� ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TestDatabase"));

// ����������� �������� ��� ��������� ����������� � �������
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();


// ��������� JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<ImageSettings>(builder.Configuration.GetSection("ImageSettings"));


// ��������� �������������� JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero // ������� �������� �� �����
    };
});

builder.Services.AddAuthorization();

// ���������� Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// ����������� ����������
app.MapAuthEndpoints();
app.MapPostEndpoints();

app.Run();
