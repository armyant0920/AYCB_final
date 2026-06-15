namespace CorporateSite.Application.Abstractions.Security;

public interface IPasswordHashService
{
    string Hash(string password);
    bool Verify(string hashedPassword, string providedPassword);
}
