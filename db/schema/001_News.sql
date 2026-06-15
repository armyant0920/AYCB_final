-- 新聞 / 公告 / ESG 內容表
CREATE TABLE dbo.News (
    NewsId        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title         NVARCHAR(200)     NOT NULL,
    Summary       NVARCHAR(500)     NOT NULL DEFAULT (N''),
    BodyHtml      NVARCHAR(MAX)     NOT NULL DEFAULT (N''),
    Category      NVARCHAR(50)      NOT NULL DEFAULT (N'CompanyNews'),
    Language      NVARCHAR(10)      NOT NULL DEFAULT (N'zh-TW'),   -- zh-TW / en-US
    CoverImageId  INT               NOT NULL DEFAULT (0),
    Status        INT               NOT NULL DEFAULT (0),           -- 0=Draft, 1=Published, 2=Archived
    PublishedAt   DATETIME2         NULL,
    CreatedAt     DATETIME2         NOT NULL DEFAULT (SYSUTCDATETIME()),
    UpdatedAt     DATETIME2         NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy     NVARCHAR(100)     NOT NULL DEFAULT (N'system'),
    IsDeleted     BIT               NOT NULL DEFAULT (0)
);
GO

CREATE INDEX IX_News_Public ON dbo.News (IsDeleted, Status, Category, Language, PublishedAt DESC);
GO
