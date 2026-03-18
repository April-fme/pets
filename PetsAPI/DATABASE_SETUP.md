# 資料庫連線設定指南

## 快速開始

1. **確認您的資料庫類型**
   - SQL Server (本地安裝)
   - SQL Server Express (免費版)
   - SQLite (不需安裝伺服器)
   - Azure SQL Database

2. **修改 `appsettings.json` 中的連線字串**

## 連線字串範例

### SQL Server Express (最常見)
```json
"ConnectionStrings": {
  "PetsDatabase": "Server=localhost\\SQLEXPRESS;Database=PetsDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### SQL Server (預設實例)
```json
"ConnectionStrings": {
  "PetsDatabase": "Server=localhost;Database=PetsDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### SQL Server 使用帳號密碼
```json
"ConnectionStrings": {
  "PetsDatabase": "Server=localhost;Database=PetsDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
}
```

### SQLite (推薦用於開發測試)
```json
"ConnectionStrings": {
  "PetsDatabase": "Data Source=pets.db"
}
```

**如果使用 SQLite，需要：**
1. 安裝套件：
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   ```

2. 修改 `Program.cs` 第 8 行：
   ```csharp
   // 將
   options.UseSqlServer(builder.Configuration.GetConnectionString("PetsDatabase")));
   
   // 改為
   options.UseSqlite(builder.Configuration.GetConnectionString("PetsDatabase")));
   ```

## 資料庫遷移步驟

完成連線字串設定後，執行以下指令建立資料庫：

```bash
# 建立遷移檔案（已完成）
dotnet ef migrations add InitialCreate

# 更新資料庫 Schema
dotnet ef database update
```

## 檢查 SQL Server 是否啟動

```powershell
# 查看 SQL Server 服務狀態
Get-Service -Name "MSSQL*"

# 啟動 SQL Server Express
Start-Service "MSSQL$SQLEXPRESS"
```

## 常見問題

**Q: 找不到 SQL Server？**
- 檢查是否安裝 SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
- 確認實例名稱是否正確（通常是 `SQLEXPRESS`）

**Q: 連線被拒絕？**
- 確認 SQL Server Browser 服務已啟動
- 檢查 SQL Server Configuration Manager 中 TCP/IP 是否啟用

**Q: 不想安裝 SQL Server？**
- 改用 SQLite（見上方 SQLite 設定）
