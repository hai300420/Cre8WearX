using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Service
{
    public class JWTService : IJWTService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JWTService(IUnitOfWork unitOfWork, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(User account)
        {
            /*
            var tokenSecret = _config["Jwt:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(tokenSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Role, account.Role!.RoleName.ToString()!.Trim())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:TokenExpiryMinutes"])),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
            */
            if (account == null)
                throw new ArgumentNullException(nameof(account), "User account is null in GenerateToken!");

            if (account.Role == null)
                throw new Exception("User role is null in GenerateToken!");

            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("User_Id",account.UserId.ToString()),
                    new Claim("Username", account.Username.ToString()),
                    new Claim(ClaimTypes.Role, account.Role.RoleName.ToLower())  // Store Role in Token
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:TokenExpiryMinutes"])),
                signingCredentials: signIn
            );
            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;

        }



        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // Không cho phép chênh lệch thời gian
            };

            return tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        }
        public async Task<User> GetCurrentUserAsync()
        {
            // Lấy token từ header Authorization
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                User existUser = null;
                return existUser;
            }

            // Giải mã token để lấy thông tin người dùng
            var claimsPrincipal = ValidateToken(token);

            // Lấy UserId từ claims
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User not found in token."); // Hoặc trả về một ResponseDTO nếu cần
            }

            int userId = int.Parse(userIdClaim.Value);

            // Lấy người dùng hiện tại
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found!"); // Hoặc trả về một ResponseDTO nếu cần
            }

            return user;
        }
    }
}
