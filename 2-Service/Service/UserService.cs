using _2_Service.Service.IService;
using _2_Service.Utils;
using _3_Repository.IRepository;
using _3_Repository.Repository;
using Azure.Core;
using BusinessObject;
using BusinessObject.Enum;
using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace _2_Service.Service
{
    //public interface IUserService
    //{
    //    #region CRUD User
    //    Task<IEnumerable<User>> GetAllUsers();
    //    Task<User> GetUserById(int id);
    //    Task AddUser(UserRegisterDTO userDto);
    //    Task UpdateUser(int id, UserDTO userDto);
    //    Task DeleteUser(int id);
    //    #endregion

    //    Task<ResponseDTO> Login(LoginRequestDTO userDto);
    //    Task<ResponseDTO> ChangePassword(ChangePasswordDTO userDto);
    //    Task<string> GoogleLoginAsync(string idToken);
    //    Task RecoverUser(int id);
    //    Task<ResponseDTO> GetUserProfile();
    //    Task<ResponseDTO> UpdateUserProfile(UserUpdateDTO userDto);
    //}


    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJWTService _jWTService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IJWTService jWTService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jWTService = jWTService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region CRUD User
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            var adminId = await _roleRepository.GetIdByNameAsync("admin");
            var staffId = await _roleRepository.GetIdByNameAsync("staff");

            if (user == null)
                return null;
            // Get the current user's role
            var currentUserRole = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Prevent staff from viewing admin users and other staffs
            if (currentUserRole.ToLower() == "staff" && (user.RoleId == adminId.RoleId || user.RoleId == staffId.RoleId))
            {
                return null;
            }
            return user;
        }


        public async Task AddUser(UserRegisterDTO userDto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(userDto.Username);
            if (existingUser != null) throw new Exception("User is existed.");

            // Get the current user's role (null for guests)
            string? currentUserRole = _httpContextAccessor.HttpContext?.User.Claims
                                        .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Get the requested role
            var requestedRole = await _roleRepository.GetIdByNameAsync(userDto.RoleName);
            var memberRole = await _roleRepository.GetIdByNameAsync("member");

            if (memberRole == null) throw new Exception("Member role does not exist.");
            if (requestedRole == null) requestedRole = memberRole;

            int assignedRoleId = requestedRole.RoleId;

            // Staff and guests can only create members
            if (string.IsNullOrEmpty(currentUserRole) || currentUserRole.ToLower() == "staff")
            {
                assignedRoleId = memberRole.RoleId;
            }

            var user = new User
            {
                Username = userDto.Username,
                // Password = userDto.Password, 
                Password = PasswordSecurity.HashPassword(userDto.Password),
                FullName = userDto.FullName,
                Email = userDto.Email,
                Gender = userDto.Gender,
                DateOfBirth = userDto.DateOfBirth,
                Address = userDto.Address,
                Phone = userDto.Phone,
                Avatar = userDto.Avatar,
                IsDeleted = false,
                RoleId = assignedRoleId
            };

            await _userRepository.AddAsync(user);
        }
        public async Task UpdateUser(int id, UserDTO userDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) throw new Exception("User not found.");

            var currentUserRole = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            existingUser.FullName = userDto.FullName;
            existingUser.Email = userDto.Email;
            existingUser.Gender = userDto.Gender;
            existingUser.DateOfBirth = userDto.DateOfBirth;
            existingUser.Address = userDto.Address;
            existingUser.Phone = userDto.Phone;
            existingUser.Avatar = userDto.Avatar;

            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task DeleteUser(int id)
        {
            await _userRepository.SoftDeleteAsync(id);
        }
        #endregion



        public async Task<ResponseDTO> Login(LoginRequestDTO userDto)
        {
            try
            {
                var account = await _userRepository.GetByUsernameAsync(userDto.Username);

                if (account == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Invalid username or password.");
                }

                // if (account.Password != userDto.Password) 
                if (!PasswordSecurity.VerifyPassword(userDto.Password, account.Password))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Invalid username or password.");
                }

                var jwt = _jWTService.GenerateToken(account);
                // Debugging: Print claims
                var claims = new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims;
                foreach (var claim in claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }
                //var loginResponse = new ResponseDTO.LoginResponse
                //{
                //    UserId = account.UserId,
                //    UserName = account.Username,
                //    Password = account.Password,
                //    Phone = account.Phone,
                //    FullName = account.FullName,
                //    IsDeleted = account.IsDeleted,
                //    RoleName = account.Role.RoleName,
                //};
                //return new ResponseDTO(Const.SUCCESS_READ_CODE, "Login successful", new
                //{
                //    Token = jwt,
                //    UserData = loginResponse
                //});

                return new ResponseDTO(Const.SUCCESS_READ_CODE, "Login successful", jwt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }


        public async Task RecoverUser(int id)
        {
            await _userRepository.RecoverAsync(id);
        }

        public async Task<string> GoogleLoginAsync(string idToken)
        {
            try
            {
                // Verify Google ID Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                string email = payload.Email;

                if (string.IsNullOrEmpty(email))
                {
                    throw new Exception("Invalid Google token.");
                }

                // Check if user exists in database
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    // Register new user if not found
                    user = new User
                    {
                        Username = string.Empty,
                        Password = string.Empty,
                        Email = email,
                        FullName = payload.Name,
                        Gender = true,
                        DateOfBirth = DateTime.UtcNow,
                        Avatar = payload.Picture,
                        IsDeleted = false,
                        RoleId = 3,
                    };

                    await _userRepository.AddAsync(user);
                    await _userRepository.GetByEmailAsync(email);
                }

                // Generate JWT token
                return _jWTService.GenerateToken(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Google Login Error: " + ex.ToString());
                throw new Exception("Google login failed: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        public async Task<ResponseDTO> GetUserProfile()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                                .FirstOrDefault(c => c.Type == "User_Id");

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Unauthorized: User ID not found in token.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Invalid User ID format.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "User not found.");
            }

            var userProfile = new
            {
                FullName = user.FullName,
                Email = user.Email,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Phone = user.Phone,
                Avatar = user.Avatar,
                RoleName = await _roleRepository.GetNameByIdAsync(user.RoleId)
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, "User Profile", userProfile);
        }

        public async Task<ResponseDTO> UpdateUserProfile(UserUpdateDTO userDto)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                                .FirstOrDefault(c => c.Type == "User_Id");

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Unauthorized: User ID not found in token.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Invalid User ID format.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "User not found.");
            }

            // Update user properties
            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.Gender = userDto.Gender;
            user.DateOfBirth = userDto.DateOfBirth;
            user.Address = userDto.Address;
            user.Phone = userDto.Phone;
            user.Avatar = userDto.Avatar;

            await _userRepository.UpdateAsync(user);

            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Profile updated successfully.", user);
        }

        public async Task<ResponseDTO> ChangePassword(ChangePasswordDTO userDto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(userDto.Username);
            if (existingUser == null) return new ResponseDTO(Const.FAIL_READ_CODE, "User is not existing");
            if (userDto.Password != userDto.ConfirmPassword) return new ResponseDTO(Const.FAIL_READ_CODE, "Confirm Password must be similar to Password");

            // existingUser.Password = userDto.Password;
            existingUser.Password = PasswordSecurity.HashPassword(userDto.Password);  // Hash password

            await _userRepository.UpdateAsync(existingUser);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Password changed successfully.");
        }

    }
}
