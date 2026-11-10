using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Services;

using Microsoft.AspNetCore.Mvc;

namespace CSharpAspnetBlog.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UserService users) : ControllerBase
{
    [HttpGet]
    public async Task<List<UserDto>> Index()
    {
        var result = await users.GetAllAsync();
        Response.Headers["X-Total-Count"] = result.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return result;
    }

    [HttpGet("{id:long}")]
    public Task<UserDto> Show(long id) => users.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserCreateDto dto)
    {
        var user = await users.CreateAsync(dto);
        return CreatedAtAction(nameof(Show), new { id = user.Id }, user);
    }

    [HttpPut("{id:long}")]
    public Task<UserDto> Update(long id, UserUpdateDto dto) => users.UpdateAsync(id, dto);
}
