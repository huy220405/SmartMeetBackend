using Microsoft.EntityFrameworkCore;
using SmartMeetBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. CHỮA BỆNH CORS: Cho phép mọi ứng dụng (kể cả Flutter Web) gọi vào Server này
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình Database SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=smartmeet.db"));

var app = builder.Build();

// 2. CHỮA BỆNH DATABASE: Ép Server phải tự tạo bảng (table) nếu chưa có khi khởi động
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Lệnh này đảm bảo Database và các bảng được tạo ra đầy đủ trên mây
    dbContext.Database.EnsureCreated();
}

// Kích hoạt CORS
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();