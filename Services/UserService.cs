using Microsoft.EntityFrameworkCore;
using Taskly.Context;
using Taskly.DTOs;
using Taskly.Exceptions;
using Taskly.Models;

namespace Taskly.Services;

public class UserService
{
    private readonly TasklyContext _context;

    public UserService(TasklyContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterUserAsync(User user)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            throw new UsernameAlreadyExistsException("Nome de usuário em uso.");
        
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            throw new EmailAlreadyExistsException("Email em uso.");
        
        var existingPasswords = await _context.Users.Select(u => u.Password).ToListAsync();
        if (existingPasswords.Any(hash => BCrypt.Net.BCrypt.Verify(user.Password, hash)))
            throw new PasswordAlreadyExistsException("A senha já está em uso. Escolha outra.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
            throw new UsernameNull("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new IncorrectPassword("Senha incorreta.");

        return user;
    }

    public async Task<User> UpdateUserAsync(int userId, User updatedUser)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new UsernameNull("Usuário não encontrado.");

        if (updatedUser.Username != user.Username &&
            await _context.Users.AnyAsync(u => u.Username == updatedUser.Username))
            throw new UsernameAlreadyExistsException("Nome de usuário em uso.");

        user.FullName = updatedUser.FullName;
        user.TitleJob = updatedUser.TitleJob;

        if (!string.IsNullOrEmpty(updatedUser.Username))
            user.Username = updatedUser.Username;

        if (!string.IsNullOrEmpty(updatedUser.Password))
            user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new UserNotFoundExcepiton("Usuário não encontrado");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserResponseDto>> SearchUserAsync(string? fullname, string? username, string? email)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(fullname))
            query = query.Where(u => u.FullName.Contains(fullname));

        if (!string.IsNullOrWhiteSpace(username))
            query = query.Where(u => u.Username.Contains(username));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email.Contains(email));

        return await query.Select(u => new UserResponseDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Username = u.Username,
            Email = u.Email,
            TitleJob = u.TitleJob
        })
            .ToListAsync();
    }
}