using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Models;

namespace CSharpAspnetBlog.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user) =>
        new(user.Id, user.Email, user.FirstName, user.LastName);

    public static User ToEntity(UserCreateDto dto) =>
        new() { Email = dto.Email, FirstName = dto.FirstName, LastName = dto.LastName };

    public static void Update(UserUpdateDto dto, User user)
    {
        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
    }
}
