using CSharpAspNetBlog.Dtos;
using CSharpAspNetBlog.Services;

using Microsoft.AspNetCore.Mvc;

namespace CSharpAspNetBlog.Controllers;

[ApiController]
[Route("api")]
public class PostCommentsController(PostCommentService comments) : ControllerBase
{
    /// <summary>Все комментарии с фильтрами postId, authorId, createdAtGt и страницами по 10.</summary>
    [HttpGet("posts_comments")]
    public Task<PageDto<PostCommentDto>> Index([FromQuery] PostCommentFilterDto filter, int page = 1) =>
        comments.GetPageAsync(filter, page);

    [HttpGet("posts/{postId:long}/comments")]
    public Task<PageDto<PostCommentDto>> IndexForPost(
        long postId, [FromQuery] PostCommentFilterDto filter, int page = 1) =>
        comments.GetPageForPostAsync(postId, filter, page);

    [HttpGet("posts/{postId:long}/comments/{id:long}")]
    public Task<PostCommentDto> Show(long postId, long id) => comments.GetAsync(postId, id);

    [HttpPost("posts/{postId:long}/comments")]
    public async Task<ActionResult<PostCommentDto>> Create(long postId, PostCommentCreateDto dto)
    {
        var comment = await comments.CreateAsync(postId, dto);
        return CreatedAtAction(nameof(Show), new { postId, id = comment.Id }, comment);
    }

    [HttpPut("posts/{postId:long}/comments/{id:long}")]
    public Task<PostCommentDto> Update(long postId, long id, PostCommentCreateDto dto) =>
        comments.UpdateAsync(postId, id, dto);

    [HttpDelete("posts/{postId:long}/comments/{id:long}")]
    public async Task<IActionResult> Destroy(long postId, long id)
    {
        await comments.DeleteAsync(postId, id);
        return NoContent();
    }
}
