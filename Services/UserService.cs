using Microsoft.EntityFrameworkCore;
using Taskly.Context;
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
}