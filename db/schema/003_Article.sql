-- 靜態文章頁面（公司簡介、ESG政策、投資人資訊等）
CREATE TABLE dbo.Article (
    ArticleId    INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Slug         NVARCHAR(100)     NOT NULL UNIQUE,        -- URL識別鍵，如 about-us
    Title        NVARCHAR(200)     NOT NULL,
    Summary      NVARCHAR(500)     NOT NULL DEFAULT (N''),
    BodyHtml     NVARCHAR(MAX)     NOT NULL DEFAULT (N''),
    Section      NVARCHAR(50)      NOT NULL DEFAULT (N'About'), -- About/ESG/Investor/Career
    Language     NVARCHAR(10)      NOT NULL DEFAULT (N'zh-TW'),
    SortOrder    INT               NOT NULL DEFAULT (0),
    Status       INT               NOT NULL DEFAULT (0),        -- 0=Draft, 1=Published
    PublishedAt  DATETIME2         NULL,
    CreatedAt    DATETIME2         NOT NULL DEFAULT (SYSUTCDATETIME()),
    UpdatedAt    DATETIME2         NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy    NVARCHAR(100)     NOT NULL DEFAULT (N'system'),
    IsDeleted    BIT               NOT NULL DEFAULT (0)
);
GO

CREATE INDEX IX_Article_Public ON dbo.Article (IsDeleted, Status, Section, Language, SortOrder);
GO
