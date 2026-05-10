using Microsoft.EntityFrameworkCore;
using TicketingAPI.Data;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> UpsertGoogleUserAsync(string googleSub, string email, string name, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleSub == googleSub, cancellationToken);

        if (existing != null)
        {
            existing.Name = name;
            existing.Email = email;
            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        var user = new User
        {
            GoogleSub = googleSub,
            Email = email,
            Name = name
        };
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }
}
