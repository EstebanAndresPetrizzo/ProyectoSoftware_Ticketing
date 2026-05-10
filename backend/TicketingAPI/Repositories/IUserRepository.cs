using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public interface IUserRepository
{
    Task<User> UpsertGoogleUserAsync(string googleSub, string email, string name, CancellationToken cancellationToken = default);
}
