-- 測試文章資料
INSERT INTO dbo.Article (Slug, Title, Summary, BodyHtml, Section, SortOrder, Status, PublishedAt, CreatedBy)
VALUES
(
    N'about-us',
    N'公司簡介',
    N'專注於高端電路板技術，服務全球半導體產業。',
    N'<p>本公司成立於1990年代，專注於高端電路板技術，持續創新，服務全球半導體測試產業。</p>',
    N'About', 1, 1, SYSUTCDATETIME(), N'admin'
),
(
    N'esg-policy',
    N'ESG 永續政策',
    N'落實環境保護、社會責任與公司治理，建立永續企業。',
    N'<p>本公司承諾在2030年前達成碳中和目標。</p>',
    N'ESG', 1, 1, SYSUTCDATETIME(), N'admin'
);
GO
