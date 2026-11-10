using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Exceptions;
using CSharpAspnetBlog.Mappers;
using CSharpAspnetBlog.Models;
using CSharpAspnetBlog.Repositories;

using Microsoft.AspNetCore.Identity;

namespace CSharpAspnetBlog.Services;

public class UserService(UserRepository users, IPasswordHasher<User> hasher)
{
    public async Task<List<UserDto>> GetAllAsync() =>
        (await users.GetAllAsync()).Select(UserMapper.ToDto).ToList();

    public async Task<UserDto> GetAsync(long id) => UserMapper.ToDto(await FindAsync(id));

    public async Task<UserDto> CreateAsync(UserCreateDto dto)
    {
        if (await users.FindByEmailAsync(dto.Email) is not null)
        {
            throw new ConflictException($"Email already taken: {dto.Email}");
        }

        var user = UserMapper.ToEntity(dto);
        user.PasswordDigest = hasher.HashPassword(user, dto.Password);
        await users.AddAsync(user);
        return UserMapper.ToDto(user);
    }

    public async Task<UserDto> UpdateAsync(long id, UserUpdateDto dto)
    {
        var user = await FindAsync(id);
        UserMapper.Update(dto, user);
        await users.UpdateAsync(user);
        return UserMapper.ToDto(user);
    }

    /// <summary>Возвращает пользователя, если email и пароль верны, иначе <c>null</c>.</summary>
    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var result = hasher.VerifyHashedPassword(user, user.PasswordDigest, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }

    private async Task<User> FindAsync(long id) =>
        await users.FindByIdAsync(id) ?? throw new ResourceNotFoundException($"User not found: {id}");
}
