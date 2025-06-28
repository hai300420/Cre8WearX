using _2_Service.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IConfiguration configuration, IUserService userService, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _userService = userService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("google-login-url")]
        public IActionResult GetGoogleLoginUrl()
        {
            var clientId = _configuration["GoogleAuth:ClientId"];
            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["client_id"] = clientId;
            queryParams["redirect_uri"] = redirectUri;
            queryParams["response_type"] = "code";
            queryParams["scope"] = "openid email profile";
            queryParams["access_type"] = "offline";
            queryParams["prompt"] = "consent";

            string url = $"https://accounts.google.com/o/oauth2/v2/auth?{queryParams}";

            return Ok(new { url });
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Missing code from Google.");
            }

            var clientId = _configuration["GoogleAuth:ClientId"];
            var clientSecret = _configuration["GoogleAuth:ClientSecret"];
            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            var httpClient = _httpClientFactory.CreateClient();

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                })
            };

            var tokenResponse = await httpClient.SendAsync(tokenRequest);
            var responseText = await tokenResponse.Content.ReadAsStringAsync();
            if (!tokenResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Google Token Response:\n" + responseText);
                return StatusCode(500, $"Failed to exchange code for tokens. {responseText}");
            }


            var payload = JsonSerializer.Deserialize<JsonElement>(await tokenResponse.Content.ReadAsStringAsync());

            string idToken = payload.GetProperty("id_token").GetString();

            // Delegate to UserService to handle login and JWT generation
            var jwtToken = await _userService.GoogleLoginAsync(idToken);

            // Return in json form
            // return Ok(new { token = jwtToken });
            // return Redirect($"https://cre8wrearx.vercel.app?token={jwtToken}");
            // return Redirect($"http://localhost:5173?token={jwtToken}");
            //var html = $@"
            //        <html>
            //        <body>
            //        <script>
            //            window.opener.postMessage({{ token: '{jwtToken}' }}, 'https://cre8wrearx.vercel.app');
            //            window.close();
            //        </script>
            //        </body>
            //        </html>";
            //return Content(html, "text/html");

            var frontendUrl = _configuration["FrontendRedirectUrl"];
            return Redirect($"{frontendUrl}?token={jwtToken}");

        }
    }
}
