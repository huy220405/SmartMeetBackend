using Microsoft.AspNetCore.Mvc;
using SmartMeetBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json; 

namespace SmartMeetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        // 👇 Client ID MỚI (Khớp với App Flutter)
        private const string ClientId = "666130042077-cib4mgdc20b1i8dc4im7npa030h0gv4r.apps.googleusercontent.com";

        public AuthController(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] LoginRequest request)
        {
            try
            {
                // 👇 Code MỚI: Dùng Access Token để hỏi Google
                var googleApiUrl = "https://www.googleapis.com/oauth2/v3/userinfo";
                
                var apiRequest = new HttpRequestMessage(HttpMethod.Get, googleApiUrl);
                apiRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken);

                var response = await _httpClient.SendAsync(apiRequest);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { Error = "Token Google không hợp lệ hoặc đã hết hạn" });
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var googleUser = JsonSerializer.Deserialize<GoogleUserInfo>(jsonString);

                if (googleUser == null || string.IsNullOrEmpty(googleUser.email))
                {
                     return BadRequest(new { Error = "Không lấy được Email từ Google" });
                }

                // 👇 Code MỚI: Tìm hoặc tạo User
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == googleUser.email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = googleUser.email,
                        FullName = googleUser.name,
                        AvatarUrl = googleUser.picture,
                        GoogleRefreshToken = "" 
                    };
                    _context.Users.Add(user);
                }
                
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Đăng nhập thành công!",
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.FullName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Lỗi xử lý: " + ex.Message });
            }
        }
    }

    // 👇 Class này dùng AccessToken (Server sẽ không đòi AuthCode nữa)
    public class LoginRequest
    {
        public string AccessToken { get; set; } = string.Empty;
    }

    public class GoogleUserInfo
    {
        public string email { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
    }
}