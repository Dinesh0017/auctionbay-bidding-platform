using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Api.Data;
using Api.Entities;
using Api.Services.EmailService;
using MailKit.Net.Smtp;
using Api.Services.FileService;
using Microsoft.Extensions.FileProviders;
using Api.Services.JwtService;
using Api.Services.UserService;
using Stripe;
using Api.Services.NftService;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var JWTSetting = builder.Configuration.GetSection("JWTSetting");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();



builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt => {
    opt.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 403; 
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new { message = "You have no access" });
            return context.Response.WriteAsync(result);
        }
    };
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = JWTSetting["ValidAudience"],
        ValidIssuer = JWTSetting["ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSetting.GetSection("securityKey").Value!))
    };
});



builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Example : 'Bearer eyeleieieekeieieie",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "outh2",
                Name="Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

});

builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<INftRepository, NftRepository>();

builder.Services.AddSingleton<SmtpClient>();
builder.Services.AddSingleton<IEmailService, EmailService>();


StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddTransient<IFileService, Api.Services.FileService.FileService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); ;
        });
});

var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(() =>
{
    app.Services.GetRequiredService<SmtpClient>().Disconnect(true);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/wwwroot"
});

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
