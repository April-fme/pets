

# 專案需求：寵物健康管理與異狀監測
## 系統功能：項圈數據整合、飲食排泄記錄、獸醫預約、異常警報。

我想開發一個『寵物健康管理與異狀監測系統』，主要是記錄生活數據並提供異常報警功能，需要以chart.js動態顯示寵物活動量趨勢圖表。

## 1. 系統架構 (Architecture)
- **前端**: React.js + Tailwind CSS + Chart.js (圖表視覺化)
- **後端**: ASP.NET Core Web API (.NET 8) + Entity Framework Core
- **資料庫**: Microsoft SQL Server (MS SQL)
- **整合**: 模擬項圈 API 數據接入（使用 Hosted Service）

請幫我規劃整體的專案目錄結構，並解釋各個資料夾的功能（例如 Controllers, Models, Routes, Services）。

## 2. 資料庫設計 (DB Schema - MS SQL)

請建立以下資料表並設定關聯：
- `Pets`: 儲存寵物基本資料 (ID, Name, Species, Birthday, WeightGoal)。
- `CollarData`: **項圈數據整合** (ID, PetID, Temperature, HeartRate, ActivityLevel, SleepQuality, Timestamp)。
- `DailyLogs`: **飲食排泄記錄** (ID, PetID, FoodAmount, WaterIntake, StoolStatus, UrineStatus, CreatedAt)。
- `Appointments`: **獸醫預約** (ID, PetID, ClinicName, VetName, AppointmentDate, Reason, Status)。
- `Alerts`: **異常警報** (ID, PetID, Type, Severity, Message, IsResolved, CreatedAt)。


## 3. 核心功能邏輯 (Core Logic)
請實作以下監測與自動化邏輯：
- **[異常警報] 數據監控**: 
    - 若 `Temperature` > 39.5°C，觸發「體溫上升」警報。
    - 若 `HeartRate` > 120bpm (靜止狀態) 或 `SleepQuality` 連續三日下降 30%，觸發「生理異常」警報。
    - 若 `StoolStatus` 標記為「血便」或「腹瀉」，觸發「消化異狀」警報。
    - 若 `FoodIntake` 低於該寵物過去平均值的 40%，觸發「食慾不振」警報。
- **[預約管理]**: 提供 CRUD 功能，並在預約日期前 24 小時標記為「即將到來」。
- **[項圈同步]**: 建立一個模擬 API，每 5 分鐘接收一次項圈數據並存入 `CollarData`。


## 4. 前端介面需求 (Frontend UI)

- **Dashboard**:
    - **項圈監控看板**: 顯示即時心率與活動量量表 (Gauge Chart)。
    - **健康紀錄表單**: 快速勾選飲食與排泄狀況 (Radio Buttons)。
    - **預約行事曆**: 顯示近期獸醫看診安排。
    - **警報中心**: 頂部通知列顯示未處理的異常警報。

## 5. 開發規範
- 所有的資料庫查詢需使用 `Stored Procedures` 或參數化查詢以防 SQL Injection。
- 異常警報觸發時，後端需 Log 記錄觸發原因。
- 前端 API 調用需處理 Token 驗證 (JWT)。

