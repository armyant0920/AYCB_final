-- 後台員工帳號表（初期本地帳號，未來可介接 OIDC 或員工系統 API）
CREATE TABLE dbo.AppUser (
    UserId       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserName     NVARCHAR(100)     NOT NULL,
    DisplayName  NVARCHAR(100)     NOT NULL DEFAULT (N''),
    PasswordHash NVARCHAR(500)     NOT NULL,             -- ASP.NET Core PasswordHasher 格式
    Roles        NVARCHAR(200)     NOT NULL DEFAULT (N'Editor'),  -- 逗號分隔：Admin,Editor
    IsActive     BIT               NOT NULL DEFAULT (1),
    CreatedAt    DATETIME2         NOT NULL DEFAULT (SYSUTCDATETIME())
);
GO

CREATE UNIQUE INDEX UQ_AppUser_UserName ON dbo.AppUser (UserName);
GO
