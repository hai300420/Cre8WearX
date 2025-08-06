using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service.IService
{
    public interface IUserService
    {
        #region CRUD User
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task AddUser(UserRegisterDTO userDto);
        Task UpdateUser(int id, UserDTO userDto);
        Task DeleteUser(int id);
        #endregion

        Task<ResponseDTO> Login(LoginRequestDTO userDto);
        Task<ResponseDTO> ChangePassword(ChangePasswordDTO userDto);
        Task<string> GoogleLoginAsync(string idToken);
        Task RecoverUser(int id);
        Task<ResponseDTO> GetUserProfile();
        Task<ResponseDTO> UpdateUserProfile(UserUpdateDTO userDto);
    }

}
