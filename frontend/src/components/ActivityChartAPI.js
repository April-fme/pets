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
import axios from 'axios';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
);

const ActivityChartAPI = ({ petId, petName = '寵物' }) => {
  const [chartData, setChartData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [useMockData, setUseMockData] = useState(true); // 開關：是否使用假數據

  useEffect(() => {
    if (useMockData) {
      // 使用假數據
      loadMockData();
    } else {
      // 從 API 取得真實數據
      loadRealData();
    }
  }, [petId, useMockData]);

  const loadMockData = () => {
    // 模擬假數據
    const mockData = {
      labels: ['08:00', '10:00', '12:00', '14:00', '16:00', '18:00', '20:00'],
      datasets: [
        {
          label: '心率 (bpm)',
          data: [85, 92, 88, 95, 90, 87, 83],
          borderColor: 'rgb(255, 99, 132)',
          backgroundColor: 'rgba(255, 99, 132, 0.5)',
          yAxisID: 'y',
        },
        {
          label: '活動量 (%)',
          data: [30, 65, 45, 80, 70, 55, 40],
          borderColor: 'rgb(53, 162, 235)',
          backgroundColor: 'rgba(53, 162, 235, 0.5)',
          yAxisID: 'y1',
        },
      ],
    };
    setChartData(mockData);
    setLoading(false);
  };

  const loadRealData = async () => {
    try {
      setLoading(true);
      // 從 API 取得項圈數據
      const response = await axios.get(`http://localhost:3001/api/collardata/pet/${petId}`);
      const collarData = response.data;

      // 處理數據格式
      const labels = collarData.map(item => {
        const date = new Date(item.timestamp);
        return `${date.getHours()}:${String(date.getMinutes()).padStart(2, '0')}`;
      });

      const heartRateData = collarData.map(item => item.heartRate);
      const activityData = collarData.map(item => item.activityLevel);

      const data = {
        labels: labels,
        datasets: [
          {
            label: '心率 (bpm)',
            data: heartRateData,
            borderColor: 'rgb(255, 99, 132)',
            backgroundColor: 'rgba(255, 99, 132, 0.5)',
            yAxisID: 'y',
          },
          {
            label: '活動量 (%)',
            data: activityData,
            borderColor: 'rgb(53, 162, 235)',
            backgroundColor: 'rgba(53, 162, 235, 0.5)',
            yAxisID: 'y1',
          },
        ],
      };

      setChartData(data);
      setLoading(false);
    } catch (err) {
      console.error('載入活動數據失敗:', err);
      setError('無法載入活動數據');
      setLoading(false);
      // 發生錯誤時回到假數據
      setUseMockData(true);
    }
  };

  const options = {
    responsive: true,
    interaction: {
      mode: 'index',
      intersect: false,
    },
    stacked: false,
    plugins: {
      title: {
        display: true,
        text: `${petName} - 活動量趨勢圖`,
        font: {
          size: 18,
        },
      },
      legend: {
        position: 'top',
      },
    },
    scales: {
      y: {
        type: 'linear',
        display: true,
        position: 'left',
        title: {
          display: true,
          text: '心率 (bpm)',
        },
      },
      y1: {
        type: 'linear',
        display: true,
        position: 'right',
        title: {
          display: true,
          text: '活動量 (%)',
        },
        grid: {
          drawOnChartArea: false,
        },
      },
    },
  };

  if (loading) {
    return <div style={{ padding: '20px', textAlign: 'center' }}>載入中...</div>;
  }

  if (error && !useMockData) {
    return <div style={{ padding: '20px', color: 'red' }}>{error}</div>;
  }

  return (
    <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <div style={{ marginBottom: '10px', display: 'flex', gap: '10px', alignItems: 'center' }}>
        <label>
          <input
            type="checkbox"
            checked={useMockData}
            onChange={(e) => setUseMockData(e.target.checked)}
          />
          使用模擬數據
        </label>
        {!useMockData && (
          <button 
            onClick={loadRealData}
            style={{ 
              padding: '5px 15px', 
              backgroundColor: '#4CAF50', 
              color: 'white', 
              border: 'none', 
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            重新載入
          </button>
        )}
      </div>

      {chartData && <Line options={options} data={chartData} />}

      <div style={{ 
        marginTop: '20px', 
        padding: '10px', 
        backgroundColor: useMockData ? '#fff3cd' : '#d4edda', 
        borderRadius: '5px',
        border: useMockData ? '1px solid #ffc107' : '1px solid #28a745'
      }}>
        <p style={{ margin: '5px 0', fontSize: '14px' }}>
          {useMockData ? (
            <>💡 目前顯示模擬數據。取消勾選「使用模擬數據」可切換至真實 API 數據。</>
          ) : (
            <>✅ 顯示來自 API 的真實項圈數據 (Pet ID: {petId})</>
          )}
        </p>
      </div>
    </div>
  );
};

export default ActivityChartAPI;
