-- 測試新聞資料
INSERT INTO dbo.News (Title, Summary, BodyHtml, Category, Status, PublishedAt, CreatedBy)
VALUES
(
    N'公司成立週年慶',
    N'感謝各界長期支持，公司持續茁壯成長。',
    N'<p>感謝各界長期支持，我們將持續努力，為客戶與社會創造更大價值。</p>',
    N'CompanyNews', 1, SYSUTCDATETIME(), N'admin'
),
(
    N'2024 ESG 年度報告發佈',
    N'本年度永續發展報告正式公告，歡迎下載閱讀。',
    N'<p>本公司致力永續發展，本年度 ESG 報告已正式對外公告，詳細內容請見附件。</p>',
    N'ESG', 1, SYSUTCDATETIME(), N'admin'
),
(
    N'草稿測試消息（不應出現在公開頁）',
    N'這是草稿，不應在公開頁面顯示。',
    N'<p>草稿內容，Status=0。</p>',
    N'CompanyNews', 0, NULL, N'admin'
);
GO
