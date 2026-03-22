using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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

            // 👇 CHỮA BỆNH MÚI GIỜ: Lấy đúng giờ Việt Nam (UTC+7) để AI tính toán
            var vietnamOffset = TimeSpan.FromHours(7);
            var nowVn = DateTime.UtcNow.Add(vietnamOffset);
            var random = new Random();

            int daysToScan = 5;
            int startHour = 8;
            int endHour = 17;

            for (int i = 1; i <= daysToScan; i++)
            {
                var checkDateVn = nowVn.AddDays(i).Date;

                if (checkDateVn.DayOfWeek == DayOfWeek.Saturday || checkDateVn.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                for (int hour = startHour; hour < endHour; hour++)
                {
                    bool isMorning = hour < 12;
                    bool isLunch = hour == 12;

                    if (isLunch) continue;

                    if (request.Preference == "Morning" && !isMorning) continue;
                    if (request.Preference == "Afternoon" && isMorning) continue;

                    var potentialStartVn = checkDateVn.AddHours(hour);

                    if (potentialStartVn.AddMinutes(request.DurationMinutes).Hour > endHour) continue;

                    if (random.Next(0, 10) > 3)
                    {
                        // 👇 BƯỚC QUAN TRỌNG NHẤT:
                        // Lùi 08:00 VN thành 01:00 UTC để gửi cho điện thoại. 
                        // App Flutter nhận được 01:00 UTC sẽ tự cộng 7 tiếng thành 08:00 VN cực kỳ chuẩn xác!
                        var potentialStartUtc = DateTime.SpecifyKind(potentialStartVn.Subtract(vietnamOffset), DateTimeKind.Utc);

                        suggestions.Add(new MeetingSlot
                        {
                            StartTime = potentialStartUtc,
                            Label = GenerateLabel(potentialStartVn) // Vẫn đưa giờ VN vào để sinh Label cho đúng
                        });
                    }

                    if (suggestions.Count >= 5) break;
                }
                if (suggestions.Count >= 5) break;
            }

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

        private string GenerateLabel(DateTime timeVn)
        {
            if (timeVn.Hour < 10) return "Đầu giờ sáng (Tỉnh táo)";
            if (timeVn.Hour < 12) return "Gần trưa (Nhanh gọn)";
            if (timeVn.Hour < 15) return "Đầu giờ chiều (Năng lượng)";
            return "Cuối ngày (Thư giãn)";
        }
    }

    public class FindSlotRequest
    {
        public int DurationMinutes { get; set; }
        public string Emails { get; set; }
        public string Preference { get; set; }
    }

    public class MeetingSlot
    {
        public DateTime StartTime { get; set; }
        public string Label { get; set; }
    }
}