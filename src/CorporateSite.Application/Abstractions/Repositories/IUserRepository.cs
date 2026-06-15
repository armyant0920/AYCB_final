using CorporateSite.Domain.Entities;

namespace CorporateSite.Application.Abstractions.Repositories;

public interface IUserRepository
{
    AppUser? GetByUserName(string userName);
}
