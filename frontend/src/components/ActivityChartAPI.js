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

  useEffect(() => {
    loadRealData();
  }, [petId]);

  const loadRealData = async () => {
    try {
      setLoading(true);
      // 從 API 取得項圈數據
      const response = await axios.get(`http://localhost:3001/api/healthdata/pet/${petId}`);
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

  if (error) {
    return <div style={{ padding: '20px', color: 'red' }}>{error}</div>;
  }

  return (
    <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <div style={{ marginBottom: '10px', display: 'flex', gap: '10px', alignItems: 'center' }}>
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
      </div>

      {chartData && <Line options={options} data={chartData} />}

      <div style={{ 
        marginTop: '20px', 
        padding: '10px', 
        backgroundColor: '#d4edda', 
        borderRadius: '5px',
        border: '1px solid #28a745'
      }}>
        <p style={{ margin: '5px 0', fontSize: '14px' }}>
          ✅ 顯示來自資料庫的真實健康數據 (Pet ID: {petId})
        </p>
      </div>
    </div>
  );
};

export default ActivityChartAPI;
