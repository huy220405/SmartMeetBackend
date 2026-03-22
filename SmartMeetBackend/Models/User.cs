using System.ComponentModel.DataAnnotations;

namespace SmartMeetBackend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        // "Chìa khóa vàng": Token này giúp Server tự động vào lịch của user 
        // để tìm khe hở chung ngay cả khi họ offline (FR2.2)
        public string? GoogleRefreshToken { get; set; }

        // Link đặt lịch cá nhân (FR4.1) - Ví dụ: smartmeet.com/huyngo
        public string? PersonalLink { get; set; }
    }
}