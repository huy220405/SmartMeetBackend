using System.ComponentModel.DataAnnotations;

namespace SmartMeetBackend.Models
{
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty; // Tiêu đề (VD: Đồ án tốt nghiệp)
        public int DurationMinutes { get; set; } = 60;    // Thời lượng (phút)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? FinalizedTime { get; set; } // Thời gian chốt cuối cùng (FR3.1)

        // Ai là người tổ chức? (Lưu ID của người dùng trong bảng User)
        public int HostId { get; set; }
        public User? Host { get; set; }

        // Danh sách khách mời (Lưu chuỗi email ngăn cách bằng dấu phẩy)
        // Ví dụ: "banA@gmail.com,banB@gmail.com"
        public string ParticipantEmails { get; set; } = string.Empty;
    }
}