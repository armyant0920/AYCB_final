using CorporateSite.Application.Abstractions.Security;
using CorporateSite.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CorporateSite.Infrastructure.Security;

public class PasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<AppUser> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new AppUser(), password);

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(new AppUser(), hashedPassword, providedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}
