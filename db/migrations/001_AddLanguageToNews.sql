-- 若 News 表已存在，執行此 migration 追加 Language 欄位
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.News') AND name = 'Language'
)
BEGIN
    ALTER TABLE dbo.News
        ADD Language NVARCHAR(10) NOT NULL DEFAULT (N'zh-TW');

    -- 更新索引以包含 Language
    DROP INDEX IF EXISTS IX_News_Public ON dbo.News;
    CREATE INDEX IX_News_Public ON dbo.News (IsDeleted, Status, Category, Language, PublishedAt DESC);

    PRINT 'Language column added to dbo.News';
END
ELSE
BEGIN
    PRINT 'Language column already exists, skipped';
END
GO
