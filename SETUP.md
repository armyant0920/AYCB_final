# 本機啟動指南

## 快速啟動（Mock 模式，無需 DB）

```bash
dotnet run --project src/CorporateSite.Web
```

`appsettings.Development.json` 中 `"UseMockData": true` 已預設開啟。
瀏覽器打開 `https://localhost:<port>/` 即可看到公開頁面，
打開 `https://localhost:<port>/Admin/News` 可操作後台。

---

## 完整模式（接真實 SQL Server）

### 1. 準備 DataProviderInfrastructure 套件

將 `DataProviderInfrastructure.2.0.27.nupkg` 放到 `local-packages/` 資料夾。
`nuget.config` 已設定此資料夾為本地來源，`dotnet restore` 會自動找到它。

### 2. 建立資料庫

```sql
-- 在 SQL Server 建立資料庫
CREATE DATABASE CorporateSite;
```

依序執行 `db/schema/` 下的 SQL：
```bash
# schema
sqlcmd -S localhost -d CorporateSite -i db/schema/001_News.sql
sqlcmd -S localhost -d CorporateSite -i db/schema/002_AppUser.sql

# migration（如有）
sqlcmd -S localhost -d CorporateSite -i db/migrations/001_AddLanguageToNews.sql

# seed 測試資料
sqlcmd -S localhost -d CorporateSite -i db/seed/002_SeedNews.sql
```

### 3. 設定連線字串

**Web 專案（Mock 改為 Api 模式）：**
```bash
# appsettings.Development.json 中改為 "UseMockData": false
# 設定 Api 位置
dotnet user-secrets set "ApiBaseUrl" "https://localhost:7001" --project src/CorporateSite.Web
```

**Api 專案：**
```bash
dotnet user-secrets set "ConnectionStrings:Default" \
  "Server=localhost;Database=CorporateSite;Trusted_Connection=True;TrustServerCertificate=True;" \
  --project src/CorporateSite.Api
```

### 4. 同時啟動兩個專案

```bash
# 終端機 1
dotnet run --project src/CorporateSite.Api --urls "https://localhost:7001"

# 終端機 2
dotnet run --project src/CorporateSite.Web --urls "https://localhost:7000"
```

---

## 專案結構

```
src/
  CorporateSite.Web/     ASP.NET Core MVC 網站（公開頁 + Admin 後台）
  CorporateSite.Api/     REST API Server（接 SQL Server，供 Web 呼叫）
db/
  schema/                建表 SQL（依序執行）
  migrations/            結構異動 SQL
  seed/                  測試資料
local-packages/          放置本地 .nupkg 檔案（不進 git）
```

## 環境需求

- .NET 8 SDK
- SQL Server 2019+（完整模式才需要）
- DataProviderInfrastructure 2.0.27.nupkg（完整模式才需要）
