using Microsoft.EntityFrameworkCore;
using Service_Apotheke.Repository.Auth;
using Service_Apotheke.Repository.Job;
using Service_Apotheke.Repository.Pharmacist;
using Service_Apotheke.Repository.Pharmacy;
using Service_Apotheke.Services.Email;
using Service_Apotheke.Services.File;
using ServiceApothekeAPI.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IPharmacistService, PharmacistService>();
builder.Services.AddScoped<IPharmacyService, PharmacyService>();
builder.Services.AddScoped<IJobService, JobService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// --- التعديل هنا يا سطة ---
// خرجنا السواجر بره الـ If عشان يفتح في أي مكان
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service Apotheke API V1");
    c.RoutePrefix = string.Empty; // ده بيخلي السواجر يفتح أول ما تفتح اللينك علطول بدل ما تكتب /swagger
});
// ------------------------

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.MapControllers();

// ده عشان يخلي المشروع يفتح على كل الـ IPs المتاحة للجهاز
app.Run();