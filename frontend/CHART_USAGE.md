# Chart.js 活動量趨勢圖元件使用說明

## 📊 已建立的元件

### 1. ActivityChart.js（純假資料版本）
最簡單的版本，使用固定的模擬數據。

**使用方式：**
```jsx
import ActivityChart from './components/ActivityChart';

<ActivityChart petName="小黃" />
```

### 2. ActivityChartAPI.js（支援真實 API 版本）⭐ 推薦
進階版本，支援假資料與真實 API 切換。

**使用方式：**
```jsx
import ActivityChartAPI from './components/ActivityChartAPI';

<ActivityChartAPI 
  petId={1} 
  petName="小黃" 
/>
```

**特色：**
- ✅ 內建假資料與真實 API 切換開關
- ✅ 自動從 `/api/collardata/pet/{petId}` 取得數據
- ✅ 包含載入中與錯誤處理
- ✅ 顯示雙 Y 軸（心率 + 活動量）
- ✅ API 失敗時自動回到假資料

## 🎨 圖表功能

- **X 軸**：時間（例如：08:00, 10:00）
- **Y 軸左**：心率 (bpm)
- **Y 軸右**：活動量 (%)
- **互動**：滑鼠懸停顯示詳細數據
- **響應式**：自動適應容器大小

## 🔄 從假資料切換到真實 API

目前 App.js 已經整合 `ActivityChartAPI`：

1. **預設顯示假資料**
   - 勾選「使用模擬數據」開關

2. **切換到真實 API**
   - 取消勾選「使用模擬數據」
   - 自動從 `http://localhost:3001/api/collardata/pet/{petId}` 取得數據

3. **API 數據格式要求**
   ```json
   [
     {
       "timestamp": "2024-03-18T08:00:00",
       "heartRate": 85,
       "activityLevel": 30
     },
     ...
   ]
   ```

## 📱 當前狀態

✅ Chart.js 與 react-chartjs-2 已安裝
✅ 圖表元件已建立
✅ 已整合到 App.js
✅ 支援多寵物切換
✅ 自動載入假資料

前端地址：http://localhost:3000

## 🚀 下一步

資料庫連接後：
1. 確保 .NET API 的 `/api/collardata/pet/{id}` 端點正常
2. 取消勾選「使用模擬數據」
3. 圖表將顯示真實的項圈數據
