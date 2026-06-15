using CorporateSite.Application.Abstractions.Repositories;
using CorporateSite.Application.Abstractions.Security;

namespace CorporateSite.Infrastructure.Security;

public class LocalUserAuthenticator : IUserAuthenticator
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHashService _hasher;

    public LocalUserAuthenticator(IUserRepository userRepo, IPasswordHashService hasher)
    {
        _userRepo = userRepo;
        _hasher = hasher;
    }

    public AuthResult ValidateCredentials(string userName, string password)
    {
        var user = _userRepo.GetByUserName(userName);
        if (user == null || !_hasher.Verify(user.PasswordHash, password))
            return new AuthResult { Success = false, ErrorMessage = "帳號或密碼錯誤" };

        return new AuthResult
        {
            Success = true,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        };
    }
}
