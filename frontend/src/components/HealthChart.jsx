import React, { useState, useEffect } from 'react';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';
import { Line } from 'react-chartjs-2';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
);

// ── 初始模擬資料 ──────────────────────────────────────────────
const generateMockEntry = (time) => ({
  timestamp: time.toLocaleTimeString('zh-TW', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  }),
  temperature: parseFloat((Math.random() * (40.5 - 37.5) + 37.5).toFixed(1)),
  heartRate: Math.floor(Math.random() * (140 - 80) + 80),
  activityLevel: Math.floor(Math.random() * 100),
  sleepQuality: Math.floor(Math.random() * 100),
});

const generateMockData = (count = 10) => {
  const now = Date.now();
  return Array.from({ length: count }, (_, i) =>
    generateMockEntry(new Date(now - (count - 1 - i) * 5000))
  );
};

// ── 預留 API 接口：未來串接 .NET API 時替換此函數 ──────────────
export const fetchData = async (petId) => {
  const response = await fetch(
    `http://localhost:3001/api/healthdata/pet/${petId}`
  );
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
  const json = await response.json();
  return json.map((item) => ({
    timestamp: new Date(item.timestamp).toLocaleTimeString('zh-TW', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    }),
    temperature: item.temperature,
    heartRate: item.heartRate,
    activityLevel: item.activityLevel,
    sleepQuality: item.sleepQuality,
  }));
};

// ── 最多保留幾個數據點 ─────────────────────────────────────────
const MAX_POINTS = 20;

// ── 主組件 ────────────────────────────────────────────────────
const HealthChart = ({ petId, petName = '寵物' }) => {
  const [dataPoints, setDataPoints] = useState(() => generateMockData(10));

  // 每 15 秒推送一筆新隨機數據（滾動式動態更新）
  useEffect(() => {
    const interval = setInterval(() => {
      const newPoint = generateMockEntry(new Date());
      setDataPoints((prev) => {
        const updated = [...prev, newPoint];
        return updated.length > MAX_POINTS
          ? updated.slice(updated.length - MAX_POINTS)
          : updated;
      });
    }, 15000);
    return () => clearInterval(interval);
  }, []);

  // ── Chart.js 資料 ──────────────────────────────────────────
  const chartData = {
    labels: dataPoints.map((d) => d.timestamp),
    datasets: [
      {
        label: '心跳 (bpm)',
        data: dataPoints.map((d) => d.heartRate),
        borderColor: 'rgb(220, 53, 69)',
        backgroundColor: 'rgba(220, 53, 69, 0.08)',
        yAxisID: 'yHeartRate',
        tension: 0.3,
        pointRadius: 3,
        pointHoverRadius: 6,
      },
      {
        label: '體溫 (°C)',
        data: dataPoints.map((d) => d.temperature),
        borderColor: 'rgb(255, 140, 0)',
        backgroundColor: 'rgba(255, 140, 0, 0.08)',
        yAxisID: 'yTemperature',
        tension: 0.3,
        pointRadius: 3,
        pointHoverRadius: 6,
      },
      {
        label: '活動量 (%)',
        data: dataPoints.map((d) => d.activityLevel),
        borderColor: 'rgb(13, 110, 253)',
        backgroundColor: 'rgba(13, 110, 253, 0.08)',
        yAxisID: 'yPercent',
        tension: 0.3,
        pointRadius: 3,
        pointHoverRadius: 6,
      },
      {
        label: '睡眠品質 (%)',
        data: dataPoints.map((d) => d.sleepQuality),
        borderColor: 'rgb(111, 66, 193)',
        backgroundColor: 'rgba(111, 66, 193, 0.08)',
        yAxisID: 'yPercent',
        tension: 0.3,
        pointRadius: 3,
        pointHoverRadius: 6,
      },
    ],
  };

  // ── Chart.js 選項 ──────────────────────────────────────────
  const options = {
    responsive: true,
    animation: { duration: 400 },
    interaction: {
      mode: 'index',
      intersect: false,
    },
    plugins: {
      title: {
        display: true,
        text: `${petName} — 健康指標即時監控`,
        font: { size: 18, weight: 'bold' },
        padding: { bottom: 16 },
      },
      legend: {
        position: 'top',
        // 點擊圖例切換顯示 / 隱藏特定指標
        onClick: (e, legendItem, legend) => {
          const index = legendItem.datasetIndex;
          const chart = legend.chart;
          if (chart.isDatasetVisible(index)) {
            chart.hide(index);
            legendItem.hidden = true;
          } else {
            chart.show(index);
            legendItem.hidden = false;
          }
        },
      },
      tooltip: {
        callbacks: {
          label: (ctx) => {
            const v = ctx.parsed.y;
            if (ctx.dataset.label.includes('體溫')) return ` 體溫: ${v} °C`;
            if (ctx.dataset.label.includes('心跳')) return ` 心跳: ${v} bpm`;
            if (ctx.dataset.label.includes('活動量')) return ` 活動量: ${v} %`;
            return ` 睡眠品質: ${v} %`;
          },
        },
      },
    },
    scales: {
      x: {
        ticks: { maxTicksLimit: 10, maxRotation: 30 },
      },
      // 左側 Y 軸：心跳 (60-160 bpm)
      yHeartRate: {
        type: 'linear',
        display: true,
        position: 'left',
        min: 40,
        max: 180,
        title: {
          display: true,
          text: '心跳 (bpm)',
          color: 'rgb(220, 53, 69)',
          font: { weight: 'bold' },
        },
        ticks: { color: 'rgb(220, 53, 69)' },
        grid: { color: 'rgba(220, 53, 69, 0.08)' },
      },
      // 右側 Y 軸：活動量 & 睡眠品質 (0-100%)
      yPercent: {
        type: 'linear',
        display: true,
        position: 'right',
        min: 0,
        max: 100,
        title: {
          display: true,
          text: '活動量 / 睡眠品質 (%)',
          color: 'rgb(13, 110, 253)',
          font: { weight: 'bold' },
        },
        ticks: {
          color: 'rgb(13, 110, 253)',
          callback: (v) => `${v}%`,
        },
        grid: { drawOnChartArea: false },
      },
      // 右側 Y 軸（隱藏刻度）：體溫 (35-42°C)，避免與百分比刻度重疊
      yTemperature: {
        type: 'linear',
        display: true,
        position: 'right',
        min: 35,
        max: 42,
        title: {
          display: true,
          text: '體溫 (°C)',
          color: 'rgb(255, 140, 0)',
          font: { weight: 'bold' },
        },
        ticks: {
          color: 'rgb(255, 140, 0)',
          callback: (v) => `${v}°C`,
          stepSize: 1,
        },
        grid: { drawOnChartArea: false },
      },
    },
  };

  return (
    <div style={{ padding: '20px', maxWidth: '960px', margin: '0 auto' }}>
      <Line options={options} data={chartData} />

      <div
        style={{
          marginTop: '12px',
          padding: '10px 14px',
          backgroundColor: '#d4edda',
          borderRadius: '6px',
          border: '1px solid #28a745',
          fontSize: '13px',
          color: '#155724',
        }}
      >
        ✅ 每 15 秒自動更新・最多顯示 {MAX_POINTS} 筆資料・
        點擊上方圖例可隱藏 / 顯示特定指標
      </div>
    </div>
  );
};

export default HealthChart;
