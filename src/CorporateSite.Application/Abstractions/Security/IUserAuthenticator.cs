namespace CorporateSite.Application.Abstractions.Security;

public class AuthResult
{
    public bool Success { get; init; }
    public string UserName { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string[] Roles { get; init; } = [];
    public string? ErrorMessage { get; init; }
}

public interface IUserAuthenticator
{
    AuthResult ValidateCredentials(string userName, string password);
}
