using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Model;

namespace Service
{
    public interface IJWTService
    {
        /// <summary>
        /// Tạo JWT token cho người dùng.
        /// </summary>
        /// <param name="account">Đối tượng người dùng.</param>
        /// <returns>Token đã tạo.</returns>
        string GenerateToken(User account);

        /// <summary>
        /// Xác thực và giải mã token JWT.
        /// </summary>
        /// <param name="token">Token JWT.</param>
        /// <returns>ClaimsPrincipal chứa thông tin người dùng.</returns>
        ClaimsPrincipal ValidateToken(string token);

        /// <summary>
        /// Lấy thông tin người dùng hiện tại từ token.
        /// </summary>
        /// <returns>Đối tượng người dùng hiện tại.</returns>
        //Task<User> GetCurrentUserAsync();


    }
}
