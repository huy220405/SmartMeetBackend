using Microsoft.EntityFrameworkCore;
using SmartMeetBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Ép chạy cổng 5000
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// 2. Kết nối Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 👇 MỚI: Thêm dịch vụ CORS (Cho phép mọi nơi truy cập)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Cho phép tất cả (Web, Mobile...)
              .AllowAnyMethod()  // Cho phép GET, POST, PUT, DELETE
              .AllowAnyHeader(); // Cho phép mọi Header
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 👇 MỚI: Kích hoạt CORS (Phải đặt trước UseAuthorization)
app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello Huy! Server SmartMeet đang chạy ngon lành!");

app.UseAuthorization();
app.MapControllers();

app.Run();