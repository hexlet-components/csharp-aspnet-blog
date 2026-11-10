using CSharpAspNetBlog.Dtos;
using CSharpAspNetBlog.Services;

using Microsoft.AspNetCore.Mvc;

namespace CSharpAspNetBlog.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController(PostService posts) : ControllerBase
{
    [HttpGet]
    public async Task<List<PostDto>> Index()
    {
        var result = await posts.GetAllAsync();
        Response.Headers["X-Total-Count"] = result.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return result;
    }

    [HttpGet("{id:long}")]
    public Task<PostDto> Show(long id) => posts.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<PostDto>> Create(PostCreateDto dto)
    {
        var post = await posts.CreateAsync(dto);
        return CreatedAtAction(nameof(Show), new { id = post.Id }, post);
    }

    [HttpPut("{id:long}")]
    public Task<PostDto> Update(long id, PostUpdateDto dto) => posts.UpdateAsync(id, dto);

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Destroy(long id)
    {
        await posts.DeleteAsync(id);
        return NoContent();
    }
}
