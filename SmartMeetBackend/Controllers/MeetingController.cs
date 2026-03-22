using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartMeetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        [HttpPost("find-slots")]
        public IActionResult FindSmartSlots([FromBody] FindSlotRequest request)
        {
            var suggestions = new List<MeetingSlot>();
            var now = DateTime.Now;
            var random = new Random(); // Tạo chút ngẫu nhiên cho tự nhiên

            // 1. CẤU HÌNH THUẬT TOÁN
            int daysToScan = 5; // Quét 5 ngày tới
            int startHour = 8;  // Bắt đầu làm lúc 8h
            int endHour = 17;   // Nghỉ làm lúc 17h

            // 2. CHẠY VÒNG LẶP QUÉT TỪNG NGÀY
            for (int i = 1; i <= daysToScan; i++)
            {
                var checkDate = now.AddDays(i).Date;

                // Bỏ qua Thứ 7, Chủ Nhật (AI khôn là phải biết nghỉ ngơi)
                if (checkDate.DayOfWeek == DayOfWeek.Saturday || checkDate.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // Quét từng giờ trong ngày làm việc
                for (int hour = startHour; hour < endHour; hour++)
                {
                    // Logic lọc theo Yêu cầu (FR2.3)
                    bool isMorning = hour < 12;
                    bool isLunch = hour == 12; // 12h-13h là giờ nghỉ trưa

                    if (isLunch) continue; // Né giờ ăn trưa

                    if (request.Preference == "Morning" && !isMorning) continue; // Chọn sáng thì bỏ chiều
                    if (request.Preference == "Afternoon" && isMorning) continue; // Chọn chiều thì bỏ sáng

                    // Tạo slot ứng viên
                    var potentialStart = checkDate.AddHours(hour);

                    // Kiểm tra thời lượng có bị lố giờ về không
                    if (potentialStart.AddMinutes(request.DurationMinutes).Hour > endHour) continue;

                    // THUẬT TOÁN NGẪU NHIÊN GIẢ LẬP ĐỘ "BẬN"
                    // (Giả vờ 30% khung giờ là đã bị bận để AI phải đi tìm cái khác)
                    if (random.Next(0, 10) > 3)
                    {
                        suggestions.Add(new MeetingSlot
                        {
                            StartTime = potentialStart,
                            Label = GenerateLabel(potentialStart)
                        });
                    }

                    // Chỉ lấy tối đa 5 đề xuất để đỡ rối mắt
                    if (suggestions.Count >= 5) break;
                }
                if (suggestions.Count >= 5) break;
            }

            // Nếu xui quá không tìm được gì (do lọc gắt quá)
            if (suggestions.Count == 0)
            {
                return Ok(new { Message = "Rất tiếc, không tìm được giờ rảnh nào!", slots = new List<object>() });
            }

            return Ok(new
            {
                Message = $"AI đã tìm thấy {suggestions.Count} khung giờ hợp lý!",
                slots = suggestions
            });
        }

        // Hàm sinh nhãn (Label) cho chuyên nghiệp
        private string GenerateLabel(DateTime time)
        {
            if (time.Hour < 10) return "Đầu giờ sáng (Tỉnh táo)";
            if (time.Hour < 12) return "Gần trưa (Nhanh gọn)";
            if (time.Hour < 15) return "Đầu giờ chiều (Năng lượng)";
            return "Cuối ngày (Thư giãn)";
        }
    }

    public class FindSlotRequest
    {
        public int DurationMinutes { get; set; }
        public string Emails { get; set; }
        public string Preference { get; set; } // "Morning", "Afternoon", "All"
    }

    public class MeetingSlot
    {
        public DateTime StartTime { get; set; }
        public string Label { get; set; }
    }
}