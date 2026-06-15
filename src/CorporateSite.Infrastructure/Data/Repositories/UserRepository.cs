using System.Data;
using CorporateSite.Application.Abstractions.Repositories;
using CorporateSite.Domain.Entities;
using DataProviderInfrastructure;
using Microsoft.Data.SqlClient;

namespace CorporateSite.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataProviderFactory _factory;
    public UserRepository(DataProviderFactory factory) => _factory = factory;

    public AppUser? GetByUserName(string userName)
    {
        const string sql = @"
            SELECT UserId, UserName, DisplayName, PasswordHash, Roles, IsActive
            FROM dbo.AppUser WHERE UserName = @UserName AND IsActive = 1";

        using var db = _factory.Create();
        return db.Query<AppUser>(sql,
            [new SqlParameter("@UserName", SqlDbType.NVarChar, 100) { Value = userName }])
            .FirstOrDefault();
    }
}
