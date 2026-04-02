# Render 部署檢查清單 - 401 登入失敗診斷

## 🔴 常見的 401 錯誤原因

在 Render 上，登入失敗通常由以下原因造成：

### 1. **CORS 配置錯誤** ⚠️ 最常見
- 後端 `AllowedOrigins` 環境變量未設置
- 導致 Vercel 前端無法訪問 Render 後端
- **症狀**: 瀏覽器顯示「Failed to load resource: 401」

### 2. **JWT SecretKey 不一致**
- 後端沒有正確的 JWT 密鑰
- 即使登入成功，生成的 token 無法被驗證
- **症狀**: 登入後仍然認證失敗

### 3. **數據庫連接失敗**
- Supabase 連線字串配置不正確
- 導致登入邏輯拋出異常，返回 500 或 401
- **症狀**: 連 admin 賬戶都無法登入

---

## ✅ 修復步驟

### 第 1 步：在 Render Dashboard 中設定環境變量

1. 登入 [Render Dashboard](https://dashboard.render.com/)
2. 找到您的 **pets-api** Web Service
3. 進入 **Environment** 標籤
4. 添加或更新以下環境變量：

#### **必需的環境變量**

| 環境變量名稱 | 值 | 說明 |
|-------------|------|------|
| `ConnectionStrings__PetsDatabase` | `Host=aws-1-ap-northeast-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.oixfmiaivcefsgqncyhy;Password=an5JiCtdvrZrQf8m;SSL Mode=Require;` | Supabase PostgreSQL 連線字串 |
| `AllowedOrigins__0` | `https://pets-beta-eight.vercel.app` | Vercel 前端地址 |
| `JwtSettings__SecretKey` | `your-secret-key-at-least-32-characters-long` | JWT 簽名密鑰（務必更改為安全的值）|
| `JwtSettings__Issuer` | `PetsAPI` | JWT Issuer（與 appsettings.json 一致）|
| `JwtSettings__Audience` | `PetsClient` | JWT Audience（與 appsettings.json 一致）|
| `JwtSettings__ExpiryMinutes` | `1440` | Token 過期時間（分鐘）|
| `ASPNETCORE_ENVIRONMENT` | `Production` | 環境類型 |
| `ASPNETCORE_URLS` | `http://+:8080` | 服務監聽 URL |

### 第 2 步：驗證 Vercel 環境變量

在 Vercel Dashboard 中確認：

1. 進入您的 **pets** 項目
2. 設置 → 環境變量
3. 確認 `REACT_APP_API_URL` = `https://pets-v48q.onrender.com/api`

### 第 3 步：重新部署

#### Render 後端
```
1. Render Dashboard → pets-api
2. 點擊「Trigger deploy」
3. 等待部署完成（查看日誌確認無錯誤）
```

#### Vercel 前端
```
1. Vercel Dashboard → pets
2. 自動觸發新部署（或手動點擊「Redeploy」）
3. 等待部署完成
```

### 第 4 步：測試登入

1. 打開 Vercel 前端：`https://pets-beta-eight.vercel.app`
2. 使用默認賬戶登入：
   - **用戶名**: `admin`
   - **密碼**: `admin123`
3. 打開瀏覽器開發者工具 (F12) → Network 標籤
4. 觀察登入請求：
   - ✅ 應該看到 200/201 響應（包含 token）
   - ❌ 如果仍然 401，查看響應內容診斷

---

## 🔍 故障排除

### 接收 401 但找不到原因？

#### 方法 1：檢查 Render 日誌

1. Render Dashboard → pets-api → Logs
2. 查看是否有異常：
   - 數據庫連接錯誤
   - CORS 錯誤
   - JWT 配置錯誤

#### 方法 2：測試後端直接

在 Render Dashboard → Logs 中執行：
```bash
# 檢查連接字符串是否正確
curl https://pets-v48q.onrender.com/health
# 應該返回 200 {"status":"healthy"}
```

試試直接 POST 登入（用 curl 或 Postman）：
```bash
curl -X POST https://pets-v48q.onrender.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

預期響應：
```json
{
  "message": "登入成功",
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

#### 方法 3：檢查驗證代碼

打開瀏覽器開發者工具查看：
```javascript
// F12 → Console
localStorage.getItem('pets_auth_token')  // 應該有 token
localStorage.getItem('pets_user')        // 應該有用戶信息
```

---

## 📋 快速診斷

如果 Render 上仍然無法登入，運行以下檢查：

- [ ] Render Dashboard 環境變量是否全部設置？
- [ ] `AllowedOrigins__0` 是否為 `https://pets-beta-eight.vercel.app`（包括 https）？
- [ ] `JwtSettings__SecretKey` 是否至少 32 個字符？
- [ ] Render 部署是否成功（檢查 Build & Deploy 日誌）？
- [ ] Vercel 前端是否重新部署？
- [ ] 瀏覽器是否清除了 localStorage 和 cookie？
- [ ] 是否使用了正確的登入憑證（admin / admin123）？

---

## 🛠️ 手動添加測試賬戶（如需）

如果 admin 賬戶無法使用，可以通過**前端註冊功能**添加新用戶：

1. 打開 `https://pets-beta-eight.vercel.app`
2. 點擊「註冊」標籤
3. 填入用戶信息並提交
4. 系統會自動創建用戶並登入

---

## 📞 其他資源

- [render.yaml 文檔](../render.yaml) - 環境變量配置定義
- [appsettings.json](../PetsAPI/appsettings.json) - 生產環境設定
- [.env.production](../frontend/.env.production) - Vercel 前端環境
