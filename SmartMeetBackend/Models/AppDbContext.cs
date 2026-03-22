using Microsoft.EntityFrameworkCore;

namespace SmartMeetBackend.Models // Lưu ý: Đặt cùng namespace với Models cho tiện
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Khai báo 2 bảng dữ liệu sẽ được tạo
        public DbSet<User> Users { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
    }
}