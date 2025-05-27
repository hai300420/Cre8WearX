using _2_Service.Service;
using BusinessObject;
using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region CRUD User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] UserRegisterDTO userDto)
        {
            try
            {
                await _userService.AddUser(userDto);
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UserDTO userDto)
        {
            await _userService.UpdateUser(id, userDto);
            return NoContent();
        }

        [Authorize(Roles = "admin, staff")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
        #endregion


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO userDto)
        {
            try
            {
                var response = await _userService.Login(userDto);

                if (response.Status == Const.FAIL_READ_CODE)
                {
                    return Unauthorized(response.Message);
                }

                if (response.Data == null) // Check if the token is missing
                {
                    return StatusCode(500, new { message = "Login failed due to internal error." });
                }
                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                // return BadRequest(ex.Message);
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDTO userDto)
        {
            try
            {
                var response = await _userService.ChangePassword(userDto);

                if (response.Status == Const.FAIL_READ_CODE)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response);
            }

            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }

        

        [Authorize(Roles = "admin, staff")]
        [HttpPut("recover/{id}")]
        public async Task<ActionResult> Recover(int id)
        {
            await _userService.RecoverUser(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var response = await _userService.GetUserProfile();
                if (response.Status == Const.FAIL_READ_CODE)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDTO userDto)
        {
            try
            {
                var response = await _userService.UpdateUserProfile(userDto);
                if (response.Status == Const.FAIL_READ_CODE)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }



        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                string jwtToken = await _userService.GoogleLoginAsync(request.IdToken);
                return Ok(new { token = jwtToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
