using Backend.Modules.Shared.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Modules.Auth.Presentation;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("upload-avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }
        
        var userId = _userService.GetUserIdFromJwt(User);

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _userService.UpdateAvatarAsync(userId.Value, file);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(new { avatarUrl = result.Value });
    }
}