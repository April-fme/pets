# 寵物健康管理系統 - 登入系統實現報告

## ✅ 已完成功能

### 1. 後端實現 (ASP.NET Core)

#### 新建模型與服務
- **User 模型** (`Models/User.cs`)
  - 包含 ID、Username、Email、PasswordHash、FullName、CreatedAt、UpdatedAt
  - 與 Pet 建立一對多關係

- **Pet 模型修改**
  - 添加 `UserID` 外鍵
  - 建立與 User 的導航屬性

- **認證服務** (`Services/AuthService.cs`)
  - 實現 `IAuthService` 接口
  - 包含註冊、登入、JWT Token 生成
  - 使用 BCrypt.Net 進行密碼雜湊

#### API 端點 (AuthController)
```
POST   /api/auth/register    - 用戶註冊
POST   /api/auth/login       - 用戶登入
GET    /api/auth/me          - 獲取當前用戶信息 (需要認證)
```

#### JWT 認證配置
- 配置於 `Program.cs`
- 使用 HS256 簽名算法
- Token 過期時間: 1440 分鐘 (24 小時)

#### 資料庫遷移
- 建立 `Users` 表
- 添加 `Pets.UserID` 外鍵
- 在遷移中插入默認 admin 用戶
  - 用戶名: `admin`
  - 密碼: `admin123` (已用 BCrypt 加密)
  - 郵箱: `admin@pets.local`

#### PetsController 更新
- 添加 `[Authorize]` 屬性
- 實現基於 UserId 的資料隔離
- 新增寵物時自動設置 UserID

### 2. 前端實現 (React)

#### 新增組件
- **LoginPage** (`components/LoginPage.js`)
  - 支持登入和註冊雙模式
  - 表單驗證
  - 錯誤提示顯示
  - 響應式設計，使用梯度背景

#### 認證服務 (`services/authService.js`)
- 登入/註冊 API 調用
- Token 存儲到 localStorage
- JWT Token 解析
- 自動設置 axios Authorization header
- 驗證 token 有效性

#### App.js 更新
- 集成登入頁面流程
- 認證狀態管理
- 用戶註出功能
- 顯示當前登入用戶名
- 未認證時導向登入頁面

### 3. 配置更新

#### appsettings.json / appsettings.Development.json
```json
"JwtSettings": {
  "SecretKey": "...",
  "Issuer": "PetsAPI",
  "Audience": "PetsClient",
  "ExpiryMinutes": 1440
}
```

#### .NET 依賴項
- `System.IdentityModel.Tokens.Jwt` v8.2.1
- `Microsoft.AspNetCore.Authentication.JwtBearer` v10.0.3
- `BCrypt.Net-Next` v4.0.3

## 🔐 安全特性

1. 密碼加密: BCrypt 雜湊 (工作因子 11)
2. JWT Token 認證: HS256 簽名
3. CORS: 限定 http://localhost:3000
4. 基於 UserId 的資料隔離
5. Token 過期驗證

## 📊 資料模型關係

```
Users (1)
  ↓
  └─→ Pets (Many)
       ├─→ HealthData
       ├─→ DailyLogs
       ├─→ Appointments
       └─→ Alerts
```

## 🚀 使用方法

### 默認帳號
- **用戶名**: admin
- **密碼**: admin123
- **郵箱**: admin@pets.local

### 啟動應用
```bash
# 後端 (在 PetsAPI 目錄)
dotnet run

# 前端 (在 frontend 目錄)
npm start
```

應用將在以下地址運行:
- 前端: http://localhost:3000
- 後端 API: http://localhost:3001

### 登入流程
1. 用戶在登入頁面輸入帳號密碼
2. 前端發送 POST /api/auth/login 請求
3. 後端驗證憑證並返回 JWT token
4. 前端存儲 token 到 localStorage
5. 後續請求自動在 Authorization header 中包含 token
6. 用戶可查看並管理所有屬於其的寵物

### 註冊新用戶
1. 點擊"註冊"標簽
2. 填寫用戶名、郵箱、密碼信息
3. 系統自動登入新用戶
4. 跳轉到寵物管理頁面

## 🔍 技術棧

**後端:**
- ASP.NET Core (.NET 10)
- Entity Framework Core
- SQL Server 2019+
- JWT 認證
- BCrypt 密碼加密

**前端:**
- React 19.2.4
- Axios HTTP 客戶端
- 本地佈局 CSS

## ✨ 功能亮點

1. **安全認証**: 使用業界標準的 JWT 和 BCrypt
2. **多用戶支持**: 每個用戶只能看到自己的寵物
3. **用戶友好**: 簡潔的登入/註冊頁面,錯誤提示清晰
4. **自動化**: 登入時自動初始化認証 Token

## 📝 後續改進建議

1. 添加郵箱驗證
2. 實現密碼重置功能
3. 添加二次認証 (2FA)
4. 實現用戶頭像上傳
5. 添加用戶角色與權限管理
6. 實現社交登入 (Google/Facebook)
7. Token 刷新機制
8. 登入日誌記錄
