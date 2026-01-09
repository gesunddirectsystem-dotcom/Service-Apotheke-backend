using Microsoft.EntityFrameworkCore;
using Service_Apotheke.Repository.Auth;
using Service_Apotheke.Repository.Job;
using Service_Apotheke.Repository.Pharmacist;
using Service_Apotheke.Repository.Pharmacy;
using Service_Apotheke.Services.Email;
using Service_Apotheke.Services.File;
using ServiceApothekeAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
       
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IPharmacistService, PharmacistService>();
builder.Services.AddScoped<IPharmacyService, PharmacyService>();
builder.Services.AddScoped<IJobService, JobService>();

// إزالة كل ما يتعلق بـ JWT و Authentication
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll");



app.MapControllers();



app.Run();