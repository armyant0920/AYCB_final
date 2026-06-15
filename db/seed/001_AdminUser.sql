-- 建立測試 Admin 帳號
-- 密碼預設為 Admin@2024
--
-- PasswordHash 必須用以下 C# 程式碼產生後替換下方的佔位字串：
--
--   using Microsoft.AspNetCore.Identity;
--   var hash = new PasswordHasher<object>().HashPassword(null!, "Admin@2024");
--   Console.WriteLine(hash);
--
-- 然後將輸出的 hash 值貼入下方 PasswordHash 欄位。

INSERT INTO dbo.AppUser (UserName, DisplayName, PasswordHash, Roles, IsActive)
VALUES (
    N'admin',
    N'系統管理員',
    N'REPLACE_WITH_HASH_FROM_PasswordHasher',
    N'Admin,Editor',
    1
);
GO
